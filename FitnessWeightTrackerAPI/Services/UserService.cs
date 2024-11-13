using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FitnessWeightTrackerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtConfiguration _jwtConfig;

        public UserService(FitnessWeightTrackerDbContext context, IOptions<JwtConfiguration> jwtConfig)
        {
            _context = context;
            _jwtConfig = jwtConfig.Value;

            _tokenValidationParameters = CreateTokenValidationParameters(_jwtConfig);
        }

        private static TokenValidationParameters CreateTokenValidationParameters(JwtConfiguration config)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config.Issuer,
                ValidAudience = config.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key!))
            };
        }

        public async Task<User> GetUserByIdAsync(int id) => await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

        public async Task<User> LoginUserAsync(UserLoginDTO login)
        {
            var user = await GetUserByUsernameOrEmailAsync(login.UsernameOrEmail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User> RegisterUserAsync(RegistrationUserDTO user, bool isExternalUser = false)
        {
            var entity = new User
            {
                PasswordHash = isExternalUser ? string.Empty : GenerateHashedPassword(user.Password),
                Email = user.Email,
                UserName = user.UserName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Name = user.Name,
                Surname = user.Surname
            };

            if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
            {
                throw new CustomValidationException(validationResults);
            }

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (existingUser != null)
            {
                _context.Users.Remove(existingUser);
                await _context.SaveChangesAsync();
            }

            return existingUser != null;
        }

        public string GenerateUserJWTToken(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            // Generate the JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Email == email);

            return user == null ? null : await GetUserByIdAsync(user.Id);
        }

        private async Task<User> GetUserByUsernameAsync(string userName)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.UserName == userName);

            return user == null ? null : await GetUserByIdAsync(user.Id);
        }

        private async Task<User> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var user = await GetUserByUsernameAsync(usernameOrEmail) ?? await GetUserByEmailAsync(usernameOrEmail);

            return user;
        }

        private string GenerateHashedPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }
    }
}
