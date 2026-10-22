using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cross_platform_development_technologies.Migrations
{
    public partial class AddMeasurementInterval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MeasurementIntervalSeconds",
                table: "Sessions",
                type: "REAL",
                nullable: false,
                defaultValue: 1.0);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasurementIntervalSeconds",
                table: "Sessions");
        }
    }
}
