using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class CollectionOfVideosAndProjectCreationTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Video_VideoNumber",
                table: "Projects");
            
            migrationBuilder.DropColumn(
                name: "Video_Reference",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Video_Script",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Video_Status",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Video_VideoNumber",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Video_VideoUri",
                table: "Projects");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAt",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "RelatedVideoNumber",
                table: "ProjectComments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Script = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false),
                    Reference = table.Column<string>(type: "TEXT", nullable: false),
                    VideoUri = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "nvarchar(16)", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Video_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Video_ProjectId",
                table: "Video",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Video_VideoNumber",
                table: "Video",
                column: "VideoNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Video");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RelatedVideoNumber",
                table: "ProjectComments");

            migrationBuilder.AddColumn<string>(
                name: "Video_Reference",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_Script",
                table: "Projects",
                type: "TEXT",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Video_Status",
                table: "Projects",
                type: "nvarchar(16)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Video_VideoNumber",
                table: "Projects",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video_VideoUri",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Video_VideoNumber",
                table: "Projects",
                column: "Video_VideoNumber",
                unique: true);
        }
    }
}
