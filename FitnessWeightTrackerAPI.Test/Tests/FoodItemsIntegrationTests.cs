using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using System.Text.Json;
using System.Text;

namespace FitnessWeightTrackerAPI.Test.Tests
{
    public class FoodItemsIntegrationTests : IntegrationTestBase
    {
        public FoodItemsIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetFoodItems_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            var response = await Client.GetAsync("/api/FoodItems");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task PostFoodItem_CreatesNewFoodItem()
        {
            // Arrange
            var foodItem = new FoodItemDTO
            {
                Name = "Banana",
                Calories = 100,
                Carbohydrates = 25,
                Protein = 1,
                Fat = 5,
                ServingSize = 100
            };
            var content = new StringContent(JsonSerializer.Serialize(foodItem), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/FoodItems", content);
            response.EnsureSuccessStatusCode();

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdFoodItem = JsonSerializer.Deserialize<FoodItem>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdFoodItem);
            Assert.Equal("Banana", createdFoodItem.Name);
        }

        [Fact]
        public async Task PutFoodItem_UpdatesExistingFoodItem()
        {
            // Arrange
            var foodItem = new FoodItemDTO
            {
                Name = "Apple",
                Calories = 52,
                Carbohydrates = 14,
                Protein = 3,
                Fat = 2,
                ServingSize = 100
            };
            var postContent = new StringContent(JsonSerializer.Serialize(foodItem), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/FoodItems", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdFoodItem = JsonSerializer.Deserialize<FoodItem>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdFoodItem);

            var updatedFoodItem = new FoodItemDTO
            {
                Name = "Apple",
                Calories = 55,
                Carbohydrates = 15,
                Protein = 4,
                Fat = 1,
                ServingSize = 100
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updatedFoodItem), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"/api/FoodItems/{createdFoodItem.Id}", putContent);
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/FoodItems/{createdFoodItem.Id}");
            getResponse.EnsureSuccessStatusCode();
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();
            var fetchedFoodItem = JsonSerializer.Deserialize<FoodItem>(getResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(fetchedFoodItem);
            Assert.Equal("Apple", fetchedFoodItem.Name);
            Assert.Equal(55, fetchedFoodItem.Calories);
            Assert.Equal(15, fetchedFoodItem.Carbohydrates);
            Assert.Equal(4, fetchedFoodItem.Protein);
            Assert.Equal(1, fetchedFoodItem.Fat);
        }

        [Fact]
        public async Task DeleteFoodItem_DeletesExistingFoodItem()
        {
            // Arrange
            var foodItem = new FoodItemDTO
            {
                Name = "Orange",
                Calories = 47,
                Carbohydrates = 12,
                Protein = 9,
                Fat = 1,
                ServingSize = 100
            };
            var postContent = new StringContent(JsonSerializer.Serialize(foodItem), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/FoodItems", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdFoodItem = JsonSerializer.Deserialize<FoodItem>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdFoodItem);

            // Act
            var response = await Client.DeleteAsync($"/api/FoodItems/{createdFoodItem.Id}");
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/FoodItems/{createdFoodItem.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
