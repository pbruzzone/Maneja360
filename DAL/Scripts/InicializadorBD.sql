IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Idioma]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Idioma] (
    [IdiomaId]    INT           IDENTITY (1, 1) NOT NULL,
    [Cultura]     NVARCHAR (10) NOT NULL,
    [Descripcion] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Idioma] PRIMARY KEY CLUSTERED ([IdiomaId] ASC)
);
SET IDENTITY_INSERT [dbo].[Idioma] ON
INSERT INTO [dbo].[Idioma] ([IdiomaId], [Cultura], [Descripcion]) VALUES (1, N'es-AR', N'Español')
INSERT INTO [dbo].[Idioma] ([IdiomaId], [Cultura], [Descripcion]) VALUES (2, N'en-US', N'Inglés')
SET IDENTITY_INSERT [dbo].[Idioma] OFF
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Perfil]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Perfil] (
    [PerfilId]    INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (100) NOT NULL,
    [Permiso]     NVARCHAR (100) NULL,
    CONSTRAINT [PK_Perfil] PRIMARY KEY CLUSTERED ([PerfilId] ASC)
);
SET IDENTITY_INSERT [dbo].[Perfil] ON
INSERT INTO [dbo].[Perfil] ([PerfilId], [Nombre], [Permiso]) VALUES (1, N'Administrador', NULL)
INSERT INTO [dbo].[Perfil] ([PerfilId], [Nombre], [Permiso]) VALUES (2, N'Operador', NULL)
INSERT INTO [dbo].[Perfil] ([PerfilId], [Nombre], [Permiso]) VALUES (3, N'Cliente', NULL)
INSERT INTO [dbo].[Perfil] ([PerfilId], [Nombre], [Permiso]) VALUES (4, N'VentaProducto', N'VentaProducto')
INSERT INTO [dbo].[Perfil] ([PerfilId], [Nombre], [Permiso]) VALUES (5, N'CompraProducto', N'CompraProducto')
SET IDENTITY_INSERT [dbo].[Perfil] OFF
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PerfilJerarquia]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PerfilJerarquia] (
    [PadreId] INT NULL,
    [HijoId]  INT NOT NULL,
    CONSTRAINT [FK_PerfilJararquia_Hijo] FOREIGN KEY ([HijoId]) REFERENCES [dbo].[Perfil] ([PerfilId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PerfilJerarquia_Padre] FOREIGN KEY ([PadreId]) REFERENCES [dbo].[Perfil] ([PerfilId])
);
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (NULL, 1)
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (NULL, 2)
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (NULL, 3)
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (1, 4)
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (1, 5)
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (2, 4)
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (2, 5)
INSERT INTO [dbo].[PerfilJerarquia] ([PadreId], [HijoId]) VALUES (3, 5)
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Usuario] (
    [UsuarioId]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]        NVARCHAR (100) NOT NULL,
    [Apellido]      NVARCHAR (100) NOT NULL,
    [DNI]           VARCHAR (20)   NOT NULL,
    [Mail]          NVARCHAR (200) NULL,
    [NombreUsuario] NVARCHAR (50)  NOT NULL,
    [Password]      NVARCHAR (200) NOT NULL,
    [Bloqueado]     BIT            NOT NULL,
    [Activo]        BIT            NOT NULL,
    [IdiomaId]      INT            NOT NULL,
    [ResetPassword] BIT            NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([UsuarioId] ASC),
    CONSTRAINT [FK_Usuario_Idioma] FOREIGN KEY ([IdiomaId]) REFERENCES [dbo].[Idioma] ([IdiomaId])
);
SET IDENTITY_INSERT [dbo].[Usuario] ON
INSERT INTO [dbo].[Usuario] ([UsuarioId], [Nombre], [Apellido], [DNI], [Mail], [NombreUsuario], [Password], [Bloqueado], [Activo], [IdiomaId], [ResetPassword]) VALUES (1, N'Administrador', N'Maneja360', N'99999999', N'admin@maneja360.com.ar', N'admin', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 0, 1, 1, 0)
INSERT INTO [dbo].[Usuario] ([UsuarioId], [Nombre], [Apellido], [DNI], [Mail], [NombreUsuario], [Password], [Bloqueado], [Activo], [IdiomaId], [ResetPassword]) VALUES (2, N'Pablo', N'Bruzzone', N'12345678', N'pbruzzone@maneja360.com.ar', N'pbruzzone', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 0, 1, 1, 0)
INSERT INTO [dbo].[Usuario] ([UsuarioId], [Nombre], [Apellido], [DNI], [Mail], [NombreUsuario], [Password], [Bloqueado], [Activo], [IdiomaId], [ResetPassword]) VALUES (3, N'Camila', N'Bruzzone', N'56372833', N'cbruzzone@maneja360.com.ar', N'belcami', N'8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 0, 1, 1, 0)
SET IDENTITY_INSERT [dbo].[Usuario] OFF
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UsuarioPerfil]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UsuarioPerfil]
(
	[UsuarioId] INT NOT NULL, 
    [PerfilId] INT NOT NULL, 
    CONSTRAINT PK_UsuarioPerfil PRIMARY KEY ([UsuarioId],[PerfilId]),
	CONSTRAINT FK_UsuarioPerfil_Usuario FOREIGN KEY ([UsuarioId]) REFERENCES Usuario(UsuarioId),
	CONSTRAINT FK_UsuarioPerfil_Perfil FOREIGN KEY ([PerfilId]) REFERENCES Perfil(PerfilId)
);
INSERT INTO [dbo].[UsuarioPerfil] ([UsuarioId], [PerfilId]) VALUES (1, 1)
INSERT INTO [dbo].[UsuarioPerfil] ([UsuarioId], [PerfilId]) VALUES (2, 2)
INSERT INTO [dbo].[UsuarioPerfil] ([UsuarioId], [PerfilId]) VALUES (3, 3)
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BitacoraEvento]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BitacoraEvento]
(
	[BitacoraEventoId]  INT IDENTITY (1, 1) NOT NULL,
	[NombreUsuario]     NVARCHAR(100) NOT NULL,
	[Fecha]             DATE NOT NULL,
	[Hora]              TIME NOT NULL,
	[Modulo]            INT NOT NULL,
	[Evento]            INT NOT NULL,
	[Criticidad]        INT NOT NULL,
    CONSTRAINT [PK_BitacoraEvento] PRIMARY KEY CLUSTERED ([BitacoraEventoId] ASC)
);
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DV]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DV] (
    [TableName] VARCHAR (50)    NOT NULL,
    [Hash]      VARBINARY(MAX)  NOT NULL,
    CONSTRAINT [PK_DV] PRIMARY KEY CLUSTERED ([TableName] ASC)
);
END