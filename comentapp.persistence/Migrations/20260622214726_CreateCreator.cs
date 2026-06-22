using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comentapp.persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MercadoPagoAccount = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    InstagramLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TikTokLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    YouTubeLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TwitchLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    KickLink = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creators_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Creators_CreatorName",
                table: "Creators",
                column: "CreatorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Creators_UserId",
                table: "Creators",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Creators");
        }
    }
}
