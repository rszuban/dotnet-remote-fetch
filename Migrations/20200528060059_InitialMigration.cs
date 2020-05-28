using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace RemoteFetch.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FetchUnits",
                columns: table => new
                {
                    UnitName = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Schedule = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetchUnits", x => x.UnitName);
                });

            migrationBuilder.CreateTable(
                name: "FetchItem",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    Xpath = table.Column<string>(nullable: true),
                    FetchUnitUnitName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetchItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FetchItem_FetchUnits_FetchUnitUnitName",
                        column: x => x.FetchUnitUnitName,
                        principalTable: "FetchUnits",
                        principalColumn: "UnitName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FetchItemValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: false),
                    FetchItemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetchItemValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FetchItemValue_FetchItem_FetchItemId",
                        column: x => x.FetchItemId,
                        principalTable: "FetchItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FetchItem_FetchUnitUnitName",
                table: "FetchItem",
                column: "FetchUnitUnitName");

            migrationBuilder.CreateIndex(
                name: "IX_FetchItemValue_FetchItemId",
                table: "FetchItemValue",
                column: "FetchItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FetchItemValue");

            migrationBuilder.DropTable(
                name: "FetchItem");

            migrationBuilder.DropTable(
                name: "FetchUnits");
        }
    }
}
