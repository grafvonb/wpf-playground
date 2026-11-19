-- Creates or updates the power stored procedure. Safe to re-run.
-- Return codes: 0 = success, 2 = overflow, 3 = zero to a negative power, 4 = not a real number.
-- Operands are DECIMAL(18,9); the result is DECIMAL(38,18). The calculation uses FLOAT, so results
-- are accurate to about 15 significant digits.
USE CalculatorDb;
GO

CREATE OR ALTER PROCEDURE dbo.Calc_Power
    @Left DECIMAL(18, 9),
    @Right DECIMAL(18, 9),
    @Result DECIMAL(38, 18) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = NULL;
    IF @Left = 0 AND @Right < 0 RETURN 3;
    IF @Left < 0 AND @Right <> FLOOR(@Right) RETURN 4;
    IF @Right = 0
    BEGIN
        SET @Result = 1;
        RETURN 0;
    END
    BEGIN TRY
        SET @Result = POWER(CAST(@Left AS FLOAT), CAST(@Right AS FLOAT));
        RETURN 0;
    END TRY
    BEGIN CATCH
        SET @Result = NULL;
        IF ERROR_NUMBER() IN (232, 3623, 8115) RETURN 2;
        THROW;
    END CATCH
END
GO
