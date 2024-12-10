using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using System.Text.Json;
using System.Text;

namespace FitnessWeightTrackerAPI.Test.Tests
{
    public class BodyWeightRecordsIntegrationTests : IntegrationTestBase
    {
        public BodyWeightRecordsIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBodyWeightRecords_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            var response = await Client.GetAsync("/api/BodyWeightRecords");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task PostBodyWeightRecord_CreatesNewBodyWeightRecord()
        {
            // Arrange
            var bodyWeightRecord = new BodyWeightRecordDTO
            {
                Weight = 70.5F,
                Date = DateTime.UtcNow
            };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/BodyWeightRecords", content);
            response.EnsureSuccessStatusCode();

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdBodyWeightRecord = JsonSerializer.Deserialize<BodyWeightRecord>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdBodyWeightRecord);
            Assert.Equal(70.5, createdBodyWeightRecord.Weight);
        }

        [Fact]
        public async Task PutBodyWeightRecord_UpdatesExistingBodyWeightRecord()
        {
            // Arrange
            var bodyWeightRecord = new BodyWeightRecordDTO
            {
                Weight = 70.5F,
                Date = DateTime.UtcNow
            };
            var postContent = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/BodyWeightRecords", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdBodyWeightRecord = JsonSerializer.Deserialize<BodyWeightRecord>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdBodyWeightRecord);

            var updatedBodyWeightRecord = new BodyWeightRecordDTO
            {
                Weight = 75.0F,
                Date = createdBodyWeightRecord.Date
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updatedBodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"/api/BodyWeightRecords/{createdBodyWeightRecord.Id}", putContent);
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/BodyWeightRecords/{createdBodyWeightRecord.Id}");
            getResponse.EnsureSuccessStatusCode();
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();
            var fetchedBodyWeightRecord = JsonSerializer.Deserialize<BodyWeightRecord>(getResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(fetchedBodyWeightRecord);
            Assert.Equal(75.0, fetchedBodyWeightRecord.Weight);
        }

        [Fact]
        public async Task DeleteBodyWeightRecord_DeletesExistingBodyWeightRecord()
        {
            // Arrange
            var bodyWeightRecord = new BodyWeightRecordDTO
            {
                Weight = 70.5F,
                Date = DateTime.UtcNow
            };
            var postContent = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");
            var postResponse = await Client.PostAsync("/api/BodyWeightRecords", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            var createdBodyWeightRecord = JsonSerializer.Deserialize<BodyWeightRecord>(postResponseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(createdBodyWeightRecord);

            // Act
            var response = await Client.DeleteAsync($"/api/BodyWeightRecords/{createdBodyWeightRecord.Id}");
            response.EnsureSuccessStatusCode();

            // Assert
            var getResponse = await Client.GetAsync($"/api/BodyWeightRecords/{createdBodyWeightRecord.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
