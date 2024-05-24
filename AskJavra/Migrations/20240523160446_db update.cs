using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskJavra.Migrations
{
    /// <inheritdoc />
    public partial class dbupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionPoints_AspNetUsers_UserId",
                table: "ContributionPoints");

            migrationBuilder.DropIndex(
                name: "IX_ContributionPoints_UserId",
                table: "ContributionPoints");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ContributionPoints",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ContributionPoints",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionPoints_UserId",
                table: "ContributionPoints",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionPoints_AspNetUsers_UserId",
                table: "ContributionPoints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
