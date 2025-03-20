    using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class SmallModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Video_VideoNumber",
                table: "Projects",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Video_Status",
                table: "Projects",
                type: "nvarchar(16)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "nvarchar(16)");

            migrationBuilder.AlterColumn<string>(
                name: "Video_Script",
                table: "Projects",
                type: "TEXT",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<string>(
                name: "Video_Reference",
                table: "Projects",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Video_VideoNumber",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Video_Status",
                table: "Projects",
                type: "nvarchar(16)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "nvarchar(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Video_Script",
                table: "Projects",
                type: "TEXT",
                maxLength: 10000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Video_Reference",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
