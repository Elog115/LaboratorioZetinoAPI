using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisLabZetino.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "correo",
                table: "t_usuariosistema",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "clave",
                table: "t_usuariosistema",
                newName: "email");

            migrationBuilder.AddColumn<int>(
                name: "idrol1",
                table: "t_usuariosistema",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_usuariosistema_idrol1",
                table: "t_usuariosistema",
                column: "idrol1");

            migrationBuilder.AddForeignKey(
                name: "FK_t_usuariosistema_t_rol_idrol1",
                table: "t_usuariosistema",
                column: "idrol1",
                principalTable: "t_rol",
                principalColumn: "idrol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_usuariosistema_t_rol_idrol1",
                table: "t_usuariosistema");

            migrationBuilder.DropIndex(
                name: "IX_t_usuariosistema_idrol1",
                table: "t_usuariosistema");

            migrationBuilder.DropColumn(
                name: "idrol1",
                table: "t_usuariosistema");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "t_usuariosistema",
                newName: "correo");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "t_usuariosistema",
                newName: "clave");
        }
    }
}
