using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class UpdatePriceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Places_priceId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Activities_priceId",
                table: "Activities");

            migrationBuilder.AlterColumn<int>(
                name: "priceId",
                table: "Places",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "priceId",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Places_priceId",
                table: "Places",
                column: "priceId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_priceId",
                table: "Activities",
                column: "priceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Places_priceId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Activities_priceId",
                table: "Activities");

            migrationBuilder.AlterColumn<int>(
                name: "priceId",
                table: "Places",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "priceId",
                table: "Activities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Places_priceId",
                table: "Places",
                column: "priceId",
                unique: true,
                filter: "[priceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_priceId",
                table: "Activities",
                column: "priceId",
                unique: true,
                filter: "[priceId] IS NOT NULL");
        }
    }
}
