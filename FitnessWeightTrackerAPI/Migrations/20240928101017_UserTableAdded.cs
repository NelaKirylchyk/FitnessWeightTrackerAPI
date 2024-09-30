using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessWeightTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BodyWeightRecords_User_UserId",
                table: "BodyWeightRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_BodyWeightTargets_User_UserId",
                table: "BodyWeightTargets");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodRecords_User_UserId",
                table: "FoodRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_NutritionTargets_User_UserId",
                table: "NutritionTargets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "BirthDate",
                value: new DateTime(2024, 9, 28, 10, 10, 15, 67, DateTimeKind.Utc).AddTicks(4751));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "BirthDate",
                value: new DateTime(2024, 9, 28, 9, 56, 30, 312, DateTimeKind.Utc).AddTicks(2352));

            migrationBuilder.AddForeignKey(
                name: "FK_BodyWeightRecords_User_UserId",
                table: "BodyWeightRecords",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BodyWeightTargets_User_UserId",
                table: "BodyWeightTargets",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodRecords_User_UserId",
                table: "FoodRecords",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionTargets_User_UserId",
                table: "NutritionTargets",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
