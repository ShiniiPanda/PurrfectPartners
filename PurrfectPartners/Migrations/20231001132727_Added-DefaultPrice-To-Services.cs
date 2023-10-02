using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurrfectPartners.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultPriceToServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DefaultPrice",
                table: "Services",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPrice",
                table: "Services");
        }
    }
}
