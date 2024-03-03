using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace weather.Migrations
{
    public partial class UpdatedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "User",
                newName: "Mobile_num");

            migrationBuilder.AddColumn<string>(
                name: "Email_id",
                table: "User",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "First_name",
                table: "User",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Last_name",
                table: "User",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email_id",
                table: "User");

            migrationBuilder.DropColumn(
                name: "First_name",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Last_name",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "Mobile_num",
                table: "User",
                newName: "Name");
        }
    }
}
