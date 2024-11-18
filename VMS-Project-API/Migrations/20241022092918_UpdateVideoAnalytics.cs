using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS_Project_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVideoAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CameraIP",
                table: "tbl_VideoAnalytic",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CameraIP",
                table: "tbl_VideoAnalytic");
        }
    }
}
