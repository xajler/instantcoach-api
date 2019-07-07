using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ic_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "InstantCoaches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    TicketId = table.Column<string>(maxLength: 64, nullable: false),
                    Reference = table.Column<string>(maxLength: 16, nullable: false),
                    EvaluatorId = table.Column<int>(nullable: false),
                    AgentId = table.Column<int>(nullable: false),
                    EvaluatorName = table.Column<string>(maxLength: 128, nullable: false),
                    AgentName = table.Column<string>(maxLength: 128, nullable: false),
                    Comments = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    BookmarkPins = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    CommentsCount = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantCoaches", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstantCoaches");

            migrationBuilder.DropSequence(
                name: "ic_hilo");
        }
    }
}
