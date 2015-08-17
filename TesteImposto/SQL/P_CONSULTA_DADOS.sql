USE [Teste]
GO

IF OBJECT_ID('dbo.P_CONSULTA_DADOS') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.P_CONSULTA_DADOS
    IF OBJECT_ID('dbo.P_CONSULTA_DADOS') IS NOT NULL
        PRINT '<<< FALHA APAGANDO A PROCEDURE dbo.P_CONSULTA_DADOS >>>'
    ELSE
        PRINT '<<< PROCEDURE dbo.P_CONSULTA_DADOS APAGADA >>>'
END
go
SET QUOTED_IDENTIFIER ON
GO
SET NOCOUNT ON 
GO 
CREATE PROCEDURE dbo.P_CONSULTA_DADOS
AS
BEGIN

	SELECT IT.Cfop,
		   SUM(IT.BaseIcms),
		   SUM(IT.ValorIcms),
		   SUM(IT.BaseIpi),
	       SUM(IT.ValorIpi)	
      FROM NotaFiscalItem AS IT
     GROUP BY IT.Cfop

END
GO
GRANT EXECUTE ON dbo.P_CONSULTA_DADOS TO [public]
go
IF OBJECT_ID('dbo.P_CONSULTA_DADOS') IS NOT NULL
    PRINT '<<< PROCEDURE dbo.P_CONSULTA_DADOS CRIADA >>>'
ELSE
    PRINT '<<< FALHA NA CRIACAO DA PROCEDURE dbo.P_CONSULTA_DADOS >>>'
go

