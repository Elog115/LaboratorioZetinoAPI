using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisLabZetino.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarResultadoCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idusuario",
                table: "t_Resultado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "nombre",
                table: "t_ordenExamen",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "nombre",
                table: "t_Muestra",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idusuario",
                table: "t_Resultado");

            migrationBuilder.DropColumn(
                name: "nombre",
                table: "t_ordenExamen");

            migrationBuilder.DropColumn(
                name: "nombre",
                table: "t_Muestra");
        }
    }
}
