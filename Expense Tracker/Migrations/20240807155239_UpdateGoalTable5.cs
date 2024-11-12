using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expense_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGoalTable5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAmount",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "GoalType",
                table: "Goals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoalType",
                table: "Goals");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentAmount",
                table: "Goals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
