using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreRelationshipsPractice.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Students");
        }
    }
}
