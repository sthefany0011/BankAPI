using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankAPI.Migrations
{
    public partial class removecontabancaria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historico_ContaBancaria_ContaBancariaId",
                table: "Historico");

            migrationBuilder.DropTable(
                name: "ContaBancaria");

            migrationBuilder.DropIndex(
                name: "IX_Historico_ContaBancariaId",
                table: "Historico");

            migrationBuilder.DropColumn(
                name: "ContaBancariaId",
                table: "Historico");

            migrationBuilder.AddColumn<int>(
                name: "ContaBancaria",
                table: "Historico",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContaBancaria",
                table: "Historico");

            migrationBuilder.AddColumn<int>(
                name: "ContaBancariaId",
                table: "Historico",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContaBancaria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumeroConta = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Saldo = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaBancaria", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Historico_ContaBancariaId",
                table: "Historico",
                column: "ContaBancariaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Historico_ContaBancaria_ContaBancariaId",
                table: "Historico",
                column: "ContaBancariaId",
                principalTable: "ContaBancaria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
