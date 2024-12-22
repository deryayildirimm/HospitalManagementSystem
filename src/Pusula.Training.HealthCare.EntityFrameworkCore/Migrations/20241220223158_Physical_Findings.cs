using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Training.HealthCare.Migrations
{
    /// <inheritdoc />
    public partial class Physical_Findings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPhysicalFindings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    BodyTemperature = table.Column<int>(type: "integer", nullable: true),
                    Pulse = table.Column<int>(type: "integer", nullable: true),
                    Vki = table.Column<int>(type: "integer", nullable: true),
                    Vya = table.Column<int>(type: "integer", nullable: true),
                    Kbs = table.Column<int>(type: "integer", nullable: true),
                    Kbd = table.Column<int>(type: "integer", nullable: true),
                    Spo2 = table.Column<int>(type: "integer", nullable: true),
                    ExaminationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPhysicalFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPhysicalFindings_AppExaminations_ExaminationId",
                        column: x => x.ExaminationId,
                        principalTable: "AppExaminations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPhysicalFindings_ExaminationId",
                table: "AppPhysicalFindings",
                column: "ExaminationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPhysicalFindings");
        }
    }
}
