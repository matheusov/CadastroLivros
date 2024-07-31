PRAGMA foreign_keys = OFF;

CREATE TABLE IF NOT EXISTS Autor_Novo (
  CodAu INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT CONSTRAINT Autor_PK,
  Nome VARCHAR(40) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Assunto_Novo (
  CodAs INTEGER PRIMARY KEY AUTOINCREMENT CONSTRAINT Assunto_PK,
  Descricao VARCHAR(40) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Livro_Autor_Novo (
  Livro_CodL INTEGER NOT NULL,
  Autor_CodAu INTEGER NOT NULL,
  CONSTRAINT Livro_Autor_FKIndex1 FOREIGN KEY (Livro_CodL) REFERENCES Livro (CodL) ON DELETE CASCADE,
  CONSTRAINT Livro_Autor_FKIndex2 FOREIGN KEY (Autor_CodAu) REFERENCES Autor (CodAu) ON DELETE CASCADE,
  UNIQUE (Livro_CodL, Autor_CodAu)
);

CREATE TABLE IF NOT EXISTS Livro_Assunto_Novo (
  Livro_CodL INTEGER NOT NULL,
  Assunto_CodAs INTEGER NOT NULL,
  CONSTRAINT Livro_Assunto_FKIndex1 FOREIGN KEY (Livro_CodL) REFERENCES Livro (CodL) ON DELETE CASCADE,
  CONSTRAINT Livro_Assunto_FKIndex2 FOREIGN KEY (Assunto_CodAs) REFERENCES Assunto (CodAs) ON DELETE CASCADE,
  UNIQUE (Livro_CodL, Assunto_CodAs)
);


INSERT INTO Autor_Novo (CodAu, Nome)
SELECT CodAu, Nome FROM Autor;

INSERT INTO Assunto_Novo (CodAs, Descricao)
SELECT CodAs, Descricao FROM Assunto;

INSERT INTO Livro_Autor_Novo (Livro_CodL, Autor_CodAu)
SELECT Livro_CodL, Autor_CodAu FROM Livro_Autor;

INSERT INTO Livro_Assunto_Novo (Livro_CodL, Assunto_CodAs)
SELECT Livro_CodL, Assunto_CodAs FROM Livro_Assunto;


ALTER TABLE Autor RENAME TO Autor_Antigo;
ALTER TABLE Assunto RENAME TO Assunto_Antigo;
ALTER TABLE Livro_Autor RENAME TO Livro_Autor_Antigo;
ALTER TABLE Livro_Assunto RENAME TO Livro_Assunto_Antigo;

ALTER TABLE Autor_Novo RENAME TO Autor;
ALTER TABLE Assunto_Novo RENAME TO Assunto;
ALTER TABLE Livro_Autor_Novo RENAME TO Livro_Autor;
ALTER TABLE Livro_Assunto_Novo RENAME TO Livro_Assunto;

DROP TABLE Autor_Antigo;
DROP TABLE Assunto_Antigo;
DROP TABLE Livro_Autor_Antigo;
DROP TABLE Livro_Assunto_Antigo;

PRAGMA foreign_keys = ON;
