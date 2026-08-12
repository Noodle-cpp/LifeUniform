using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeUniform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductColorSizeStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductColorSizeStocks",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsInStock = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColorSizeStocks", x => new { x.ProductId, x.ColorName, x.SizeId });
                    table.ForeignKey(
                        name: "FK_ProductColorSizeStocks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductColorSizeStocks_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductColorSizeStocks_ProductId_ColorName",
                table: "ProductColorSizeStocks",
                columns: new[] { "ProductId", "ColorName" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductColorSizeStocks_SizeId",
                table: "ProductColorSizeStocks",
                column: "SizeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductColorSizeStocks");
        }
    }
}
