
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllArticles]
AS
BEGIN

	SELECT Codigo, Nombre, idClaseArticulo, Precio, EsActivo FROM Articulo
	ORDER BY Nombre

END
GO