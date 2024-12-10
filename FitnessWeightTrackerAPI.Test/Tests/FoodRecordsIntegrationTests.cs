using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using System.Text.Json;
using System.Text;

namespace FitnessWeightTrackerAPI.Test.Tests
{
    public class FoodRecordsIntegrationTests : IntegrationTestBase
    {
        public FoodRecordsIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetFoodRecords_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            var response = await Client.GetAsync("/api/FoodRecords");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task PostFoodRecord_CreatesNewFoodRecord()
        {
            // Arrange
            var foodRecord = new FoodRecordDTO
            {
                FoodItemId = Factory.FakeFoodItem.Id,
                Quantity = 100,
                ConsumptionDate = DateTime.UtcNow
            };
            var content = new StringContent(JsonSerializer.Serialize(foodRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/FoodRecords", content);

            // Log error if response indicates a bad request
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Assert.True(response.IsSuccessStatusCode, $"Error: {errorContent}");
            }
            response.EnsureSuccessStatusCode();

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdFoodRecord = JsonSerializer.Deserialize<FoodRecord>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdFoodRecord);
            Assert.Equal(foodRecord.FoodItemId, createdFoodRecord.FoodItemId);
        }

        [Fact]
        public async Task PutFoodRecord_UpdatesExistingFoodRecord()
        {
            // Arrange
            var foodRecord = new FoodRecordDTO
            {
                FoodItemId = Factory.FakeFoodItem.Id,
                Quantity = 100,
                ConsumptionDate = DateTime.UtcNow
            };
            var postContent = new StringContent(JsonSerializer.Serialize(foodRecord), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/FoodRecords", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdFoodRecord = JsonSerializer.Deserialize<FoodRecord>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdFoodRecord);

            var updatedFoodRecord = new FoodRecordDTO
            {
                FoodItemId = Factory.FakeFoodItem.Id,
                Quantity = 200,
                ConsumptionDate = DateTime.UtcNow
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updatedFoodRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"/api/FoodRecords/{createdFoodRecord.Id}", putContent);
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/FoodRecords/{createdFoodRecord.Id}");
            getResponse.EnsureSuccessStatusCode();
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();
            var fetchedFoodRecord = JsonSerializer.Deserialize<FoodRecord>(getResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(fetchedFoodRecord);
            Assert.Equal(foodRecord.FoodItemId, fetchedFoodRecord.FoodItemId);
            Assert.Equal(200, fetchedFoodRecord.Quantity);
        }

        [Fact]
        public async Task DeleteFoodRecord_DeletesExistingFoodRecord()
        {
            // Arrange
            var foodRecord = new FoodRecordDTO
            {
                FoodItemId = 1,
                Quantity = 100,
                ConsumptionDate = DateTime.UtcNow
            };
            var postContent = new StringContent(JsonSerializer.Serialize(foodRecord), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/FoodRecords", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdFoodRecord = JsonSerializer.Deserialize<FoodRecord>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdFoodRecord);

            // Act
            var response = await Client.DeleteAsync($"/api/FoodRecords/{createdFoodRecord.Id}");
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/FoodRecords/{createdFoodRecord.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }

}
