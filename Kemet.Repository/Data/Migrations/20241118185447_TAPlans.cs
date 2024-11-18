﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class TAPlans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TravelAgencyPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanAvailability = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TravelAgencyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TravelAgencyId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelAgencyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelAgencyPlans_AspNetUsers_TravelAgencyId",
                        column: x => x.TravelAgencyId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelAgencyPlans_AspNetUsers_TravelAgencyId1",
                        column: x => x.TravelAgencyId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelAgencyPlans_TravelAgencyId",
                table: "TravelAgencyPlans",
                column: "TravelAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelAgencyPlans_TravelAgencyId1",
                table: "TravelAgencyPlans",
                column: "TravelAgencyId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelAgencyPlans");
        }
    }
}
