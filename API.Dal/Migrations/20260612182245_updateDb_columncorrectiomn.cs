using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Dal.Migrations
{
    /// <inheritdoc />
    public partial class updateDb_columncorrectiomn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "requesHash",
                table: "idempotencyRecords",
                newName: "requestHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "requestHash",
                table: "idempotencyRecords",
                newName: "requesHash");
        }
    }
}
