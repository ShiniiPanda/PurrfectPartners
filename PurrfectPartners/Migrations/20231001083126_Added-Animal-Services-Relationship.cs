using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurrfectPartners.Migrations
{
    /// <inheritdoc />
    public partial class AddedAnimalServicesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalServices",
                columns: table => new
                {
                    AnimalId = table.Column<int>(type: "int", nullable: false),
                    TrainingServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalServices", x => new { x.AnimalId, x.TrainingServiceId });
                    table.ForeignKey(
                        name: "FK_AnimalServices_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalServices_Services_TrainingServiceId",
                        column: x => x.TrainingServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalServices_TrainingServiceId",
                table: "AnimalServices",
                column: "TrainingServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalServices");
        }
    }
}
