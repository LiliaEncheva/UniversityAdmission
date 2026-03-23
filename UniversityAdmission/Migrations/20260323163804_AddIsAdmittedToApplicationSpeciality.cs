using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityAdmission.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAdmittedToApplicationSpeciality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmitted",
                table: "ApplicationSpecialities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmitted",
                table: "ApplicationSpecialities");
        }
    }
}
