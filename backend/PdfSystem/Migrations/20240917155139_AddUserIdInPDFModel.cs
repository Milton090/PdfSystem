using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdInPDFModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Pdfs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Pdfs");
        }
    }
}
