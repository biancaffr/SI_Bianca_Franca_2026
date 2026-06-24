using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Cliente;

public partial class Index
{
    [Inject] private ClientesService ClientesService { get; set; } = default!;

    private List<Clientes>? clientes;
    private string filtroStatus = "ativo";
    private string termoPesquisa = string.Empty;

    private ClientesModal _modalCliente = default!;

    private IEnumerable<Clientes> ClientesFiltrados =>
        (clientes ?? [])
        .Where(c => filtroStatus switch
        {
            "ativo" => c.Ativo,
            "inativo" => !c.Ativo,
            _ => true
        })
        .Where(c => string.IsNullOrWhiteSpace(termoPesquisa) ||
                    c.NomeRazaoSocial.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (c.CpfCnpj != null && c.CpfCnpj.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    (c.ApelidoNomeFantasia != null && c.ApelidoNomeFantasia.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Telefone != null && c.Telefone.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    (c.OPais != null && c.OPais.Pais.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarClientes();
    }

    private async Task CarregarClientes()
    {
        clientes = await ClientesService.ListarTudo();
    }

    private async Task AbrirCadastro() => await _modalCliente.AbrirNovo();

    private async Task AbrirEdicao(int id) => await _modalCliente.AbrirEditar(id);

    private async Task OnClienteSalvo(Clientes cliente)
    {
        await CarregarClientes();
    }
}
