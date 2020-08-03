using Microsoft.EntityFrameworkCore.Migrations;

namespace Ask_Clone.Migrations
{
    public partial class ModifyingDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionFromUserame",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionToUsername",
                table: "Questions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionFromUserame",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionToUsername",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
