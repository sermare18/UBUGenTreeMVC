using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UBUGenTreeMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    _id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    _nombre = table.Column<string>(type: "TEXT", nullable: false),
                    _email = table.Column<string>(type: "TEXT", nullable: false),
                    _contrasenaHash = table.Column<string>(type: "TEXT", nullable: false),
                    _rol = table.Column<int>(type: "INTEGER", nullable: false),
                    _ancestrosJSON = table.Column<string>(type: "TEXT", nullable: true),
                    _estado = table.Column<int>(type: "INTEGER", nullable: false),
                    _ultimoIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    _intentosFallidos = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x._id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
