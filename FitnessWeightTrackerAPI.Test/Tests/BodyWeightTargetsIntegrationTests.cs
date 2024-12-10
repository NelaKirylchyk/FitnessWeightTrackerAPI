using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text;

namespace FitnessWeightTrackerAPI.Test.Tests
{
    public class BodyWeightTargetsIntegrationTests : IntegrationTestBase
    {
        public BodyWeightTargetsIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBodyWeightTargets_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            await DeleteBodyWeightTargetsForCurrentUser();

            // Act
            var response = await Client.GetAsync("/api/BodyWeightTargets");

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }
            else
            {
                response.EnsureSuccessStatusCode();
                Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            }
        }

        [Fact]
        public async Task GetBodyWeightTargetById_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            await DeleteBodyWeightTargetsForCurrentUser();
            var bodyWeightTarget = new BodyWeightTargetDTO
            {
                TargetWeight = 65.0F,
                TargetDate = DateTime.UtcNow.AddMonths(3)
            };
            var postContent = new StringContent(JsonSerializer.Serialize(bodyWeightTarget), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/BodyWeightTargets", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdBodyWeightTarget = JsonSerializer.Deserialize<BodyWeightTarget>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Act
            var response = await Client.GetAsync($"/api/BodyWeightTargets/{createdBodyWeightTarget.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task PostBodyWeightTarget_CreatesNewBodyWeightTarget()
        {
            // Arrange
            await DeleteBodyWeightTargetsForCurrentUser();

            var bodyWeightTarget = new BodyWeightTargetDTO
            {
                TargetWeight = 65.0F,
                TargetDate = DateTime.UtcNow.AddMonths(3)
            };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightTarget), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/BodyWeightTargets", content);
            response.EnsureSuccessStatusCode();

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdBodyWeightTarget = JsonSerializer.Deserialize<BodyWeightTarget>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdBodyWeightTarget);
            Assert.Equal(65.0, createdBodyWeightTarget.TargetWeight);
        }

        [Fact]
        public async Task PostBodyWeightTarget_ReturnsConflictWhenUserAlreadyHasTarget()
        {
            // Arrange
            await DeleteBodyWeightTargetsForCurrentUser();

            var bodyWeightTarget = new BodyWeightTargetDTO
            {
                TargetWeight = 65.0F,
                TargetDate = DateTime.UtcNow.AddMonths(3)
            };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightTarget), Encoding.UTF8, "application/json");

            // Act
            var firstResponse = await Client.PostAsync("/api/BodyWeightTargets", content);
            firstResponse.EnsureSuccessStatusCode();

            var secondResponse = await Client.PostAsync("/api/BodyWeightTargets", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Conflict, secondResponse.StatusCode);
            var errorResponseContent = await secondResponse.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorResponseContent);
            Assert.Equal("User already has a bodyweight target.", errorResponse["message"]);
        }

        [Fact]
        public async Task PutBodyWeightTarget_UpdatesExistingBodyWeightTarget()
        {
            // Arrange
            await DeleteBodyWeightTargetsForCurrentUser();

            var bodyWeightTarget = new BodyWeightTargetDTO
            {
                TargetWeight = 65.0F,
                TargetDate = DateTime.UtcNow.AddMonths(3)
            };
            var postContent = new StringContent(JsonSerializer.Serialize(bodyWeightTarget), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/BodyWeightTargets", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdBodyWeightTarget = JsonSerializer.Deserialize<BodyWeightTarget>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdBodyWeightTarget);

            var updatedBodyWeightTarget = new BodyWeightTargetDTO
            {
                TargetWeight = 60.0F,
                TargetDate = createdBodyWeightTarget.TargetDate
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updatedBodyWeightTarget), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"/api/BodyWeightTargets/{createdBodyWeightTarget.Id}", putContent);
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/BodyWeightTargets/{createdBodyWeightTarget.Id}");
            getResponse.EnsureSuccessStatusCode();
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();
            var fetchedBodyWeightTarget = JsonSerializer.Deserialize<BodyWeightTarget>(getResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(fetchedBodyWeightTarget);
            Assert.Equal(60.0, fetchedBodyWeightTarget.TargetWeight);
        }

        [Fact]
        public async Task DeleteBodyWeightTarget_DeletesExistingBodyWeightTarget()
        {
            // Arrange
            await DeleteBodyWeightTargetsForCurrentUser();

            var bodyWeightTarget = new BodyWeightTargetDTO
            {
                TargetWeight = 65.0F,
                TargetDate = DateTime.UtcNow.AddMonths(3)
            };
            var postContent = new StringContent(JsonSerializer.Serialize(bodyWeightTarget), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/BodyWeightTargets", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdBodyWeightTarget = JsonSerializer.Deserialize<BodyWeightTarget>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdBodyWeightTarget);

            // Act
            var response = await Client.DeleteAsync($"/api/BodyWeightTargets/{createdBodyWeightTarget.Id}");
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/BodyWeightTargets/{createdBodyWeightTarget.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        private async Task DeleteBodyWeightTargetsForCurrentUser()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FitnessWeightTrackerDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<FitnessUser>>();
                var user = await userManager.FindByNameAsync("testuser@test.com");

                var bodyWeightTargets = context.BodyWeightTargets.Where(t => t.UserId == user.Id);

                context.BodyWeightTargets.RemoveRange(bodyWeightTargets);
                await context.SaveChangesAsync();
            }
        }
    }


}
