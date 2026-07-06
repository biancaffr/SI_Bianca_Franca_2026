using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Cliente;

public partial class Index
{
    [Inject] private ClientesService ClientesService { get; set; } = default!;

    private List<Clientes>? _clientes;
    private FiltroBaseDto _filtro = new();
    private ClientesModal _modalCliente = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarClientes();
    }

    private async Task CarregarClientes()
    {
        _clientes = await ClientesService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarClientes();
    }

    private async Task AbrirCadastro() => await _modalCliente.AbrirNovo();

    private async Task AbrirEdicao(int id) => await _modalCliente.AbrirEditar(id);

    private async Task OnClienteSalvo(Clientes cliente)
    {
        await CarregarClientes();
    }
}