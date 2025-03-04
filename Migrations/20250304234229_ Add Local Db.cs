using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiWithRoleAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "d94d6451-d159-47e8-87ed-a0ccdc87dcb6" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d94d6451-d159-47e8-87ed-a0ccdc87dcb6");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "39ce48d7-c99b-4cb9-83d4-e45ef7a921be", 0, "297bb54d-824f-46e8-8a2f-1f980f3362d9", "group1@group1.com", true, false, null, "GROUP1@GROUP1.COM", "GROUP1@GROUP1.COM", "AQAAAAIAAYagAAAAEC3Hce1S96dQGmCdBvsmWSjlt98znX8LUTR+K+NYvtm924MhGHKWsqZrAms0PQIqlQ==", "0912345678", true, "2818e3a2-1cfc-44ae-81ee-5f032b12ee3b", false, "group1@group1.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "39ce48d7-c99b-4cb9-83d4-e45ef7a921be" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "39ce48d7-c99b-4cb9-83d4-e45ef7a921be" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "39ce48d7-c99b-4cb9-83d4-e45ef7a921be");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d94d6451-d159-47e8-87ed-a0ccdc87dcb6", 0, "521b9b83-3d0d-4f95-8da5-4c35ae379e9c", "freetrained@freetrained.com", true, false, null, "FREETRAINED@FREETRAINED.COM", "FREETRAINED@FREETRAINED.COM", "AQAAAAIAAYagAAAAEBx03SPRMcNxzketnW+if2YLK11PJ9bgSBW8VFCxVDbWmv1UG6Te2rMxEJP9L8kLYA==", "1234567890", true, "9d019822-7890-44ca-84e4-c19ff695a456", false, "freetrained@freetrained.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "d94d6451-d159-47e8-87ed-a0ccdc87dcb6" });
        }
    }
}
