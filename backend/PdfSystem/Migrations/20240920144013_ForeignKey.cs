using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfSystem.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Pdfs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pdfs_UserId",
                table: "Pdfs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pdfs_AspNetUsers_UserId",
                table: "Pdfs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pdfs_AspNetUsers_UserId",
                table: "Pdfs");

            migrationBuilder.DropIndex(
                name: "IX_Pdfs_UserId",
                table: "Pdfs");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Pdfs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
