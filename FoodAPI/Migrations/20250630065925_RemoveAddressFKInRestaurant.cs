using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressFKInRestaurant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_ShippingInfoId",
                table: "Ratings");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ShippingInfoId",
                table: "Ratings",
                column: "ShippingInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_ShippingInfoId",
                table: "Ratings");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ShippingInfoId",
                table: "Ratings",
                column: "ShippingInfoId",
                unique: true);
        }
    }
}
