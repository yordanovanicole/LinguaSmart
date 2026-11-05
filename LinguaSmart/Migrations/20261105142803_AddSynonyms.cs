using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaSmart.Migrations
{
    /// <inheritdoc />
    public partial class AddSynonyms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Synonyms",
                table: "Words",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Synonyms",
                table: "Words");
        }
    }
}
