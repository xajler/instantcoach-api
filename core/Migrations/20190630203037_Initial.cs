using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstantCoaches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    Status = table.Column<byte>(nullable: false, defaultValue: (byte)1),
                    TicketId = table.Column<string>(maxLength: 64, nullable: false),
                    Reference = table.Column<string>(maxLength: 16, nullable: false),
                    EvaluatorId = table.Column<int>(nullable: false),
                    AgentId = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    CommentsCount = table.Column<int>(nullable: false),
                    BookmarkPins = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantCoaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstantCoaches_Users_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstantCoaches_Users_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstantCoaches_AgentId",
                table: "InstantCoaches",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantCoaches_EvaluatorId",
                table: "InstantCoaches",
                column: "EvaluatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstantCoaches");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
