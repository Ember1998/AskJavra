using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskJavra.Migrations
{
    /// <inheritdoc />
    public partial class Contributionentityadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContributionPointTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Point = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionPointTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContributionRanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RankMinPoint = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionRanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContributionPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ContributionPointId = table.Column<int>(type: "int", nullable: false),
                    ContributionPointTypeId = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContributionPoints_ContributionPointTypes_ContributionPointTypeId",
                        column: x => x.ContributionPointTypeId,
                        principalTable: "ContributionPointTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContributionPoints_ContributionPointTypeId",
                table: "ContributionPoints",
                column: "ContributionPointTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContributionPoints");

            migrationBuilder.DropTable(
                name: "ContributionRanks");

            migrationBuilder.DropTable(
                name: "ContributionPointTypes");
        }
    }
}
