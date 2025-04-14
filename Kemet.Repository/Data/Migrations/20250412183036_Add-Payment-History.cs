using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class AddPaymentHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BookedPrice",
                table: "BookedTrips",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "BookedTrips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "BookedTrips",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentId",
                table: "BookedTrips",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookedTripsId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    StripePaymentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StripeEventId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_BookedTrips_BookedTripsId",
                        column: x => x.BookedTripsId,
                        principalTable: "BookedTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_BookedTripsId",
                table: "PaymentHistories",
                column: "BookedTripsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "BookedPrice",
                table: "BookedTrips");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "BookedTrips");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "BookedTrips");

            migrationBuilder.DropColumn(
                name: "StripePaymentId",
                table: "BookedTrips");
        }
    }
}
