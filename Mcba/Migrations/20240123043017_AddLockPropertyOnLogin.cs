using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcba.Migrations
{
    /// <inheritdoc />
    public partial class AddLockPropertyOnLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Logins",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Logins");
        }
    }
}
