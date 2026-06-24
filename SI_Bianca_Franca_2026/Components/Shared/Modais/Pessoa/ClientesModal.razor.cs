using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

public partial class ClientesModal : PessoaModalBase<ClientesDTO>
{
    [Inject] private ClientesService ClientesService { get; set; } = default!;

    [Parameter] public EventCallback<Clientes> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;

    private string LimiteCreditoTexto
    {
        get => _dto.LimiteCredito == 0 ? string.Empty : _dto.LimiteCredito.ToString("N2");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _dto.LimiteCredito = 0;
            else if (decimal.TryParse(value, System.Globalization.NumberStyles.Any,
                     System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var resultado))
                _dto.LimiteCredito = resultado;
        }
    }

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new ClientesDTO { Ativo = true };
        _idEstadoSelecionado = 0;
        _estadosFiltrados = new();
        _cidadesFiltradas = new();
        await CarregarPaisesAsync();
        _aberto = true;
        StateHasChanged();
    }

    public async Task AbrirEditar(int id)
    {
        _erro = string.Empty;
        _carregando = true;
        _aberto = true;
        StateHasChanged();

        await CarregarPaisesAsync();

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
            IdPais = cliente.IdPais,
            IdCidade = cliente.IdCidade,
            Bairro = cliente.Bairro,
            Logradouro = cliente.Logradouro,
            Numero = cliente.Numero,
            Complemento = cliente.Complemento,
            Cep = cliente.Cep,
            Telefone = cliente.Telefone,
            Email = cliente.Email,
            LimiteCredito = cliente.LimiteCredito,
            Observacao = cliente.Observacao,
            Ativo = cliente.Ativo,
            DataCriacao = cliente.DataCriacao,
            DataUltimaAlteracao = cliente.DataUltimaAlteracao,
            NomeUsuarioAlteracao = cliente.NomeUsuarioAlteracao
        };

        await CarregarLocalizacaoDoRegistroAsync();

        _carregando = false;
        StateHasChanged();
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
        IdPais = _dto.IdPais,
        IdCidade = _dto.IdCidade == 0 ? null : _dto.IdCidade,
        Bairro = _dto.Bairro,
        Logradouro = _dto.Logradouro,
        Numero = _dto.Numero,
        Complemento = _dto.Complemento,
        Cep = _dto.Cep,
        Telefone = _dto.Telefone,
        Email = _dto.Email,
        LimiteCredito = _dto.LimiteCredito,
        Observacao = _dto.Observacao,
        Ativo = _dto.Ativo,
        DataCriacao = _dto.DataCriacao
    };
}
