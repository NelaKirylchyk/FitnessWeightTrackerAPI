using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using System.Text.Json;
using System.Text;
using FitnessWeightTrackerAPI.Data;
using Microsoft.AspNetCore.Identity;

namespace FitnessWeightTrackerAPI.Test.Tests
{
    public class NutritionTargetsIntegrationTests : IntegrationTestBase
    {
        public NutritionTargetsIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetNutritionTargets_ReturnsSuccessAndCorrectContentType()
        {
            var response = await Client.GetAsync("/api/NutritionTargets");

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
        public async Task PostNutritionTarget_CreatesNewNutritionTarget()
        {
            // Arrange

            await DeleteNutritionTargetsForCurrentUser();

            var nutritionTarget = new NutritionTargetDTO
            {
                DailyCalories = 2000,
                DailyCarbonohydrates = 250,
                DailyProtein = 100,
                DailyFat = 70
            };
            var content = new StringContent(JsonSerializer.Serialize(nutritionTarget), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/NutritionTargets", content);
            response.EnsureSuccessStatusCode();

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdNutritionTarget = JsonSerializer.Deserialize<NutritionTarget>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdNutritionTarget);
            Assert.Equal(2000, createdNutritionTarget.DailyCalories);
        }

        [Fact]
        public async Task PostNutritionTarget_ReturnsConflictWhenUserAlreadyHasTarget()
        {
            // Arrange

            await DeleteNutritionTargetsForCurrentUser();

            var nutritionTarget = new NutritionTargetDTO
            {
                DailyCalories = 2000,
                DailyCarbonohydrates = 250,
                DailyProtein = 100,
                DailyFat = 70
            };
            var content = new StringContent(JsonSerializer.Serialize(nutritionTarget), Encoding.UTF8, "application/json");

            // Act
            var firstResponse = await Client.PostAsync("/api/NutritionTargets", content);
            firstResponse.EnsureSuccessStatusCode();

            var secondResponse = await Client.PostAsync("/api/NutritionTargets", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Conflict, secondResponse.StatusCode);
            var errorResponseContent = await secondResponse.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorResponseContent);
            Assert.Equal("User already has a nutrition target.", errorResponse["message"]);
        }

        [Fact]
        public async Task PutNutritionTarget_UpdatesExistingNutritionTarget()
        {
            // Arrange

            await DeleteNutritionTargetsForCurrentUser();

            var nutritionTarget = new NutritionTargetDTO
            {
                DailyCalories = 2000,
                DailyCarbonohydrates = 250,
                DailyProtein = 100,
                DailyFat = 70
            };
            var postContent = new StringContent(JsonSerializer.Serialize(nutritionTarget), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/NutritionTargets", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdNutritionTarget = JsonSerializer.Deserialize<NutritionTarget>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdNutritionTarget);

            var updatedNutritionTarget = new NutritionTargetDTO
            {
                DailyCalories = 2500,
                DailyCarbonohydrates = 300,
                DailyProtein = 120,
                DailyFat = 80
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updatedNutritionTarget), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"/api/NutritionTargets/{createdNutritionTarget.Id}", putContent);
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/NutritionTargets/{createdNutritionTarget.Id}");
            getResponse.EnsureSuccessStatusCode();
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();
            var fetchedNutritionTarget = JsonSerializer.Deserialize<NutritionTarget>(getResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(fetchedNutritionTarget);
            Assert.Equal(2500, fetchedNutritionTarget.DailyCalories);
        }

        [Fact]
        public async Task DeleteNutritionTarget_DeletesExistingNutritionTarget()
        {
            // Arrange

            await DeleteNutritionTargetsForCurrentUser();

            var nutritionTarget = new NutritionTargetDTO
            {
                DailyCalories = 2000,
                DailyCarbonohydrates = 250,
                DailyProtein = 100,
                DailyFat = 70
            };
            var postContent = new StringContent(JsonSerializer.Serialize(nutritionTarget), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/NutritionTargets", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdNutritionTarget = JsonSerializer.Deserialize<NutritionTarget>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdNutritionTarget);

            // Act
            var response = await Client.DeleteAsync($"/api/NutritionTargets/{createdNutritionTarget.Id}");
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/NutritionTargets/{createdNutritionTarget.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        private async Task DeleteNutritionTargetsForCurrentUser()
        {
            using (var scope = Factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FitnessWeightTrackerDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<FitnessUser>>();
                var user = await userManager.FindByNameAsync("testuser@test.com");
                var nutritionTargets = context.NutritionTargets.Where(t => t.UserId == user.Id);
                context.NutritionTargets.RemoveRange(nutritionTargets);
                await context.SaveChangesAsync();
            }
        }
    }

}
