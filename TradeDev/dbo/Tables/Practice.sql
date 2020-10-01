CREATE TABLE [dbo].[Practice] (
    [ID]      INT           IDENTITY (1, 1) NOT NULL,
    [Name]    VARCHAR (100) NULL,
    [Address] VARCHAR (60)  NULL,
    [Specialty] VARBINARY(50) NULL,
    [Secret]  VARCHAR (10)  NULL,     
    CONSTRAINT [PK_Practice] PRIMARY KEY CLUSTERED ([ID] ASC)
);

