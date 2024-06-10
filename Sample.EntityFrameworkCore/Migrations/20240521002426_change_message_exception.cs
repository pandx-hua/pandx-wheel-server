using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class change_message_exception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "JobExecutions",
                newName: "Exception");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Exception",
                table: "JobExecutions",
                newName: "Message");
        }
    }
}
