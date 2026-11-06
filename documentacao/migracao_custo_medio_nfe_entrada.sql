-- 2026-11-06: custo medio por item da NF-e de entrada
ALTER TABLE nfes_entrada_itens
  ADD COLUMN custo_medio DECIMAL(15,4) DEFAULT NULL AFTER custo_final;
