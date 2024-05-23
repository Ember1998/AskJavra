using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskJavra.Migrations
{
    /// <inheritdoc />
    public partial class TypeID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContributionPointId",
                table: "ContributionPoints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContributionPointId",
                table: "ContributionPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
