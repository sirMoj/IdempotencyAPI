using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Dal.Migrations
{
    /// <inheritdoc />
    public partial class updateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "idempotencyKey",
                table: "idempotencyRecords",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "paymentModelId",
                table: "idempotencyRecords",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_idempotencyRecords_idempotencyKey",
                table: "idempotencyRecords",
                column: "idempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_idempotencyRecords_paymentModelId",
                table: "idempotencyRecords",
                column: "paymentModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_idempotencyRecords_Payment_paymentModelId",
                table: "idempotencyRecords",
                column: "paymentModelId",
                principalTable: "Payment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_idempotencyRecords_Payment_paymentModelId",
                table: "idempotencyRecords");

            migrationBuilder.DropIndex(
                name: "IX_idempotencyRecords_idempotencyKey",
                table: "idempotencyRecords");

            migrationBuilder.DropIndex(
                name: "IX_idempotencyRecords_paymentModelId",
                table: "idempotencyRecords");

            migrationBuilder.DropColumn(
                name: "paymentModelId",
                table: "idempotencyRecords");

            migrationBuilder.AlterColumn<string>(
                name: "idempotencyKey",
                table: "idempotencyRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
