USE MEDPASS;
GO

/* =========================
   TABLAS
========================= */

IF OBJECT_ID('dbo.Clinicas', 'U') IS NULL
BEGIN
    CREATE TABLE Clinicas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(120) NOT NULL,
        Direccion NVARCHAR(200) NOT NULL,
        Telefono NVARCHAR(20) NOT NULL,
        Codigo NVARCHAR(50) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Pacientes', 'U') IS NULL
BEGIN
    CREATE TABLE Pacientes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CURP NVARCHAR(18) NOT NULL,
        Nombre NVARCHAR(120) NOT NULL,
        FechaNac DATE NOT NULL,
        TipoSangre NVARCHAR(5) NOT NULL,
        Telefono NVARCHAR(20) NOT NULL,
        Email NVARCHAR(120) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Medicos', 'U') IS NULL
BEGIN
    CREATE TABLE Medicos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(120) NOT NULL,
        Especialidad NVARCHAR(100) NOT NULL,
        Cedula NVARCHAR(50) NOT NULL,
        ClinicaId INT NOT NULL,
        CONSTRAINT FK_Medicos_Clinicas FOREIGN KEY (ClinicaId) REFERENCES Clinicas(Id)
    );
END
GO

IF OBJECT_ID('dbo.Consultas', 'U') IS NULL
BEGIN
    CREATE TABLE Consultas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Fecha DATETIME NOT NULL,
        Motivo NVARCHAR(250) NOT NULL,
        Diagnostico NVARCHAR(250) NOT NULL,
        PacienteId INT NOT NULL,
        MedicoId INT NOT NULL,
        ClinicaId INT NOT NULL,
        CONSTRAINT FK_Consultas_Pacientes FOREIGN KEY (PacienteId) REFERENCES Pacientes(Id),
        CONSTRAINT FK_Consultas_Medicos FOREIGN KEY (MedicoId) REFERENCES Medicos(Id),
        CONSTRAINT FK_Consultas_Clinicas FOREIGN KEY (ClinicaId) REFERENCES Clinicas(Id)
    );
END
GO

IF OBJECT_ID('dbo.Alergias', 'U') IS NULL
BEGIN
    CREATE TABLE Alergias (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Sustancia NVARCHAR(120) NOT NULL,
        Reaccion NVARCHAR(250) NOT NULL,
        Severidad NVARCHAR(50) NOT NULL,
        PacienteId INT NOT NULL,
        CONSTRAINT FK_Alergias_Pacientes FOREIGN KEY (PacienteId) REFERENCES Pacientes(Id)
    );
END
GO

IF OBJECT_ID('dbo.Recetas', 'U') IS NULL
BEGIN
    CREATE TABLE Recetas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Medicamento NVARCHAR(120) NOT NULL,
        Dosis NVARCHAR(120) NOT NULL,
        Duracion NVARCHAR(120) NOT NULL,
        ConsultaId INT NOT NULL,
        CONSTRAINT FK_Recetas_Consultas FOREIGN KEY (ConsultaId) REFERENCES Consultas(Id)
    );
END
GO

IF OBJECT_ID('dbo.Estudios', 'U') IS NULL
BEGIN
    CREATE TABLE Estudios (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Tipo NVARCHAR(120) NOT NULL,
        Resultado NVARCHAR(250) NOT NULL,
        Fecha DATE NOT NULL,
        PacienteId INT NOT NULL,
        ClinicaId INT NOT NULL,
        CONSTRAINT FK_Estudios_Pacientes FOREIGN KEY (PacienteId) REFERENCES Pacientes(Id),
        CONSTRAINT FK_Estudios_Clinicas FOREIGN KEY (ClinicaId) REFERENCES Clinicas(Id)
    );
END
GO

/* =========================
   DATOS DE PRUEBA
========================= */

IF NOT EXISTS (SELECT 1 FROM Pacientes WHERE CURP = 'MALC980512HSONPR01')
INSERT INTO Pacientes (CURP, Nombre, FechaNac, TipoSangre, Telefono, Email)
VALUES ('MALC980512HSONPR01', 'Carlos Martinez Lopez', '1998-05-12', 'O+', '6311234567', 'carlos@mail.com');

IF NOT EXISTS (SELECT 1 FROM Pacientes WHERE CURP = 'GASL000818MSONTR02')
INSERT INTO Pacientes (CURP, Nombre, FechaNac, TipoSangre, Telefono, Email)
VALUES ('GASL000818MSONTR02', 'Lucia Gastelum Soto', '2000-08-18', 'A+', '6319876543', 'lucia@mail.com');

