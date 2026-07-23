using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystemInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addeddatabasenameforsomeconstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "ix_user_email");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                newName: "ix_tag_name");

            migrationBuilder.RenameIndex(
                name: "IX_AttributeDefinitions_Name",
                table: "AttributeDefinitions",
                newName: "ix_attribute_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_user_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "ix_tag_name",
                table: "Tags",
                newName: "IX_Tags_Name");

            migrationBuilder.RenameIndex(
                name: "ix_attribute_name",
                table: "AttributeDefinitions",
                newName: "IX_AttributeDefinitions_Name");
        }
    }
}
