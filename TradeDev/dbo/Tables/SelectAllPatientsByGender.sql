CREATE PROCEDURE [dbo].[SelectAllPatientsByGender]
	@gender char(1)
AS
	SELECT * FROM Patient WHERE Gender = @gender
GO;
