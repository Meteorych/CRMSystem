using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicesToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Projects",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProjectNumber",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectNumber",
                table: "Projects",
                column: "ProjectNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Video_VideoNumber",
                table: "Projects",
                column: "Video_VideoNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectNumber",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Video_VideoNumber",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "Projects");
        }
    }
}
