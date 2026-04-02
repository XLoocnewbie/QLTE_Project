using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_QLTE.UserService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_TypeLogin_AuthId_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoDuTaiKhoan",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "AuthId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeLogin",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TypeLogin",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<decimal>(
                name: "SoDuTaiKhoan",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
