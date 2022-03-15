using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Data.Migrations
{
    public partial class UpdatedIssue2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AllBooks_BooksPartialId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_BooksPartialId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BooksPartialId",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "BookBooksPartial",
                columns: table => new
                {
                    BooksBookId = table.Column<int>(type: "int", nullable: false),
                    BooksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookBooksPartial", x => new { x.BooksBookId, x.BooksId });
                    table.ForeignKey(
                        name: "FK_BookBooksPartial_AllBooks_BooksId",
                        column: x => x.BooksId,
                        principalTable: "AllBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookBooksPartial_Books_BooksBookId",
                        column: x => x.BooksBookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookBooksPartial_BooksId",
                table: "BookBooksPartial",
                column: "BooksId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookBooksPartial");

            migrationBuilder.AddColumn<int>(
                name: "BooksPartialId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_BooksPartialId",
                table: "Books",
                column: "BooksPartialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AllBooks_BooksPartialId",
                table: "Books",
                column: "BooksPartialId",
                principalTable: "AllBooks",
                principalColumn: "Id");
        }
    }
}
