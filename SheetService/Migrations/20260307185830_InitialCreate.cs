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
                name: "monsters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    max_hp = table.Column<int>(type: "integer", nullable: false),
                    ac = table.Column<int>(type: "integer", nullable: false),
                    str = table.Column<int>(type: "integer", nullable: false),
                    dex = table.Column<int>(type: "integer", nullable: false),
                    con = table.Column<int>(type: "integer", nullable: false),
                    @int = table.Column<int>(name: "int", type: "integer", nullable: false),
                    wis = table.Column<int>(type: "integer", nullable: false),
                    cha = table.Column<int>(type: "integer", nullable: false),
                    danger = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monsters", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_monsters_created_by",
                table: "monsters",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "idx_monsters_status",
                table: "monsters",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "monsters");
        }
    }
}
