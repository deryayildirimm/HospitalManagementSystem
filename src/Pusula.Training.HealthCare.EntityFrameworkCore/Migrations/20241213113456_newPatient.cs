using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Training.HealthCare.Migrations
{
    /// <inheritdoc />
    public partial class newPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdentityAndPassportNumber",
                table: "AppPatients",
                newName: "IdentityNumber");

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "AppPatients",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "AppPatients");

            migrationBuilder.RenameColumn(
                name: "IdentityNumber",
                table: "AppPatients",
                newName: "IdentityAndPassportNumber");
        }
    }
}
