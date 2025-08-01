using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdministrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Administrators",
                columns: new[] { "Id", "Email", "Password", "Perfil" },
                values: new object[] { 1, "administrator@test.com", "abc123", "Adm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
