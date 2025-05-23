using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingOffApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsActiveToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StreamingOffers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StreamingOffers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "StreamingOffers");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StreamingOffers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
