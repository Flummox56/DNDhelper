using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SheetService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_sheets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    character_name = table.Column<string>(type: "text", maxLength: 100, nullable: false),
                    race = table.Column<string>(type: "text", maxLength: 50, nullable: false),
                    @class = table.Column<string>(name: "class", type: "text", maxLength: 50, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    strength = table.Column<int>(type: "integer", nullable: false),
                    dexterity = table.Column<int>(type: "integer", nullable: false),
                    constitution = table.Column<int>(type: "integer", nullable: false),
                    intelligence = table.Column<int>(type: "integer", nullable: false),
                    wisdom = table.Column<int>(type: "integer", nullable: false),
                    charisma = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    skills = table.Column<string>(type: "jsonb", nullable: true),
                    inventory = table.Column<string>(type: "jsonb", nullable: true),
                    spells = table.Column<string>(type: "jsonb", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_sheets", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_character_sheets_user_id",
                table: "character_sheets",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_sheets");
        }
    }
}
