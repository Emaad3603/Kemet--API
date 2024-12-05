using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class Addwishlist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WishlistID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WishlistActivites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistID = table.Column<int>(type: "int", nullable: false),
                    ActivityID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistActivites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistActivites_Activities_ActivityID",
                        column: x => x.ActivityID,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishlistActivites_Wishlists_WishlistID",
                        column: x => x.WishlistID,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlistPlaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistID = table.Column<int>(type: "int", nullable: false),
                    PlaceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistPlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistPlaces_Places_PlaceID",
                        column: x => x.PlaceID,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishlistPlaces_Wishlists_WishlistID",
                        column: x => x.WishlistID,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlistPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistID = table.Column<int>(type: "int", nullable: false),
                    TravelAgencyPlanID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistPlans_TravelAgencyPlans_TravelAgencyPlanID",
                        column: x => x.TravelAgencyPlanID,
                        principalTable: "TravelAgencyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishlistPlans_Wishlists_WishlistID",
                        column: x => x.WishlistID,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WishlistID",
                table: "AspNetUsers",
                column: "WishlistID",
                unique: true,
                filter: "[WishlistID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistActivites_ActivityID",
                table: "WishlistActivites",
                column: "ActivityID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistActivites_WishlistID",
                table: "WishlistActivites",
                column: "WishlistID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistPlaces_PlaceID",
                table: "WishlistPlaces",
                column: "PlaceID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistPlaces_WishlistID",
                table: "WishlistPlaces",
                column: "WishlistID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistPlans_TravelAgencyPlanID",
                table: "WishlistPlans",
                column: "TravelAgencyPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistPlans_WishlistID",
                table: "WishlistPlans",
                column: "WishlistID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Wishlists_WishlistID",
                table: "AspNetUsers",
                column: "WishlistID",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Wishlists_WishlistID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "WishlistActivites");

            migrationBuilder.DropTable(
                name: "WishlistPlaces");

            migrationBuilder.DropTable(
                name: "WishlistPlans");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WishlistID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WishlistID",
                table: "AspNetUsers");
        }
    }
}
