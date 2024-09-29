using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLinked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Recommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetJobRecommendations",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvertisementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetJobRecommendations", x => new { x.UserId, x.AdvertisementId });
                    table.ForeignKey(
                        name: "FK_AspNetJobRecommendations_AspNetJobAdvertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "AspNetJobAdvertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetJobRecommendations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetPostRecommendations",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetPostRecommendations", x => new { x.UserId, x.PostId });
                    table.ForeignKey(
                        name: "FK_AspNetPostRecommendations_AspNetPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "AspNetPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetPostRecommendations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetJobRecommendations_AdvertisementId",
                table: "AspNetJobRecommendations",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetJobRecommendations_UserId_AdvertisementId",
                table: "AspNetJobRecommendations",
                columns: new[] { "UserId", "AdvertisementId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostRecommendations_PostId",
                table: "AspNetPostRecommendations",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostRecommendations_UserId_PostId",
                table: "AspNetPostRecommendations",
                columns: new[] { "UserId", "PostId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetJobRecommendations");

            migrationBuilder.DropTable(
                name: "AspNetPostRecommendations");
        }
    }
}
