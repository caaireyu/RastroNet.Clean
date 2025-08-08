using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rastro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndexName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_users_username",
                schema: "public",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_username",
                schema: "public",
                table: "users");
        }
    }
}
