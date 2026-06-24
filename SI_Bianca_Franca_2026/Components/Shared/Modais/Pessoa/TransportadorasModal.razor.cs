using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

public partial class TransportadorasModal : PessoaModalBase<TransportadorasDTO>
{
    [Inject] private TransportadorasService TransportadorasService { get; set; } = default!;

    [Parameter] public EventCallback<Transportadoras> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new TransportadorasDTO { Ativo = true };
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

        var transportadora = await TransportadorasService.Pesquisar(id);
        if (transportadora is null) { _aberto = false; StateHasChanged(); return; }

        _dto = new TransportadorasDTO
        {
            Id = transportadora.Id,
            TipoPessoa = transportadora.TipoPessoa,
            NomeRazaoSocial = transportadora.NomeRazaoSocial,
            CpfCnpj = transportadora.CpfCnpj,
            RgIe = transportadora.RgIe,
            ApelidoNomeFantasia = transportadora.ApelidoNomeFantasia,
            IdPais = transportadora.IdPais,
            IdCidade = transportadora.IdCidade,
            Bairro = transportadora.Bairro,
            Logradouro = transportadora.Logradouro,
            Numero = transportadora.Numero,
            Complemento = transportadora.Complemento,
            Cep = transportadora.Cep,
            Telefone = transportadora.Telefone,
            Email = transportadora.Email,
            Observacao = transportadora.Observacao,
            Rntrc = transportadora.Rntrc,
            Ativo = transportadora.Ativo,
            DataCriacao = transportadora.DataCriacao,
            DataUltimaAlteracao = transportadora.DataUltimaAlteracao,
            NomeUsuarioAlteracao = transportadora.NomeUsuarioAlteracao
        };

        await CarregarLocalizacaoDoRegistroAsync();

        _carregando = false;
        StateHasChanged();
    }

    private void Fechar() { _aberto = false; StateHasChanged(); }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;
            if (EhBrasileiro && string.IsNullOrWhiteSpace(_dto.CpfCnpj))
            {
                _erro = "CPF/CNPJ é obrigatório para transportadoras do Brasil.";
                return;
            }
            var transportadora = MapearParaModel();
            if (transportadora.Id == 0) await TransportadorasService.Inserir(transportadora);
            else await TransportadorasService.Atualizar(transportadora);
            await AoSalvarComSucesso.InvokeAsync(transportadora);
            Fechar();
        }
        catch (Exception ex) { _erro = ex.Message; }
    }

    private Transportadoras MapearParaModel() => new()
    {
        Id = _dto.Id,
        TipoPessoa = _dto.TipoPessoa,
        NomeRazaoSocial = _dto.NomeRazaoSocial,
        CpfCnpj = _dto.CpfCnpj != null ? new string(_dto.CpfCnpj.Where(char.IsDigit).ToArray()) : null,
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
        Observacao = _dto.Observacao,
        Rntrc = _dto.Rntrc,
        Ativo = _dto.Ativo,
        DataCriacao = _dto.DataCriacao
    };
}
