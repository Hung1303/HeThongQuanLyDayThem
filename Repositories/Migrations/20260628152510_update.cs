using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccountStatus", "CreatedAt", "DateOfBirth", "Email", "Fullname", "Gender", "IsDeleted", "LastUpdatedAt", "Password", "PhoneNumber", "UserRole", "Username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2003, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "tuanhung01032003@gmail.com", "Đỗ Tuấn Hùng", 1, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "123456", "0932760162", 1, "tuanhung0103" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
