CREATE DATABASE  IF NOT EXISTS `papelariabd` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `papelariabd`;
-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: papelariabd
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `categorias`
--

DROP TABLE IF EXISTS `categorias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categorias` (
  `id` int NOT NULL AUTO_INCREMENT,
  `categoria` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `categoria` (`categoria`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `categorias_ibfk_1` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categorias`
--

LOCK TABLES `categorias` WRITE;
/*!40000 ALTER TABLE `categorias` DISABLE KEYS */;
INSERT INTO `categorias` VALUES (1,'PAPÉIS','2026-06-12 20:10:24.000000','2026-07-01 22:30:58.902520',1,1),(2,'TECIDOS E LAMINAÇÃO','2026-06-12 20:10:24.000000','2026-07-01 22:31:01.235347',1,1),(3,'FECHAMENTOS','2026-06-12 20:10:24.000000','2026-07-01 22:31:03.452905',1,1),(4,'ACESSÓRIOS','2026-06-12 20:10:24.000000','2026-07-01 22:31:05.228713',1,1),(5,'Produtos Finais','2026-06-12 20:10:24.000000','2026-06-24 22:35:37.403120',1,0),(6,'AGENDAS','2026-06-24 22:35:42.680774','2026-07-01 22:31:08.251455',1,1);
/*!40000 ALTER TABLE `categorias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cidades`
--

DROP TABLE IF EXISTS `cidades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cidades` (
  `id` int NOT NULL AUTO_INCREMENT,
  `cidade` varchar(55) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_estado` int NOT NULL,
  `codigo_ibge` varchar(7) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ddd` varchar(2) COLLATE utf8mb4_unicode_ci NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `codigo_ibge` (`codigo_ibge`),
  UNIQUE KEY `uk_cidade_id_estado` (`cidade`,`id_estado`),
  KEY `id_estado` (`id_estado`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `cidades_ibfk_1` FOREIGN KEY (`id_estado`) REFERENCES `estados` (`id`) ON DELETE CASCADE,
  CONSTRAINT `cidades_ibfk_2` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cidades`
--

LOCK TABLES `cidades` WRITE;
/*!40000 ALTER TABLE `cidades` DISABLE KEYS */;
INSERT INTO `cidades` VALUES (1,'FOZ DO IGUAÇU',1,'4108304','45','2026-06-06 17:23:32.000000','2026-07-01 22:24:47.774263',1,1),(2,'SÃO PAULO',2,'3550308','11','2026-06-06 17:23:32.000000','2026-07-01 22:24:49.725661',1,1),(3,'RIO DE JANEIRO',3,'3304557','21','2026-06-06 17:23:32.000000','2026-07-01 22:24:52.100559',1,1),(4,'FLORIANÓPOLIS',4,'4205407','48','2026-06-06 17:23:32.000000','2026-07-01 22:24:54.399986',1,1),(5,'PORTO ALEGRE',5,'4314902','51','2026-06-06 17:23:32.000000','2026-07-01 22:24:59.567803',1,1),(6,'MISSAL',1,'4116059','45','2026-06-19 15:24:04.584654','2026-07-01 22:24:57.265059',1,1),(7,'LAS VEGAS',6,'2496','72','2026-07-01 18:00:21.690597','2026-07-01 22:25:01.666761',1,1);
/*!40000 ALTER TABLE `cidades` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clientes`
--

DROP TABLE IF EXISTS `clientes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clientes` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tipo_pessoa` enum('FISICA','JURIDICA') COLLATE utf8mb4_unicode_ci NOT NULL,
  `nome_razao_social` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `cpf_cnpj` varchar(14) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rg_ie` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apelido_nome_fantasia` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cidade` int NOT NULL,
  `bairro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `logradouro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `numero` varchar(5) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `complemento` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cep` varchar(8) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `telefone` varchar(12) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `limite_credito` decimal(7,2) NOT NULL DEFAULT '0.00',
  `id_condicao_pagamento` int DEFAULT NULL,
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_cnpj` (`cpf_cnpj`),
  KEY `id_cidade` (`id_cidade`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  KEY `id_condicao_pagamento` (`id_condicao_pagamento`),
  CONSTRAINT `clientes_ibfk_1` FOREIGN KEY (`id_cidade`) REFERENCES `cidades` (`id`),
  CONSTRAINT `clientes_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `clientes_ibfk_4` FOREIGN KEY (`id_condicao_pagamento`) REFERENCES `condicoes_pagamentos` (`id`),
  CONSTRAINT `ck_clientes_cpf_cnpj` CHECK (((`cpf_cnpj` is null) or (length(trim(`cpf_cnpj`)) > 0))),
  CONSTRAINT `ck_clientes_limite_credito` CHECK ((`limite_credito` >= 0)),
  CONSTRAINT `ck_clientes_nome` CHECK ((length(trim(`nome_razao_social`)) > 0))
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clientes`
--

LOCK TABLES `clientes` WRITE;
/*!40000 ALTER TABLE `clientes` DISABLE KEYS */;
INSERT INTO `clientes` VALUES (1,'FISICA','JOÃO DA SILVA','12345678901','123456789','JOÃO',1,'CENTRO','RUA DAS FLORES','100',NULL,'85851000','45999990001','joao@email.com',1000.00,3,NULL,'2026-06-13 13:11:06.000000','2026-07-04 21:13:42.476130',1,1),(2,'JURIDICA','EMPRESA ALPHA LTDA','11222333000181','111222333','ALPHA',2,'JARDIM AMÉRICA','AV. PAULISTA','200','SALA 1','01310100','11999990002','alpha@empresa.com',500.00,NULL,NULL,'2026-06-13 13:11:06.000000','2026-07-01 22:25:33.894235',1,1),(3,'FISICA','MARIA OLIVEIRA','23456789012','234567890','MARIA',3,'COPACABANA','RUA BARATA RIBEIRO','300',NULL,'22011010','21999990003','maria@email.com',2000.00,NULL,NULL,'2026-06-13 13:11:06.000000','2026-07-01 22:25:36.837355',1,1),(4,'JURIDICA','BETA COMÉRCIO S.A.','22333444000192','222333444','BETA',4,'CENTRO','RUA FELIPE SCHMIDT','400',NULL,'88010000','48999990004','beta@comercio.com',200.00,NULL,NULL,'2026-06-13 13:11:06.000000','2026-07-01 22:25:43.172153',1,1),(5,'FISICA','CARLOS PEREIRA','34567890123','345678901','CARLOS',5,'MOINHOS DE VENTO','RUA PADRE CHAGAS','500','APTO 2','90570080','51999990005','carlos@email.com',500.00,1,NULL,'2026-06-13 13:11:06.000000','2026-07-04 21:13:58.427860',1,1),(6,'JURIDICA','GAMA SERVIÇOS EIRELI','33444555000103','333444555','GAMA',1,'VILA PORTES','AV. DAS CATARATAS','600',NULL,'85853000','45999990006','gama@servicos.com',3000.00,NULL,NULL,'2026-06-13 13:11:06.000000','2026-07-01 22:25:49.317100',1,1),(7,'FISICA','ANA COSTA','45678901234','456789012','ANA',2,'VILA MARIANA','RUA DOMINGOS DE MORAIS','700',NULL,'04010000','11999990007','ana@email.com',1500.00,2,NULL,'2026-06-13 13:11:06.000000','2026-07-04 21:14:07.089817',1,1),(8,'JURIDICA','DELTA TECNOLOGIA ME','44555666000114','444555666','DELTA',3,'BARRA DA TIJUCA','AV. DAS AMÉRICAS','800','BLOCO B','22640102','21999990008','delta@tech.com',1300.00,NULL,NULL,'2026-06-13 13:11:06.000000','2026-07-01 22:25:54.078247',1,1),(9,'FISICA','PEDRO SANTOS','56789012345','567890123','PEDRO',4,'TRINDADE','RUA JOÃO PINTO','900',NULL,'88036001','48999990009','pedro@email.com',800.00,NULL,NULL,'2026-06-13 13:11:06.000000','2026-07-01 22:25:56.678273',1,0),(10,'JURIDICA','ÉPSILON DISTRIBUIDORA LTDA','55666777000125','555666777','ÉPSILON',5,'CENTRO HISTÓRICO','RUA DOS ANDRADAS','10',NULL,'90020006','51999990010','epsilon@distrib.com',4000.00,NULL,NULL,'2026-06-13 13:11:06.000000','2026-07-01 22:25:59.621545',1,1),(11,'FISICA','BIANCA EDUARDA FRANÇA','11111111111',NULL,NULL,6,'RENASCER','RUA MACHADO','123',NULL,'85890000','45988334455','bianca@gmail.com',1000.00,NULL,NULL,'2026-06-13 14:33:42.732966','2026-07-01 22:26:01.717398',1,0),(12,'FISICA','BIANCA EDUARDA FRANÇA','2222222222',NULL,NULL,6,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0.00,NULL,NULL,'2026-06-19 15:25:49.424420','2026-07-01 22:26:04.988791',1,1),(13,'JURIDICA','CENTRO UNIVERSITÁRIO DINÂMICA DAS CATARATAS','01208350000100',NULL,'UDC',1,'CENTRO','RUA CASTELO BRANCO','440',NULL,'85852150','35236900','UDC@UDC.EDU.BR',0.00,NULL,NULL,'2026-06-27 16:51:20.735478','2026-07-01 22:26:07.100994',1,1);
/*!40000 ALTER TABLE `clientes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `compras`
--

DROP TABLE IF EXISTS `compras`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `compras` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_fornecedor` int NOT NULL,
  `id_condicao_pagamento` int DEFAULT NULL,
  `data_compra` datetime(6) NOT NULL,
  `data_previsao_entrega` datetime(6) DEFAULT NULL,
  `data_entrega` datetime(6) DEFAULT NULL,
  `status` enum('AGUARDANDO_ENTREGA','ENTREGUE_PARCIAL','ENTREGUE','CANCELADA') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'AGUARDANDO_ENTREGA',
  `valor_total` decimal(7,2) NOT NULL DEFAULT '0.00',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_fornecedor` (`id_fornecedor`),
  KEY `id_condicao_pagamento` (`id_condicao_pagamento`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `compras_ibfk_1` FOREIGN KEY (`id_fornecedor`) REFERENCES `fornecedores` (`id`),
  CONSTRAINT `compras_ibfk_2` FOREIGN KEY (`id_condicao_pagamento`) REFERENCES `condicoes_pagamentos` (`id`),
  CONSTRAINT `compras_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_compras_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `compras`
--

LOCK TABLES `compras` WRITE;
/*!40000 ALTER TABLE `compras` DISABLE KEYS */;
/*!40000 ALTER TABLE `compras` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `compras_itens`
--

DROP TABLE IF EXISTS `compras_itens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `compras_itens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_compra` int NOT NULL,
  `id_produto` int NOT NULL,
  `quantidade` decimal(10,4) NOT NULL,
  `valor_unitario` decimal(7,2) NOT NULL,
  `valor_desconto` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_total` decimal(7,2) NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_compra` (`id_compra`),
  KEY `id_produto` (`id_produto`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `compras_itens_ibfk_1` FOREIGN KEY (`id_compra`) REFERENCES `compras` (`id`),
  CONSTRAINT `compras_itens_ibfk_2` FOREIGN KEY (`id_produto`) REFERENCES `produtos` (`id`),
  CONSTRAINT `compras_itens_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_compras_itens_quantidade` CHECK ((`quantidade` > 0)),
  CONSTRAINT `ck_compras_itens_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_compras_itens_valor_total` CHECK ((`valor_total` >= 0)),
  CONSTRAINT `ck_compras_itens_valor_unitario` CHECK ((`valor_unitario` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `compras_itens`
--

LOCK TABLES `compras_itens` WRITE;
/*!40000 ALTER TABLE `compras_itens` DISABLE KEYS */;
/*!40000 ALTER TABLE `compras_itens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `condicoes_pagamentos`
--

DROP TABLE IF EXISTS `condicoes_pagamentos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `condicoes_pagamentos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `condicao_pagamento` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `desconto_percentual` decimal(5,2) NOT NULL DEFAULT '0.00',
  `multa_percentual` decimal(5,2) NOT NULL DEFAULT '0.00',
  `taxa_juros_percentual` decimal(5,2) NOT NULL DEFAULT '0.00',
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `descricao` (`condicao_pagamento`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `condicoes_pagamentos_ibfk_2` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_condicoes_pagamentos_desconto` CHECK (((`desconto_percentual` >= 0) and (`desconto_percentual` <= 100))),
  CONSTRAINT `ck_condicoes_pagamentos_juros` CHECK (((`taxa_juros_percentual` >= 0) and (`taxa_juros_percentual` <= 100))),
  CONSTRAINT `ck_condicoes_pagamentos_multa` CHECK (((`multa_percentual` >= 0) and (`multa_percentual` <= 100)))
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `condicoes_pagamentos`
--

LOCK TABLES `condicoes_pagamentos` WRITE;
/*!40000 ALTER TABLE `condicoes_pagamentos` DISABLE KEYS */;
INSERT INTO `condicoes_pagamentos` VALUES (1,'À VISTA DINHEIRO',5.00,0.00,0.00,'2026-07-04 14:47:35.495814','2026-07-04 14:47:35.495816',1,1),(2,'À VISTA PIX',3.00,0.00,0.00,'2026-07-04 15:13:18.348925','2026-07-04 15:13:18.348926',1,1),(3,'2X SEM JUROS',0.00,2.00,0.00,'2026-07-04 15:15:04.603859','2026-07-04 15:24:40.933148',1,1),(5,'30/60/90 BOLETO BANCÁRIO',2.00,5.00,2.00,'2026-10-09 20:03:32.535174','2026-10-09 20:03:32.535176',1,1);
/*!40000 ALTER TABLE `condicoes_pagamentos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `condicoes_pagamentos_parcelas`
--

DROP TABLE IF EXISTS `condicoes_pagamentos_parcelas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `condicoes_pagamentos_parcelas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_condicao_pagamento` int NOT NULL,
  `numero_parcela` int NOT NULL,
  `percentual` decimal(7,4) NOT NULL,
  `prazo_dias` int NOT NULL,
  `id_metodo_pagamento` int NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_condicoes_pagamentos_parcelas` (`id_condicao_pagamento`,`numero_parcela`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  KEY `id_metodo_pagamento` (`id_metodo_pagamento`),
  CONSTRAINT `condicoes_pagamentos_parcelas_ibfk_1` FOREIGN KEY (`id_condicao_pagamento`) REFERENCES `condicoes_pagamentos` (`id`),
  CONSTRAINT `condicoes_pagamentos_parcelas_ibfk_2` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `condicoes_pagamentos_parcelas_ibfk_3` FOREIGN KEY (`id_metodo_pagamento`) REFERENCES `metodos_pagamento` (`id`),
  CONSTRAINT `ck_condicoes_pagamentos_parcelas_numero` CHECK ((`numero_parcela` > 0)),
  CONSTRAINT `ck_condicoes_pagamentos_parcelas_percentual` CHECK (((`percentual` > 0) and (`percentual` <= 100))),
  CONSTRAINT `ck_condicoes_pagamentos_parcelas_prazo` CHECK ((`prazo_dias` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `condicoes_pagamentos_parcelas`
--

LOCK TABLES `condicoes_pagamentos_parcelas` WRITE;
/*!40000 ALTER TABLE `condicoes_pagamentos_parcelas` DISABLE KEYS */;
INSERT INTO `condicoes_pagamentos_parcelas` VALUES (1,1,1,100.0000,0,4,'2026-07-04 14:47:35.503002','2026-07-04 14:47:35.503003',1,1),(2,2,1,100.0000,0,2,'2026-07-04 15:13:18.355493','2026-07-04 15:13:18.355493',1,1),(4,3,1,50.0000,30,3,'2026-07-04 15:24:41.516425','2026-07-04 15:24:41.516427',1,1),(5,3,2,50.0000,60,3,'2026-07-04 15:24:41.516428','2026-07-04 15:24:41.516428',1,1),(7,5,1,33.3334,30,1,'2026-10-09 20:03:32.547632','2026-10-09 20:03:32.547634',1,1),(8,5,2,33.3333,60,1,'2026-10-09 20:03:32.547641','2026-10-09 20:03:32.547641',1,1),(9,5,3,33.3333,90,1,'2026-10-09 20:03:32.547641','2026-10-09 20:03:32.547642',1,1);
/*!40000 ALTER TABLE `condicoes_pagamentos_parcelas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contas_pagar`
--

DROP TABLE IF EXISTS `contas_pagar`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contas_pagar` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nota_numero` varchar(9) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nota_modelo` varchar(2) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nota_serie` varchar(3) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_fornecedor` int NOT NULL,
  `id_condicao_pagamento` int DEFAULT NULL,
  `descricao` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `numero_parcela` smallint NOT NULL DEFAULT '1',
  `total_parcelas` smallint NOT NULL DEFAULT '1',
  `data_emissao` date DEFAULT NULL,
  `data_vencimento` date NOT NULL,
  `data_pagamento` date DEFAULT NULL,
  `valor_original` decimal(7,2) NOT NULL,
  `valor_pago` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_desconto` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_juros` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_multa` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_total` decimal(7,2) NOT NULL,
  `status` enum('PENDENTE','PAGO','PARCIAL','CANCELADO') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDENTE',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `nota_numero` (`nota_numero`,`nota_modelo`,`nota_serie`,`id_fornecedor`),
  KEY `id_fornecedor` (`id_fornecedor`),
  KEY `id_condicao_pagamento` (`id_condicao_pagamento`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `contas_pagar_ibfk_1` FOREIGN KEY (`nota_numero`, `nota_modelo`, `nota_serie`, `id_fornecedor`) REFERENCES `nfes_entrada` (`numero`, `modelo`, `serie`, `id_fornecedor`),
  CONSTRAINT `contas_pagar_ibfk_2` FOREIGN KEY (`id_fornecedor`) REFERENCES `fornecedores` (`id`),
  CONSTRAINT `contas_pagar_ibfk_3` FOREIGN KEY (`id_condicao_pagamento`) REFERENCES `condicoes_pagamentos` (`id`),
  CONSTRAINT `contas_pagar_ibfk_4` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_contas_pagar_numero_parcela` CHECK ((`numero_parcela` > 0)),
  CONSTRAINT `ck_contas_pagar_valor_original` CHECK ((`valor_original` >= 0)),
  CONSTRAINT `ck_contas_pagar_valor_pago` CHECK ((`valor_pago` >= 0)),
  CONSTRAINT `ck_contas_pagar_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contas_pagar`
--

LOCK TABLES `contas_pagar` WRITE;
/*!40000 ALTER TABLE `contas_pagar` DISABLE KEYS */;
/*!40000 ALTER TABLE `contas_pagar` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contas_receber`
--

DROP TABLE IF EXISTS `contas_receber`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contas_receber` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nota_numero` varchar(9) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nota_modelo` varchar(2) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nota_serie` varchar(3) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_emitente` int DEFAULT NULL,
  `id_cliente` int NOT NULL,
  `id_venda` int DEFAULT NULL,
  `id_condicao_pagamento` int DEFAULT NULL,
  `descricao` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `numero_parcela` smallint NOT NULL DEFAULT '1',
  `total_parcelas` smallint NOT NULL DEFAULT '1',
  `data_emissao` date DEFAULT NULL,
  `data_vencimento` date NOT NULL,
  `data_recebimento` date DEFAULT NULL,
  `valor_original` decimal(7,2) NOT NULL,
  `valor_recebido` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_desconto` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_juros` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_multa` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_total` decimal(7,2) NOT NULL,
  `status` enum('PENDENTE','RECEBIDO','PARCIAL','CANCELADO') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDENTE',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `nota_numero` (`nota_numero`,`nota_modelo`,`nota_serie`,`id_emitente`),
  KEY `id_cliente` (`id_cliente`),
  KEY `id_venda` (`id_venda`),
  KEY `id_condicao_pagamento` (`id_condicao_pagamento`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `contas_receber_ibfk_1` FOREIGN KEY (`nota_numero`, `nota_modelo`, `nota_serie`, `id_emitente`) REFERENCES `nfes_saida` (`numero`, `modelo`, `serie`, `id_emitente`),
  CONSTRAINT `contas_receber_ibfk_2` FOREIGN KEY (`id_cliente`) REFERENCES `clientes` (`id`),
  CONSTRAINT `contas_receber_ibfk_3` FOREIGN KEY (`id_venda`) REFERENCES `vendas` (`id`),
  CONSTRAINT `contas_receber_ibfk_4` FOREIGN KEY (`id_condicao_pagamento`) REFERENCES `condicoes_pagamentos` (`id`),
  CONSTRAINT `contas_receber_ibfk_5` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_contas_receber_numero_parcela` CHECK ((`numero_parcela` > 0)),
  CONSTRAINT `ck_contas_receber_valor_original` CHECK ((`valor_original` >= 0)),
  CONSTRAINT `ck_contas_receber_valor_recebido` CHECK ((`valor_recebido` >= 0)),
  CONSTRAINT `ck_contas_receber_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contas_receber`
--

LOCK TABLES `contas_receber` WRITE;
/*!40000 ALTER TABLE `contas_receber` DISABLE KEYS */;
/*!40000 ALTER TABLE `contas_receber` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `emitentes`
--

DROP TABLE IF EXISTS `emitentes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `emitentes` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tipo_pessoa` enum('FISICA','JURIDICA') COLLATE utf8mb4_unicode_ci NOT NULL,
  `nome_razao_social` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `cpf_cnpj` varchar(14) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rg_ie` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apelido_nome_fantasia` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cidade` int NOT NULL,
  `bairro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `logradouro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `numero` varchar(5) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `complemento` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cep` varchar(8) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `telefone` varchar(12) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `inscricao_municipal` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `regime_tributario` enum('SIMPLES_NACIONAL','SIMPLES_NACIONAL_EXCESSO','LUCRO_PRESUMIDO','LUCRO_REAL','MEI') COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_cnpj` (`cpf_cnpj`),
  KEY `id_cidade` (`id_cidade`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `emitentes_ibfk_1` FOREIGN KEY (`id_cidade`) REFERENCES `cidades` (`id`),
  CONSTRAINT `emitentes_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_emitentes_cpf_cnpj` CHECK (((`cpf_cnpj` is null) or (length(trim(`cpf_cnpj`)) > 0))),
  CONSTRAINT `ck_emitentes_nome` CHECK ((length(trim(`nome_razao_social`)) > 0))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `emitentes`
--

LOCK TABLES `emitentes` WRITE;
/*!40000 ALTER TABLE `emitentes` DISABLE KEYS */;
INSERT INTO `emitentes` VALUES (1,'JURIDICA','EMITENTE ALFA COMÉRCIO LTDA','70777888000197','700777888','ALFA COM.',1,'CENTRO','RUA ALMIRANTE BARROSO','111',NULL,'85851000','45977770001','alfa@comercio.com','123456','SIMPLES_NACIONAL',NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:28:52.258662',1,1),(2,'JURIDICA','BETA INDÚSTRIA S.A.','80888999000108','800888999','BETA IND.',2,'SANTO ANDRÉ','AV. INDUSTRIAL','222','BLOCO A','09030000','11977770002','beta@industria.com','234567','LUCRO_REAL',NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:02.262137',1,1),(3,'FISICA','CLÁUDIO FERREIRA','01234567890','012345678','CLÁUDIO',5,'MENINO DEUS','RUA BARÃO DE ITAPARA','333',NULL,'90130051','51977770003','claudio@email.com',NULL,NULL,NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:05.639588',1,1),(4,'JURIDICA','GAMA SERVIÇOS ME','91999000000119','910999000','GAMA SVC.',4,'COQUEIROS','RUA ARARANGUÁ','444',NULL,'88080000','48977770004','gama@servicos.com','345678','MEI',NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:09.108182',1,1),(5,'JURIDICA','DELTA CONSTRUÇÕES EIRELI','82888111000120','820888111','DELTA CONST.',3,'BOTAFOGO','RUA VOLUNTÁRIOS DA PÁTRIA','555',NULL,'22270010','21977770005','delta@obras.com','456789','LUCRO_PRESUMIDO',NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:14.557589',1,1),(6,'FISICA','SIMONE MARTINS','11234567891','112345678','SIMONE',1,'TRÊS LAGOAS','AV. JK','666','APTO 3','85860000','45977770006','simone@email.com',NULL,NULL,NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:18.151423',1,1),(7,'JURIDICA','ÉPSILON CONSULTORIA LTDA','73777666000131','730777666','ÉPSILON CON.',2,'PINHEIROS','RUA DOS PINHEIROS','777','SALA 5','05422000','11977770007','epsilon@consult.com','567890','SIMPLES_NACIONAL_EXCESSO',NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:20.964299',1,1),(8,'JURIDICA','ZETA ATACADO S.A.','64666555000142','640666555','ZETA ATAC.',5,'BOM FIM','RUA JOÃO ALFREDO','888',NULL,'90050230','51977770008','zeta@atacado.com','678901','LUCRO_REAL',NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:23.518867',1,0),(9,'FISICA','RICARDO SOUZA','21234567892','212345678','RICARDO',3,'TIJUCA','RUA CONDE DE BONFIM','999',NULL,'20520050','21977770009','ricardo@email.com',NULL,NULL,NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:25.901400',1,1),(10,'JURIDICA','ETA IMPORTAÇÕES ME','55444333000153','550444333','ETA IMPORT.',4,'CENTRO','RUA TRAJANO','10','GALPÃO 2','88010010','48977770010','eta@importa.com','789012','LUCRO_PRESUMIDO',NULL,'2026-06-14 20:08:41.000000','2026-07-04 15:29:28.605454',1,1);
/*!40000 ALTER TABLE `emitentes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `estados`
--

DROP TABLE IF EXISTS `estados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estados` (
  `id` int NOT NULL AUTO_INCREMENT,
  `estado` varchar(55) COLLATE utf8mb4_unicode_ci NOT NULL,
  `uf` varchar(2) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_pais` int NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_estado_id_pais` (`estado`,`id_pais`),
  UNIQUE KEY `uk_uf_id_pais` (`uf`,`id_pais`),
  KEY `id_pais` (`id_pais`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `estados_ibfk_1` FOREIGN KEY (`id_pais`) REFERENCES `paises` (`id`) ON DELETE CASCADE,
  CONSTRAINT `estados_ibfk_2` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estados`
--

LOCK TABLES `estados` WRITE;
/*!40000 ALTER TABLE `estados` DISABLE KEYS */;
INSERT INTO `estados` VALUES (1,'PARANÁ','PR',1,'2026-06-06 17:23:29.000000','2026-07-01 22:24:32.167954',1,1),(2,'SÃO PAULO','SP',1,'2026-06-06 17:23:29.000000','2026-07-01 22:24:34.220652',1,1),(3,'RIO DE JANEIRO','RJ',1,'2026-06-06 17:23:29.000000','2026-07-01 22:24:36.680990',1,1),(4,'SANTA CATARINA','SC',1,'2026-06-06 17:23:29.000000','2026-07-01 22:24:39.403625',1,1),(5,'RIO GRANDE DO SUL','RS',1,'2026-06-06 17:23:29.000000','2026-07-01 22:24:41.364757',1,1),(6,'NEVADA','NV',7,'2026-06-20 14:48:20.318190','2026-07-01 22:24:43.446936',1,1);
/*!40000 ALTER TABLE `estados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `fornecedores`
--

DROP TABLE IF EXISTS `fornecedores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fornecedores` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tipo_pessoa` enum('FISICA','JURIDICA') COLLATE utf8mb4_unicode_ci NOT NULL,
  `nome_razao_social` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `cpf_cnpj` varchar(14) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rg_ie` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apelido_nome_fantasia` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cidade` int NOT NULL,
  `bairro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `logradouro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `numero` varchar(5) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `complemento` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cep` varchar(8) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `telefone` varchar(12) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_cnpj` (`cpf_cnpj`),
  KEY `id_cidade` (`id_cidade`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `fornecedores_ibfk_1` FOREIGN KEY (`id_cidade`) REFERENCES `cidades` (`id`),
  CONSTRAINT `fornecedores_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_fornecedores_cpf_cnpj` CHECK (((`cpf_cnpj` is null) or (length(trim(`cpf_cnpj`)) > 0))),
  CONSTRAINT `ck_fornecedores_nome` CHECK ((length(trim(`nome_razao_social`)) > 0))
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `fornecedores`
--

LOCK TABLES `fornecedores` WRITE;
/*!40000 ALTER TABLE `fornecedores` DISABLE KEYS */;
INSERT INTO `fornecedores` VALUES (1,'JURIDICA','FORNECEDOR ALFA LTDA','10111222000131','100111222','ALFA',2,'LAPA','RUA GUAICURUS','101',NULL,'01212000','11988880001','alfa@fornecedor.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:27:43.176768',1,1),(2,'JURIDICA','BETA INSUMOS S.A.','20222333000142','200222333','BETA',1,'VILA INDUSTRIAL','AV. PARANÁ','202','GALPÃO 1','85852000','45988880002','beta@insumos.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:27:57.581792',1,1),(3,'FISICA','ROBERTO LIMA','67890123456','678901234','ROBERTO',5,'PETRÓPOLIS','RUA MOSTARDEIRO','303',NULL,'90430001','51988880003','roberto@email.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:20.737790',1,1),(4,'JURIDICA','GAMA MATERIAIS EIRELI','30333444000153','300333444','GAMA MAT.',3,'MADUREIRA','RUA CAROLINA MACHADO','404',NULL,'21310000','21988880004','gama@materiais.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:23.860246',1,1),(5,'FISICA','FERNANDA ROCHA','78901234567','789012345','FERNANDA',4,'AGRONÔMICA','RUA ESTEVES JÚNIOR','505','CASA','88015130','48988880005','fernanda@email.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:27.616603',1,1),(6,'JURIDICA','DELTA PEÇAS ME','40444555000164','400444555','DELTA PEÇAS',1,'CENTRO','RUA ALMIRANTE BARROSO','606',NULL,'85851000','45988880006','delta@pecas.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:29.793428',1,1),(7,'FISICA','LUCAS MENDES','89012345678','890123456','LUCAS',2,'SANTANA','RUA VOLUNTÁRIOS DA PÁTRIA','707',NULL,'02010000','11988880007','lucas@email.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:32.211187',1,1),(8,'JURIDICA','ÉPSILON EMBALAGENS LTDA','50555666000175','500555666','ÉPSILON EMB',5,'HUMAITÁ','RUA GENERAL LIMA E SILVA','808',NULL,'90050170','51988880008','epsilon@embalagens.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:34.542138',1,1),(9,'JURIDICA','ZETA LOGÍSTICA S.A.','60666777000186','600666777','ZETA LOG.',3,'JACAREPAGUÁ','ESTRADA DO CAFUNDÁ','909','DOCK 3','22750000','21988880009','zeta@logistica.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:43.574589',1,0),(10,'FISICA','PATRÍCIA ALVES','90123456789','901234567','PATRÍCIA',4,'ESTREITO','RUA LAURO LINHARES','10',NULL,'88075000','48988880010','patricia@email.com',NULL,'2026-06-13 13:11:23.000000','2026-07-04 15:28:37.772954',1,1);
/*!40000 ALTER TABLE `fornecedores` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `metodos_pagamento`
--

DROP TABLE IF EXISTS `metodos_pagamento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `metodos_pagamento` (
  `id` int NOT NULL AUTO_INCREMENT,
  `sigla` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `metodo` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `codigo` (`sigla`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `metodos_pagamento_ibfk_1` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_metodos_pagamento_metodo` CHECK ((length(trim(`metodo`)) > 0))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `metodos_pagamento`
--

LOCK TABLES `metodos_pagamento` WRITE;
/*!40000 ALTER TABLE `metodos_pagamento` DISABLE KEYS */;
INSERT INTO `metodos_pagamento` VALUES (1,'BOL','BOLETO','2026-06-27 14:14:01.664741','2026-07-01 22:33:34.191149',1,1),(2,'PIX','PIX','2026-06-27 14:14:09.328622','2026-07-01 22:33:36.977446',1,1),(3,'CRED','CARTÃO DE CRÉDITO','2026-06-27 14:15:05.729610','2026-07-01 22:33:39.378429',1,1),(4,'DIN','DINHEIRO','2026-07-04 14:16:06.882509','2026-07-04 14:16:06.882511',1,1);
/*!40000 ALTER TABLE `metodos_pagamento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movimentacoes_estoque`
--

DROP TABLE IF EXISTS `movimentacoes_estoque`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimentacoes_estoque` (
  `id` int NOT NULL AUTO_INCREMENT,
  `data_movimentacao` datetime(6) NOT NULL,
  `tipo_movimentacao` enum('ENTRADA','SAIDA','AJUSTE','VENDA','PRODUCAO') COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_usuario` int DEFAULT NULL,
  `id_venda` int DEFAULT NULL,
  `id_ordem_servico` int DEFAULT NULL,
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_usuario` (`id_usuario`),
  KEY `id_venda` (`id_venda`),
  KEY `id_ordem_servico` (`id_ordem_servico`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `movimentacoes_estoque_ibfk_1` FOREIGN KEY (`id_usuario`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `movimentacoes_estoque_ibfk_2` FOREIGN KEY (`id_venda`) REFERENCES `vendas` (`id`),
  CONSTRAINT `movimentacoes_estoque_ibfk_3` FOREIGN KEY (`id_ordem_servico`) REFERENCES `ordens_servico` (`id`),
  CONSTRAINT `movimentacoes_estoque_ibfk_4` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_movimentacoes_estoque_producao` CHECK (((`tipo_movimentacao` <> _utf8mb4'PRODUCAO') or ((`tipo_movimentacao` = _utf8mb4'PRODUCAO') and (`id_ordem_servico` is not null)))),
  CONSTRAINT `ck_movimentacoes_estoque_venda` CHECK (((`tipo_movimentacao` <> _utf8mb4'VENDA') or ((`tipo_movimentacao` = _utf8mb4'VENDA') and (`id_venda` is not null))))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movimentacoes_estoque`
--

LOCK TABLES `movimentacoes_estoque` WRITE;
/*!40000 ALTER TABLE `movimentacoes_estoque` DISABLE KEYS */;
/*!40000 ALTER TABLE `movimentacoes_estoque` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movimentacoes_estoque_itens`
--

DROP TABLE IF EXISTS `movimentacoes_estoque_itens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimentacoes_estoque_itens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_movimentacao_estoque` int NOT NULL,
  `id_produto` int NOT NULL,
  `quantidade` decimal(10,4) NOT NULL,
  `custo_unitario` decimal(7,2) NOT NULL DEFAULT '0.00',
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_movimentacao_estoque` (`id_movimentacao_estoque`),
  KEY `id_produto` (`id_produto`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `movimentacoes_estoque_itens_ibfk_1` FOREIGN KEY (`id_movimentacao_estoque`) REFERENCES `movimentacoes_estoque` (`id`),
  CONSTRAINT `movimentacoes_estoque_itens_ibfk_2` FOREIGN KEY (`id_produto`) REFERENCES `produtos` (`id`),
  CONSTRAINT `movimentacoes_estoque_itens_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_movimentacoes_estoque_itens_custo` CHECK ((`custo_unitario` >= 0)),
  CONSTRAINT `ck_movimentacoes_estoque_itens_quantidade` CHECK ((`quantidade` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movimentacoes_estoque_itens`
--

LOCK TABLES `movimentacoes_estoque_itens` WRITE;
/*!40000 ALTER TABLE `movimentacoes_estoque_itens` DISABLE KEYS */;
/*!40000 ALTER TABLE `movimentacoes_estoque_itens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nfes`
--

DROP TABLE IF EXISTS `nfes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nfes` (
  `id` int NOT NULL AUTO_INCREMENT,
  `chave_acesso` varchar(44) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `numero` int NOT NULL,
  `serie` smallint NOT NULL,
  `data_emissao` datetime(6) NOT NULL,
  `data_saida` datetime(6) DEFAULT NULL,
  `id_emitente` int NOT NULL,
  `emitente_nome_razao_social` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `emitente_cpf_cnpj` varchar(14) COLLATE utf8mb4_unicode_ci NOT NULL,
  `emitente_rg_ie` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emitente_apelido_nome_fantasia` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emitente_endereco` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emitente_bairro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emitente_telefone` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `emitente_email` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cliente` int NOT NULL,
  `cliente_nome_razao_social` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `cliente_cpf_cnpj` varchar(14) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cliente_rg_ie` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cliente_apelido_nome_fantasia` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cliente_endereco` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cliente_bairro` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cliente_telefone` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cliente_email` varchar(254) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tipo_operacao` enum('ENTRADA','SAIDA') COLLATE utf8mb4_unicode_ci NOT NULL,
  `status_nfe` enum('RASCUNHO','EMITIDA','CANCELADA') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'RASCUNHO',
  `id_transportadora` int DEFAULT NULL,
  `id_venda` int DEFAULT NULL,
  `valor_produtos` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_desconto` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_frete` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_seguro` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_outras_despesas` decimal(7,2) NOT NULL DEFAULT '0.00',
  `valor_total` decimal(7,2) NOT NULL DEFAULT '0.00',
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_nfes_emitente_serie_numero` (`id_emitente`,`serie`,`numero`),
  UNIQUE KEY `chave_acesso` (`chave_acesso`),
  KEY `id_cliente` (`id_cliente`),
  KEY `id_transportadora` (`id_transportadora`),
  KEY `id_venda` (`id_venda`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `nfes_ibfk_1` FOREIGN KEY (`id_emitente`) REFERENCES `emitentes` (`id`),
  CONSTRAINT `nfes_ibfk_2` FOREIGN KEY (`id_cliente`) REFERENCES `clientes` (`id`),
  CONSTRAINT `nfes_ibfk_3` FOREIGN KEY (`id_transportadora`) REFERENCES `transportadoras` (`id`),
  CONSTRAINT `nfes_ibfk_4` FOREIGN KEY (`id_venda`) REFERENCES `vendas` (`id`),
  CONSTRAINT `nfes_ibfk_5` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_nfes_chave_acesso` CHECK ((((`status_nfe` = _utf8mb4'RASCUNHO') and (`chave_acesso` is null)) or ((`status_nfe` in (_utf8mb4'EMITIDA',_utf8mb4'CANCELADA')) and regexp_like(`chave_acesso`,_utf8mb4'^[0-9]{44}$')))),
  CONSTRAINT `ck_nfes_cliente_nome` CHECK ((length(trim(`cliente_nome_razao_social`)) > 0)),
  CONSTRAINT `ck_nfes_emitente_cpf_cnpj` CHECK ((length(trim(`emitente_cpf_cnpj`)) > 0)),
  CONSTRAINT `ck_nfes_emitente_nome` CHECK ((length(trim(`emitente_nome_razao_social`)) > 0)),
  CONSTRAINT `ck_nfes_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_nfes_valor_frete` CHECK ((`valor_frete` >= 0)),
  CONSTRAINT `ck_nfes_valor_outras` CHECK ((`valor_outras_despesas` >= 0)),
  CONSTRAINT `ck_nfes_valor_produtos` CHECK ((`valor_produtos` >= 0)),
  CONSTRAINT `ck_nfes_valor_seguro` CHECK ((`valor_seguro` >= 0)),
  CONSTRAINT `ck_nfes_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nfes`
--

LOCK TABLES `nfes` WRITE;
/*!40000 ALTER TABLE `nfes` DISABLE KEYS */;
/*!40000 ALTER TABLE `nfes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nfes_entrada`
--

DROP TABLE IF EXISTS `nfes_entrada`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nfes_entrada` (
  `numero` varchar(9) COLLATE utf8mb4_unicode_ci NOT NULL,
  `modelo` varchar(2) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '55',
  `serie` varchar(3) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '1',
  `id_fornecedor` int NOT NULL,
  `id_transportadora` int DEFAULT NULL,
  `id_condicao_pagamento` int DEFAULT NULL,
  `data_emissao` date NOT NULL,
  `data_chegada` date DEFAULT NULL,
  `tipo_frete` enum('CIF','FOB','SEM') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'CIF',
  `valor_produtos` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_frete` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_seguro` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_outras_despesas` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_desconto` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_total` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `situacao` enum('PENDENTE','CONFIRMADA','CANCELADA') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDENTE',
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`numero`,`modelo`,`serie`,`id_fornecedor`),
  KEY `id_fornecedor` (`id_fornecedor`),
  KEY `id_transportadora` (`id_transportadora`),
  KEY `id_condicao_pagamento` (`id_condicao_pagamento`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `nfes_entrada_ibfk_1` FOREIGN KEY (`id_fornecedor`) REFERENCES `fornecedores` (`id`),
  CONSTRAINT `nfes_entrada_ibfk_2` FOREIGN KEY (`id_transportadora`) REFERENCES `transportadoras` (`id`),
  CONSTRAINT `nfes_entrada_ibfk_3` FOREIGN KEY (`id_condicao_pagamento`) REFERENCES `condicoes_pagamentos` (`id`),
  CONSTRAINT `nfes_entrada_ibfk_4` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_nfes_entrada_data_chegada` CHECK (((`data_chegada` is null) or (`data_chegada` >= `data_emissao`))),
  CONSTRAINT `ck_nfes_entrada_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_nfes_entrada_valor_frete` CHECK ((`valor_frete` >= 0)),
  CONSTRAINT `ck_nfes_entrada_valor_outras` CHECK ((`valor_outras_despesas` >= 0)),
  CONSTRAINT `ck_nfes_entrada_valor_produtos` CHECK ((`valor_produtos` >= 0)),
  CONSTRAINT `ck_nfes_entrada_valor_seguro` CHECK ((`valor_seguro` >= 0)),
  CONSTRAINT `ck_nfes_entrada_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nfes_entrada`
--

LOCK TABLES `nfes_entrada` WRITE;
/*!40000 ALTER TABLE `nfes_entrada` DISABLE KEYS */;
INSERT INTO `nfes_entrada` VALUES ('111111111','55','1',2,2,3,'2026-10-01','2026-10-01','CIF',50.0000,20.0000,0.0000,0.0000,0.0000,70.0000,NULL,'CANCELADA','2026-10-09 16:36:00.002074','2026-10-09 18:59:19.503907',1,0),('111111111','55','1',6,2,3,'2026-10-08','2026-10-09','',50.0000,15.0000,0.0000,0.0000,0.0000,65.0000,NULL,'CONFIRMADA','2026-10-09 18:58:12.268940','2026-10-09 18:58:12.268942',1,1),('111111111','55','1',8,2,5,'2026-10-01','2026-10-09','CIF',336.4800,27.0500,16.1600,0.0000,0.0000,379.6900,NULL,'CONFIRMADA','2026-10-09 20:08:58.644461','2026-10-09 20:08:58.644461',1,1),('222222222','55','1',2,NULL,NULL,'2026-10-09','2026-10-09','CIF',225.0000,30.0000,0.0000,0.0000,0.0000,255.0000,NULL,'CANCELADA','2026-10-09 19:11:56.907432','2026-10-09 19:20:25.571872',1,0);
/*!40000 ALTER TABLE `nfes_entrada` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nfes_entrada_itens`
--

DROP TABLE IF EXISTS `nfes_entrada_itens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nfes_entrada_itens` (
  `nota_numero` varchar(9) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nota_modelo` varchar(2) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nota_serie` varchar(3) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_fornecedor` int NOT NULL,
  `id_produto` int NOT NULL,
  `sequencia` smallint NOT NULL,
  `quantidade` decimal(15,4) NOT NULL,
  `valor_unitario` decimal(15,4) NOT NULL,
  `valor_desconto` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `percentual_desconto` decimal(5,2) NOT NULL DEFAULT '0.00',
  `valor_total` decimal(15,4) NOT NULL,
  `rateio_frete` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `rateio_seguro` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `rateio_outras` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `custo_final` decimal(15,4) DEFAULT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  PRIMARY KEY (`nota_numero`,`nota_modelo`,`nota_serie`,`id_fornecedor`,`id_produto`,`sequencia`),
  KEY `id_produto` (`id_produto`),
  CONSTRAINT `nfes_entrada_itens_ibfk_1` FOREIGN KEY (`nota_numero`, `nota_modelo`, `nota_serie`, `id_fornecedor`) REFERENCES `nfes_entrada` (`numero`, `modelo`, `serie`, `id_fornecedor`) ON DELETE CASCADE,
  CONSTRAINT `nfes_entrada_itens_ibfk_2` FOREIGN KEY (`id_produto`) REFERENCES `produtos` (`id`),
  CONSTRAINT `ck_nfes_entrada_itens_percentual` CHECK (((`percentual_desconto` >= 0) and (`percentual_desconto` <= 100))),
  CONSTRAINT `ck_nfes_entrada_itens_quantidade` CHECK ((`quantidade` > 0)),
  CONSTRAINT `ck_nfes_entrada_itens_sequencia` CHECK ((`sequencia` > 0)),
  CONSTRAINT `ck_nfes_entrada_itens_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_nfes_entrada_itens_valor_total` CHECK ((`valor_total` >= 0)),
  CONSTRAINT `ck_nfes_entrada_itens_valor_unitario` CHECK ((`valor_unitario` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nfes_entrada_itens`
--

LOCK TABLES `nfes_entrada_itens` WRITE;
/*!40000 ALTER TABLE `nfes_entrada_itens` DISABLE KEYS */;
INSERT INTO `nfes_entrada_itens` VALUES ('111111111','55','1',2,2,1,100.0000,0.5000,0.0000,0.00,50.0000,20.0000,0.0000,0.0000,0.7000,'2026-10-09 16:36:00.001332','2026-10-09 16:36:00.001395'),('111111111','55','1',6,2,1,100.0000,0.5000,0.0000,0.00,50.0000,15.0000,0.0000,0.0000,0.6500,'2026-10-09 18:58:12.268246','2026-10-09 18:58:12.268362'),('111111111','55','1',8,2,2,24.0000,6.7100,0.0000,0.00,161.0400,12.9462,7.7342,0.0000,7.5717,'2026-10-09 20:08:58.644317','2026-10-09 20:08:58.644317'),('111111111','55','1',8,6,1,24.0000,7.3100,0.0000,0.00,175.4400,14.1038,8.4258,0.0000,8.2487,'2026-10-09 20:08:58.644167','2026-10-09 20:08:58.644230'),('222222222','55','1',2,5,1,5.0000,45.0000,0.0000,0.00,225.0000,30.0000,0.0000,0.0000,51.0000,'2026-10-09 19:11:56.907430','2026-10-09 19:11:56.907430');
/*!40000 ALTER TABLE `nfes_entrada_itens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nfes_saida`
--

DROP TABLE IF EXISTS `nfes_saida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nfes_saida` (
  `numero` varchar(9) COLLATE utf8mb4_unicode_ci NOT NULL,
  `modelo` varchar(2) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '55',
  `serie` varchar(3) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '1',
  `id_emitente` int NOT NULL,
  `id_cliente` int NOT NULL,
  `id_transportadora` int DEFAULT NULL,
  `id_venda` int DEFAULT NULL,
  `id_condicao_pagamento` int DEFAULT NULL,
  `data_emissao` date NOT NULL,
  `data_saida` date DEFAULT NULL,
  `tipo_frete` enum('CIF','FOB','SEM') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'CIF',
  `valor_produtos` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_frete` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_seguro` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_outras_despesas` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_desconto` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `valor_total` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `situacao` enum('PENDENTE','CONFIRMADA','CANCELADA') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PENDENTE',
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`numero`,`modelo`,`serie`,`id_emitente`),
  KEY `id_emitente` (`id_emitente`),
  KEY `id_cliente` (`id_cliente`),
  KEY `id_transportadora` (`id_transportadora`),
  KEY `id_venda` (`id_venda`),
  KEY `id_condicao_pagamento` (`id_condicao_pagamento`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `nfes_saida_ibfk_1` FOREIGN KEY (`id_emitente`) REFERENCES `emitentes` (`id`),
  CONSTRAINT `nfes_saida_ibfk_2` FOREIGN KEY (`id_cliente`) REFERENCES `clientes` (`id`),
  CONSTRAINT `nfes_saida_ibfk_3` FOREIGN KEY (`id_transportadora`) REFERENCES `transportadoras` (`id`),
  CONSTRAINT `nfes_saida_ibfk_4` FOREIGN KEY (`id_venda`) REFERENCES `vendas` (`id`),
  CONSTRAINT `nfes_saida_ibfk_5` FOREIGN KEY (`id_condicao_pagamento`) REFERENCES `condicoes_pagamentos` (`id`),
  CONSTRAINT `nfes_saida_ibfk_6` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_nfes_saida_data_saida` CHECK (((`data_saida` is null) or (`data_saida` >= `data_emissao`))),
  CONSTRAINT `ck_nfes_saida_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_nfes_saida_valor_frete` CHECK ((`valor_frete` >= 0)),
  CONSTRAINT `ck_nfes_saida_valor_outras` CHECK ((`valor_outras_despesas` >= 0)),
  CONSTRAINT `ck_nfes_saida_valor_produtos` CHECK ((`valor_produtos` >= 0)),
  CONSTRAINT `ck_nfes_saida_valor_seguro` CHECK ((`valor_seguro` >= 0)),
  CONSTRAINT `ck_nfes_saida_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nfes_saida`
--

LOCK TABLES `nfes_saida` WRITE;
/*!40000 ALTER TABLE `nfes_saida` DISABLE KEYS */;
/*!40000 ALTER TABLE `nfes_saida` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nfes_saida_itens`
--

DROP TABLE IF EXISTS `nfes_saida_itens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nfes_saida_itens` (
  `nota_numero` varchar(9) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nota_modelo` varchar(2) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nota_serie` varchar(3) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_emitente` int NOT NULL,
  `id_produto` int NOT NULL,
  `sequencia` smallint NOT NULL,
  `quantidade` decimal(15,4) NOT NULL,
  `valor_unitario` decimal(15,4) NOT NULL,
  `valor_desconto` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `percentual_desconto` decimal(5,2) NOT NULL DEFAULT '0.00',
  `valor_total` decimal(15,4) NOT NULL,
  `rateio_frete` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `rateio_seguro` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `rateio_outras` decimal(15,4) NOT NULL DEFAULT '0.0000',
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  PRIMARY KEY (`nota_numero`,`nota_modelo`,`nota_serie`,`id_emitente`,`id_produto`,`sequencia`),
  KEY `id_produto` (`id_produto`),
  CONSTRAINT `nfes_saida_itens_ibfk_1` FOREIGN KEY (`nota_numero`, `nota_modelo`, `nota_serie`, `id_emitente`) REFERENCES `nfes_saida` (`numero`, `modelo`, `serie`, `id_emitente`) ON DELETE CASCADE,
  CONSTRAINT `nfes_saida_itens_ibfk_2` FOREIGN KEY (`id_produto`) REFERENCES `produtos` (`id`),
  CONSTRAINT `ck_nfes_saida_itens_percentual` CHECK (((`percentual_desconto` >= 0) and (`percentual_desconto` <= 100))),
  CONSTRAINT `ck_nfes_saida_itens_quantidade` CHECK ((`quantidade` > 0)),
  CONSTRAINT `ck_nfes_saida_itens_sequencia` CHECK ((`sequencia` > 0)),
  CONSTRAINT `ck_nfes_saida_itens_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_nfes_saida_itens_valor_total` CHECK ((`valor_total` >= 0)),
  CONSTRAINT `ck_nfes_saida_itens_valor_unitario` CHECK ((`valor_unitario` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nfes_saida_itens`
--

LOCK TABLES `nfes_saida_itens` WRITE;
/*!40000 ALTER TABLE `nfes_saida_itens` DISABLE KEYS */;
/*!40000 ALTER TABLE `nfes_saida_itens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ordens_servico`
--

DROP TABLE IF EXISTS `ordens_servico`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ordens_servico` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_pedido` int NOT NULL,
  `id_emitente` int NOT NULL,
  `id_cliente` int NOT NULL,
  `data_abertura` datetime(6) NOT NULL,
  `data_previsao_conclusao` datetime(6) DEFAULT NULL,
  `data_conclusao` datetime(6) DEFAULT NULL,
  `status` enum('ABERTA','EM_ANDAMENTO','AGUARDANDO_MATERIAL','CONCLUIDA','CANCELADA') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'ABERTA',
  `valor_total` decimal(7,2) NOT NULL DEFAULT '0.00',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_pedido` (`id_pedido`),
  KEY `id_emitente` (`id_emitente`),
  KEY `id_cliente` (`id_cliente`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `ordens_servico_ibfk_1` FOREIGN KEY (`id_pedido`) REFERENCES `pedidos` (`id`),
  CONSTRAINT `ordens_servico_ibfk_2` FOREIGN KEY (`id_emitente`) REFERENCES `emitentes` (`id`),
  CONSTRAINT `ordens_servico_ibfk_3` FOREIGN KEY (`id_cliente`) REFERENCES `clientes` (`id`),
  CONSTRAINT `ordens_servico_ibfk_4` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_ordens_servico_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ordens_servico`
--

LOCK TABLES `ordens_servico` WRITE;
/*!40000 ALTER TABLE `ordens_servico` DISABLE KEYS */;
/*!40000 ALTER TABLE `ordens_servico` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ordens_servico_itens`
--

DROP TABLE IF EXISTS `ordens_servico_itens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ordens_servico_itens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_ordem_servico` int NOT NULL,
  `id_pedido_item` int NOT NULL,
  `id_produto` int NOT NULL,
  `descricao_personalizacao` text COLLATE utf8mb4_unicode_ci,
  `quantidade` decimal(10,4) NOT NULL,
  `valor_unitario` decimal(7,4) NOT NULL,
  `valor_desconto` decimal(7,4) NOT NULL DEFAULT '0.0000',
  `valor_total` decimal(7,4) NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_ordem_servico` (`id_ordem_servico`),
  KEY `id_pedido_item` (`id_pedido_item`),
  KEY `id_produto` (`id_produto`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `ordens_servico_itens_ibfk_1` FOREIGN KEY (`id_ordem_servico`) REFERENCES `ordens_servico` (`id`),
  CONSTRAINT `ordens_servico_itens_ibfk_2` FOREIGN KEY (`id_pedido_item`) REFERENCES `pedidos_itens` (`id`),
  CONSTRAINT `ordens_servico_itens_ibfk_3` FOREIGN KEY (`id_produto`) REFERENCES `produtos` (`id`),
  CONSTRAINT `ordens_servico_itens_ibfk_4` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_ordens_servico_itens_quantidade` CHECK ((`quantidade` > 0)),
  CONSTRAINT `ck_ordens_servico_itens_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_ordens_servico_itens_valor_total` CHECK ((`valor_total` >= 0)),
  CONSTRAINT `ck_ordens_servico_itens_valor_unitario` CHECK ((`valor_unitario` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ordens_servico_itens`
--

LOCK TABLES `ordens_servico_itens` WRITE;
/*!40000 ALTER TABLE `ordens_servico_itens` DISABLE KEYS */;
/*!40000 ALTER TABLE `ordens_servico_itens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `paises`
--

DROP TABLE IF EXISTS `paises`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `paises` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pais` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sigla` varchar(3) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ddi` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `moeda` varchar(3) COLLATE utf8mb4_unicode_ci NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `pais` (`pais`),
  UNIQUE KEY `sigla` (`sigla`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `paises_ibfk_1` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `paises`
--

LOCK TABLES `paises` WRITE;
/*!40000 ALTER TABLE `paises` DISABLE KEYS */;
INSERT INTO `paises` VALUES (1,'BRASIL','BRA','+55','BRL','2026-06-06 17:23:28.000000','2026-07-01 22:00:08.002273',1,1),(2,'ARGENTINA','ARG','+54','ARS','2026-06-06 17:23:28.000000','2026-07-01 22:24:10.880668',1,1),(4,'URUGUAI','URY','+598','UYU','2026-06-06 17:23:28.000000','2026-07-01 22:24:18.817307',1,1),(5,'CHILE','CHL','+56','CLP','2026-06-06 17:23:28.000000','2026-07-01 22:24:21.783065',1,1),(6,'ITÁLIA','ITA','+39','BRL','2026-06-06 17:25:14.264832','2026-07-01 22:24:24.625419',1,1),(7,'ESTADOS UNIDOS DA AMÉRICA','EUA','+1','USD','2026-06-20 14:46:38.735292','2026-07-01 22:25:16.166459',1,0);
/*!40000 ALTER TABLE `paises` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pedidos`
--

DROP TABLE IF EXISTS `pedidos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pedidos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_cliente` int NOT NULL,
  `id_emitente` int NOT NULL,
  `data_pedido` datetime(6) NOT NULL,
  `data_previsao_entrega` datetime(6) DEFAULT NULL,
  `data_entrega` datetime(6) DEFAULT NULL,
  `status` enum('AGUARDANDO_APROVACAO_ARTE','ARTE_APROVADA','EM_PRODUCAO','PRONTO_RETIRADA','ENVIADO','ENTREGUE','CANCELADO') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'AGUARDANDO_APROVACAO_ARTE',
  `id_venda` int DEFAULT NULL,
  `valor_total` decimal(7,2) NOT NULL DEFAULT '0.00',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_cliente` (`id_cliente`),
  KEY `id_emitente` (`id_emitente`),
  KEY `id_venda` (`id_venda`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `pedidos_ibfk_1` FOREIGN KEY (`id_cliente`) REFERENCES `clientes` (`id`),
  CONSTRAINT `pedidos_ibfk_2` FOREIGN KEY (`id_emitente`) REFERENCES `emitentes` (`id`),
  CONSTRAINT `pedidos_ibfk_3` FOREIGN KEY (`id_venda`) REFERENCES `vendas` (`id`),
  CONSTRAINT `pedidos_ibfk_4` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_pedidos_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pedidos`
--

LOCK TABLES `pedidos` WRITE;
/*!40000 ALTER TABLE `pedidos` DISABLE KEYS */;
/*!40000 ALTER TABLE `pedidos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pedidos_itens`
--

DROP TABLE IF EXISTS `pedidos_itens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pedidos_itens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_pedido` int NOT NULL,
  `id_produto` int NOT NULL,
  `descricao_personalizacao` text COLLATE utf8mb4_unicode_ci,
  `quantidade` decimal(10,4) NOT NULL,
  `valor_unitario` decimal(7,4) NOT NULL,
  `valor_desconto` decimal(7,4) NOT NULL DEFAULT '0.0000',
  `valor_total` decimal(7,4) NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_pedido` (`id_pedido`),
  KEY `id_produto` (`id_produto`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `pedidos_itens_ibfk_1` FOREIGN KEY (`id_pedido`) REFERENCES `pedidos` (`id`),
  CONSTRAINT `pedidos_itens_ibfk_2` FOREIGN KEY (`id_produto`) REFERENCES `produtos` (`id`),
  CONSTRAINT `pedidos_itens_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_pedidos_itens_quantidade` CHECK ((`quantidade` > 0)),
  CONSTRAINT `ck_pedidos_itens_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_pedidos_itens_valor_total` CHECK ((`valor_total` >= 0)),
  CONSTRAINT `ck_pedidos_itens_valor_unitario` CHECK ((`valor_unitario` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pedidos_itens`
--

LOCK TABLES `pedidos_itens` WRITE;
/*!40000 ALTER TABLE `pedidos_itens` DISABLE KEYS */;
/*!40000 ALTER TABLE `pedidos_itens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `produtos`
--

DROP TABLE IF EXISTS `produtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `produtos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `produto` varchar(70) COLLATE utf8mb4_unicode_ci NOT NULL,
  `referencia` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `saldo` decimal(10,4) NOT NULL DEFAULT '0.0000',
  `preco_custo` decimal(7,2) DEFAULT NULL,
  `preco_venda` decimal(7,2) DEFAULT NULL,
  `estoque_minimo` decimal(10,4) NOT NULL DEFAULT '0.0000',
  `valor_ultima_compra` decimal(7,2) DEFAULT NULL,
  `tipo` enum('PRODUTO_FINAL','MATERIA_PRIMA') COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PRODUTO_FINAL',
  `id_categoria` int NOT NULL,
  `id_unidade_medida` int NOT NULL,
  `ncm` varchar(8) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cest` varchar(7) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `origem` tinyint NOT NULL DEFAULT '0',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_categoria` (`id_categoria`),
  KEY `id_unidade_medida` (`id_unidade_medida`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `produtos_ibfk_1` FOREIGN KEY (`id_categoria`) REFERENCES `categorias` (`id`),
  CONSTRAINT `produtos_ibfk_2` FOREIGN KEY (`id_unidade_medida`) REFERENCES `unidades_medida` (`id`),
  CONSTRAINT `produtos_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `produtos`
--

LOCK TABLES `produtos` WRITE;
/*!40000 ALTER TABLE `produtos` DISABLE KEYS */;
INSERT INTO `produtos` VALUES (1,'AGENDA A5 PERSONALIZADA',NULL,1.0000,0.00,50.00,0.0000,NULL,'PRODUTO_FINAL',6,1,NULL,NULL,0,NULL,'2026-06-23 11:25:37.363190','2026-07-04 16:50:13.260058',1,1),(2,'PAPEL SULFITE 75G/A4 RESMA REPORT',NULL,424.0000,NULL,NULL,100.0000,7.57,'MATERIA_PRIMA',1,2,NULL,NULL,0,NULL,'2026-06-23 11:38:54.218650','2026-07-04 15:56:16.128098',1,1),(5,'LIVRO INFANTIL',NULL,1.0000,10.00,NULL,1.0000,51.00,'PRODUTO_FINAL',6,1,NULL,NULL,0,NULL,'2026-07-06 20:05:34.219356','2026-07-06 20:05:34.219360',1,1),(6,'PAPEL SULFITE REPORT A4',NULL,24.0000,NULL,NULL,0.0000,8.25,'MATERIA_PRIMA',1,8,NULL,NULL,0,NULL,'2026-10-09 19:50:28.630711','2026-10-09 19:50:28.630716',1,1);
/*!40000 ALTER TABLE `produtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transportadoras`
--

DROP TABLE IF EXISTS `transportadoras`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `transportadoras` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tipo_pessoa` enum('FISICA','JURIDICA') COLLATE utf8mb4_unicode_ci NOT NULL,
  `nome_razao_social` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `cpf_cnpj` varchar(14) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rg_ie` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `apelido_nome_fantasia` varchar(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_cidade` int NOT NULL,
  `bairro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `logradouro` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `numero` varchar(5) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `complemento` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `cep` varchar(8) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `telefone` varchar(12) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rntrc` varchar(8) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_cnpj` (`cpf_cnpj`),
  KEY `id_cidade` (`id_cidade`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `transportadoras_ibfk_1` FOREIGN KEY (`id_cidade`) REFERENCES `cidades` (`id`),
  CONSTRAINT `transportadoras_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_transportadoras_cpf_cnpj` CHECK (((`cpf_cnpj` is null) or (length(trim(`cpf_cnpj`)) > 0))),
  CONSTRAINT `ck_transportadoras_nome` CHECK ((length(trim(`nome_razao_social`)) > 0))
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transportadoras`
--

LOCK TABLES `transportadoras` WRITE;
/*!40000 ALTER TABLE `transportadoras` DISABLE KEYS */;
INSERT INTO `transportadoras` VALUES (1,'JURIDICA','TRANSPORTADORA ALFA LTDA','46444222000164','460444222','ALFA TRANS.',1,'VILA PORTES','AV. DAS CATARATAS','111',NULL,'85853000','45966660001','alfa@trans.com','77536741',NULL,'2026-06-13 13:21:51.000000','2026-07-04 15:29:36.718804',1,1),(2,'JURIDICA','BETA CARGAS S.A.','37333111000175','370333111','BETA CARGAS',2,'INTERLAGOS','AV. INTERLAGOS','222','PÁTIO 1','04661000','11966660002','beta@cargas.com','00151829',NULL,'2026-06-13 13:21:51.000000','2026-07-04 15:29:38.988653',1,1);
/*!40000 ALTER TABLE `transportadoras` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `unidades_medida`
--

DROP TABLE IF EXISTS `unidades_medida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `unidades_medida` (
  `id` int NOT NULL AUTO_INCREMENT,
  `sigla` varchar(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `unidade_medida` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `categoria` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `sigla` (`sigla`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `unidades_medida_ibfk_1` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_unidades_medida_categoria` CHECK ((length(trim(`categoria`)) > 0)),
  CONSTRAINT `ck_unidades_medida_descricao` CHECK ((length(trim(`unidade_medida`)) > 0)),
  CONSTRAINT `ck_unidades_medida_sigla` CHECK ((length(trim(`sigla`)) > 0))
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unidades_medida`
--

LOCK TABLES `unidades_medida` WRITE;
/*!40000 ALTER TABLE `unidades_medida` DISABLE KEYS */;
INSERT INTO `unidades_medida` VALUES (1,'UN','UNIDADE','QUANTIDADE','2026-06-12 20:25:13.000000','2026-07-04 15:31:37.925652',1,1),(2,'FL','FOLHA','QUANTIDADE','2026-06-12 20:25:13.000000','2026-07-04 15:31:40.689328',1,1),(3,'MT','METRO','COMPRIMENTO','2026-06-12 20:25:13.000000','2026-07-04 15:31:43.179129',1,1),(4,'CM','CENTÍMETRO','COMPRIMENTO','2026-06-12 20:25:13.000000','2026-07-04 15:31:46.237315',1,1),(5,'M2','METRO QUADRADO','ÁREA','2026-06-12 20:25:13.000000','2026-07-04 15:31:49.128799',1,1),(6,'PCT','PACOTE','QUANTIDADE','2026-06-12 20:25:13.000000','2026-07-04 15:31:51.477914',1,1),(7,'RL','ROLO','QUANTIDADE','2026-06-12 20:25:13.000000','2026-07-04 15:31:53.739576',1,1),(8,'RES','RESMA','PAPÉIS','2026-10-09 19:44:43.247295','2026-10-09 19:44:43.247296',1,1);
/*!40000 ALTER TABLE `unidades_medida` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nome` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `senha` varchar(60) COLLATE utf8mb4_unicode_ci NOT NULL,
  `telefone` varchar(12) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ultimo_acesso` datetime(6) DEFAULT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'Admin','admin@sistema.com','admin',NULL,NULL,'2026-06-06 13:05:42.000000','2026-06-06 13:05:42.000000',1);
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `veiculos`
--

DROP TABLE IF EXISTS `veiculos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `veiculos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_transportadora` int DEFAULT NULL,
  `id_estado` int NOT NULL,
  `placa` varchar(7) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rntrc` varchar(8) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `renavam` varchar(11) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tipo_veiculo` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `marca_modelo` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_transportadora` (`id_transportadora`),
  KEY `id_estado` (`id_estado`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `veiculos_ibfk_1` FOREIGN KEY (`id_transportadora`) REFERENCES `transportadoras` (`id`),
  CONSTRAINT `veiculos_ibfk_2` FOREIGN KEY (`id_estado`) REFERENCES `estados` (`id`),
  CONSTRAINT `veiculos_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_veiculos_placa` CHECK ((length(trim(`placa`)) > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `veiculos`
--

LOCK TABLES `veiculos` WRITE;
/*!40000 ALTER TABLE `veiculos` DISABLE KEYS */;
/*!40000 ALTER TABLE `veiculos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vendas`
--

DROP TABLE IF EXISTS `vendas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vendas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_emitente` int NOT NULL,
  `id_cliente` int NOT NULL,
  `data_venda` datetime(6) NOT NULL,
  `valor_total` decimal(7,2) NOT NULL DEFAULT '0.00',
  `observacao` text COLLATE utf8mb4_unicode_ci,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_emitente` (`id_emitente`),
  KEY `id_cliente` (`id_cliente`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `vendas_ibfk_1` FOREIGN KEY (`id_emitente`) REFERENCES `emitentes` (`id`),
  CONSTRAINT `vendas_ibfk_2` FOREIGN KEY (`id_cliente`) REFERENCES `clientes` (`id`),
  CONSTRAINT `vendas_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_vendas_valor_total` CHECK ((`valor_total` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendas`
--

LOCK TABLES `vendas` WRITE;
/*!40000 ALTER TABLE `vendas` DISABLE KEYS */;
/*!40000 ALTER TABLE `vendas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vendas_itens`
--

DROP TABLE IF EXISTS `vendas_itens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vendas_itens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_venda` int NOT NULL,
  `id_produto` int NOT NULL,
  `quantidade` decimal(10,4) NOT NULL,
  `valor_unitario` decimal(7,4) NOT NULL,
  `valor_desconto` decimal(7,4) NOT NULL DEFAULT '0.0000',
  `valor_total` decimal(7,4) NOT NULL,
  `data_criacao` datetime(6) NOT NULL,
  `data_ultima_alteracao` datetime(6) NOT NULL,
  `id_usuario_ultima_alteracao` int NOT NULL,
  `ativo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `id_venda` (`id_venda`),
  KEY `id_produto` (`id_produto`),
  KEY `id_usuario_ultima_alteracao` (`id_usuario_ultima_alteracao`),
  CONSTRAINT `vendas_itens_ibfk_1` FOREIGN KEY (`id_venda`) REFERENCES `vendas` (`id`),
  CONSTRAINT `vendas_itens_ibfk_2` FOREIGN KEY (`id_produto`) REFERENCES `produtos` (`id`),
  CONSTRAINT `vendas_itens_ibfk_3` FOREIGN KEY (`id_usuario_ultima_alteracao`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `ck_vendas_itens_quantidade` CHECK ((`quantidade` > 0)),
  CONSTRAINT `ck_vendas_itens_valor_desconto` CHECK ((`valor_desconto` >= 0)),
  CONSTRAINT `ck_vendas_itens_valor_total` CHECK ((`valor_total` >= 0)),
  CONSTRAINT `ck_vendas_itens_valor_unitario` CHECK ((`valor_unitario` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendas_itens`
--

LOCK TABLES `vendas_itens` WRITE;
/*!40000 ALTER TABLE `vendas_itens` DISABLE KEYS */;
/*!40000 ALTER TABLE `vendas_itens` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-06 14:43:42
