using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class AddtravelAgencynametobooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TravelAgencyName",
                table: "BookedTrips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TravelAgencyName",
                table: "BookedTrips");
        }
    }
}
