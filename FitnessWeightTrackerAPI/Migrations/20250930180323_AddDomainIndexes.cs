using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessWeightTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodRecords_UserId",
                table: "FoodRecords");

            migrationBuilder.DropIndex(
                name: "IX_BodyWeightRecords_UserId",
                table: "BodyWeightRecords");

            migrationBuilder.CreateIndex(
                name: "IX_FoodRecords_UserId_ConsumptionDate",
                table: "FoodRecords",
                columns: new[] { "UserId", "ConsumptionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BodyWeightRecords_UserId_Date",
                table: "BodyWeightRecords",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodRecords_UserId_ConsumptionDate",
                table: "FoodRecords");

            migrationBuilder.DropIndex(
                name: "IX_BodyWeightRecords_UserId_Date",
                table: "BodyWeightRecords");

            migrationBuilder.CreateIndex(
                name: "IX_FoodRecords_UserId",
                table: "FoodRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BodyWeightRecords_UserId",
                table: "BodyWeightRecords",
                column: "UserId");
        }
    }
}