IF NOT EXISTS (SELECT 1 FROM Pacientes WHERE CURP = 'MATO020615MSONRR03')
INSERT INTO Pacientes (CURP, Nombre, FechaNac, TipoSangre, Telefono, Email)
VALUES ('MATO020615MSONRR03', 'Mariana Torres Ortega', '2002-06-15', 'B+', '6315558899', 'mariana@mail.com');

IF NOT EXISTS (SELECT 1 FROM Clinicas WHERE Codigo = 'CLIN-NOG-001')
INSERT INTO Clinicas (Nombre, Direccion, Telefono, Codigo)
VALUES ('Clinica Central Nogales', 'Av. Obregon 120, Nogales, Sonora', '6311112233', 'CLIN-NOG-001');

IF NOT EXISTS (SELECT 1 FROM Clinicas WHERE Codigo = 'CLIN-CAN-002')
INSERT INTO Clinicas (Nombre, Direccion, Telefono, Codigo)
VALUES ('Hospital General Cananea', 'Calle Juarez 45, Cananea, Sonora', '6452223344', 'CLIN-CAN-002');

IF NOT EXISTS (SELECT 1 FROM Clinicas WHERE Codigo = 'CLIN-HER-003')
INSERT INTO Clinicas (Nombre, Direccion, Telefono, Codigo)
VALUES ('Centro Medico Hermosillo', 'Blvd. Morelos 500, Hermosillo, Sonora', '6623334455', 'CLIN-HER-003');

DECLARE @Clinica1 INT = (SELECT Id FROM Clinicas WHERE Codigo = 'CLIN-NOG-001');
DECLARE @Clinica2 INT = (SELECT Id FROM Clinicas WHERE Codigo = 'CLIN-CAN-002');
DECLARE @Clinica3 INT = (SELECT Id FROM Clinicas WHERE Codigo = 'CLIN-HER-003');

IF NOT EXISTS (SELECT 1 FROM Medicos WHERE Cedula = 'MED-1001')
INSERT INTO Medicos (Nombre, Especialidad, Cedula, ClinicaId)
VALUES ('Dr. Jose Ramirez Soto', 'Medicina General', 'MED-1001', @Clinica1);

IF NOT EXISTS (SELECT 1 FROM Medicos WHERE Cedula = 'MED-1002')
INSERT INTO Medicos (Nombre, Especialidad, Cedula, ClinicaId)
VALUES ('Dra. Ana Lopez Garcia', 'Cardiologia', 'MED-1002', @Clinica2);

IF NOT EXISTS (SELECT 1 FROM Medicos WHERE Cedula = 'MED-1003')
INSERT INTO Medicos (Nombre, Especialidad, Cedula, ClinicaId)
VALUES ('Dr. Miguel Hernandez Ruiz', 'Pediatria', 'MED-1003', @Clinica3);

DECLARE @Paciente1 INT = (SELECT Id FROM Pacientes WHERE CURP = 'MALC980512HSONPR01');
DECLARE @Paciente2 INT = (SELECT Id FROM Pacientes WHERE CURP = 'GASL000818MSONTR02');
DECLARE @Paciente3 INT = (SELECT Id FROM Pacientes WHERE CURP = 'MATO020615MSONRR03');

DECLARE @Medico1 INT = (SELECT Id FROM Medicos WHERE Cedula = 'MED-1001');
DECLARE @Medico2 INT = (SELECT Id FROM Medicos WHERE Cedula = 'MED-1002');
DECLARE @Medico3 INT = (SELECT Id FROM Medicos WHERE Cedula = 'MED-1003');

IF NOT EXISTS (SELECT 1 FROM Consultas WHERE Motivo = 'Dolor de cabeza frecuente')
INSERT INTO Consultas (Fecha, Motivo, Diagnostico, PacienteId, MedicoId, ClinicaId)
VALUES ('2026-03-01T10:30:00', 'Dolor de cabeza frecuente', 'Migrana tensional leve', @Paciente1, @Medico1, @Clinica1);

IF NOT EXISTS (SELECT 1 FROM Consultas WHERE Motivo = 'Revision general y chequeo de presion')
INSERT INTO Consultas (Fecha, Motivo, Diagnostico, PacienteId, MedicoId, ClinicaId)
VALUES ('2026-03-23T09:00:00', 'Revision general y chequeo de presion', 'Presion arterial elevada, dieta baja en sodio', @Paciente2, @Medico2, @Clinica2);

