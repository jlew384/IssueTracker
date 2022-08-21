using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracker.Migrations
{
    public partial class updatedbyissuehistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "IssuesHistory",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_IssuesHistory_UpdatedByUserId",
                table: "IssuesHistory",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IssuesHistory_AspNetUsers_UpdatedByUserId",
                table: "IssuesHistory",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssuesHistory_AspNetUsers_UpdatedByUserId",
                table: "IssuesHistory");

            migrationBuilder.DropIndex(
                name: "IX_IssuesHistory_UpdatedByUserId",
                table: "IssuesHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "IssuesHistory");
        }
    }
}
