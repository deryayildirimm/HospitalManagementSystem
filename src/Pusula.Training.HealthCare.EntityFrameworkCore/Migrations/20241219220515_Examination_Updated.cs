using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Training.HealthCare.Migrations
{
    /// <inheritdoc />
    public partial class Examination_Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppBackgrounds_AppExaminations_ExaminationId",
                table: "AppBackgrounds");

            migrationBuilder.DropForeignKey(
                name: "FK_AppFamilyHistories_AppExaminations_ExaminationId",
                table: "AppFamilyHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_AppBackgrounds_AppExaminations_ExaminationId",
                table: "AppBackgrounds",
                column: "ExaminationId",
                principalTable: "AppExaminations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppFamilyHistories_AppExaminations_ExaminationId",
                table: "AppFamilyHistories",
                column: "ExaminationId",
                principalTable: "AppExaminations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppBackgrounds_AppExaminations_ExaminationId",
                table: "AppBackgrounds");

            migrationBuilder.DropForeignKey(
                name: "FK_AppFamilyHistories_AppExaminations_ExaminationId",
                table: "AppFamilyHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_AppBackgrounds_AppExaminations_ExaminationId",
                table: "AppBackgrounds",
                column: "ExaminationId",
                principalTable: "AppExaminations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppFamilyHistories_AppExaminations_ExaminationId",
                table: "AppFamilyHistories",
                column: "ExaminationId",
                principalTable: "AppExaminations",
                principalColumn: "Id");
        }
    }
}
