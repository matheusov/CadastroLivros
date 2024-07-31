DROP VIEW IF EXISTS uvwLivroAutor;

CREATE VIEW IF NOT EXISTS uvwLivroAutor AS
WITH LivroValores AS (
  SELECT
    l.CodL
    ,GROUP_CONCAT(fc.Descricao || ':' || CAST(lv.Valor AS DOUBLE), ',') AS Valores
  FROM Livro l
  LEFT JOIN LivroValor lv ON lv.CodL = l.CodL AND lv.Valor IS NOT NULL
  LEFT JOIN FormaCompra fc ON fc.IdFormaCompra = lv.IdFormaCompra
)
SELECT
  au.CodAu
  ,au.Nome AS NomeAutor
  ,l.CodL
  ,l.Titulo AS TituloLivro
  ,l.Editora
  ,l.Edicao
  ,l.AnoPublicacao
  ,ass.Descricao
  ,GROUP_CONCAT(ass.Descricao, ', ') AS Assuntos
  ,lv.Valores
FROM Autor au
LEFT JOIN Livro_Autor lau ON lau.Autor_CodAu = au.CodAu
LEFT JOIN Livro l ON l.CodL = lau.Livro_CodL
LEFT JOIN Livro_Assunto las ON las.Livro_CodL = l.CodL
LEFT JOIN Assunto ass ON ass.CodAs = las.Assunto_CodAs
LEFT JOIN LivroValores lv On lv.CodL = l.CodL
GROUP BY au.CodAu, au.Nome, l.CodL, l.Titulo, l.Editora, l.Edicao, l.AnoPublicacao, lv.Valores
