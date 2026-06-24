using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Comercial;
using SI_Bianca_Franca_2026.Services.Comercial;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Comercial;

namespace SI_Bianca_Franca_2026.Components.Pages.Comercial.Pedido;

public partial class Index
{
    [Inject] private PedidosService PedidosService { get; set; } = default!;

    private List<Pedidos>? _pedidos;
    private string _filtroStatus = "todos";
    private string _filtroAtivo = "ativo";
    private string _termoPesquisa = string.Empty;

    private PedidosModal _modalPedido = default!;

    private static readonly Dictionary<string, string> NomesStatus = new()
    {
        { "AGUARDANDO_APROVACAO_ARTE", "Aguardando Aprovação de Arte" },
        { "ARTE_APROVADA", "Arte Aprovada" },
        { "EM_PRODUCAO", "Em Produção" },
        { "PRONTO_RETIRADA", "Pronto para Retirada" },
        { "ENVIADO", "Enviado" },
        { "ENTREGUE", "Entregue" },
        { "CANCELADO", "Cancelado" }
    };

    private static readonly Dictionary<string, string> ClassesStatus = new()
    {
        { "AGUARDANDO_APROVACAO_ARTE", "bg-secondary" },
        { "ARTE_APROVADA", "bg-info text-dark" },
        { "EM_PRODUCAO", "bg-warning text-dark" },
        { "PRONTO_RETIRADA", "bg-primary" },
        { "ENVIADO", "bg-primary" },
        { "ENTREGUE", "bg-success" },
        { "CANCELADO", "bg-danger" }
    };

    private string ObterNomeStatus(string status) =>
        NomesStatus.TryGetValue(status, out var nome) ? nome : status;

    private string ObterClasseStatus(string status) =>
        ClassesStatus.TryGetValue(status, out var classe) ? classe : "bg-secondary";

    private IEnumerable<Pedidos> PedidosFiltrados =>
        (_pedidos ?? [])
        .Where(p => _filtroAtivo switch
        {
            "ativo" => p.Ativo,
            "inativo" => !p.Ativo,
            _ => true
        })
        .Where(p => _filtroStatus == "todos" || p.Status == _filtroStatus)
        .Where(p => string.IsNullOrWhiteSpace(_termoPesquisa) ||
                    (p.OCliente != null && p.OCliente.NomeRazaoSocial.Contains(_termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    (p.OEmitente != null && p.OEmitente.NomeRazaoSocial.Contains(_termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    p.Id.ToString().Contains(_termoPesquisa));

    protected override async Task OnInitializedAsync()
    {
        await CarregarPedidos();
    }

    private async Task CarregarPedidos()
    {
        _pedidos = await PedidosService.ListarTudo();
    }

    private async Task AbrirCadastro() => await _modalPedido.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalPedido.AbrirEditar(id);

    private async Task OnPedidoSalvo(Pedidos pedido)
    {
        await CarregarPedidos();
    }
}