IF NOT EXISTS (SELECT 1 FROM Consultas WHERE Motivo = 'Dolor abdominal y nauseas')
INSERT INTO Consultas (Fecha, Motivo, Diagnostico, PacienteId, MedicoId, ClinicaId)
VALUES ('2026-04-10T12:15:00', 'Dolor abdominal y nauseas', 'Gastritis leve, seguimiento en una semana', @Paciente3, @Medico3, @Clinica3);

IF NOT EXISTS (SELECT 1 FROM Alergias WHERE Sustancia = 'Penicilina' AND PacienteId = @Paciente1)
INSERT INTO Alergias (Sustancia, Reaccion, Severidad, PacienteId)
VALUES ('Penicilina', 'Erupcion cutanea generalizada', 'Alta', @Paciente1);

IF NOT EXISTS (SELECT 1 FROM Alergias WHERE Sustancia = 'Ibuprofeno' AND PacienteId = @Paciente2)
INSERT INTO Alergias (Sustancia, Reaccion, Severidad, PacienteId)
VALUES ('Ibuprofeno', 'Inflamacion y dificultad para respirar', 'Media', @Paciente2);

IF NOT EXISTS (SELECT 1 FROM Alergias WHERE Sustancia = 'Mariscos' AND PacienteId = @Paciente3)
INSERT INTO Alergias (Sustancia, Reaccion, Severidad, PacienteId)
VALUES ('Mariscos', 'Urticaria y dolor estomacal', 'Alta', @Paciente3);

DECLARE @Consulta1 INT = (SELECT Id FROM Consultas WHERE Motivo = 'Dolor de cabeza frecuente');
DECLARE @Consulta2 INT = (SELECT Id FROM Consultas WHERE Motivo = 'Revision general y chequeo de presion');
DECLARE @Consulta3 INT = (SELECT Id FROM Consultas WHERE Motivo = 'Dolor abdominal y nauseas');

IF NOT EXISTS (SELECT 1 FROM Recetas WHERE Medicamento = 'Paracetamol' AND ConsultaId = @Consulta1)
INSERT INTO Recetas (Medicamento, Dosis, Duracion, ConsultaId)
VALUES ('Paracetamol', '500 mg cada 8 horas', '3 dias', @Consulta1);

IF NOT EXISTS (SELECT 1 FROM Recetas WHERE Medicamento = 'Losartan' AND ConsultaId = @Consulta2)
INSERT INTO Recetas (Medicamento, Dosis, Duracion, ConsultaId)
VALUES ('Losartan', '50 mg cada 24 horas', '30 dias', @Consulta2);

IF NOT EXISTS (SELECT 1 FROM Recetas WHERE Medicamento = 'Omeprazol' AND ConsultaId = @Consulta3)
INSERT INTO Recetas (Medicamento, Dosis, Duracion, ConsultaId)
VALUES ('Omeprazol', '20 mg antes del desayuno', '14 dias', @Consulta3);

IF NOT EXISTS (SELECT 1 FROM Estudios WHERE Tipo = 'Biometria hematica' AND PacienteId = @Paciente1)
INSERT INTO Estudios (Tipo, Resultado, Fecha, PacienteId, ClinicaId)
VALUES ('Biometria hematica', 'Valores dentro del rango normal', '2026-03-02', @Paciente1, @Clinica1);

IF NOT EXISTS (SELECT 1 FROM Estudios WHERE Tipo = 'Electrocardiograma' AND PacienteId = @Paciente2)
INSERT INTO Estudios (Tipo, Resultado, Fecha, PacienteId, ClinicaId)
VALUES ('Electrocardiograma', 'Ritmo sinusal, sin alteraciones graves', '2026-03-24', @Paciente2, @Clinica2);

IF NOT EXISTS (SELECT 1 FROM Estudios WHERE Tipo = 'Ultrasonido abdominal' AND PacienteId = @Paciente3)
INSERT INTO Estudios (Tipo, Resultado, Fecha, PacienteId, ClinicaId)
VALUES ('Ultrasonido abdominal', 'Inflamacion leve, sin masas visibles', '2026-04-11', @Paciente3, @Clinica3);
GO

/* =========================
   PROCEDIMIENTOS PACIENTES
========================= */

CREATE OR ALTER PROCEDURE sp_ObtenerPacientes
AS
BEGIN
    SELECT Id, CURP, Nombre, FechaNac, TipoSangre, Telefono, Email FROM Pacientes;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerPacientePorId
