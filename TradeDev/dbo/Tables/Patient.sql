CREATE TABLE [dbo].[Patient] (
    [ID]        INT          IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (50) NULL,
    [Surname]   VARCHAR (50) NULL,
    [Birthdate] DATETIME     NULL,
    [Gender]    CHAR         NULL,
    [Email]     VARCHAR (60) NULL,
    [Health_card_id] INT     NULL,
    [Secret]    VARCHAR (10) NULL,
    CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_Patient_Health_card] FOREIGN KEY ([Health_card_id]) REFERENCES [dbo].[Health_card]([ID])
);

