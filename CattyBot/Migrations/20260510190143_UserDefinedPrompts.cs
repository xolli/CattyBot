using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CattyBot.Migrations
{
    /// <inheritdoc />
    public partial class UserDefinedPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "SystemPrompts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "SystemPrompts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "SystemPrompts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SystemPrompts");
        }
    }
}
