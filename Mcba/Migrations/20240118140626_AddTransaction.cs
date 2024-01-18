using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcba.Migrations
{
    /// <inheritdoc />
    public partial class AddTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table =>
                    new
                    {
                        TransactionID = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        TransactionType = table.Column<string>(
                            type: "nvarchar(1)",
                            nullable: false
                        ),
                        AccountNumber = table.Column<int>(type: "int", nullable: false),
                        DestinationAccountNumber = table.Column<int>(type: "int", nullable: true),
                        Amount = table.Column<decimal>(type: "money", nullable: false),
                        Comment = table.Column<string>(
                            type: "nvarchar(30)",
                            maxLength: 30,
                            nullable: true
                        ),
                        TransactionTimeUtc = table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_DestinationAccountNumber",
                        column: x => x.DestinationAccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountNumber",
                table: "Transactions",
                column: "AccountNumber"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DestinationAccountNumber",
                table: "Transactions",
                column: "DestinationAccountNumber"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Transactions");
        }
    }
}
