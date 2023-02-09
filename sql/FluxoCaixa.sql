CREATE DATABASE FluxoCaixaDB
GO

USE FluxoCaixaDB
GO

IF OBJECT_ID ('dbo.Contas') IS NOT NULL
	DROP TABLE dbo.Contas
GO

CREATE TABLE dbo.Contas
	(
	Id           UNIQUEIDENTIFIER NOT NULL,
	Descricao    VARCHAR (500) NOT NULL,
	Ativo        BIT NOT NULL,
	DataCadastro DATETIME2 NOT NULL,
	ContaTipo    INT NOT NULL,
	CONSTRAINT PK_Contas PRIMARY KEY (Id)
	)
GO

IF OBJECT_ID ('dbo.Lancamentos') IS NOT NULL
	DROP TABLE dbo.Lancamentos
GO

CREATE TABLE dbo.Lancamentos
	(
	Id               UNIQUEIDENTIFIER NOT NULL,
	Valor            DECIMAL (18, 2) NOT NULL,
	DataEscrituracao DATETIME2 NOT NULL,
	DataCadastro     DATETIME2 NOT NULL,
	ContaId          UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT PK_Lancamentos PRIMARY KEY (Id)
	)
GO

USE FluxoCaixaDB
GO

-- Cadastramento de Contas

INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('A8C400CC-DAD9-4D8A-ABC6-093777AD4F48', 'Empréstimos', 1, '2023-01-30 22:06:29.563189', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('2A867831-EE12-44F1-A689-17BE2892D591', 'Energia elétrica', 1, '2023-01-30 22:06:29.563188', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('59AC3E1A-0EED-4879-AAAB-1F7F7C148D83', 'Impostos', 1, '2023-01-30 22:06:29.56319', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('297F9108-0F10-4466-8819-228284385ECC', 'Financiamento', 1, '2023-01-30 22:06:29.563186', 2)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('0B5C8E13-4BD0-4C3F-948E-7B64E58EA266', 'Serviços contabilidade', 1, '2023-01-30 22:06:29.563189', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('39A87037-BFA0-4FF9-AD5A-7C04C2E13C2B', 'Financiamentos', 1, '2023-01-30 22:06:29.563189', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('03C3A557-C972-49AD-B913-93304B6147AE', 'Folha de pagamento', 1, '2023-01-30 22:06:29.563187', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('4E40C50D-A514-4FDD-9EB5-A551A4D41F55', 'Telefone', 1, '2023-01-30 22:06:29.563188', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('27AB124E-207B-4AD2-8B3E-BCAA68C87120', 'Advogado', 1, '2023-01-30 22:06:29.56319', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('1BF47BE9-926D-4BEB-86D3-BDA42A17382B', 'Saldo Inicial', 1, '2023-01-30 22:06:29.559574', 1)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('09277791-7FFB-4F01-BE9E-BE7FA32C1273', 'Retiradas sócios', 1, '2023-01-30 22:06:29.563187', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('69562CEF-9241-4D24-8999-D333D91587AA', 'Outros pagamentos', 1, '2023-01-30 22:06:29.563189', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('0CF8E5A2-0AF0-4185-8494-D4B830C7D972', 'Materiais', 1, '2023-01-30 22:06:29.563187', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('3D4EADD8-DD32-428A-88CF-DF3E8A89848F', 'Investimentos', 1, '2023-01-30 22:06:29.563189', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('9BC32333-7E08-4B47-8E20-E8ADE041845F', 'Vendas', 1, '2023-01-30 22:06:29.563182', 2)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('CCC1011E-FE3E-4ABA-B6A6-E9AFB8646AEC', 'Fornecedores', 1, '2023-01-30 22:06:29.563186', 3)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('E043A7C3-96E5-4A3A-9533-EC2456FA8118', 'Entrada', 1, '2023-01-30 22:06:29.563186', 2)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('27AD1FC4-582F-40BE-BE67-ED3C8729B0BA', 'Juros', 1, '2023-01-30 22:06:29.563186', 2)
GO
INSERT INTO Contas (Id, Descricao, Ativo, DataCadastro, ContaTipo) VALUES ('230CB33D-B4CB-409D-A34B-FCBB672C8ACC', 'Aluguel', 1, '2023-01-30 22:06:29.563188', 3)
GO
