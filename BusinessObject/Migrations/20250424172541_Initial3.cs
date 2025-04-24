using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObject.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OJTRegistrations_Users_UserId",
                table: "OJTRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_OJTResults_OJTRegistrations_OJTRegistrationOJTId",
                table: "OJTResults");

            migrationBuilder.AlterColumn<string>(
                name: "MSSV",
                table: "Users",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "Grade",
                table: "StudentGrades",
                type: "decimal(4,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<int>(
                name: "OJTRegistrationOJTId",
                table: "OJTResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "OJTResults",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ResultId",
                table: "OJTResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "OJTRegistrations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OJTRegistrations",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RegistrationId",
                table: "OJTRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Requirements",
                table: "OJTPrograms",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "OJTPrograms",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "OJTPrograms",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OJTPrograms",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Enterprises",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ContactPhone",
                table: "Enterprises",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Enterprises",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "IsPassed",
                table: "StudentGrades",
                type: "int",
                nullable: true,
                computedColumnSql: "CASE WHEN Grade >= 5.0 THEN 1 ELSE 0 END",
                oldClrType: typeof(int),
                oldType: "int",
                oldComputedColumnSql: "CASE WHEN Grade >= 5.0 THEN 1 ELSE 0 END");

            migrationBuilder.AddForeignKey(
                name: "FK_OJTRegistrations_Users_UserId",
                table: "OJTRegistrations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OJTResults_OJTRegistrations_OJTRegistrationOJTId",
                table: "OJTResults",
                column: "OJTRegistrationOJTId",
                principalTable: "OJTRegistrations",
                principalColumn: "OJTId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OJTRegistrations_Users_UserId",
                table: "OJTRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_OJTResults_OJTRegistrations_OJTRegistrationOJTId",
                table: "OJTResults");

            migrationBuilder.DropColumn(
                name: "ResultId",
                table: "OJTResults");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "OJTRegistrations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OJTPrograms");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Enterprises");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "MSSV",
                keyValue: null,
                column: "MSSV",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "MSSV",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "Grade",
                table: "StudentGrades",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,2)");

            migrationBuilder.AlterColumn<int>(
                name: "OJTRegistrationOJTId",
                table: "OJTResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "OJTResults",
                keyColumn: "Comments",
                keyValue: null,
                column: "Comments",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "OJTResults",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "OJTRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "OJTRegistrations",
                keyColumn: "Status",
                keyValue: null,
                column: "Status",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OJTRegistrations",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "OJTPrograms",
                keyColumn: "Requirements",
                keyValue: null,
                column: "Requirements",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Requirements",
                table: "OJTPrograms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "OJTPrograms",
                keyColumn: "ProgramName",
                keyValue: null,
                column: "ProgramName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProgramName",
                table: "OJTPrograms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "OJTPrograms",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "OJTPrograms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Enterprises",
                keyColumn: "Name",
                keyValue: null,
                column: "Name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Enterprises",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Enterprises",
                keyColumn: "ContactPhone",
                keyValue: null,
                column: "ContactPhone",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ContactPhone",
                table: "Enterprises",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Enterprises",
                keyColumn: "ContactEmail",
                keyValue: null,
                column: "ContactEmail",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Enterprises",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "IsPassed",
                table: "StudentGrades",
                type: "int",
                nullable: false,
                computedColumnSql: "CASE WHEN Grade >= 5.0 THEN 1 ELSE 0 END",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComputedColumnSql: "CASE WHEN Grade >= 5.0 THEN 1 ELSE 0 END");

            migrationBuilder.AddForeignKey(
                name: "FK_OJTRegistrations_Users_UserId",
                table: "OJTRegistrations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OJTResults_OJTRegistrations_OJTRegistrationOJTId",
                table: "OJTResults",
                column: "OJTRegistrationOJTId",
                principalTable: "OJTRegistrations",
                principalColumn: "OJTId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
