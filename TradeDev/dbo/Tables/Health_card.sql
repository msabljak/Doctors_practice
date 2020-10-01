CREATE TABLE [dbo].[Health_card]
(
	[ID] INT IDENTITY(1,1) NOT NULL,
	[History_of_illness] VARCHAR(300) NULL,
	[Blood_type] VARCHAR(3) NULL,
	[Hereditary_diseases] VARCHAR(300) NULL, 
    CONSTRAINT [PK_Health_card] PRIMARY KEY CLUSTERED([ID] ASC)
)
