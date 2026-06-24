using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

public partial class ProdutosModal
{
    [Inject] private ProdutosService ProdutosService { get; set; } = default!;
    [Inject] private SkusService SkusService { get; set; } = default!;
    [Inject] private FichasTecnicasService FichasTecnicasService { get; set; } = default!;
    [Inject] private CategoriasService CategoriasService { get; set; } = default!;
    [Inject] private UnidadesMedidaService UnidadesMedidaService { get; set; } = default!;
    [Inject] private SkuAtributosChavesService SkuAtributosChavesService { get; set; } = default!;
    [Inject] private SkusAtributosValoresService SkusAtributosValoresService { get; set; } = default!;

    [Parameter] public EventCallback<Produtos> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;
    private bool _possuiVariacoes = true;
    private VariacaoGerada _skuUnico = new();

    private ProdutosDTO _dto = new();
    private List<Categorias> _categorias = new();
    private List<UnidadesMedida> _unidades = new();
    private List<SkuAtributosChaves> _chaves = new();
    private List<Produtos> _materiais = new();

    private List<OpcaoVariacao> _opcoes = new();
    private List<VariacaoGerada> _variacoes = new();

    private List<FichaTecnicaItem> _fichaItens = new();

    private SkuAtributosChavesListaModal _modalListaChaves = default!;

