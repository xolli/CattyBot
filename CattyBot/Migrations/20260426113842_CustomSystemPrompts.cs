using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CattyBot.Migrations
{
    /// <inheritdoc />
    public partial class CustomSystemPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "ResponseConfigs");

            migrationBuilder.DropColumn(
                name: "ChatMode",
                table: "ModelsAnalytics");

            migrationBuilder.DropColumn(
                name: "ChatMode",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "SystemPromptId",
                table: "ResponseConfigs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SystemPromptId",
                table: "ModelsAnalytics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SystemPromptId",
                table: "Messages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SystemPrompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPrompts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResponseConfigs_SystemPromptId",
                table: "ResponseConfigs",
                column: "SystemPromptId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelsAnalytics_SystemPromptId",
                table: "ModelsAnalytics",
                column: "SystemPromptId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SystemPromptId",
                table: "Messages",
                column: "SystemPromptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SystemPrompts_SystemPromptId",
                table: "Messages",
                column: "SystemPromptId",
                principalTable: "SystemPrompts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelsAnalytics_SystemPrompts_SystemPromptId",
                table: "ModelsAnalytics",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SystemPrompts_SystemPromptId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelsAnalytics_SystemPrompts_SystemPromptId",
                table: "ModelsAnalytics");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponseConfigs_SystemPrompts_SystemPromptId",
                table: "ResponseConfigs");

            migrationBuilder.DropTable(
                name: "SystemPrompts");

            migrationBuilder.DropIndex(
                name: "IX_ResponseConfigs_SystemPromptId",
                table: "ResponseConfigs");

            migrationBuilder.DropIndex(
                name: "IX_ModelsAnalytics_SystemPromptId",
                table: "ModelsAnalytics");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SystemPromptId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SystemPromptId",
                table: "ResponseConfigs");

            migrationBuilder.DropColumn(
                name: "SystemPromptId",
                table: "ModelsAnalytics");

            migrationBuilder.DropColumn(
                name: "SystemPromptId",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "ResponseConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChatMode",
                table: "ModelsAnalytics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChatMode",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
