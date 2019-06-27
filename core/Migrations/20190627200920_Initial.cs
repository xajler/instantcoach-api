using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "test");

            migrationBuilder.CreateTable(
                name: "Templates",
                schema: "test",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "test",
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
                schema: "test",
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
                    TemplateId = table.Column<int>(nullable: false),
                    QuestionComments = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    QuestionCount = table.Column<int>(nullable: false),
                    BookmarkPins = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantCoaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstantCoaches_Users_AgentId",
                        column: x => x.AgentId,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstantCoaches_Users_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalSchema: "test",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstantCoaches_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "test",
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstantCoaches_AgentId",
                schema: "test",
                table: "InstantCoaches",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantCoaches_EvaluatorId",
                schema: "test",
                table: "InstantCoaches",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_InstantCoaches_TemplateId",
                schema: "test",
                table: "InstantCoaches",
                column: "TemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstantCoaches",
                schema: "test");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "test");

            migrationBuilder.DropTable(
                name: "Templates",
                schema: "test");
        }
    }
}
