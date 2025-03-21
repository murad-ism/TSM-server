using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradingSystemsMonitoring.DataModel.Migrations.TradingDataDb
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Securities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Exchange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Securities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountClosedTrades",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fk_SecurityId = table.Column<long>(type: "bigint", nullable: false),
                    OpeningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpeningPrice = table.Column<double>(type: "float", nullable: false),
                    ClosingPrice = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Operation = table.Column<byte>(type: "tinyint", nullable: false),
                    FutCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultInTicks = table.Column<double>(type: "float", nullable: false),
                    ResultInPercent = table.Column<double>(type: "float", nullable: false),
                    ResultInCash = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountClosedTrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountClosedTrades_Securities_fk_SecurityId",
                        column: x => x.fk_SecurityId,
                        principalTable: "Securities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountClosedTrades_fk_SecurityId",
                table: "AccountClosedTrades",
                column: "fk_SecurityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountClosedTrades");

            migrationBuilder.DropTable(
                name: "Securities");
        }
    }
}
