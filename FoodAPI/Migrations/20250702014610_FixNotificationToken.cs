using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixNotificationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationToken_Users_UserId",
                table: "NotificationToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationToken",
                table: "NotificationToken");

            migrationBuilder.RenameTable(
                name: "NotificationToken",
                newName: "NotificationTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationTokens",
                table: "NotificationTokens",
                columns: new[] { "UserId", "DeviceToken" });

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTokens_Users_UserId",
                table: "NotificationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationTokens_Users_UserId",
                table: "NotificationTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationTokens",
                table: "NotificationTokens");

            migrationBuilder.RenameTable(
                name: "NotificationTokens",
                newName: "NotificationToken");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationToken",
                table: "NotificationToken",
                columns: new[] { "UserId", "DeviceToken" });

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationToken_Users_UserId",
                table: "NotificationToken",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
