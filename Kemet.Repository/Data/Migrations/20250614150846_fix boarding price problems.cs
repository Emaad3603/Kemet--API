using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class fixboardingpriceproblems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FullBoardPriceAddition",
                table: "TravelAgencyPlans",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HalfBoardPriceAddittion",
                table: "TravelAgencyPlans",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FullBookedPrice",
                table: "BookedTrips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullBoardPriceAddition",
                table: "TravelAgencyPlans");

            migrationBuilder.DropColumn(
                name: "HalfBoardPriceAddittion",
                table: "TravelAgencyPlans");

            migrationBuilder.DropColumn(
                name: "FullBookedPrice",
                table: "BookedTrips");
        }
    }
}
