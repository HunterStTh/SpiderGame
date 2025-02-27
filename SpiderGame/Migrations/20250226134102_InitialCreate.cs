using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiderGame.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spiders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    Armor = table.Column<int>(type: "INTEGER", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spiders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spiders");
        }
    }
}
