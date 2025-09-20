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

        [Fact]
        public async Task GetBodyWeightRecord_NonExistentId_ReturnsNotFound()
        {
            // Act
            var response = await Client.GetAsync("/api/BodyWeightRecords/999999");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutBodyWeightRecord_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var updatedBodyWeightRecord = new BodyWeightRecordDTO
            {
                Weight = 80.0F,
                Date = DateTime.UtcNow
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updatedBodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync("/api/BodyWeightRecords/999999", putContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteBodyWeightRecord_NonExistentId_ReturnsNotFound()
        {
            // Act
            var response = await Client.DeleteAsync("/api/BodyWeightRecords/999999");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostBodyWeightRecord_InvalidModel_ReturnsBadRequest()
        {
            // Arrange: Missing required Weight
            var bodyWeightRecord = new BodyWeightRecordDTO
            {
                // Weight = missing
                Date = DateTime.UtcNow
            };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/BodyWeightRecords", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutBodyWeightRecord_InvalidModel_ReturnsBadRequest()
        {
            // Arrange: First create a valid record
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

            // Now try to update with invalid data (missing Weight)
            var updatedBodyWeightRecord = new BodyWeightRecordDTO
            {
                // Weight = missing
                Date = DateTime.UtcNow
            };
            var putContent = new StringContent(JsonSerializer.Serialize(updatedBodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync($"/api/BodyWeightRecords/{createdBodyWeightRecord.Id}", putContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        // 1. GET with ascendingOrder=true returns records in ascending order
        [Fact]
        public async Task GetBodyWeightRecords_AscendingOrder_ReturnsRecordsInAscendingOrder()
        {
            // Arrange: Create two records with different dates
            var record1 = new BodyWeightRecordDTO { Weight = 70.5F, Date = DateTime.UtcNow.AddDays(-2) };
            var record2 = new BodyWeightRecordDTO { Weight = 72.0F, Date = DateTime.UtcNow.AddDays(-1) };
            await Client.PostAsync("/api/BodyWeightRecords", new StringContent(JsonSerializer.Serialize(record1), Encoding.UTF8, "application/json"));
            await Client.PostAsync("/api/BodyWeightRecords", new StringContent(JsonSerializer.Serialize(record2), Encoding.UTF8, "application/json"));

            // Act
            var response = await Client.GetAsync("/api/BodyWeightRecords?ascendingOrder=true");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var records = JsonSerializer.Deserialize<List<BodyWeightRecord>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.True(records.Count >= 2);
            Assert.True(records[0].Date <= records[1].Date);
        }

        // 2. GET returns all created records
        [Fact]
        public async Task GetBodyWeightRecords_ReturnsAllCreatedRecords()
        {
            // Arrange: Create two records
            var record1 = new BodyWeightRecordDTO { Weight = 60.0F, Date = DateTime.UtcNow.AddDays(-3) };
            var record2 = new BodyWeightRecordDTO { Weight = 65.0F, Date = DateTime.UtcNow.AddDays(-2) };
            await Client.PostAsync("/api/BodyWeightRecords", new StringContent(JsonSerializer.Serialize(record1), Encoding.UTF8, "application/json"));
            await Client.PostAsync("/api/BodyWeightRecords", new StringContent(JsonSerializer.Serialize(record2), Encoding.UTF8, "application/json"));

            // Act
            var response = await Client.GetAsync("/api/BodyWeightRecords");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var records = JsonSerializer.Deserialize<List<BodyWeightRecord>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.True(records.Count >= 2);
            Assert.Contains(records, r => r.Weight == 60.0F);
            Assert.Contains(records, r => r.Weight == 65.0F);
        }

        // 3. POST returns correct Location header
        [Fact]
        public async Task PostBodyWeightRecord_ReturnsCorrectLocationHeader()
        {
            // Arrange
            var bodyWeightRecord = new BodyWeightRecordDTO { Weight = 80.0F, Date = DateTime.UtcNow };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/BodyWeightRecords", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Headers.Location);
            Assert.Contains("/api/BodyWeightRecords/", response.Headers.Location.ToString());
        }

        // 4. POST with future date returns BadRequest
        [Fact]
        public async Task PostBodyWeightRecord_FutureDate_ReturnsBadRequest()
        {
            // Arrange
            var bodyWeightRecord = new BodyWeightRecordDTO { Weight = 70.0F, Date = DateTime.UtcNow.AddDays(1) };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/BodyWeightRecords", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        // 5. POST with min/max weight values
        [Theory]
        [InlineData(0.01F)]
        [InlineData(500.0F)]
        public async Task PostBodyWeightRecord_WeightBoundaryValues_ReturnsSuccess(float weight)
        {
            // Arrange
            var bodyWeightRecord = new BodyWeightRecordDTO { Weight = weight, Date = DateTime.UtcNow };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/BodyWeightRecords", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        // 6. POST with weight out of range returns BadRequest
        [Theory]
        [InlineData(0.0F)]
        [InlineData(0.009F)]
        [InlineData(500.1F)]
        public async Task PostBodyWeightRecord_WeightOutOfRange_ReturnsBadRequest(float weight)
        {
            // Arrange
            var bodyWeightRecord = new BodyWeightRecordDTO { Weight = weight, Date = DateTime.UtcNow };
            var content = new StringContent(JsonSerializer.Serialize(bodyWeightRecord), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("/api/BodyWeightRecords", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
