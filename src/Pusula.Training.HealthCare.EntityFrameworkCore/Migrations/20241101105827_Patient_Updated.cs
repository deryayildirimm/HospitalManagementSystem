using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Training.HealthCare.Migrations
{
    /// <inheritdoc />
    public partial class Patient_Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MobilePhoneNumber",
                table: "AppPatients",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AppPatients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountGroup",
                table: "AppPatients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FathersName",
                table: "AppPatients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceNo",
                table: "AppPatients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "InsuranceType",
                table: "AppPatients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MothersName",
                table: "AppPatients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Nationality",
                table: "AppPatients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "AppPatients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientNumber",
                table: "AppPatients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientType",
                table: "AppPatients",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "DiscountGroup",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "FathersName",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "InsuranceNo",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "InsuranceType",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "MothersName",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PatientNumber",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PatientType",
                table: "AppPatients");

            migrationBuilder.AlterColumn<string>(
                name: "MobilePhoneNumber",
                table: "AppPatients",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);
        }
    }
}