@Id INT
AS
BEGIN
    SELECT Id, CURP, Nombre, FechaNac, TipoSangre, Telefono, Email
    FROM Pacientes WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_CrearPaciente
@CURP NVARCHAR(18),
@Nombre NVARCHAR(120),
@FechaNac DATE,
@TipoSangre NVARCHAR(5),
@Telefono NVARCHAR(20),
@Email NVARCHAR(120)
AS
BEGIN
    INSERT INTO Pacientes (CURP, Nombre, FechaNac, TipoSangre, Telefono, Email)
    VALUES (@CURP, @Nombre, @FechaNac, @TipoSangre, @Telefono, @Email);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarPaciente
@Id INT,
@CURP NVARCHAR(18),
@Nombre NVARCHAR(120),
@FechaNac DATE,
@TipoSangre NVARCHAR(5),
@Telefono NVARCHAR(20),
@Email NVARCHAR(120)
AS
BEGIN
    UPDATE Pacientes
    SET CURP = @CURP, Nombre = @Nombre, FechaNac = @FechaNac,
        TipoSangre = @TipoSangre, Telefono = @Telefono, Email = @Email
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarPaciente
@Id INT
AS
BEGIN
    DELETE FROM Pacientes WHERE Id = @Id;
END
GO

/* =========================
   PROCEDIMIENTOS CLINICAS
========================= */

CREATE OR ALTER PROCEDURE sp_ObtenerClinicas
AS
BEGIN
    SELECT Id, Nombre, Direccion, Telefono, Codigo FROM Clinicas;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerClinicaPorId
@Id INT
AS
BEGIN
    SELECT Id, Nombre, Direccion, Telefono, Codigo FROM Clinicas WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_CrearClinica
@Nombre NVARCHAR(120),
@Direccion NVARCHAR(200),
@Telefono NVARCHAR(20),
@Codigo NVARCHAR(50)
AS
BEGIN
    INSERT INTO Clinicas (Nombre, Direccion, Telefono, Codigo)
    VALUES (@Nombre, @Direccion, @Telefono, @Codigo);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarClinica
@Id INT,
@Nombre NVARCHAR(120),
@Direccion NVARCHAR(200),
@Telefono NVARCHAR(20),
@Codigo NVARCHAR(50)
AS
BEGIN
    UPDATE Clinicas
    SET Nombre = @Nombre, Direccion = @Direccion, Telefono = @Telefono, Codigo = @Codigo
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarClinica
@Id INT
AS
BEGIN
    DELETE FROM Clinicas WHERE Id = @Id;
END
GO

/* =========================
   PROCEDIMIENTOS MEDICOS
========================= */

CREATE OR ALTER PROCEDURE sp_ObtenerMedicos
AS
BEGIN
    SELECT Id, Nombre, Especialidad, Cedula, ClinicaId FROM Medicos;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerMedicoPorId
@Id INT
AS
BEGIN
    SELECT Id, Nombre, Especialidad, Cedula, ClinicaId FROM Medicos WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_CrearMedico
@Nombre NVARCHAR(120),
@Especialidad NVARCHAR(100),
@Cedula NVARCHAR(50),
@ClinicaId INT
AS
BEGIN
    INSERT INTO Medicos (Nombre, Especialidad, Cedula, ClinicaId)
    VALUES (@Nombre, @Especialidad, @Cedula, @ClinicaId);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarMedico
@Id INT,
@Nombre NVARCHAR(120),
@Especialidad NVARCHAR(100),
@Cedula NVARCHAR(50),
@ClinicaId INT
AS
BEGIN
    UPDATE Medicos
    SET Nombre = @Nombre, Especialidad = @Especialidad, Cedula = @Cedula, ClinicaId = @ClinicaId
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarMedico
@Id INT
AS
BEGIN
    DELETE FROM Medicos WHERE Id = @Id;
END
GO

/* =========================
   PROCEDIMIENTOS CONSULTAS
========================= */

CREATE OR ALTER PROCEDURE sp_ObtenerConsultas
AS
BEGIN
    SELECT Id, Fecha, Motivo, Diagnostico, PacienteId, MedicoId, ClinicaId FROM Consultas;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerConsultaPorId
@Id INT
AS
BEGIN
    SELECT Id, Fecha, Motivo, Diagnostico, PacienteId, MedicoId, ClinicaId
    FROM Consultas WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_CrearConsulta
