using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeUniform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClientPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ReviewText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPhotos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientPhotos_IsActive_SortOrder",
                table: "ClientPhotos",
                columns: new[] { "IsActive", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientPhotos");
        }
    }
}
