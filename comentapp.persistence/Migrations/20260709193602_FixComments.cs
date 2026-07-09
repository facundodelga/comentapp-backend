using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comentapp.persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Creators_CreatorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Creators_CreatorId1",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Creators_CreatorName",
                table: "Creators");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CreatorId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CreatorId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "LastModifiedDate",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "MercadoPagoAccount",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Creators_CreatorId",
                table: "Comments",
                column: "CreatorId",
                principalTable: "Creators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Creators_CreatorId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "LastModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreatedDate");

            migrationBuilder.AlterColumn<string>(
                name: "MercadoPagoAccount",
                table: "Creators",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId1",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Creators_CreatorName",
                table: "Creators",
                column: "CreatorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatorId1",
                table: "Comments",
                column: "CreatorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId1",
                table: "Comments",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Creators_CreatorId",
                table: "Comments",
                column: "CreatorId",
                principalTable: "Creators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Creators_CreatorId1",
                table: "Comments",
                column: "CreatorId1",
                principalTable: "Creators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId1",
                table: "Comments",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
