using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Curriculums",
                columns: table => new
                {
                    CurriculumId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curriculums", x => x.CurriculumId);
                });

            migrationBuilder.CreateTable(
                name: "Enterprises",
                columns: table => new
                {
                    EnterpriseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprises", x => x.EnterpriseId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CohortCurriculums",
                columns: table => new
                {
                    Cohort = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurriculumId = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CohortCurriculums", x => new { x.Cohort, x.CurriculumId });
                    table.ForeignKey(
                        name: "FK_CohortCurriculums_Curriculums_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curriculums",
                        principalColumn: "CurriculumId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OJTPrograms",
                columns: table => new
                {
                    ProgramId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnterpriseId = table.Column<int>(type: "int", nullable: false),
                    ProgramName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OJTPrograms", x => x.ProgramId);
                    table.ForeignKey(
                        name: "FK_OJTPrograms_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "EnterpriseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentGrades",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CurriculumId = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false, computedColumnSql: "CASE WHEN Grade >= 5.0 THEN 1 ELSE 0 END")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGrades", x => new { x.UserId, x.CurriculumId });
                    table.ForeignKey(
                        name: "FK_StudentGrades_Curriculums_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curriculums",
                        principalColumn: "CurriculumId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGrades_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Cohort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Major = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalCredits = table.Column<int>(type: "int", nullable: false),
                    DebtCredits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OJTRegistrations",
                columns: table => new
                {
                    OJTId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    EnterpriseId = table.Column<int>(type: "int", nullable: false),
                    ProgramId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OJTRegistrations", x => x.OJTId);
                    table.ForeignKey(
                        name: "FK_OJTRegistrations_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "EnterpriseId");
                    table.ForeignKey(
                        name: "FK_OJTRegistrations_OJTPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "OJTPrograms",
                        principalColumn: "ProgramId");
                    table.ForeignKey(
                        name: "FK_OJTRegistrations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OJTFeedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OJTId = table.Column<int>(type: "int", nullable: false),
                    GivenBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OJTFeedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_OJTFeedbacks_OJTRegistrations_OJTId",
                        column: x => x.OJTId,
                        principalTable: "OJTRegistrations",
                        principalColumn: "OJTId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OJTResults",
                columns: table => new
                {
                    OJTId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OJTRegistrationOJTId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OJTResults", x => x.OJTId);
                    table.ForeignKey(
                        name: "FK_OJTResults_OJTRegistrations_OJTRegistrationOJTId",
                        column: x => x.OJTRegistrationOJTId,
                        principalTable: "OJTRegistrations",
                        principalColumn: "OJTId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CohortCurriculums_CurriculumId",
                table: "CohortCurriculums",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_OJTFeedbacks_OJTId",
                table: "OJTFeedbacks",
                column: "OJTId");

            migrationBuilder.CreateIndex(
                name: "IX_OJTPrograms_EnterpriseId",
                table: "OJTPrograms",
                column: "EnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_OJTRegistrations_EnterpriseId",
                table: "OJTRegistrations",
                column: "EnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_OJTRegistrations_ProgramId",
                table: "OJTRegistrations",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_OJTRegistrations_UserId",
                table: "OJTRegistrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OJTResults_OJTRegistrationOJTId",
                table: "OJTResults",
                column: "OJTRegistrationOJTId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGrades_CurriculumId",
                table: "StudentGrades",
                column: "CurriculumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CohortCurriculums");

            migrationBuilder.DropTable(
                name: "OJTFeedbacks");

            migrationBuilder.DropTable(
                name: "OJTResults");

            migrationBuilder.DropTable(
                name: "StudentGrades");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "OJTRegistrations");

            migrationBuilder.DropTable(
                name: "Curriculums");

            migrationBuilder.DropTable(
                name: "OJTPrograms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Enterprises");
        }
    }
}
