using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixPfpName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrpUrl",
                table: "Users",
                newName: "PfpUrl");

            migrationBuilder.RenameColumn(
                name: "PrpPublicId",
                table: "Users",
                newName: "PfpPublicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PfpUrl",
                table: "Users",
                newName: "PrpUrl");

            migrationBuilder.RenameColumn(
                name: "PfpPublicId",
                table: "Users",
                newName: "PrpPublicId");
        }
    }
}
