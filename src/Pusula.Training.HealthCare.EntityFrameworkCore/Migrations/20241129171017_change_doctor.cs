using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Training.HealthCare.Migrations
{
    /// <inheritdoc />
    public partial class change_doctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDoctors_AppDepartments_DepartmentId1",
                table: "AppDoctors");

            migrationBuilder.DropForeignKey(
                name: "FK_AppDoctors_AppTitles_TitleId1",
                table: "AppDoctors");

            migrationBuilder.DropIndex(
                name: "IX_AppDoctors_DepartmentId1",
                table: "AppDoctors");

            migrationBuilder.DropIndex(
                name: "IX_AppDoctors_TitleId1",
                table: "AppDoctors");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "AppDoctors");

            migrationBuilder.DropColumn(
                name: "TitleId1",
                table: "AppDoctors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId1",
                table: "AppDoctors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TitleId1",
                table: "AppDoctors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppDoctors_DepartmentId1",
                table: "AppDoctors",
                column: "DepartmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppDoctors_TitleId1",
                table: "AppDoctors",
                column: "TitleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDoctors_AppDepartments_DepartmentId1",
                table: "AppDoctors",
                column: "DepartmentId1",
                principalTable: "AppDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppDoctors_AppTitles_TitleId1",
                table: "AppDoctors",
                column: "TitleId1",
                principalTable: "AppTitles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