    private const int TamanhoMaximoSku = 10; 
    public static Dictionary<byte, string> OrigensDisponiveis => new()
    {
        { 0, "0 — Nacional" },
        { 1, "1 — Estrangeira (importação direta)" },
        { 2, "2 — Estrangeira (adquirida no mercado interno)" },
        { 3, "3 — Nacional com conteúdo superior a 40%" },
        { 4, "4 — Nacional (produção conforme processos básicos)" },
        { 5, "5 — Nacional com conteúdo inferior a 40%" },
        { 6, "6 — Estrangeira (importação direta, sem similar nacional)" },
        { 7, "7 — Estrangeira (adquirida no mercado interno, sem similar nacional)" },
        { 8, "8 — Nacional com conteúdo superior a 70%" }
    };

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new ProdutosDTO { Ativo = true, Tipo = "PRODUTO_FINAL" };
        _opcoes = new();
        _variacoes = new();
        _fichaItens = new();
        _possuiVariacoes = true;
        _skuUnico = new VariacaoGerada { Ativo = true };
        await CarregarListas();
        _aberto = true;
        StateHasChanged();
    }

    public async Task AbrirEditar(int id)
    {
        _erro = string.Empty;
        _carregando = true;
        _aberto = true;
        StateHasChanged();

        await CarregarListas();

        var produto = await ProdutosService.Pesquisar(id);
        if (produto is null)
        {
            _aberto = false;
            StateHasChanged();
            return;
        }

        _dto = new ProdutosDTO
        {
            Id = produto.Id,
            Produto = produto.Produto,
            Tipo = produto.Tipo,
            IdCategoria = produto.IdCategoria,
            IdUnidadeMedida = produto.IdUnidadeMedida,
            Ncm = produto.Ncm,
            Cest = produto.Cest,
            Origem = produto.Origem,
            Observacao = produto.Observacao,
            Ativo = produto.Ativo,
            DataCriacao = produto.DataCriacao,
            DataUltimaAlteracao = produto.DataUltimaAlteracao,
            NomeUsuarioAlteracao = produto.NomeUsuarioAlteracao
        };

        if (produto.Tipo == "MATERIA_PRIMA")
        {
            var skusExistentes = await SkusService.ListarPorProduto(id);
            await CarregarOpcoesEVariacoesDoBanco(skusExistentes);
        }

        if (produto.Tipo == "PRODUTO_FINAL")
        {
            var fichas = await FichasTecnicasService.ListarPorProduto(id);
            _fichaItens = fichas.Select(f => new FichaTecnicaItem
            {
                Id = f.Id,
                IdProdutoMaterial = f.IdProdutoMaterial,
                Quantidade = f.Quantidade,
                Observacao = f.Observacao
            }).ToList();

            var mats = await ProdutosService.ListarPorTipo("MATERIA_PRIMA");
            _materiais = mats.Where(m => m.Ativo).ToList();
        }

        _carregando = false;
        StateHasChanged();
    }

    private async Task CarregarListas()
    {
        var cats = await CategoriasService.ListarTudo();
        _categorias = cats.Where(c => c.Ativo).ToList();

        var uns = await UnidadesMedidaService.ListarTudo();
        _unidades = uns.Where(u => u.Ativo).ToList();

        var chs = await SkuAtributosChavesService.ListarTudo();
        _chaves = chs.Where(c => c.Ativo).ToList();

        var mats = await ProdutosService.ListarPorTipo("MATERIA_PRIMA");
        _materiais = mats.Where(m => m.Ativo).ToList();
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

            if (_dto.Tipo == "MATERIA_PRIMA")
            {
                if (_possuiVariacoes)
                {
                    var invalida = _variacoes.FirstOrDefault(v => v.Ativo && v.Sku.Length > TamanhoMaximoSku);
                    if (invalida != null)
                    {
                        _erro = $"O código de SKU \"{invalida.Sku}\" excede o limite de {TamanhoMaximoSku} caracteres.";
                        return;
                    }

                    if (_variacoes.Where(v => v.Ativo).Any(v => string.IsNullOrWhiteSpace(v.Sku)))
                    {
                        _erro = "Todos os SKUs precisam ter um código preenchido.";
                        return;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_skuUnico.Sku))
                    {
                        _erro = "Informe o código do SKU.";
                        return;
                    }

                    if (_skuUnico.Sku.Length > TamanhoMaximoSku)
                    {
                        _erro = $"O código de SKU \"{_skuUnico.Sku}\" excede o limite de {TamanhoMaximoSku} caracteres.";
                        return;
                    }
                }
            }

            var produto = new Produtos
            {
                Id = _dto.Id,
                Produto = _dto.Produto,
                Tipo = _dto.Tipo,
                IdCategoria = _dto.IdCategoria,
                IdUnidadeMedida = _dto.IdUnidadeMedida,
                Ncm = _dto.Ncm,
                Cest = _dto.Cest,
                Origem = _dto.Origem,
                Observacao = _dto.Observacao,
                Ativo = _dto.Ativo,
                DataCriacao = _dto.DataCriacao
            };

            int idProduto;
            if (_dto.Id == 0)
            {
                idProduto = await ProdutosService.Inserir(produto);
                produto.Id = idProduto;
            }
            else
            {
                idProduto = _dto.Id;
                await ProdutosService.Atualizar(produto);
            }

            if (_dto.Tipo == "MATERIA_PRIMA")
            {
                if (_possuiVariacoes)
                    await SalvarVariacoesAsync(idProduto);
                else
                    await SalvarSkuUnicoAsync(idProduto);
            }

            if (_dto.Tipo == "PRODUTO_FINAL")
                await SalvarFichaTecnicaAsync(idProduto);

            await AoSalvarComSucesso.InvokeAsync(produto);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private async Task SalvarSkuUnicoAsync(int idProduto)
    {
        var sku = new Skus
        {
            Sku = _skuUnico.Sku,
            IdProduto = idProduto,
            GtinEan = _skuUnico.GtinEan,
            PrecoCusto = _skuUnico.PrecoCusto,
            Estoque = _skuUnico.Estoque,
            EstoqueMinimo = _skuUnico.EstoqueMinimo,
            Ativo = _skuUnico.Ativo
        };

        if (string.IsNullOrEmpty(_skuUnico.SkuOriginalNoBanco))
            await SkusService.Inserir(sku, new List<SkusAtributosValoresRelacionamento>());
        else
            await SkusService.Atualizar(sku, new List<SkusAtributosValoresRelacionamento>());
    }

    private void AdicionarOpcao() => _opcoes.Add(new OpcaoVariacao());

    private void RemoverOpcao(int idx)
    {
        _opcoes.RemoveAt(idx);
        RecalcularVariacoes();
    }

    private void RecalcularVariacoes()
    {
        var combinacoes = VariacaoSkuGerador.GerarCombinacoes(_opcoes);
        var chavesCombinacoesAtuais = combinacoes
            .Select(c => string.Join("|", c.OrderBy(x => x.IdValor).Select(x => x.IdValor)))
            .ToHashSet();

        var variacoesAnteriores = _variacoes;
        var novasVariacoes = new List<VariacaoGerada>();

        foreach (var combinacao in combinacoes)
        {
            var chaveComparacao = string.Join("|", combinacao.OrderBy(c => c.IdValor).Select(c => c.IdValor));
            var existente = variacoesAnteriores.FirstOrDefault(v => v.ChaveComparacao == chaveComparacao);

            if (existente != null)
            {
                existente.Combinacao = combinacao;
                existente.SeraDesativada = false;
                existente.Ativo = true; 
                novasVariacoes.Add(existente);
            }
            else
            {
                novasVariacoes.Add(new VariacaoGerada
                {
                    Combinacao = combinacao,
                    Sku = VariacaoSkuGerador.SugerirCodigoSku(_dto.Produto, combinacao),
                    Ativo = true
                });
            }
        }

        foreach (var antiga in variacoesAnteriores.Where(v =>
                     !string.IsNullOrEmpty(v.SkuOriginalNoBanco) &&
                     !chavesCombinacoesAtuais.Contains(v.ChaveComparacao)))
        {
            antiga.SeraDesativada = true;
            antiga.Ativo = false;
            novasVariacoes.Add(antiga);
        }

        _variacoes = novasVariacoes;
    }

    private void AtualizarPreco(VariacaoGerada v, string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) { v.PrecoCusto = null; return; }
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            v.PrecoCusto = r;
    }

    private void AtualizarEstoque(VariacaoGerada v, string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) { v.Estoque = 0; return; }
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            v.Estoque = r;
    }

    private void AtualizarEstoqueMinimo(VariacaoGerada v, string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) { v.EstoqueMinimo = 0; return; }
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            v.EstoqueMinimo = r;
    }

    private bool _possuiVariacoesProxy
    {
        get => _possuiVariacoes;
        set => AoMudarPossuiVariacoes(value);
    }

    private void AoMudarPossuiVariacoes(bool novoValor)
    {
        if (!novoValor && _variacoes.Any(v => !string.IsNullOrEmpty(v.SkuOriginalNoBanco) && v.Ativo))
        {
            _erro = "Este produto já possui SKUs com variações cadastradas. Desative cada variação manualmente antes de desmarcar esta opção.";
            StateHasChanged();
            return;
        }

        _erro = string.Empty;
        _possuiVariacoes = novoValor;
    }

  
    private async Task CarregarOpcoesEVariacoesDoBanco(List<Skus> skusExistentes)
    {
        _opcoes = new();
        _variacoes = new();
        _skuUnico = new VariacaoGerada { Ativo = true };

        if (!skusExistentes.Any())
        {
            _possuiVariacoes = true;
            return;
        }

        bool nenhumTemAtributo = skusExistentes.All(s => !s.Atributos.Any());

        if (nenhumTemAtributo && skusExistentes.Count == 1)
        {
            _possuiVariacoes = false;
            var sku = skusExistentes.First();
            _skuUnico = new VariacaoGerada
            {
                Sku = sku.Sku,
                SkuOriginalNoBanco = sku.Sku,
                GtinEan = sku.GtinEan,
                PrecoCusto = sku.PrecoCusto,
                Estoque = sku.Estoque,
                EstoqueMinimo = sku.EstoqueMinimo,
                Ativo = sku.Ativo
            };
            return;
        }

        _possuiVariacoes = true;

        var chavesUsadas = skusExistentes
            .SelectMany(s => s.Atributos)
            .GroupBy(a => a.OValor!.IdChave)
            .Select(g => new
            {
                IdChave = g.Key,
                Nome = g.First().OValor!.OChave?.Chave ?? string.Empty
            })
            .ToList();

        foreach (var chaveUsada in chavesUsadas)
        {
            var catalogoCompleto = await SkusAtributosValoresService.ListarPorChave(chaveUsada.IdChave);

            var valoresSelecionadosAtualmente = skusExistentes
                .SelectMany(s => s.Atributos)
                .Where(a => a.OValor!.IdChave == chaveUsada.IdChave)
                .Select(a => a.OValor!.Id)
                .Distinct()
                .ToList();

            var valoresDisponiveis = catalogoCompleto
                .Where(v => v.Ativo || valoresSelecionadosAtualmente.Contains(v.Id))
                .Select(v => (v.Id, v.Valor))
                .ToList();

            _opcoes.Add(new OpcaoVariacao
            {
                IdChave = chaveUsada.IdChave,
                NomeChave = chaveUsada.Nome,
                ValoresDisponiveis = valoresDisponiveis,
                ValoresSelecionados = valoresSelecionadosAtualmente
            });
        }

        var combinacoes = VariacaoSkuGerador.GerarCombinacoes(_opcoes);

        _variacoes = combinacoes.Select(combinacao =>
        {
            var chaveComparacao = string.Join("|", combinacao.OrderBy(c => c.IdValor).Select(c => c.IdValor));

            var skuCorrespondente = skusExistentes.FirstOrDefault(s =>
            {
                var chaveDoSku = string.Join("|", s.Atributos.OrderBy(a => a.IdValor).Select(a => a.IdValor));
                return chaveDoSku == chaveComparacao;
            });

            if (skuCorrespondente != null)
            {
                return new VariacaoGerada
                {
                    Sku = skuCorrespondente.Sku,
                    SkuOriginalNoBanco = skuCorrespondente.Sku,
                    Combinacao = combinacao,
                    GtinEan = skuCorrespondente.GtinEan,
                    PrecoCusto = skuCorrespondente.PrecoCusto,
                    Estoque = skuCorrespondente.Estoque,
                    EstoqueMinimo = skuCorrespondente.EstoqueMinimo,
                    Ativo = skuCorrespondente.Ativo
                };
            }

            return new VariacaoGerada
            {
                Combinacao = combinacao,
                Sku = VariacaoSkuGerador.SugerirCodigoSku(_dto.Produto, combinacao),
                Ativo = true
            };
        }).ToList();

        var chavesAtuais = _variacoes.Select(v => v.ChaveComparacao).ToHashSet();
        foreach (var sku in skusExistentes.Where(s => s.Atributos.Any()))
        {
            var chaveDoSku = string.Join("|", sku.Atributos.OrderBy(a => a.IdValor).Select(a => a.IdValor));
            if (chavesAtuais.Contains(chaveDoSku)) continue;

            _variacoes.Add(new VariacaoGerada
            {
                Sku = sku.Sku,
                SkuOriginalNoBanco = sku.Sku,
                Combinacao = sku.Atributos.Select(a => (a.IdValor, a.OValor!.IdChave, a.OValor!.OChave?.Chave ?? string.Empty, a.OValor!.Valor)).ToList(),
                GtinEan = sku.GtinEan,
                PrecoCusto = sku.PrecoCusto,
                Estoque = sku.Estoque,
                EstoqueMinimo = sku.EstoqueMinimo,
                Ativo = sku.Ativo,
                SeraDesativada = sku.Ativo
            });
        }
    }

    private async Task SalvarVariacoesAsync(int idProduto)
    {
        var skusExistentes = await SkusService.ListarPorProduto(idProduto);
        var skusNaTela = _variacoes
            .Where(v => !string.IsNullOrEmpty(v.SkuOriginalNoBanco))
            .ToDictionary(v => v.SkuOriginalNoBanco);

        foreach (var v in _variacoes.Where(v => !string.IsNullOrEmpty(v.SkuOriginalNoBanco) && !v.Ativo))
            await SkusService.Desativar(v.SkuOriginalNoBanco);

        foreach (var skuAntigo in skusExistentes.Where(s => !skusNaTela.ContainsKey(s.Sku)))
            await SkusService.Desativar(skuAntigo.Sku);

        foreach (var v in _variacoes.Where(v => v.Ativo))
        {
            var relacionamentos = v.Combinacao.Select(c => new SkusAtributosValoresRelacionamento
            {
                IdValor = c.IdValor
            }).ToList();

            if (string.IsNullOrEmpty(v.SkuOriginalNoBanco))
            {
                var sku = new Skus
                {
                    Sku = v.Sku,
                    IdProduto = idProduto,
                    GtinEan = v.GtinEan,
                    PrecoCusto = v.PrecoCusto,
                    Estoque = v.Estoque,
                    EstoqueMinimo = v.EstoqueMinimo,
                    Ativo = v.Ativo
                };
                foreach (var r in relacionamentos) r.Sku = v.Sku;
                await SkusService.Inserir(sku, relacionamentos);
            }
            else
            {
                var sku = new Skus
                {
                    Sku = v.SkuOriginalNoBanco,
                    IdProduto = idProduto,
                    GtinEan = v.GtinEan,
                    PrecoCusto = v.PrecoCusto,
                    Estoque = v.Estoque,
                    EstoqueMinimo = v.EstoqueMinimo,
                    Ativo = v.Ativo
                };
                foreach (var r in relacionamentos) r.Sku = v.SkuOriginalNoBanco;
                await SkusService.Atualizar(sku, relacionamentos);
            }
        }
    }





    private async Task AbrirListaChaves() => await _modalListaChaves.Abrir();

    private async Task OnListaChavesAtualizada()
    {
        var chs = await SkuAtributosChavesService.ListarTudo();
        _chaves = chs.Where(c => c.Ativo).ToList();

        foreach (var opcao in _opcoes.Where(o => o.IdChave > 0))
        {
            var valores = await SkusAtributosValoresService.ListarPorChave(opcao.IdChave);
            opcao.ValoresDisponiveis = valores
            .Where(v => v.Ativo || opcao.ValoresSelecionados.Contains(v.Id))
            .Select(v => (v.Id, v.Valor))
            .ToList();

            opcao.ValoresSelecionados = opcao.ValoresSelecionados
                .Where(id => valores.Any(v => v.Id == id))
                .ToList();
        }

        RecalcularVariacoes();
    }




    private void AdicionarItemFicha() => _fichaItens.Add(new FichaTecnicaItem());

    private void RemoverItemFicha(FichaTecnicaItem item) => _fichaItens.Remove(item);

    private void AtualizarMaterial(FichaTecnicaItem item, string? texto)
    {
        if (int.TryParse(texto, out var id))
            item.IdProdutoMaterial = id;
    }

    private void AtualizarQuantidade(FichaTecnicaItem item, string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) { item.Quantidade = 0; return; }
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            item.Quantidade = r;
    }

    private async Task SalvarFichaTecnicaAsync(int idProduto)
    {
        var fichasExistentes = await FichasTecnicasService.ListarPorProduto(idProduto);

        var idsAtuaisNaTela = _fichaItens.Where(i => i.Id != 0).Select(i => i.Id).ToHashSet();

        foreach (var antiga in fichasExistentes.Where(f => !idsAtuaisNaTela.Contains(f.Id)))
            await FichasTecnicasService.Remover(antiga.Id);

        foreach (var item in _fichaItens)
        {
            var ficha = new FichasTecnicas
            {
                Id = item.Id,
                IdProduto = idProduto,
                IdProdutoMaterial = item.IdProdutoMaterial,
                Quantidade = item.Quantidade,
                Observacao = item.Observacao
            };

            if (item.Id == 0)
                await FichasTecnicasService.Inserir(ficha);
            else
                await FichasTecnicasService.Atualizar(ficha);
        }
    }
}