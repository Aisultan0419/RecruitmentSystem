using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecruitmentSystemInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttributeDefinitions_Category",
                table: "AttributeDefinitions");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "AttributeDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "UserProfiles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "AttributeCategoryId",
                table: "AttributeDefinitions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AttributeCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeCategory", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AttributeCategory",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("06e94f80-f673-4a97-986f-36278ac51ac8"), "Personal Information" },
                    { new Guid("2f6e4b7d-5260-447a-a78a-7b3401ffbc9d"), "Domain Knowledge" },
                    { new Guid("4259a6c1-2112-432f-8499-5a52687b0ff9"), "Certification" },
                    { new Guid("cb188329-0a8a-4def-aa72-23847feeb4cd"), "Soft Skills" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_AttributeCategoryId",
                table: "AttributeDefinitions",
                column: "AttributeCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinitions_AttributeCategory_AttributeCategoryId",
                table: "AttributeDefinitions",
                column: "AttributeCategoryId",
                principalTable: "AttributeCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinitions_AttributeCategory_AttributeCategoryId",
                table: "AttributeDefinitions");

            migrationBuilder.DropTable(
                name: "AttributeCategory");

            migrationBuilder.DropIndex(
                name: "IX_AttributeDefinitions_AttributeCategoryId",
                table: "AttributeDefinitions");

            migrationBuilder.DropColumn(
                name: "AttributeCategoryId",
                table: "AttributeDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "AttributeDefinitions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_Category",
                table: "AttributeDefinitions",
                column: "Category");
        }
    }
}
