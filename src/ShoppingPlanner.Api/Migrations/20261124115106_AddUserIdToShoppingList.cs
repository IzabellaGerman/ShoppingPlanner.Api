using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingPlanner.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToShoppingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ShoppingLists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                         UPDATE ""ShoppingLists""
                        SET ""UserId"" = (
                        SELECT ""Id""
                        FROM ""AspNetUsers""
                        WHERE ""NormalizedEmail"" = 'TEST@TEST.COM');");

            
            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_UserId",
                table: "ShoppingLists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingLists_AspNetUsers_UserId",
                table: "ShoppingLists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingLists_AspNetUsers_UserId",
                table: "ShoppingLists");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingLists_UserId",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShoppingLists");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1726));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1729));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1730));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1730));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1731));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1732));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1733));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1734));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1734));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 8, 9, 41, 23, 795, DateTimeKind.Utc).AddTicks(1735));
        }
    }
}
