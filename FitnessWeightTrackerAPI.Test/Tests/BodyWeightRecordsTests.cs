using Microsoft.AspNetCore.Mvc.Testing;

namespace FitnessWeightTrackerAPI.Test.Tests
{
    [Collection("DatabaseCollection")]
    public class BodyWeightRecordsTests
    {
        private readonly DatabaseFixture _fixture;

        public BodyWeightRecordsTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("/api/BodyWeightRecords")]
        public async Task Get_BodyWeightRecords(string url)
        {
            // Arrange
            var client = _fixture.Client;

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

        }
    }
}
