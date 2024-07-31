CREATE VIEW IF NOT EXISTS uvwLivroAutor AS
SELECT
  au.CodAu
  ,au.Nome AS NomeAutor
  ,l.CodL
  ,l.Titulo AS TituloLivro
  ,l.Editora
  ,l.Edicao
  ,l.AnoPublicacao
  ,GROUP_CONCAT(ass.Descricao, ', ') AS Assuntos
FROM Autor au
LEFT JOIN Livro_Autor lau ON lau.Autor_CodAu = au.CodAu
LEFT JOIN Livro l ON l.CodL = lau.Livro_CodL
LEFT JOIN Livro_Assunto las ON las.Livro_CodL = l.CodL
LEFT JOIN Assunto ass ON ass.CodAs = las.Assunto_CodAs
GROUP BY au.CodAu ,au.Nome ,l.CodL ,l.Titulo ,l.Editora ,l.Edicao ,l.AnoPublicacao
