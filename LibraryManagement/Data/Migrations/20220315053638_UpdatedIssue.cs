using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Data.Migrations
{
    public partial class UpdatedIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BooksPartialId",
                table: "Issues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_BooksPartialId",
                table: "Issues",
                column: "BooksPartialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_AllBooks_BooksPartialId",
                table: "Issues",
                column: "BooksPartialId",
                principalTable: "AllBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_AllBooks_BooksPartialId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_BooksPartialId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "BooksPartialId",
                table: "Issues");
        }
    }
}
