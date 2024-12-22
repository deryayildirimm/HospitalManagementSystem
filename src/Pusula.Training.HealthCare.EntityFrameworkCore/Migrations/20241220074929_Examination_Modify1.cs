using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Training.HealthCare.Migrations
{
    /// <inheritdoc />
    public partial class Examination_Modify1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppExaminationIcds_AppExaminations_ExaminationId",
                table: "AppExaminationIcds");

            migrationBuilder.DropForeignKey(
                name: "FK_AppExaminationIcds_AppIcds_IcdId",
                table: "AppExaminationIcds");

            migrationBuilder.AddForeignKey(
                name: "FK_AppExaminationIcds_AppExaminations_ExaminationId",
                table: "AppExaminationIcds",
                column: "ExaminationId",
                principalTable: "AppExaminations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppExaminationIcds_AppIcds_IcdId",
                table: "AppExaminationIcds",
                column: "IcdId",
                principalTable: "AppIcds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppExaminationIcds_AppExaminations_ExaminationId",
                table: "AppExaminationIcds");

            migrationBuilder.DropForeignKey(
                name: "FK_AppExaminationIcds_AppIcds_IcdId",
                table: "AppExaminationIcds");

            migrationBuilder.AddForeignKey(
                name: "FK_AppExaminationIcds_AppExaminations_ExaminationId",
                table: "AppExaminationIcds",
                column: "ExaminationId",
                principalTable: "AppExaminations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppExaminationIcds_AppIcds_IcdId",
                table: "AppExaminationIcds",
                column: "IcdId",
                principalTable: "AppIcds",
                principalColumn: "Id");
        }
    }
}
