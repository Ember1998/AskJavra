using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskJavra.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ContributionRanks");

            migrationBuilder.AddColumn<int>(
                name: "RankMaxPoint",
                table: "ContributionRanks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RankMaxPoint",
                table: "ContributionRanks");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ContributionRanks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
