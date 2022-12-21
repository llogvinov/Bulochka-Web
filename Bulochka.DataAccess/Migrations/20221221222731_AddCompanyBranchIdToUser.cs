using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulochka.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyBranchIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyBranchId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyBranchId",
                table: "AspNetUsers",
                column: "CompanyBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CompanyBranches_CompanyBranchId",
                table: "AspNetUsers",
                column: "CompanyBranchId",
                principalTable: "CompanyBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CompanyBranches_CompanyBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyBranchId",
                table: "AspNetUsers");
        }
    }
}
