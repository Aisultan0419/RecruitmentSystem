using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentSystemInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changedrelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateAttributeValues_Users_UserId",
                table: "CandidateAttributeValues");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CandidateAttributeValues",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateAttributeValues_UserId_AttributeId",
                table: "CandidateAttributeValues",
                newName: "IX_CandidateAttributeValues_UserProfileId_AttributeId");

            migrationBuilder.AddColumn<Guid>(
                name: "AttributeDefinitionId",
                table: "CandidateAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAttributeValues_AttributeDefinitionId",
                table: "CandidateAttributeValues",
                column: "AttributeDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateAttributeValues_AttributeDefinitions_AttributeDefi~",
                table: "CandidateAttributeValues",
                column: "AttributeDefinitionId",
                principalTable: "AttributeDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateAttributeValues_UserProfiles_UserProfileId",
                table: "CandidateAttributeValues",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateAttributeValues_AttributeDefinitions_AttributeDefi~",
                table: "CandidateAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateAttributeValues_UserProfiles_UserProfileId",
                table: "CandidateAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_CandidateAttributeValues_AttributeDefinitionId",
                table: "CandidateAttributeValues");

            migrationBuilder.DropColumn(
                name: "AttributeDefinitionId",
                table: "CandidateAttributeValues");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "CandidateAttributeValues",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidateAttributeValues_UserProfileId_AttributeId",
                table: "CandidateAttributeValues",
                newName: "IX_CandidateAttributeValues_UserId_AttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateAttributeValues_Users_UserId",
                table: "CandidateAttributeValues",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
