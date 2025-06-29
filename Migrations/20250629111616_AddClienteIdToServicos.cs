using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotasAlcoafinal.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteIdToServicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Servicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_ClienteId",
                table: "Servicos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Clientes_ClienteId",
                table: "Servicos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Clientes_ClienteId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_ClienteId",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Servicos");
        }
    }
}