@Fecha DATETIME,
@Motivo NVARCHAR(250),
@Diagnostico NVARCHAR(250),
@PacienteId INT,
@MedicoId INT,
@ClinicaId INT
AS
BEGIN
    INSERT INTO Consultas (Fecha, Motivo, Diagnostico, PacienteId, MedicoId, ClinicaId)
    VALUES (@Fecha, @Motivo, @Diagnostico, @PacienteId, @MedicoId, @ClinicaId);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarConsulta
@Id INT,
@Motivo NVARCHAR(250),
@Diagnostico NVARCHAR(250)
AS
BEGIN
    UPDATE Consultas
    SET Motivo = @Motivo, Diagnostico = @Diagnostico
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarConsulta
@Id INT
AS
BEGIN
    DELETE FROM Consultas WHERE Id = @Id;
END
GO

/* =========================
   PROCEDIMIENTOS ALERGIAS
========================= */

CREATE OR ALTER PROCEDURE sp_ObtenerAlergias
AS
BEGIN
    SELECT Id, Sustancia, Reaccion, Severidad, PacienteId FROM Alergias;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerAlergiaPorId
@Id INT
AS
BEGIN
    SELECT Id, Sustancia, Reaccion, Severidad, PacienteId FROM Alergias WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_CrearAlergia
@Sustancia NVARCHAR(120),
@Reaccion NVARCHAR(250),
@Severidad NVARCHAR(50),
@PacienteId INT
AS
BEGIN
    INSERT INTO Alergias (Sustancia, Reaccion, Severidad, PacienteId)
    VALUES (@Sustancia, @Reaccion, @Severidad, @PacienteId);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarAlergia
@Id INT,
@Sustancia NVARCHAR(120),
@Reaccion NVARCHAR(250),
@Severidad NVARCHAR(50)
AS
BEGIN
    UPDATE Alergias
    SET Sustancia = @Sustancia, Reaccion = @Reaccion, Severidad = @Severidad
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarAlergia
@Id INT
AS
BEGIN
    DELETE FROM Alergias WHERE Id = @Id;
END
GO

/* =========================
   PROCEDIMIENTOS RECETAS
========================= */

CREATE OR ALTER PROCEDURE sp_ObtenerRecetas
AS
BEGIN
    SELECT Id, Medicamento, Dosis, Duracion, ConsultaId FROM Recetas;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerRecetaPorId
@Id INT
AS
BEGIN
    SELECT Id, Medicamento, Dosis, Duracion, ConsultaId FROM Recetas WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_CrearReceta
@Medicamento NVARCHAR(120),
@Dosis NVARCHAR(120),
@Duracion NVARCHAR(120),
@ConsultaId INT
AS
BEGIN
    INSERT INTO Recetas (Medicamento, Dosis, Duracion, ConsultaId)
    VALUES (@Medicamento, @Dosis, @Duracion, @ConsultaId);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarReceta
@Id INT,
@Medicamento NVARCHAR(120),
@Dosis NVARCHAR(120),
@Duracion NVARCHAR(120)
AS
BEGIN
    UPDATE Recetas
    SET Medicamento = @Medicamento, Dosis = @Dosis, Duracion = @Duracion
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarReceta
@Id INT
AS
BEGIN
    DELETE FROM Recetas WHERE Id = @Id;
END
GO

/* =========================
   PROCEDIMIENTOS ESTUDIOS
========================= */

CREATE OR ALTER PROCEDURE sp_ObtenerEstudios
AS
BEGIN
    SELECT Id, Tipo, Resultado, Fecha, PacienteId, ClinicaId FROM Estudios;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerEstudioPorId
@Id INT
AS
BEGIN
    SELECT Id, Tipo, Resultado, Fecha, PacienteId, ClinicaId FROM Estudios WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_CrearEstudio
@Tipo NVARCHAR(120),
@Resultado NVARCHAR(250),
@Fecha DATE,
@PacienteId INT,
@ClinicaId INT
AS
BEGIN
    INSERT INTO Estudios (Tipo, Resultado, Fecha, PacienteId, ClinicaId)
    VALUES (@Tipo, @Resultado, @Fecha, @PacienteId, @ClinicaId);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarEstudio
@Id INT,
@Tipo NVARCHAR(120),
@Resultado NVARCHAR(250)
AS
BEGIN
    UPDATE Estudios
    SET Tipo = @Tipo, Resultado = @Resultado
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarEstudio
@Id INT
AS
BEGIN
    DELETE FROM Estudios WHERE Id = @Id;
END
GO
