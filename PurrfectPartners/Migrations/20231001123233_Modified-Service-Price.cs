using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurrfectPartners.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedServicePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "Services");

            migrationBuilder.AddColumn<double>(
                name: "StartingPrice",
                table: "AnimalServices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "AnimalServices");

            migrationBuilder.AddColumn<double>(
                name: "StartingPrice",
                table: "Services",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
