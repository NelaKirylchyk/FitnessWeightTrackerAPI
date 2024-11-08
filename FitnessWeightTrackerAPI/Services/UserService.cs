namespace FitnessWeightTrackerAPI.Services
{
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
    using Microsoft.IdentityModel.Tokens;

    public class UserService : IUserService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public UserService(FitnessWeightTrackerDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            _tokenValidationParameters = new ()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] !))
            };
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> LoginUserAsync(UserLoginDTO login)
        {
            // Get a user by username or email
            var user = await GetUserByUsernameAsync(login.UsernameOrEmail) ?? await GetUserByEmailAsync(login.UsernameOrEmail);

            // Assert user exists
            if (user == null)
            {
                return null;
            }

            // Verify the password is correct
            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<User> RegisterUserAsync(RegistrationUserDTO user)
        {
            string hashedPassword = GenerateHashedPassword(user.Password);

            var entity = new User
            {
                PasswordHash = hashedPassword,
                Email = user.Email,
                UserName = user.UserName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Name = user.Name,
                Surname = user.Surname
            };

            // Validate Entity
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
            var existingUser = await _context.Users.FirstOrDefaultAsync(r => r.Id == id);
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.Email == email);

            return user == null ? null : await GetUserByIdAsync(user.Id);
        }

        private async Task<User> GetUserByUsernameAsync(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.UserName == userName);

            return user == null ? null : await GetUserByIdAsync(user.Id);
        }

        private string GenerateHashedPassword(string password)
        {
            string hashedPassword = string.Empty;
            if (password != null)
            {
                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            }

            return hashedPassword;
        }
    }
}
