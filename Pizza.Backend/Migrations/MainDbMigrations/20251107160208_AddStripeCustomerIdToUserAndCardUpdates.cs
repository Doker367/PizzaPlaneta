using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza.Backend.Migrations.MainDbMigrations
{
    /// <inheritdoc />
    public partial class AddStripeCustomerIdToUserAndCardUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_vencimiento",
                table: "tarjetas");

            migrationBuilder.DropColumn(
                name: "token_pago",
                table: "tarjetas");

            migrationBuilder.RenameColumn(
                name: "marca",
                table: "tarjetas",
                newName: "Marca");

            migrationBuilder.RenameColumn(
                name: "nombre_tarjeta",
                table: "tarjetas",
                newName: "NombreTarjeta");

            migrationBuilder.RenameColumn(
                name: "numero_enmascarado",
                table: "tarjetas",
                newName: "stripe_payment_method_id");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Marca",
                table: "tarjetas",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "exp_month",
                table: "tarjetas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "exp_year",
                table: "tarjetas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "last4",
                table: "tarjetas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "exp_month",
                table: "tarjetas");

            migrationBuilder.DropColumn(
                name: "exp_year",
                table: "tarjetas");

            migrationBuilder.DropColumn(
                name: "last4",
                table: "tarjetas");

            migrationBuilder.RenameColumn(
                name: "Marca",
                table: "tarjetas",
                newName: "marca");

            migrationBuilder.RenameColumn(
                name: "NombreTarjeta",
                table: "tarjetas",
                newName: "nombre_tarjeta");

            migrationBuilder.RenameColumn(
                name: "stripe_payment_method_id",
                table: "tarjetas",
                newName: "numero_enmascarado");

            migrationBuilder.AlterColumn<string>(
                name: "marca",
                table: "tarjetas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "fecha_vencimiento",
                table: "tarjetas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token_pago",
                table: "tarjetas",
                type: "text",
                nullable: true);
        }
    }
}
