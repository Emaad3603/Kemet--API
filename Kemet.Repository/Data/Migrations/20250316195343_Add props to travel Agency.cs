using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class AddpropstotravelAgency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TravelAgencyID",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TravelAgency_Bio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TravelAgencyID",
                table: "Reviews",
                column: "TravelAgencyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_TravelAgencyID",
                table: "Reviews",
                column: "TravelAgencyID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_TravelAgencyID",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_TravelAgencyID",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TravelAgencyID",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TravelAgency_Bio",
                table: "AspNetUsers");
        }
    }
}
