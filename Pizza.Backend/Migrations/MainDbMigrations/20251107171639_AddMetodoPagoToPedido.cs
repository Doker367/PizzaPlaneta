using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza.Backend.Migrations.MainDbMigrations
{
    /// <inheritdoc />
    public partial class AddMetodoPagoToPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetodoPago",
                table: "Pedido",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetodoPago",
                table: "Pedido");
        }
    }
}
