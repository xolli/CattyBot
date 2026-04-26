using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CattyBot.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemPromptRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelsAnalytics_SystemPrompts_SystemPromptId",
                table: "ModelsAnalytics");

            migrationBuilder.DropIndex(
                name: "IX_ModelsAnalytics_SystemPromptId",
                table: "ModelsAnalytics");

            migrationBuilder.DropColumn(
                name: "SystemPromptId",
                table: "ModelsAnalytics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SystemPromptId",
                table: "ModelsAnalytics",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelsAnalytics_SystemPromptId",
                table: "ModelsAnalytics",
                column: "SystemPromptId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelsAnalytics_SystemPrompts_SystemPromptId",
                table: "ModelsAnalytics",
                column: "SystemPromptId",
                principalTable: "SystemPrompts",
                principalColumn: "Id");
        }
    }
}
