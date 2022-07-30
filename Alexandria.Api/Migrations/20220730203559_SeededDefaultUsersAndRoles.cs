using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alexandria.Api.Migrations
{
    public partial class SeededDefaultUsersAndRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c868aebb-0d51-4c91-8f47-9533d6ceecdd", "a0412831-dbd5-42cd-b975-3bf8b01fb981", "Administrator", "ADMINISTRATOR" },
                    { "e4225d87-3b1c-41bf-bfb6-34a2c238c6cb", "389db709-02cc-4b7d-b278-400b6023cbe8", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "31b9ecdc-05e1-42b9-ae4a-965bc7d0aebf", 0, "947cca8f-7241-4f8e-9d3a-291ed5a474de", "admin@alexandria.com", false, "Kibonga", "Kimur", false, null, "ADMIN@ALEXANDRIA.COM", "ADMIN@ALEXANDRIA.COM", "AQAAAAEAACcQAAAAEP3Esey6BbmtffFAmGmVunyIyVDgwze7cMxPhu6gbIN0YocYY6VH1LO6/lVKhkR5TA==", null, false, "35e04b37-00d7-4989-806b-310cb24cab31", false, "admin@alexandria.com" },
                    { "3d7f652f-c44d-4d48-9231-0b4f8ea1e89a", 0, "44d95fee-a906-4074-be99-77fc444c635a", "user@alexandria.com", false, "Suga", "Sean", false, null, "USER@ALEXANDRIA.COM", "USER@ALEXANDRIA.COM", "AQAAAAEAACcQAAAAEIB4xH50dJFdOme6sY/jetwnxu6ZXaMY3k711PGXB815861LietGMqRCZqRBD8JVxQ==", null, false, "bcf4a8ed-6637-4335-a514-3963e154ec63", false, "user@alexandria.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c868aebb-0d51-4c91-8f47-9533d6ceecdd", "31b9ecdc-05e1-42b9-ae4a-965bc7d0aebf" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "e4225d87-3b1c-41bf-bfb6-34a2c238c6cb", "3d7f652f-c44d-4d48-9231-0b4f8ea1e89a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c868aebb-0d51-4c91-8f47-9533d6ceecdd", "31b9ecdc-05e1-42b9-ae4a-965bc7d0aebf" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "e4225d87-3b1c-41bf-bfb6-34a2c238c6cb", "3d7f652f-c44d-4d48-9231-0b4f8ea1e89a" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c868aebb-0d51-4c91-8f47-9533d6ceecdd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4225d87-3b1c-41bf-bfb6-34a2c238c6cb");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "31b9ecdc-05e1-42b9-ae4a-965bc7d0aebf");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3d7f652f-c44d-4d48-9231-0b4f8ea1e89a");
        }
    }
}
