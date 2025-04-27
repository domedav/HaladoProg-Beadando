using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaladoProg2.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class UserMoney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "UserMoney",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelling",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserMoney",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSelling",
                table: "Transactions");
        }
    }
}
