using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class add_login_attemptes4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MimeType",
                table: "BinaryObjects",
                newName: "ContentType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "BinaryObjects",
                newName: "MimeType");
        }
    }
}
