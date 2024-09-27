using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLinked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Reactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetCommentReactions");

            migrationBuilder.DropTable(
                name: "AspNetPostReactions");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AspNetComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AspNetReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetReactions_AspNetComments_PostId",
                        column: x => x.PostId,
                        principalTable: "AspNetComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetReactions_AspNetPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "AspNetPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetReactions_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetReactions_CreatorId",
                table: "AspNetReactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetReactions_Id",
                table: "AspNetReactions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetReactions_PostId",
                table: "AspNetReactions",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetReactions");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AspNetComments");

            migrationBuilder.CreateTable(
                name: "AspNetCommentReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetCommentReactions_AspNetComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "AspNetComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetCommentReactions_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetPostReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetPostReactions_AspNetPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "AspNetPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetPostReactions_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetCommentReactions_CommentId",
                table: "AspNetCommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetCommentReactions_CreatorId",
                table: "AspNetCommentReactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetCommentReactions_Id",
                table: "AspNetCommentReactions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostReactions_CreatorId",
                table: "AspNetPostReactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostReactions_Id",
                table: "AspNetPostReactions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostReactions_PostId",
                table: "AspNetPostReactions",
                column: "PostId");
        }
    }
}
