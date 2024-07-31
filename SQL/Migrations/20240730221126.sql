CREATE TABLE IF NOT EXISTS FormaCompra (
  IdFormaCompra INTEGER PRIMARY KEY CONSTRAINT FormaCompra_PK,
  Descricao VARCHAR(40) NOT NULL
);

INSERT INTO FormaCompra (IdFormaCompra, Descricao)
VALUES
  (1, 'Balcão'),
  (2, 'Self-service'),
  (3, 'Internet'),
  (4, 'Evento');

CREATE TABLE IF NOT EXISTS LivroValor (
  CodL INTEGER NOT NULL CONSTRAINT LivroValor_Livro_FK REFERENCES Livro (CodL),
  IdFormaCompra INTEGER NOT NULL CONSTRAINT LivroValor_FormaCompra_FK REFERENCES FormaCompra (IdFormaCompra),
  Valor DECIMAL(10, 2),
  PRIMARY KEY (CodL, IdFormaCompra) CONSTRAINT LivroValor_PK
);

