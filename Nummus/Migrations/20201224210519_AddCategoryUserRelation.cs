using Microsoft.EntityFrameworkCore.Migrations;

namespace Nummus.Migrations
{
    public partial class AddCategoryUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingLines_Accounts_AccountId",
                table: "BookingLines");

            migrationBuilder.AddColumn<int>(
                name: "NummusUserId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "BookingLines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NummusUserId",
                table: "Categories",
                column: "NummusUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingLines_Accounts_AccountId",
                table: "BookingLines",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_NummusUsers_NummusUserId",
                table: "Categories",
                column: "NummusUserId",
                principalTable: "NummusUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingLines_Accounts_AccountId",
                table: "BookingLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_NummusUsers_NummusUserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NummusUserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "NummusUserId",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "BookingLines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingLines_Accounts_AccountId",
                table: "BookingLines",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
