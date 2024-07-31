CREATE TABLE IF NOT EXISTS Livro_Assunto (
  Livro_CodL INTEGER NOT NULL,
  Assunto_CodAs INTEGER NOT NULL,
  CONSTRAINT Livro_Assunto_FKIndex1 FOREIGN KEY (Livro_CodL) REFERENCES Livro (CodL),
  CONSTRAINT Livro_Assunto_FKIndex2 FOREIGN KEY (Assunto_CodAs) REFERENCES Assunto (CodAs)
);
