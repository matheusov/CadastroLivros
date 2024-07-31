CREATE TABLE IF NOT EXISTS LivroValor (
  CodL INTEGER NOT NULL CONSTRAINT LivroValor_Livro_FK REFERENCES Livro (CodL),
  IdFormaCompra INTEGER NOT NULL CONSTRAINT LivroValor_FormaCompra_FK REFERENCES FormaCompra (IdFormaCompra),
  Valor DECIMAL(10, 2),
  PRIMARY KEY (CodL, IdFormaCompra) CONSTRAINT LivroValor_PK
);
