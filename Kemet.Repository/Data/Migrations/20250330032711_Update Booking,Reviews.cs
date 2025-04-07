using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class UpdateBookingReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookedTripsCustomer");

            migrationBuilder.DropColumn(
                name: "BookedPrice",
                table: "BookedTrips");

            migrationBuilder.AddColumn<string>(
                name: "PlanLocation",
                table: "TravelAgencyPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerID",
                table: "BookedTrips",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BookedCategory",
                table: "BookedTrips",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BookedTrips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_BookedTrips_CustomerID",
                table: "BookedTrips",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedTrips_AspNetUsers_CustomerID",
                table: "BookedTrips",
                column: "CustomerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookedTrips_AspNetUsers_CustomerID",
                table: "BookedTrips");

            migrationBuilder.DropIndex(
                name: "IX_BookedTrips_CustomerID",
                table: "BookedTrips");

            migrationBuilder.DropColumn(
                name: "PlanLocation",
                table: "TravelAgencyPlans");

            migrationBuilder.DropColumn(
                name: "BookedCategory",
                table: "BookedTrips");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BookedTrips");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerID",
                table: "BookedTrips",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<decimal>(
                name: "BookedPrice",
                table: "BookedTrips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "BookedTripsCustomer",
                columns: table => new
                {
                    BookedTripsId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedTripsCustomer", x => new { x.BookedTripsId, x.CustomerId });
                    table.ForeignKey(
                        name: "FK_BookedTripsCustomer_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookedTripsCustomer_BookedTrips_BookedTripsId",
                        column: x => x.BookedTripsId,
                        principalTable: "BookedTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedTripsCustomer_CustomerId",
                table: "BookedTripsCustomer",
                column: "CustomerId");
        }
    }
}
