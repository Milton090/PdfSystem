using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePdfModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Pdfs");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Pdfs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Pdfs");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Pdfs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
