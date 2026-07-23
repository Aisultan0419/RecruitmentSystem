using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystemInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableCategory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinitions_AttributeCategory_AttributeCategoryId",
                table: "AttributeDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeCategory",
                table: "AttributeCategory");

            migrationBuilder.RenameTable(
                name: "AttributeCategory",
                newName: "AttributeCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeCategories",
                table: "AttributeCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinitions_AttributeCategories_AttributeCategoryId",
                table: "AttributeDefinitions",
                column: "AttributeCategoryId",
                principalTable: "AttributeCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttributeDefinitions_AttributeCategories_AttributeCategoryId",
                table: "AttributeDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttributeCategories",
                table: "AttributeCategories");

            migrationBuilder.RenameTable(
                name: "AttributeCategories",
                newName: "AttributeCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttributeCategory",
                table: "AttributeCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttributeDefinitions_AttributeCategory_AttributeCategoryId",
                table: "AttributeDefinitions",
                column: "AttributeCategoryId",
                principalTable: "AttributeCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
