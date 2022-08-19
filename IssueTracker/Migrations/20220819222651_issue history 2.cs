using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Migrations
{
    public partial class issuehistory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssuesHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueId = table.Column<int>(type: "int", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AssignedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCreatorUpdated = table.Column<bool>(type: "bit", nullable: false),
                    IsAssignedUserUpdated = table.Column<bool>(type: "bit", nullable: false),
                    IsDescUpdated = table.Column<bool>(type: "bit", nullable: false),
                    IsStatusUpdated = table.Column<bool>(type: "bit", nullable: false),
                    IsPriorityUpdated = table.Column<bool>(type: "bit", nullable: false),
                    IsTypeUpdated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuesHistory_AspNetUsers_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IssuesHistory_AspNetUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IssuesHistory_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssuesHistory_AssignedUserId",
                table: "IssuesHistory",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuesHistory_CreatorUserId",
                table: "IssuesHistory",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuesHistory_IssueId",
                table: "IssuesHistory",
                column: "IssueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuesHistory");
        }
    }
}
