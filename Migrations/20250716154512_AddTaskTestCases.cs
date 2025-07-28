using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniCodeProject.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskTestCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedOutput",
                table: "TaskModels");

            migrationBuilder.CreateTable(
                name: "TaskTestCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InputData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTestCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTestCases_TaskModels_TaskModelId",
                        column: x => x.TaskModelId,
                        principalTable: "TaskModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskTestCases_TaskModelId",
                table: "TaskTestCases",
                column: "TaskModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskTestCases");

            migrationBuilder.AddColumn<string>(
                name: "ExpectedOutput",
                table: "TaskModels",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
