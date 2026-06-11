using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShoppingPlanner.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Dairy" },
                    { 2, "Bakery" },
                    { 3, "Vegetables" },
                    { 4, "Fruits" },
                    { 5, "Beverages" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "DefaultUnit", "Name" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(565), "l", "Milk" },
                    { 2, 1, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(567), "kg", "Butter" },
                    { 3, 2, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(568), "pcs", "Sourdough Bread" },
                    { 4, 2, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(569), "pcs", "Baguette" },
                    { 5, 3, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(570), "kg", "Carrots" },
                    { 6, 3, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(570), "kg", "Tomatoes" },
                    { 7, 3, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(571), "kg", "Spinach" },
                    { 8, 4, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(572), "kg", "Apples" },
                    { 9, 4, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(573), "kg", "Bananas" },
                    { 10, 5, new DateTime(2026, 6, 11, 9, 59, 53, 885, DateTimeKind.Utc).AddTicks(574), "l", "Orange Juice" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
