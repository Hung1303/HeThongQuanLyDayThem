using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class updateCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CenterProfiles_CenterProfileId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_TeacherProfiles_TeacherProfileId",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ClassStatus",
                table: "Courses",
                column: "ClassStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Grade_Subject",
                table: "Courses",
                columns: new[] { "Grade", "Subject" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Subject",
                table: "Courses",
                column: "Subject");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CenterProfiles_CenterProfileId",
                table: "Courses",
                column: "CenterProfileId",
                principalTable: "CenterProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_TeacherProfiles_TeacherProfileId",
                table: "Courses",
                column: "TeacherProfileId",
                principalTable: "TeacherProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CenterProfiles_CenterProfileId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_TeacherProfiles_TeacherProfileId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ClassStatus",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Grade_Subject",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Subject",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CenterProfiles_CenterProfileId",
                table: "Courses",
                column: "CenterProfileId",
                principalTable: "CenterProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_TeacherProfiles_TeacherProfileId",
                table: "Courses",
                column: "TeacherProfileId",
                principalTable: "TeacherProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
