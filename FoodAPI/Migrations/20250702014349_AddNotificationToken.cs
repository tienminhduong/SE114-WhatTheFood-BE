using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationToken",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationToken", x => new { x.UserId, x.DeviceToken });
                    table.ForeignKey(
                        name: "FK_NotificationToken_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationToken");
        }
    }
}
