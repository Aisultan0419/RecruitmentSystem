using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystemInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addednewfieldinposition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Positions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Positions");
        }
    }
}
