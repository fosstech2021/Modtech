using Microsoft.EntityFrameworkCore.Migrations;

namespace BasePackageModule2.Migrations
{
    public partial class AdvancedMenuSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisplayAsADropdownMenu",
                table: "Menus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisplayAsAProductList",
                table: "Menus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomeScreen",
                table: "Menus",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayAsADropdownMenu",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "DisplayAsAProductList",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ShowOnHomeScreen",
                table: "Menus");
        }
    }
}
