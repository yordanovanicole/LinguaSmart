using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaSmart.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFavourites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Words");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Words",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
