using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kemet.Repository.Data.Migrations
{
    public partial class Editreview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "USERNAME",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserImageURl",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "USERNAME",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserImageURl",
                table: "Reviews");
        }
    }
}
