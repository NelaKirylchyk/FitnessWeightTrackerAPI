using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessWeightTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGenderField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add a temporary column
            migrationBuilder.AddColumn<int>(
                name: "GenderTemp",
                table: "Users",
                nullable: true);

            // Update the temporary column with default value, assuming 0 as 'NotSpecified'
            migrationBuilder.Sql("UPDATE Users SET GenderTemp = CASE Gender WHEN 'M' THEN 0 WHEN 'W' THEN 1 ELSE 2 END");

            // Remove the original column
            migrationBuilder.DropColumn(name: "Gender", table: "Users");

            // Rename the temporary column to the original column name
            migrationBuilder.RenameColumn(
                name: "GenderTemp",
                table: "Users",
                newName: "Gender");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the renaming
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Users",
                newName: "GenderTemp");

            // Add back the original column
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            // Copy the data back to the original column
            migrationBuilder.Sql("UPDATE Users SET Gender = CASE GenderTemp WHEN 0 THEN 'M' WHEN 1 THEN 'W' ELSE 'NotSpecified' END");

            // Drop the temporary column
            migrationBuilder.DropColumn(name: "GenderTemp", table: "Users");
        }
    }
}
