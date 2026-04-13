using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KaymazLabs.AzureStorage.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredFileNameToFileInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "Files");
        }
    }
}
