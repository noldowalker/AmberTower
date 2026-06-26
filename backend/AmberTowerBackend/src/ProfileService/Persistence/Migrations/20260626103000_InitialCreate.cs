using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileService.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "profile");

            migrationBuilder.CreateTable(
                name: "player_profiles",
                schema: "profile",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Nickname = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_profiles", x => x.PlayerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_profiles_AuthUserId",
                schema: "profile",
                table: "player_profiles",
                column: "AuthUserId",
                unique: true);

            migrationBuilder.Sql("""
                UPDATE profile.player_profiles
                SET "Nickname" = split_part("Email", '@', 1),
                    "UpdatedAtUtc" = NOW()
                WHERE "Nickname" IS NULL OR btrim("Nickname") = '';
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_profiles",
                schema: "profile");
        }
    }
}
