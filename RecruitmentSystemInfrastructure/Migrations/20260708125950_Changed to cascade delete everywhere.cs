using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystemInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changedtocascadedeleteeverywhere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateAttributeValues_AttributeDefinitions_AttributeId",
                table: "CandidateAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Positions_PositionId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionPosts_Users_AuthorId",
                table: "DiscussionPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAccessRules_AttributeDefinitions_AttributeDefinitio~",
                table: "PositionAccessRules");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAttributes_AttributeDefinitions_AttributeDefinition~",
                table: "PositionAttributes");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateAttributeValues_AttributeDefinitions_AttributeId",
                table: "CandidateAttributeValues",
                column: "AttributeId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Positions_PositionId",
                table: "CVs",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionPosts_Users_AuthorId",
                table: "DiscussionPosts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAccessRules_AttributeDefinitions_AttributeDefinitio~",
                table: "PositionAccessRules",
                column: "AttributeDefinitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAttributes_AttributeDefinitions_AttributeDefinition~",
                table: "PositionAttributes",
                column: "AttributeDefinitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateAttributeValues_AttributeDefinitions_AttributeId",
                table: "CandidateAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Positions_PositionId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionPosts_Users_AuthorId",
                table: "DiscussionPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAccessRules_AttributeDefinitions_AttributeDefinitio~",
                table: "PositionAccessRules");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAttributes_AttributeDefinitions_AttributeDefinition~",
                table: "PositionAttributes");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateAttributeValues_AttributeDefinitions_AttributeId",
                table: "CandidateAttributeValues",
                column: "AttributeId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Positions_PositionId",
                table: "CVs",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionPosts_Users_AuthorId",
                table: "DiscussionPosts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAccessRules_AttributeDefinitions_AttributeDefinitio~",
                table: "PositionAccessRules",
                column: "AttributeDefinitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAttributes_AttributeDefinitions_AttributeDefinition~",
                table: "PositionAttributes",
                column: "AttributeDefinitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
