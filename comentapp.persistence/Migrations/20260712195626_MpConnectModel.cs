using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comentapp.persistence.Migrations
{
    /// <inheritdoc />
    public partial class MpConnectModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Payment_PaymentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Creators_CreatorId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentStatus_PaymentStatusId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Users_UserId",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentStatus",
                table: "PaymentStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "MercadoPagoAccount",
                table: "Creators");

            migrationBuilder.RenameTable(
                name: "PaymentStatus",
                newName: "PaymentStatuses");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_UserId",
                table: "Payments",
                newName: "IX_Payments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PaymentStatusId",
                table: "Payments",
                newName: "IX_Payments_PaymentStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_CreatorId",
                table: "Payments",
                newName: "IX_Payments_CreatorId");

            migrationBuilder.AddColumn<string>(
                name: "CollectorId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MarketplaceFee",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MpRawStatus",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetReceivedAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "StatusDetail",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentStatuses",
                table: "PaymentStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CreatorMercadoPagoAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    MpUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccessTokenEncrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenEncrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsConnected = table.Column<bool>(type: "bit", nullable: false),
                    ConnectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorMercadoPagoAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreatorMercadoPagoAccounts_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MercadoPagoOAuthStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MercadoPagoOAuthStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MercadoPagoOAuthStates_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreatorMercadoPagoAccounts_CreatorId",
                table: "CreatorMercadoPagoAccounts",
                column: "CreatorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreatorMercadoPagoAccounts_MpUserId",
                table: "CreatorMercadoPagoAccounts",
                column: "MpUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MercadoPagoOAuthStates_CreatorId",
                table: "MercadoPagoOAuthStates",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MercadoPagoOAuthStates_State",
                table: "MercadoPagoOAuthStates",
                column: "State",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Payments_PaymentId",
                table: "Comments",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Creators_CreatorId",
                table: "Payments",
                column: "CreatorId",
                principalTable: "Creators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentStatuses_PaymentStatusId",
                table: "Payments",
                column: "PaymentStatusId",
                principalTable: "PaymentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Payments_PaymentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Creators_CreatorId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentStatuses_PaymentStatusId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "CreatorMercadoPagoAccounts");

            migrationBuilder.DropTable(
                name: "MercadoPagoOAuthStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentStatuses",
                table: "PaymentStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CollectorId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MarketplaceFee",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MpRawStatus",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "NetReceivedAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StatusDetail",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "PaymentStatuses",
                newName: "PaymentStatus");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_UserId",
                table: "Payment",
                newName: "IX_Payment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentStatusId",
                table: "Payment",
                newName: "IX_Payment_PaymentStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_CreatorId",
                table: "Payment",
                newName: "IX_Payment_CreatorId");

            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoAccount",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentStatus",
                table: "PaymentStatus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Payment_PaymentId",
                table: "Comments",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Creators_CreatorId",
                table: "Payment",
                column: "CreatorId",
                principalTable: "Creators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentStatus_PaymentStatusId",
                table: "Payment",
                column: "PaymentStatusId",
                principalTable: "PaymentStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Users_UserId",
                table: "Payment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
