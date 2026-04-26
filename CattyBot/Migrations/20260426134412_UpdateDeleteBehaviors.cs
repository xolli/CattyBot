using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CattyBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SystemPrompts_SystemPromptId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseConfigs_SystemPrompts_SystemPromptId",
                table: "ResponseConfigs");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SystemPrompts_SystemPromptId",
                table: "Messages",
                column: "SystemPromptId",
                principalTable: "SystemPrompts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseConfigs_SystemPrompts_SystemPromptId",
                table: "ResponseConfigs",
                column: "SystemPromptId",
                principalTable: "SystemPrompts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SystemPrompts_SystemPromptId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseConfigs_SystemPrompts_SystemPromptId",
                table: "ResponseConfigs");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SystemPrompts_SystemPromptId",
                table: "Messages",
                column: "SystemPromptId",
                principalTable: "SystemPrompts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseConfigs_SystemPrompts_SystemPromptId",
                table: "ResponseConfigs",
                column: "SystemPromptId",
                principalTable: "SystemPrompts",
                principalColumn: "Id");
        }
    }
}
