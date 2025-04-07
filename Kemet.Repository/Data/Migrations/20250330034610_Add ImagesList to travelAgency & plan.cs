using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class AddImagesListtotravelAgencyplan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TravelAgencyImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageURl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TravelAgencyID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelAgencyImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelAgencyImages_AspNetUsers_TravelAgencyID",
                        column: x => x.TravelAgencyID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TravelAgencyPlanImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageURl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TravelAgencyPlanID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelAgencyPlanImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelAgencyPlanImages_TravelAgencyPlans_TravelAgencyPlanID",
                        column: x => x.TravelAgencyPlanID,
                        principalTable: "TravelAgencyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelAgencyImages_TravelAgencyID",
                table: "TravelAgencyImages",
                column: "TravelAgencyID");

            migrationBuilder.CreateIndex(
                name: "IX_TravelAgencyPlanImages_TravelAgencyPlanID",
                table: "TravelAgencyPlanImages",
                column: "TravelAgencyPlanID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelAgencyImages");

            migrationBuilder.DropTable(
                name: "TravelAgencyPlanImages");
        }
    }
}
