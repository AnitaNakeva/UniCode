using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCodeProject.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedInputDataToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InputData",
                table: "TaskModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputData",
                table: "TaskModels");
        }
    }
}
