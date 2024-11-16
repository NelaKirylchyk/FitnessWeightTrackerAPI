using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessWeightTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BodyWeightRecords_Users_UserId",
                table: "BodyWeightRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_BodyWeightTargets_Users_UserId",
                table: "BodyWeightTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodRecords_Users_UserId",
                table: "FoodRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_NutritionTargets_Users_UserId",
                table: "NutritionTargets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_BodyWeightRecords_AspNetUsers_UserId",
                table: "BodyWeightRecords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BodyWeightTargets_AspNetUsers_UserId",
                table: "BodyWeightTargets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodRecords_AspNetUsers_UserId",
                table: "FoodRecords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionTargets_AspNetUsers_UserId",
                table: "NutritionTargets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BodyWeightRecords_AspNetUsers_UserId",
                table: "BodyWeightRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_BodyWeightTargets_AspNetUsers_UserId",
                table: "BodyWeightTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodRecords_AspNetUsers_UserId",
                table: "FoodRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_NutritionTargets_AspNetUsers_UserId",
                table: "NutritionTargets");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "Email", "Gender", "Name", "PasswordHash", "Surname", "UserName" },
                values: new object[] { 1, new DateTime(2024, 11, 14, 18, 25, 38, 50, DateTimeKind.Utc).AddTicks(9820), "name.surname@gmail.com", 0, "New temp user name", "hash", "New temp user surname", "UserName" });

            migrationBuilder.AddForeignKey(
                name: "FK_BodyWeightRecords_Users_UserId",
                table: "BodyWeightRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BodyWeightTargets_Users_UserId",
                table: "BodyWeightTargets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodRecords_Users_UserId",
                table: "FoodRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionTargets_Users_UserId",
                table: "NutritionTargets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
