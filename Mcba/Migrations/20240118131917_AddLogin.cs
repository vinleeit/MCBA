using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcba.Migrations
{
    /// <inheritdoc />
    public partial class AddLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table =>
                    new
                    {
                        LoginID = table.Column<string>(type: "char(8)", nullable: false),
                        CustomerID = table.Column<int>(type: "int", nullable: false),
                        PasswordHash = table.Column<string>(type: "char(94)", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.LoginID);
                    table.ForeignKey(
                        name: "FK_Logins_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Logins_CustomerID",
                table: "Logins",
                column: "CustomerID"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Logins");
        }
    }
}
