PRAGMA foreign_keys = OFF;

CREATE TABLE IF NOT EXISTS Livro_Autor_Novo (
  Livro_CodL INTEGER NOT NULL,
  Autor_CodAu INTEGER NOT NULL,
  CONSTRAINT Livro_Autor_FKIndex1 FOREIGN KEY (Livro_CodL) REFERENCES Livro (CodL) ON DELETE CASCADE,
  CONSTRAINT Livro_Autor_FKIndex2 FOREIGN KEY (Autor_CodAu) REFERENCES Autor (CodAu) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Livro_Assunto_Novo (
  Livro_CodL INTEGER NOT NULL,
  Assunto_CodAs INTEGER NOT NULL,
  CONSTRAINT Livro_Assunto_FKIndex1 FOREIGN KEY (Livro_CodL) REFERENCES Livro (CodL) ON DELETE CASCADE,
  CONSTRAINT Livro_Assunto_FKIndex2 FOREIGN KEY (Assunto_CodAs) REFERENCES Assunto (CodAs) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS LivroValor_Novo (
  CodL INTEGER NOT NULL CONSTRAINT LivroValor_Livro_FK REFERENCES Livro (CodL) ON DELETE CASCADE,
  IdFormaCompra INTEGER NOT NULL CONSTRAINT LivroValor_FormaCompra_FK REFERENCES FormaCompra (IdFormaCompra) ON DELETE CASCADE,
  Valor DECIMAL(10, 2),
  PRIMARY KEY (CodL, IdFormaCompra) CONSTRAINT LivroValor_PK
);


INSERT INTO Livro_Autor_Novo (Livro_CodL, Autor_CodAu)
SELECT Livro_CodL, Autor_CodAu FROM Livro_Autor;

INSERT INTO Livro_Assunto_Novo (Livro_CodL, Assunto_CodAs)
SELECT Livro_CodL, Assunto_CodAs FROM Livro_Assunto;

INSERT INTO LivroValor_Novo (CodL, IdFormaCompra, Valor)
SELECT CodL, IdFormaCompra, Valor FROM LivroValor;


ALTER TABLE Livro_Autor RENAME TO Livro_Autor_Antigo;
ALTER TABLE Livro_Assunto RENAME TO Livro_Assunto_Antigo;
ALTER TABLE LivroValor RENAME TO LivroValor_Antigo;

ALTER TABLE Livro_Autor_Novo RENAME TO Livro_Autor;
ALTER TABLE Livro_Assunto_Novo RENAME TO Livro_Assunto;
ALTER TABLE LivroValor_Novo RENAME TO LivroValor;


DROP TABLE Livro_Autor_Antigo;
DROP TABLE Livro_Assunto_Antigo;
DROP TABLE LivroValor_Antigo;

PRAGMA foreign_keys = ON;
