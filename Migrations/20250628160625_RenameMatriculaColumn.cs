using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotasAlcoafinal.Migrations
{
    /// <inheritdoc />
    public partial class RenameMatriculaColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Placa",
                table: "Motocicletas",
                newName: "Matricula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Matricula",
                table: "Motocicletas",
                newName: "Placa");
        }
    }
}
