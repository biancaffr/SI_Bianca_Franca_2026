using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Financeiro;
using SI_Bianca_Franca_2026.Services.Pessoa;
using System.Globalization;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

public partial class ClientesModal : PessoaModalBase<ClientesDTO>
{
    [Inject] private ClientesService ClientesService { get; set; } = default!;
    [Inject] private CondicoesPagamentosService CondicoesPagamentosService { get; set; } = default!;

    [Parameter] public EventCallback<Clientes> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback<int> AoExcluirComSucesso { get; set; }


    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;
    private List<CondicoesPagamentos> _condicoesPagamento = new();
    private ModalConfirmacao _modalConfirmacao = default!;


    private string LimiteCreditoTexto
    {
        get => _dto.LimiteCredito == 0
            ? string.Empty
            : _dto.LimiteCredito.ToString("0.##", CultureInfo.GetCultureInfo("pt-BR"));
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _dto.LimiteCredito = 0;
            else if (decimal.TryParse(value, NumberStyles.Any,
                     CultureInfo.GetCultureInfo("pt-BR"), out var r))
                _dto.LimiteCredito = r;
        }
    }

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new ClientesDTO { Ativo = true };
        await CarregarCidadesAsync();
        await CarregarCondicoesAsync();
        _aberto = true;
        StateHasChanged();
    }

    public async Task AbrirEditar(int id)
    {
        _erro = string.Empty;
        _carregando = true;
        _aberto = true;
        StateHasChanged();

        var cliente = await ClientesService.Pesquisar(id);
        if (cliente is null)
        {
            _aberto = false;
            StateHasChanged();
            return;
        }

        _dto = new ClientesDTO
        {
            Id = cliente.Id,
            TipoPessoa = cliente.TipoPessoa,
            NomeRazaoSocial = cliente.NomeRazaoSocial,
            CpfCnpj = cliente.CpfCnpj,
            RgIe = cliente.RgIe,
            ApelidoNomeFantasia = cliente.ApelidoNomeFantasia,
            IdCidade = cliente.IdCidade,
            Bairro = cliente.Bairro,
            Logradouro = cliente.Logradouro,
            Numero = cliente.Numero,
            Complemento = cliente.Complemento,
            Cep = cliente.Cep,
            Telefone = cliente.Telefone,
            Email = cliente.Email,
            LimiteCredito = cliente.LimiteCredito,
            IdCondicaoPagamento = cliente.IdCondicaoPagamento,
            Observacao = cliente.Observacao,
            Ativo = cliente.Ativo,
            DataCriacao = cliente.DataCriacao,
            DataUltimaAlteracao = cliente.DataUltimaAlteracao,
            NomeUsuarioAlteracao = cliente.NomeUsuarioAlteracao
        };

        await CarregarCidadesAsync();
        await CarregarCondicoesAsync();

        _carregando = false;
        StateHasChanged();
    }
    private async Task CarregarCondicoesAsync()
    {
        var todas = await CondicoesPagamentosService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _condicoesPagamento = todas.ToList();
    }
    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;

            if (EhBrasileiro && string.IsNullOrWhiteSpace(_dto.CpfCnpj))
            {
                _erro = "CPF/CNPJ é obrigatório para clientes do Brasil.";
                return;
            }

            var cliente = MapearParaModel();

            if (cliente.Id == 0)
                await ClientesService.Inserir(cliente);
            else
                await ClientesService.Atualizar(cliente);

            await AoSalvarComSucesso.InvokeAsync(cliente);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir($"Deseja realmente excluir o cliente \"{_dto.NomeRazaoSocial}\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await ClientesService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private Clientes MapearParaModel() => new()
    {
        Id = _dto.Id,
        TipoPessoa = _dto.TipoPessoa,
        NomeRazaoSocial = _dto.NomeRazaoSocial,
        CpfCnpj = _dto.CpfCnpj != null
            ? new string(_dto.CpfCnpj.Where(char.IsDigit).ToArray())
            : null,
        RgIe = _dto.RgIe,
        ApelidoNomeFantasia = _dto.ApelidoNomeFantasia,
        IdCidade = _dto.IdCidade,
        Bairro = _dto.Bairro,
        Logradouro = _dto.Logradouro,
        Numero = _dto.Numero,
        Complemento = _dto.Complemento,
        Cep = _dto.Cep,
        Telefone = _dto.Telefone,
        Email = _dto.Email,
        LimiteCredito = _dto.LimiteCredito,
        IdCondicaoPagamento = _dto.IdCondicaoPagamento,
        Observacao = _dto.Observacao,
        Ativo = _dto.Ativo,
        DataCriacao = _dto.DataCriacao
    };
}