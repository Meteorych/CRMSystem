using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class ChangePropertiesOfTechTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechTask_TechTaskUrl",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "TechTask_FileName",
                table: "Projects",
                type: "TEXT",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechTask_FileName",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "TechTask_TechTaskUrl",
                table: "Projects",
                type: "TEXT",
                nullable: true);
        }
    }
}
