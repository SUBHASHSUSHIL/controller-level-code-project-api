using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMS_Project_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "tbl_VideoAnalytic",
                newName: "RTSPUrl");

            migrationBuilder.AddColumn<int>(
                name: "CameraId",
                table: "tbl_VideoAnalytic",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ObjectList",
                table: "tbl_VideoAnalytic",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_VideoAnalytic_CameraId",
                table: "tbl_VideoAnalytic",
                column: "CameraId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_VideoAnalytic_Cameras_CameraId",
                table: "tbl_VideoAnalytic",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_VideoAnalytic_Cameras_CameraId",
                table: "tbl_VideoAnalytic");

            migrationBuilder.DropIndex(
                name: "IX_tbl_VideoAnalytic_CameraId",
                table: "tbl_VideoAnalytic");

            migrationBuilder.DropColumn(
                name: "CameraId",
                table: "tbl_VideoAnalytic");

            migrationBuilder.DropColumn(
                name: "ObjectList",
                table: "tbl_VideoAnalytic");

            migrationBuilder.RenameColumn(
                name: "RTSPUrl",
                table: "tbl_VideoAnalytic",
                newName: "Name");
        }
    }
}
