CREATE TABLE [dbo].[Doctor] (
    [ID]          INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NULL,
    [Surname]     VARCHAR (50) NULL,
    [Birthdate]   DATETIME     NULL,
    [Telephone]   VARCHAR(14)  NULL,
    [Email]       VARCHAR(60)  NULL,
    [Practice_id] INT          NULL,
    [Secret]      VARCHAR (10) NULL,
    CONSTRAINT [PK_Doctor] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Doctor_Practice] FOREIGN KEY ([Practice_id]) REFERENCES [dbo].[Practice] ([ID])
);

