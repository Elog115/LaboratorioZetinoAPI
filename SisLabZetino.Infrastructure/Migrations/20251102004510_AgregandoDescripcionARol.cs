using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisLabZetino.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregandoDescripcionARol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "descripcion",
                table: "t_rol",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "descripcion",
                table: "t_rol");
        }
    }
}
