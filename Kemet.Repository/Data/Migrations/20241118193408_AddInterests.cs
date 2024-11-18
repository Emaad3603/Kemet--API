using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class AddInterests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Categories_CategoryId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Locations_LocationId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Locations_LocationId1",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Places_PlaceId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Places_PlaceId1",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_Locations_locationId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Places_locationId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Activities_LocationId1",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_PlaceId1",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "LocationId1",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "PlaceId1",
                table: "Activities");

            migrationBuilder.AlterColumn<int>(
                name: "locationId",
                table: "Places",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CategoryType",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "Activities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Activities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Activities",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerInterests",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInterests", x => new { x.CustomerId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CustomerInterests_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerInterests_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Places_locationId",
                table: "Places",

                column: "locationId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInterests_CategoryId",
                table: "CustomerInterests",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Categories_CategoryId",
                table: "Activities",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Locations_LocationId",
                table: "Activities",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Places_PlaceId",
                table: "Activities",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Locations_locationId",
                table: "Places",
                column: "locationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Categories_CategoryId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Locations_LocationId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Places_PlaceId",
                
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_Locations_locationId",
                table: "Places");

            migrationBuilder.DropTable(
                name: "CustomerInterests");

            migrationBuilder.DropIndex(
                name: "IX_Places_locationId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "locationId",
                table: "Places",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Activities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LocationId1",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlaceId1",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Places_locationId",
                table: "Places",
                column: "locationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_LocationId1",
                table: "Activities",
                column: "LocationId1",
                unique: true,
                filter: "[LocationId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PlaceId1",
                table: "Activities",
                column: "PlaceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Categories_CategoryId",
                table: "Activities",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Locations_LocationId",
                table: "Activities",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Locations_LocationId1",
                table: "Activities",
                column: "LocationId1",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Places_PlaceId",
                table: "Activities",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Places_PlaceId1",
                table: "Activities",
                column: "PlaceId1",
                principalTable: "Places",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Locations_locationId",
                table: "Places",
                column: "locationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
