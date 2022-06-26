using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Migrations
{
    public partial class revertlastchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProjects_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProjects_Projects_ProjectId",
                table: "ApplicationUserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Projects_ProjectId",
                table: "Issues");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProjects_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserProjects",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProjects_Projects_ProjectId",
                table: "ApplicationUserProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Projects_ProjectId",
                table: "Issues",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProjects_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserProjects_Projects_ProjectId",
                table: "ApplicationUserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Projects_ProjectId",
                table: "Issues");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProjects_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserProjects",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserProjects_Projects_ProjectId",
                table: "ApplicationUserProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Projects_ProjectId",
                table: "Issues",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
