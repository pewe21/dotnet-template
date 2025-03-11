using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learnjwt.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdtoPublisher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Publishers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_UserID",
                table: "Publishers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Publishers_Users_UserID",
                table: "Publishers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publishers_Users_UserID",
                table: "Publishers");

            migrationBuilder.DropIndex(
                name: "IX_Publishers_UserID",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Publishers");
        }
    }
}
