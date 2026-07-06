using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

public partial class EmitentesModal : PessoaModalBase<EmitentesDTO>
{
    [Inject] private EmitentesService EmitentesService { get; set; } = default!;

    [Parameter] public EventCallback<Emitentes> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback<int> AoExcluirComSucesso { get; set; }


    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;

    private ModalConfirmacao _modalConfirmacao = default!;

    private static Dictionary<string, string> RegimesTributarios => new()
    {
        { "SIMPLES_NACIONAL",         "Simples Nacional" },
        { "SIMPLES_NACIONAL_EXCESSO", "Simples Nacional — Excesso de sublimite" },
        { "LUCRO_PRESUMIDO",          "Lucro Presumido" },
        { "LUCRO_REAL",               "Lucro Real" },
        { "MEI",                      "MEI" }
    };

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new EmitentesDTO { Ativo = true };
        await CarregarCidadesAsync();
        _aberto = true;
        StateHasChanged();
    }

    public async Task AbrirEditar(int id)
    {
        _erro = string.Empty;
        _carregando = true;
        _aberto = true;
        StateHasChanged();

        var emitente = await EmitentesService.Pesquisar(id);
        if (emitente is null) { _aberto = false; StateHasChanged(); return; }

        _dto = new EmitentesDTO
        {
            Id = emitente.Id,
            TipoPessoa = emitente.TipoPessoa,
            NomeRazaoSocial = emitente.NomeRazaoSocial,
            CpfCnpj = emitente.CpfCnpj,
            RgIe = emitente.RgIe,
            ApelidoNomeFantasia = emitente.ApelidoNomeFantasia,
            IdCidade = emitente.IdCidade,
            Bairro = emitente.Bairro,
            Logradouro = emitente.Logradouro,
            Numero = emitente.Numero,
            Complemento = emitente.Complemento,
            Cep = emitente.Cep,
            Telefone = emitente.Telefone,
            Email = emitente.Email,
            InscricaoMunicipal = emitente.InscricaoMunicipal,
            RegimeTributario = emitente.RegimeTributario,
            Observacao = emitente.Observacao,
            Ativo = emitente.Ativo,
            DataCriacao = emitente.DataCriacao,
            DataUltimaAlteracao = emitente.DataUltimaAlteracao,
            NomeUsuarioAlteracao = emitente.NomeUsuarioAlteracao
        };

        await CarregarCidadesAsync();

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
                _erro = "CPF/CNPJ é obrigatório para emitentes do Brasil.";
                return;
            }
            var emitente = MapearParaModel();
            if (emitente.Id == 0) await EmitentesService.Inserir(emitente);
            else await EmitentesService.Atualizar(emitente);
            await AoSalvarComSucesso.InvokeAsync(emitente);
            Fechar();
        }
        catch (Exception ex) { _erro = ex.Message; }
    }

    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir($"Deseja realmente excluir o emitente \"{_dto.NomeRazaoSocial}\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await EmitentesService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
    private Emitentes MapearParaModel() => new()
    {
        Id = _dto.Id,
        TipoPessoa = _dto.TipoPessoa,
        NomeRazaoSocial = _dto.NomeRazaoSocial,
        CpfCnpj = _dto.CpfCnpj != null ? new string(_dto.CpfCnpj.Where(char.IsDigit).ToArray()) : null,
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
        InscricaoMunicipal = _dto.InscricaoMunicipal,
        RegimeTributario = _dto.RegimeTributario,
        Observacao = _dto.Observacao,
        Ativo = _dto.Ativo,
        DataCriacao = _dto.DataCriacao
    };
}