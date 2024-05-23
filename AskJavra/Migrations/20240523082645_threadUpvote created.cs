using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskJavra.Migrations
{
    /// <inheritdoc />
    public partial class threadUpvotecreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThreadUpVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadUpVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreadUpVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadUpVotes_PostThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "PostThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThreadUpVotes_ThreadId",
                table: "ThreadUpVotes",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadUpVotes_UserId",
                table: "ThreadUpVotes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreadUpVotes");
        }
    }
}
