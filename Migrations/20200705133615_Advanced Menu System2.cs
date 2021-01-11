using Microsoft.EntityFrameworkCore.Migrations;

namespace BasePackageModule2.Migrations
{
    public partial class AdvancedMenuSystem2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayAsADropdownMenu",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "DisplayAsAProductList",
                table: "Menus");

            migrationBuilder.AddColumn<bool>(
                name: "DisplayAs",
                table: "Menus",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayAs",
                table: "Menus");

            migrationBuilder.AddColumn<bool>(
                name: "DisplayAsADropdownMenu",
                table: "Menus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DisplayAsAProductList",
                table: "Menus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
