using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace myLeave.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeEmailToLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeEmail",
                table: "Leaves",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeEmail",
                table: "Leaves");
        }
    }
}
