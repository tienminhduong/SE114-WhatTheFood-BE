using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantNFoodPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CldnrPublicId",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CldnrUrl",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CldnrPublicId",
                table: "FoodItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CldnrUrl",
                table: "FoodItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CldnrPublicId",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CldnrUrl",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CldnrPublicId",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "CldnrUrl",
                table: "FoodItems");
        }
    }
}
