using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sample.Fluxo.Caixa.PlanoContas.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(500)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContaTipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contas", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Contas",
                columns: new[] { "Id", "Ativo", "ContaTipo", "DataCadastro", "Descricao" },
                values: new object[,]
                {
                    { new Guid("1bf47be9-926d-4beb-86d3-bda42a17382b"), true, 1, new DateTime(2023, 1, 30, 22, 6, 29, 559, DateTimeKind.Local).AddTicks(5748), "Saldo Inicial" },
                    { new Guid("3d4eadd8-dd32-428a-88cf-df3e8a89848f"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1899), "Investimentos" },
                    { new Guid("69562cef-9241-4d24-8999-d333d91587aa"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1897), "Outros pagamentos" },
                    { new Guid("39a87037-bfa0-4ff9-ad5a-7c04c2e13c2b"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1895), "Financiamentos" },
                    { new Guid("a8c400cc-dad9-4d8a-abc6-093777ad4f48"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1893), "Empréstimos" },
                    { new Guid("0b5c8e13-4bd0-4c3f-948e-7b64e58ea266"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1891), "Serviços contabilidade" },
                    { new Guid("4e40c50d-a514-4fdd-9eb5-a551a4d41f55"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1889), "Telefone" },
                    { new Guid("2a867831-ee12-44f1-a689-17be2892d591"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1886), "Energia elétrica" },
                    { new Guid("59ac3e1a-0eed-4879-aaab-1f7f7c148d83"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1903), "Impostos" },
                    { new Guid("230cb33d-b4cb-409d-a34b-fcbb672c8acc"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1884), "Aluguel" },
                    { new Guid("09277791-7ffb-4f01-be9e-be7fa32c1273"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1874), "Retiradas sócios" },
                    { new Guid("0cf8e5a2-0af0-4185-8494-d4b830c7d972"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1872), "Materiais" },
                    { new Guid("ccc1011e-fe3e-4aba-b6a6-e9afb8646aec"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1869), "Fornecedores" },
                    { new Guid("e043a7c3-96e5-4a3a-9533-ec2456fa8118"), true, 2, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1867), "Entrada" },
                    { new Guid("297f9108-0f10-4466-8819-228284385ecc"), true, 2, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1864), "Financiamento" },
                    { new Guid("27ad1fc4-582f-40be-be67-ed3c8729b0ba"), true, 2, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1860), "Juros" },
                    { new Guid("9bc32333-7e08-4b47-8e20-e8ade041845f"), true, 2, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1827), "Vendas" },
                    { new Guid("03c3a557-c972-49ad-b913-93304b6147ae"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1877), "Folha de pagamento" },
                    { new Guid("27ab124e-207b-4ad2-8b3e-bcaa68c87120"), true, 3, new DateTime(2023, 1, 30, 22, 6, 29, 563, DateTimeKind.Local).AddTicks(1905), "Advogado" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contas");
        }
    }
}
