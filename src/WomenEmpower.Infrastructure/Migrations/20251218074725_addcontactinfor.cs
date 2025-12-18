using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WomenEmpower.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcontactinfor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactInfo",
                table: "Reports");
        }
    }
}
