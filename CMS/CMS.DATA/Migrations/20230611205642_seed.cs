using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.DATA.Migrations
{
    public partial class seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizReviews_Quizs_QuizId",
                table: "QuizReviews");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "UserQuizTaken");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "QuizReviews");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "QuizReviews",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizReviews_QuizId",
                table: "QuizReviews",
                newName: "IX_QuizReviews_CourseId");

            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "UserQuizTaken",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSatisfied",
                table: "QuizReviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizReviews_Courses_CourseId",
                table: "QuizReviews",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizReviews_Courses_CourseId",
                table: "QuizReviews");

            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "UserQuizTaken");

            migrationBuilder.DropColumn(
                name: "IsSatisfied",
                table: "QuizReviews");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "QuizReviews",
                newName: "QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizReviews_CourseId",
                table: "QuizReviews",
                newName: "IX_QuizReviews_QuizId");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "UserQuizTaken",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "QuizReviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizReviews_Quizs_QuizId",
                table: "QuizReviews",
                column: "QuizId",
                principalTable: "Quizs",
                principalColumn: "Id");
        }
    }
}
