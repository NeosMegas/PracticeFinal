using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeFinal.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBlogItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "MusketeerBlogItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "MusketeerBlogItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "MusketeerBlogItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LargeImage",
                table: "MusketeerBlogItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "MusketeerBlogItems");

            migrationBuilder.DropColumn(
                name: "Header",
                table: "MusketeerBlogItems");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "MusketeerBlogItems");

            migrationBuilder.DropColumn(
                name: "LargeImage",
                table: "MusketeerBlogItems");
        }
    }
}
