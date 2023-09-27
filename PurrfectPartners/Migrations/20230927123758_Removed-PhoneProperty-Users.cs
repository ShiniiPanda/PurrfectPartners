using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurrfectPartners.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPhonePropertyUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                nullable: true);
        }
    }
}
