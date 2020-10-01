CREATE TABLE [dbo].[Appointment] (
    [ID]         INT           IDENTITY (1, 1) NOT NULL,
    [Doctor_id]  INT           NOT NULL,
    [Patient_id] INT           NOT NULL,
    [Diagnosis_code] VARCHAR(6) NULL, 
    [Diagnosis_description] VARCHAR(100) NULL,
    [Date]       DATETIME      NULL,
    [Secret]     VARCHAR (10)  NULL,    
    CONSTRAINT [PK_Appointment] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Appointment_Doctor] FOREIGN KEY ([Doctor_id]) REFERENCES [dbo].[Doctor] ([ID]),
    CONSTRAINT [FK_Appointment_Patient] FOREIGN KEY ([Patient_id]) REFERENCES [dbo].[Patient] ([ID])
);


GO

CREATE UNIQUE INDEX [IX_Appointment_ID] ON [dbo].[Appointment] ([ID])
