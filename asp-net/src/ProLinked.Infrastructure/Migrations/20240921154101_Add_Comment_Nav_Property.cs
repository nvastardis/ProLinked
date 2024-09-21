using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLinked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Comment_Nav_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetComments_ParentId",
                table: "AspNetComments");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetComments_ParentId",
                table: "AspNetComments",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetComments_ParentId",
                table: "AspNetComments");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetComments_ParentId",
                table: "AspNetComments",
                column: "ParentId",
                unique: true,
                filter: "[ParentId] IS NOT NULL");
        }
    }
}
