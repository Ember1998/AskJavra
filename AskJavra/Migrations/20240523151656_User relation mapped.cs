using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskJavra.Migrations
{
    /// <inheritdoc />
    public partial class Userrelationmapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ContributionPoints",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContributionPoints_AspNetUsers_UserId",
                table: "ContributionPoints");

            migrationBuilder.DropIndex(
                name: "IX_ContributionPoints_UserId",
                table: "ContributionPoints");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ContributionPoints",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
