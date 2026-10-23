PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Propietarios (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Codigo TEXT NOT NULL UNIQUE,
    Nombre TEXT NOT NULL,
    Telefono TEXT NOT NULL,
    Correo TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Mascotas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Codigo TEXT NOT NULL UNIQUE,
    Nombre TEXT NOT NULL,
    Especie TEXT NOT NULL,
    PropietarioId INTEGER NOT NULL,
    FOREIGN KEY (PropietarioId) REFERENCES Propietarios(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Insumos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Codigo TEXT NOT NULL UNIQUE,
    Nombre TEXT NOT NULL,
    Precio REAL NOT NULL CHECK (Precio > 0),
    Stock INTEGER NOT NULL CHECK (Stock >= 0)
);

CREATE TABLE IF NOT EXISTS Consultas (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MascotaId INTEGER NOT NULL,
    Servicio TEXT NOT NULL,
    Veterinario TEXT NOT NULL,
    Peso REAL NOT NULL CHECK (Peso > 0),
    Temperatura REAL NOT NULL CHECK (Temperatura BETWEEN 35 AND 43),
    Costo REAL NOT NULL CHECK (Costo > 0),
    InsumoId INTEGER NULL,
    CantidadInsumo INTEGER NOT NULL DEFAULT 0 CHECK (CantidadInsumo >= 0),
    Fecha TEXT NOT NULL,
    CHECK ((InsumoId IS NULL AND CantidadInsumo = 0)
        OR (InsumoId IS NOT NULL AND CantidadInsumo > 0)),
    FOREIGN KEY (MascotaId) REFERENCES Mascotas(Id) ON DELETE CASCADE,
    FOREIGN KEY (InsumoId) REFERENCES Insumos(Id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS IX_Mascotas_PropietarioId ON Mascotas(PropietarioId);
CREATE INDEX IF NOT EXISTS IX_Consultas_MascotaId ON Consultas(MascotaId);
CREATE INDEX IF NOT EXISTS IX_Consultas_InsumoId ON Consultas(InsumoId);