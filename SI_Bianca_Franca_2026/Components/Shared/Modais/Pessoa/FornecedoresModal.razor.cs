using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

public partial class FornecedoresModal : PessoaModalBase<FornecedoresDTO>
{
    [Inject] private FornecedoresService FornecedoresService { get; set; } = default!;

    [Parameter] public EventCallback<Fornecedores> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new FornecedoresDTO { Ativo = true };
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

        var fornecedor = await FornecedoresService.Pesquisar(id);
        if (fornecedor is null) { _aberto = false; StateHasChanged(); return; }

        _dto = new FornecedoresDTO
        {
            Id = fornecedor.Id,
            TipoPessoa = fornecedor.TipoPessoa,
            NomeRazaoSocial = fornecedor.NomeRazaoSocial,
            CpfCnpj = fornecedor.CpfCnpj,
            RgIe = fornecedor.RgIe,
            ApelidoNomeFantasia = fornecedor.ApelidoNomeFantasia,
            IdPais = fornecedor.IdPais,
            IdCidade = fornecedor.IdCidade,
            Bairro = fornecedor.Bairro,
            Logradouro = fornecedor.Logradouro,
            Numero = fornecedor.Numero,
            Complemento = fornecedor.Complemento,
            Cep = fornecedor.Cep,
            Telefone = fornecedor.Telefone,
            Email = fornecedor.Email,
            Observacao = fornecedor.Observacao,
            Ativo = fornecedor.Ativo,
            DataCriacao = fornecedor.DataCriacao,
            DataUltimaAlteracao = fornecedor.DataUltimaAlteracao,
            NomeUsuarioAlteracao = fornecedor.NomeUsuarioAlteracao
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
                _erro = "CPF/CNPJ é obrigatório para fornecedores do Brasil.";
                return;
            }
            var fornecedor = MapearParaModel();
            if (fornecedor.Id == 0) await FornecedoresService.Inserir(fornecedor);
            else await FornecedoresService.Atualizar(fornecedor);
            await AoSalvarComSucesso.InvokeAsync(fornecedor);
            Fechar();
        }
        catch (Exception ex) { _erro = ex.Message; }
    }

    private Fornecedores MapearParaModel() => new()
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
        Ativo = _dto.Ativo,
        DataCriacao = _dto.DataCriacao
    };
}
