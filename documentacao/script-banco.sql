-- ============================================
-- Tabela: usuarios
-- ============================================
CREATE TABLE IF NOT EXISTS usuarios (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  nome VARCHAR(50) NOT NULL,
  email VARCHAR(50) NOT NULL UNIQUE,
  senha VARCHAR(60) NOT NULL,
  telefone VARCHAR(12) NULL,
  ultimo_acesso DATETIME(6) NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: paises
-- ============================================
CREATE TABLE IF NOT EXISTS paises (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  pais VARCHAR(60) NOT NULL UNIQUE,
  sigla VARCHAR(3) NOT NULL UNIQUE,
  ddi VARCHAR(5) NOT NULL,
  moeda VARCHAR(3) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: estados
-- ============================================
CREATE TABLE IF NOT EXISTS estados (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  estado VARCHAR(55) NOT NULL,
  uf VARCHAR(2) NOT NULL,
  id_pais INT NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  UNIQUE KEY uk_estado_id_pais (estado, id_pais),
  UNIQUE KEY uk_uf_id_pais (uf, id_pais),
  FOREIGN KEY (id_pais) REFERENCES paises(id) ON DELETE CASCADE,
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: cidades
-- ============================================
CREATE TABLE IF NOT EXISTS cidades (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  cidade VARCHAR(55) NOT NULL,
  id_estado INT NOT NULL,
  codigo_ibge VARCHAR(7) NOT NULL UNIQUE,
  ddd VARCHAR(2) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  UNIQUE KEY uk_cidade_id_estado (cidade, id_estado),
  FOREIGN KEY (id_estado) REFERENCES estados(id) ON DELETE CASCADE,
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: metodos_pagamento
-- ============================================
CREATE TABLE IF NOT EXISTS metodos_pagamento (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  sigla VARCHAR(5) NOT NULL UNIQUE,
  metodo VARCHAR(50) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_metodos_pagamento_metodo CHECK (LENGTH(TRIM(metodo)) > 0),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: condicoes_pagamentos
-- ============================================
CREATE TABLE IF NOT EXISTS condicoes_pagamentos (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  condicao_pagamento VARCHAR(50) NOT NULL UNIQUE,
  desconto_percentual DECIMAL(5,2) NOT NULL DEFAULT 0,
  multa_percentual DECIMAL(5,2) NOT NULL DEFAULT 0,
  taxa_juros_percentual DECIMAL(5,2) NOT NULL DEFAULT 0,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_condicoes_pagamentos_desconto CHECK (desconto_percentual >= 0 AND desconto_percentual <= 100),
  CONSTRAINT ck_condicoes_pagamentos_multa CHECK (multa_percentual >= 0 AND multa_percentual <= 100),
  CONSTRAINT ck_condicoes_pagamentos_juros CHECK (taxa_juros_percentual >= 0 AND taxa_juros_percentual <= 100),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: condicoes_pagamentos_parcelas
-- ============================================
CREATE TABLE IF NOT EXISTS condicoes_pagamentos_parcelas (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_condicao_pagamento INT NOT NULL,
  numero_parcela INT NOT NULL,
  percentual DECIMAL(7,4) NOT NULL,
  prazo_dias INT NOT NULL,
  id_metodo_pagamento INT NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  UNIQUE KEY uk_condicoes_pagamentos_parcelas (id_condicao_pagamento, numero_parcela),
  CONSTRAINT ck_condicoes_pagamentos_parcelas_numero CHECK (numero_parcela > 0),
  CONSTRAINT ck_condicoes_pagamentos_parcelas_percentual CHECK (percentual > 0 AND percentual <= 100),
  CONSTRAINT ck_condicoes_pagamentos_parcelas_prazo CHECK (prazo_dias >= 0),
  FOREIGN KEY (id_condicao_pagamento) REFERENCES condicoes_pagamentos(id),
  FOREIGN KEY (id_metodo_pagamento) REFERENCES metodos_pagamento(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: clientes
-- ============================================
CREATE TABLE IF NOT EXISTS clientes (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  tipo_pessoa ENUM('FISICA', 'JURIDICA') NOT NULL,
  nome_razao_social VARCHAR(60) NOT NULL,
  cpf_cnpj VARCHAR(14) NULL UNIQUE,
  rg_ie VARCHAR(15) NULL,
  apelido_nome_fantasia VARCHAR(40) NULL,
  id_cidade INT NOT NULL,
  bairro VARCHAR(50) NULL,
  logradouro VARCHAR(50) NULL,
  numero VARCHAR(5) NULL,
  complemento VARCHAR(50) NULL,
  cep VARCHAR(8) NULL,
  telefone VARCHAR(12) NULL,
  email VARCHAR(50) NULL,
  limite_credito DECIMAL(7,2) NOT NULL DEFAULT 0,
  id_condicao_pagamento INT NULL,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_clientes_nome CHECK (LENGTH(TRIM(nome_razao_social)) > 0),
  CONSTRAINT ck_clientes_cpf_cnpj CHECK (cpf_cnpj IS NULL OR LENGTH(TRIM(cpf_cnpj)) > 0),
  CONSTRAINT ck_clientes_limite_credito CHECK (limite_credito >= 0),
  FOREIGN KEY (id_cidade) REFERENCES cidades(id),
  FOREIGN KEY (id_condicao_pagamento) REFERENCES condicoes_pagamentos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: emitentes
-- ============================================
CREATE TABLE IF NOT EXISTS emitentes (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  tipo_pessoa ENUM('FISICA', 'JURIDICA') NOT NULL,
  nome_razao_social VARCHAR(60) NOT NULL,
  cpf_cnpj VARCHAR(14) NULL UNIQUE,
  rg_ie VARCHAR(15) NULL,
  apelido_nome_fantasia VARCHAR(40) NULL,
  id_cidade INT NOT NULL,
  bairro VARCHAR(50) NULL,
  logradouro VARCHAR(50) NULL,
  numero VARCHAR(5) NULL,
  complemento VARCHAR(50) NULL,
  cep VARCHAR(8) NULL,
  telefone VARCHAR(12) NULL,
  email VARCHAR(50) NULL,
  inscricao_municipal VARCHAR(15) NULL,
  regime_tributario ENUM('SIMPLES_NACIONAL','SIMPLES_NACIONAL_EXCESSO','LUCRO_PRESUMIDO','LUCRO_REAL','MEI') NULL,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_emitentes_nome CHECK (LENGTH(TRIM(nome_razao_social)) > 0),
  CONSTRAINT ck_emitentes_cpf_cnpj CHECK (cpf_cnpj IS NULL OR LENGTH(TRIM(cpf_cnpj)) > 0),
  FOREIGN KEY (id_cidade) REFERENCES cidades(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: fornecedores
-- ============================================
CREATE TABLE IF NOT EXISTS fornecedores (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  tipo_pessoa ENUM('FISICA', 'JURIDICA') NOT NULL,
  nome_razao_social VARCHAR(60) NOT NULL,
  cpf_cnpj VARCHAR(14) NULL UNIQUE,
  rg_ie VARCHAR(15) NULL,
  apelido_nome_fantasia VARCHAR(40) NULL,
  id_cidade INT NOT NULL,
  bairro VARCHAR(50) NULL,
  logradouro VARCHAR(50) NULL,
  numero VARCHAR(5) NULL,
  complemento VARCHAR(50) NULL,
  cep VARCHAR(8) NULL,
  telefone VARCHAR(12) NULL,
  email VARCHAR(50) NULL,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_fornecedores_nome CHECK (LENGTH(TRIM(nome_razao_social)) > 0),
  CONSTRAINT ck_fornecedores_cpf_cnpj CHECK (cpf_cnpj IS NULL OR LENGTH(TRIM(cpf_cnpj)) > 0),
  FOREIGN KEY (id_cidade) REFERENCES cidades(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: transportadoras
-- ============================================
CREATE TABLE IF NOT EXISTS transportadoras (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  tipo_pessoa ENUM('FISICA', 'JURIDICA') NOT NULL,
  nome_razao_social VARCHAR(60) NOT NULL,
  cpf_cnpj VARCHAR(14) NULL UNIQUE,
  rg_ie VARCHAR(15) NULL,
  apelido_nome_fantasia VARCHAR(40) NULL,
  id_cidade INT NOT NULL,
  bairro VARCHAR(50) NULL,
  logradouro VARCHAR(50) NULL,
  numero VARCHAR(5) NULL,
  complemento VARCHAR(50) NULL,
  cep VARCHAR(8) NULL,
  telefone VARCHAR(12) NULL,
  email VARCHAR(50) NULL,
  rntrc VARCHAR(8) NULL,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_transportadoras_nome CHECK (LENGTH(TRIM(nome_razao_social)) > 0),
  CONSTRAINT ck_transportadoras_cpf_cnpj CHECK (cpf_cnpj IS NULL OR LENGTH(TRIM(cpf_cnpj)) > 0),
  FOREIGN KEY (id_cidade) REFERENCES cidades(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: veiculos
-- ============================================
CREATE TABLE IF NOT EXISTS veiculos (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_transportadora INT NULL,
  id_estado INT NOT NULL,
  placa VARCHAR(7) NOT NULL,
  rntrc VARCHAR(8) NULL,
  renavam VARCHAR(11) NULL,
  tipo_veiculo VARCHAR(50) NULL,
  marca_modelo VARCHAR(50) NULL,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_veiculos_placa CHECK (LENGTH(TRIM(placa)) > 0),
  FOREIGN KEY (id_transportadora) REFERENCES transportadoras(id),
  FOREIGN KEY (id_estado) REFERENCES estados(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: categorias
-- ============================================
CREATE TABLE IF NOT EXISTS categorias (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  categoria VARCHAR(30) NOT NULL UNIQUE,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: unidades_medida
-- ============================================
CREATE TABLE IF NOT EXISTS unidades_medida (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  sigla VARCHAR(5) NOT NULL UNIQUE,
  unidade_medida VARCHAR(30) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_unidades_medida_sigla CHECK (LENGTH(TRIM(sigla)) > 0),
  CONSTRAINT ck_unidades_medida_descricao CHECK (LENGTH(TRIM(unidade_medida)) > 0),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: produtos
-- ============================================
CREATE TABLE IF NOT EXISTS produtos (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  produto VARCHAR(70) NOT NULL,
  referencia VARCHAR(20) NULL,
  preco_venda DECIMAL(7,2) NULL,
  estoque_minimo DECIMAL(10,4) NOT NULL DEFAULT 0,
  valor_ultima_compra DECIMAL(7,2) NULL,
  tipo ENUM('PRODUTO_FINAL', 'MATERIA_PRIMA') NOT NULL DEFAULT 'PRODUTO_FINAL',
  id_categoria INT NOT NULL,
  id_unidade_medida INT NOT NULL,
  ncm VARCHAR(8) NULL,
  cest VARCHAR(7) NULL,
  origem TINYINT NOT NULL DEFAULT 0,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (id_categoria) REFERENCES categorias(id),
  FOREIGN KEY (id_unidade_medida) REFERENCES unidades_medida(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: vendas
-- ============================================
CREATE TABLE IF NOT EXISTS vendas (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_emitente INT NOT NULL,
  id_cliente INT NOT NULL,
  data_venda DATETIME(6) NOT NULL,
  valor_total DECIMAL(7,2) NOT NULL DEFAULT 0,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_vendas_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_emitente) REFERENCES emitentes(id),
  FOREIGN KEY (id_cliente) REFERENCES clientes(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: vendas_itens
-- ============================================
CREATE TABLE IF NOT EXISTS vendas_itens (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_venda INT NOT NULL,
  id_produto INT NOT NULL,
  quantidade DECIMAL(10,4) NOT NULL,
  valor_unitario DECIMAL(7,4) NOT NULL,
  valor_desconto DECIMAL(7,4) NOT NULL DEFAULT 0,
  valor_total DECIMAL(7,4) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_vendas_itens_quantidade CHECK (quantidade > 0),
  CONSTRAINT ck_vendas_itens_valor_unitario CHECK (valor_unitario >= 0),
  CONSTRAINT ck_vendas_itens_valor_desconto CHECK (valor_desconto >= 0),
  CONSTRAINT ck_vendas_itens_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_venda) REFERENCES vendas(id),
  FOREIGN KEY (id_produto) REFERENCES produtos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: pedidos
-- ============================================
CREATE TABLE IF NOT EXISTS pedidos (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_cliente INT NOT NULL,
  id_emitente INT NOT NULL,
  data_pedido DATETIME(6) NOT NULL,
  data_previsao_entrega DATETIME(6) NULL,
  data_entrega DATETIME(6) NULL,
  `status` ENUM(
    'AGUARDANDO_APROVACAO_ARTE',
    'ARTE_APROVADA',
    'EM_PRODUCAO',
    'PRONTO_RETIRADA',
    'ENVIADO',
    'ENTREGUE',
    'CANCELADO'
  ) NOT NULL DEFAULT 'AGUARDANDO_APROVACAO_ARTE',
  id_venda INT NULL,
  valor_total DECIMAL(7,2) NOT NULL DEFAULT 0,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_pedidos_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_cliente) REFERENCES clientes(id),
  FOREIGN KEY (id_emitente) REFERENCES emitentes(id),
  FOREIGN KEY (id_venda) REFERENCES vendas(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: pedidos_itens
-- ============================================
CREATE TABLE IF NOT EXISTS pedidos_itens (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_pedido INT NOT NULL,
  id_produto INT NOT NULL,
  descricao_personalizacao TEXT NULL,
  quantidade DECIMAL(10,4) NOT NULL,
  valor_unitario DECIMAL(7,4) NOT NULL,
  valor_desconto DECIMAL(7,4) NOT NULL DEFAULT 0,
  valor_total DECIMAL(7,4) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_pedidos_itens_quantidade CHECK (quantidade > 0),
  CONSTRAINT ck_pedidos_itens_valor_unitario CHECK (valor_unitario >= 0),
  CONSTRAINT ck_pedidos_itens_valor_desconto CHECK (valor_desconto >= 0),
  CONSTRAINT ck_pedidos_itens_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_pedido) REFERENCES pedidos(id),
  FOREIGN KEY (id_produto) REFERENCES produtos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: ordens_servico
-- ============================================
CREATE TABLE IF NOT EXISTS ordens_servico (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_pedido INT NOT NULL,
  id_emitente INT NOT NULL,
  id_cliente INT NOT NULL,
  data_abertura DATETIME(6) NOT NULL,
  data_previsao_conclusao DATETIME(6) NULL,
  data_conclusao DATETIME(6) NULL,
  `status` ENUM(
    'ABERTA',
    'EM_ANDAMENTO',
    'AGUARDANDO_MATERIAL',
    'CONCLUIDA',
    'CANCELADA'
  ) NOT NULL DEFAULT 'ABERTA',
  valor_total DECIMAL(7,2) NOT NULL DEFAULT 0,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_ordens_servico_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_pedido) REFERENCES pedidos(id),
  FOREIGN KEY (id_emitente) REFERENCES emitentes(id),
  FOREIGN KEY (id_cliente) REFERENCES clientes(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: ordens_servico_itens
-- ============================================
CREATE TABLE IF NOT EXISTS ordens_servico_itens (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_ordem_servico INT NOT NULL,
  id_pedido_item INT NOT NULL,
  id_produto INT NOT NULL,
  descricao_personalizacao TEXT NULL,
  quantidade DECIMAL(10,4) NOT NULL,
  valor_unitario DECIMAL(7,4) NOT NULL,
  valor_desconto DECIMAL(7,4) NOT NULL DEFAULT 0,
  valor_total DECIMAL(7,4) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_ordens_servico_itens_quantidade CHECK (quantidade > 0),
  CONSTRAINT ck_ordens_servico_itens_valor_unitario CHECK (valor_unitario >= 0),
  CONSTRAINT ck_ordens_servico_itens_valor_desconto CHECK (valor_desconto >= 0),
  CONSTRAINT ck_ordens_servico_itens_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_ordem_servico) REFERENCES ordens_servico(id),
  FOREIGN KEY (id_pedido_item) REFERENCES pedidos_itens(id),
  FOREIGN KEY (id_produto) REFERENCES produtos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: movimentacoes_estoque
-- ============================================
CREATE TABLE IF NOT EXISTS movimentacoes_estoque (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  data_movimentacao DATETIME(6) NOT NULL,
  tipo_movimentacao ENUM(
    'ENTRADA',
    'SAIDA',
    'AJUSTE',
    'VENDA',
    'PRODUCAO'
  ) NOT NULL,
  id_usuario INT NULL,
  id_nfe INT NULL,
  id_venda INT NULL,
  id_ordem_servico INT NULL,
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_movimentacoes_estoque_venda CHECK (
    (tipo_movimentacao <> 'VENDA') OR
    (tipo_movimentacao = 'VENDA' AND id_venda IS NOT NULL)
  ),
  CONSTRAINT ck_movimentacoes_estoque_producao CHECK (
    (tipo_movimentacao <> 'PRODUCAO') OR
    (tipo_movimentacao = 'PRODUCAO' AND id_ordem_servico IS NOT NULL)
  ),
  FOREIGN KEY (id_usuario) REFERENCES usuarios(id),
  FOREIGN KEY (id_venda) REFERENCES vendas(id),
  FOREIGN KEY (id_ordem_servico) REFERENCES ordens_servico(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: movimentacoes_estoque_itens
-- ============================================
CREATE TABLE IF NOT EXISTS movimentacoes_estoque_itens (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_movimentacao_estoque INT NOT NULL,
  id_produto INT NOT NULL,
  quantidade DECIMAL(10,4) NOT NULL,
  custo_unitario DECIMAL(15,4) NOT NULL DEFAULT 0,
  saldo_apos DECIMAL(15,4) NOT NULL DEFAULT 0,
  custo_medio DECIMAL(15,4) NOT NULL DEFAULT 0,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_movimentacoes_estoque_itens_quantidade CHECK (quantidade > 0),
  CONSTRAINT ck_movimentacoes_estoque_itens_custo CHECK (custo_unitario >= 0),
  CONSTRAINT ck_movimentacoes_estoque_itens_saldo_apos CHECK (saldo_apos >= 0),
  CONSTRAINT ck_movimentacoes_estoque_itens_custo_medio CHECK (custo_medio >= 0),
  FOREIGN KEY (id_movimentacao_estoque) REFERENCES movimentacoes_estoque(id),
  FOREIGN KEY (id_produto) REFERENCES produtos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: nfes
-- ============================================
CREATE TABLE IF NOT EXISTS nfes (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  chave_acesso VARCHAR(44) NULL UNIQUE,
  numero INT NOT NULL,
  serie SMALLINT NOT NULL,
  data_emissao DATETIME(6) NOT NULL,
  data_saida DATETIME(6) NULL,
  id_emitente INT NOT NULL,
  emitente_nome_razao_social VARCHAR(150) NOT NULL,
  emitente_cpf_cnpj VARCHAR(14) NOT NULL,
  emitente_rg_ie VARCHAR(15) NULL,
  emitente_apelido_nome_fantasia VARCHAR(100) NULL,
  emitente_endereco VARCHAR(50) NULL,
  emitente_bairro VARCHAR(50) NULL,
  emitente_telefone VARCHAR(20) NULL,
  emitente_email VARCHAR(50) NULL,
  id_cliente INT NOT NULL,
  cliente_nome_razao_social VARCHAR(150) NOT NULL,
  cliente_cpf_cnpj VARCHAR(14) NULL,
  cliente_rg_ie VARCHAR(15) NULL,
  cliente_apelido_nome_fantasia VARCHAR(100) NULL,
  cliente_endereco VARCHAR(150) NULL,
  cliente_bairro VARCHAR(100) NULL,
  cliente_telefone VARCHAR(20) NULL,
  cliente_email VARCHAR(254) NULL,
  tipo_operacao ENUM('ENTRADA', 'SAIDA') NOT NULL,
  status_nfe ENUM('RASCUNHO', 'EMITIDA', 'CANCELADA') NOT NULL DEFAULT 'RASCUNHO',
  id_transportadora INT NULL,
  id_venda INT NULL,
  valor_produtos DECIMAL(7,2) NOT NULL DEFAULT 0,
  valor_desconto DECIMAL(7,2) NOT NULL DEFAULT 0,
  valor_frete DECIMAL(7,2) NOT NULL DEFAULT 0,
  valor_seguro DECIMAL(7,2) NOT NULL DEFAULT 0,
  valor_outras_despesas DECIMAL(7,2) NOT NULL DEFAULT 0,
  valor_total DECIMAL(7,2) NOT NULL DEFAULT 0,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT uk_nfes_emitente_serie_numero UNIQUE (id_emitente, serie, numero),
  CONSTRAINT ck_nfes_chave_acesso CHECK (
    (status_nfe = 'RASCUNHO' AND chave_acesso IS NULL)
    OR (status_nfe IN ('EMITIDA', 'CANCELADA') AND chave_acesso REGEXP '^[0-9]{44}$')
  ),
  CONSTRAINT ck_nfes_emitente_nome CHECK (LENGTH(TRIM(emitente_nome_razao_social)) > 0),
  CONSTRAINT ck_nfes_emitente_cpf_cnpj CHECK (LENGTH(TRIM(emitente_cpf_cnpj)) > 0),
  CONSTRAINT ck_nfes_cliente_nome CHECK (LENGTH(TRIM(cliente_nome_razao_social)) > 0),
  CONSTRAINT ck_nfes_valor_produtos CHECK (valor_produtos >= 0),
  CONSTRAINT ck_nfes_valor_desconto CHECK (valor_desconto >= 0),
  CONSTRAINT ck_nfes_valor_frete CHECK (valor_frete >= 0),
  CONSTRAINT ck_nfes_valor_seguro CHECK (valor_seguro >= 0),
  CONSTRAINT ck_nfes_valor_outras CHECK (valor_outras_despesas >= 0),
  CONSTRAINT ck_nfes_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_emitente) REFERENCES emitentes(id),
  FOREIGN KEY (id_cliente) REFERENCES clientes(id),
  FOREIGN KEY (id_transportadora) REFERENCES transportadoras(id),
  FOREIGN KEY (id_venda) REFERENCES vendas(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: nfes_itens
-- ============================================
CREATE TABLE IF NOT EXISTS nfes_itens (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_nfe INT NOT NULL,
  numero_item INT NOT NULL,
  id_produto INT NULL,
  descricao_item VARCHAR(150) NOT NULL,
  id_unidade_medida INT NOT NULL,
  cfop VARCHAR(5) NOT NULL,
  quantidade DECIMAL(10,4) NOT NULL,
  valor_unitario DECIMAL(7,4) NOT NULL,
  valor_desconto DECIMAL(7,4) NOT NULL DEFAULT 0,
  valor_total DECIMAL(7,4) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  UNIQUE KEY uk_nfes_itens_nfe_numero (id_nfe, numero_item),
  CONSTRAINT ck_nfes_itens_numero_item CHECK (numero_item > 0),
  CONSTRAINT ck_nfes_itens_quantidade CHECK (quantidade > 0),
  CONSTRAINT ck_nfes_itens_valor_unitario CHECK (valor_unitario >= 0),
  CONSTRAINT ck_nfes_itens_valor_desconto CHECK (valor_desconto >= 0),
  CONSTRAINT ck_nfes_itens_valor_total CHECK (valor_total >= 0),
  FOREIGN KEY (id_nfe) REFERENCES nfes(id),
  FOREIGN KEY (id_produto) REFERENCES produtos(id),
  FOREIGN KEY (id_unidade_medida) REFERENCES unidades_medida(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: nfes_pagamentos
-- ============================================
CREATE TABLE IF NOT EXISTS nfes_pagamentos (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_nfe INT NOT NULL,
  id_metodo_pagamento INT NOT NULL,
  indicador_pagamento ENUM('A_VISTA', 'A_PRAZO') NOT NULL,
  valor_pagamento DECIMAL(7,2) NOT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_nfes_pagamentos_valor CHECK (valor_pagamento >= 0),
  FOREIGN KEY (id_nfe) REFERENCES nfes(id),
  FOREIGN KEY (id_metodo_pagamento) REFERENCES metodos_pagamento(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: nfes_transportes
-- ============================================
CREATE TABLE IF NOT EXISTS nfes_transportes (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_nfe INT NOT NULL UNIQUE,
  modalidade_frete ENUM('EMITENTE','DESTINATARIO','TERCEIROS','REMETENTE_PROPRIO','DESTINATARIO_PROPRIO','SEM_TRANSPORTE') NOT NULL,
  transportadora_nome_razao_social VARCHAR(150) NULL,
  transportadora_cpf_cnpj VARCHAR(14) NULL,
  transportadora_rg_ie VARCHAR(15) NULL,
  id_veiculo INT NULL,
  veiculo_placa VARCHAR(10) NULL,
  veiculo_uf CHAR(2) NULL,
  veiculo_rntrc VARCHAR(20) NULL,
  quantidade_volumes INT NULL,
  especie_volume VARCHAR(50) NULL,
  marca_volume VARCHAR(50) NULL,
  numeracao_volume VARCHAR(50) NULL,
  peso_bruto DECIMAL(4,3) NULL,
  peso_liquido DECIMAL(14,3) NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_nfes_transportes_transportadora_nome CHECK (transportadora_nome_razao_social IS NULL OR LENGTH(TRIM(transportadora_nome_razao_social)) > 0),
  CONSTRAINT ck_nfes_transportes_transportadora_cpf_cnpj CHECK (transportadora_cpf_cnpj IS NULL OR LENGTH(TRIM(transportadora_cpf_cnpj)) > 0),
  CONSTRAINT ck_nfes_transportes_veiculo_uf CHECK (veiculo_uf IS NULL OR veiculo_uf REGEXP '^[A-Z]{2}$'),
  CONSTRAINT ck_nfes_transportes_quantidade_volumes CHECK (quantidade_volumes IS NULL OR quantidade_volumes >= 0),
  CONSTRAINT ck_nfes_transportes_peso_bruto CHECK (peso_bruto IS NULL OR peso_bruto >= 0),
  CONSTRAINT ck_nfes_transportes_peso_liquido CHECK (peso_liquido IS NULL OR peso_liquido >= 0),
  FOREIGN KEY (id_nfe) REFERENCES nfes(id),
  FOREIGN KEY (id_veiculo) REFERENCES veiculos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: nfes_informacoes_adicionais
-- ============================================
CREATE TABLE IF NOT EXISTS nfes_informacoes_adicionais (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_nfe INT NOT NULL UNIQUE,
  informacoes_fisco TEXT NULL,
  informacoes_complementares TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  FOREIGN KEY (id_nfe) REFERENCES nfes(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: contas_pagar
-- ============================================
CREATE TABLE IF NOT EXISTS contas_pagar (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_fornecedor INT NOT NULL,
  id_nfe INT NULL,
  id_condicao_pagamento INT NULL,
  descricao VARCHAR(150) NOT NULL,
  data_emissao DATE NULL,
  data_vencimento DATE NULL,
  valor_original DECIMAL(7,2) NOT NULL,
  valor_saldo DECIMAL(7,2) NOT NULL,
  `status` ENUM('ABERTO','PARCIAL','PAGO','CANCELADO') NOT NULL DEFAULT 'ABERTO',
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_contas_pagar_valor_original CHECK (valor_original >= 0),
  CONSTRAINT ck_contas_pagar_valor_saldo CHECK (valor_saldo >= 0),
  FOREIGN KEY (id_fornecedor) REFERENCES fornecedores(id),
  FOREIGN KEY (id_nfe) REFERENCES nfes(id),
  FOREIGN KEY (id_condicao_pagamento) REFERENCES condicoes_pagamentos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: contas_pagar_parcelas
-- ============================================
CREATE TABLE IF NOT EXISTS contas_pagar_parcelas (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_conta_pagar INT NOT NULL,
  numero_parcela INT NOT NULL,
  data_vencimento DATE NULL,
  valor_parcela DECIMAL(7,2) NOT NULL,
  valor_pago DECIMAL(7,2) NOT NULL DEFAULT 0,
  `status` ENUM('ABERTO','PARCIAL','PAGO','CANCELADO') NOT NULL DEFAULT 'ABERTO',
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  UNIQUE KEY uk_contas_pagar_parcelas (id_conta_pagar, numero_parcela),
  CONSTRAINT ck_contas_pagar_parcelas_numero CHECK (numero_parcela > 0),
  CONSTRAINT ck_contas_pagar_parcelas_valor_parcela CHECK (valor_parcela >= 0),
  CONSTRAINT ck_contas_pagar_parcelas_valor_pago CHECK (valor_pago >= 0),
  FOREIGN KEY (id_conta_pagar) REFERENCES contas_pagar(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: contas_receber
-- ============================================
CREATE TABLE IF NOT EXISTS contas_receber (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_cliente INT NOT NULL,
  id_venda INT NULL,
  id_nfe INT NULL,
  id_condicao_pagamento INT NULL,
  descricao VARCHAR(50) NOT NULL,
  data_emissao DATE NULL,
  data_vencimento DATE NULL,
  valor_original DECIMAL(7,2) NOT NULL,
  valor_saldo DECIMAL(7,2) NOT NULL,
  `status` ENUM('ABERTO','PARCIAL','PAGO','CANCELADO') NOT NULL DEFAULT 'ABERTO',
  observacao TEXT NULL,
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  CONSTRAINT ck_contas_receber_valor_original CHECK (valor_original >= 0),
  CONSTRAINT ck_contas_receber_valor_saldo CHECK (valor_saldo >= 0),
  FOREIGN KEY (id_cliente) REFERENCES clientes(id),
  FOREIGN KEY (id_venda) REFERENCES vendas(id),
  FOREIGN KEY (id_nfe) REFERENCES nfes(id),
  FOREIGN KEY (id_condicao_pagamento) REFERENCES condicoes_pagamentos(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabela: contas_receber_parcelas
-- ============================================
CREATE TABLE IF NOT EXISTS contas_receber_parcelas (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_conta_receber INT NOT NULL,
  numero_parcela INT NOT NULL,
  data_vencimento DATE NULL,
  valor_parcela DECIMAL(7,2) NOT NULL,
  valor_recebido DECIMAL(7,2) NOT NULL DEFAULT 0,
  `status` ENUM('ABERTO','PARCIAL','PAGO','CANCELADO') NOT NULL DEFAULT 'ABERTO',
  data_criacao DATETIME(6) NOT NULL,
  data_ultima_alteracao DATETIME(6) NOT NULL,
  id_usuario_ultima_alteracao INT NOT NULL,
  ativo BOOLEAN NOT NULL DEFAULT TRUE,
  UNIQUE KEY uk_contas_receber_parcelas (id_conta_receber, numero_parcela),
  CONSTRAINT ck_contas_receber_parcelas_numero CHECK (numero_parcela > 0),
  CONSTRAINT ck_contas_receber_parcelas_valor_parcela CHECK (valor_parcela >= 0),
  CONSTRAINT ck_contas_receber_parcelas_valor_recebido CHECK (valor_recebido >= 0),
  FOREIGN KEY (id_conta_receber) REFERENCES contas_receber(id),
  FOREIGN KEY (id_usuario_ultima_alteracao) REFERENCES usuarios(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;