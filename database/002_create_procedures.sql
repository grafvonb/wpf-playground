-- Creates or updates the calculator stored procedures. Safe to re-run.
-- Return codes: 0 = success, 1 = division by zero, 2 = overflow.
-- Operands are DECIMAL(18,9); results are DECIMAL(38,18).
USE CalculatorDb;
GO

CREATE OR ALTER PROCEDURE dbo.Calc_Add
    @Left DECIMAL(18, 9),
    @Right DECIMAL(18, 9),
    @Result DECIMAL(38, 18) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = NULL;
    BEGIN TRY
        SET @Result = @Left + @Right;
        RETURN 0;
    END TRY
    BEGIN CATCH
        SET @Result = NULL;
        IF ERROR_NUMBER() = 8115 RETURN 2;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.Calc_Subtract
    @Left DECIMAL(18, 9),
    @Right DECIMAL(18, 9),
    @Result DECIMAL(38, 18) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = NULL;
    BEGIN TRY
        SET @Result = @Left - @Right;
        RETURN 0;
    END TRY
    BEGIN CATCH
        SET @Result = NULL;
        IF ERROR_NUMBER() = 8115 RETURN 2;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.Calc_Multiply
    @Left DECIMAL(18, 9),
    @Right DECIMAL(18, 9),
    @Result DECIMAL(38, 18) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = NULL;
    BEGIN TRY
        SET @Result = @Left * @Right;
        RETURN 0;
    END TRY
    BEGIN CATCH
        SET @Result = NULL;
        IF ERROR_NUMBER() = 8115 RETURN 2;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.Calc_Divide
    @Left DECIMAL(18, 9),
    @Right DECIMAL(18, 9),
    @Result DECIMAL(38, 18) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Result = NULL;
    IF @Right = 0 RETURN 1;
    BEGIN TRY
        SET @Result = @Left / @Right;
        RETURN 0;
    END TRY
    BEGIN CATCH
        SET @Result = NULL;
        IF ERROR_NUMBER() = 8115 RETURN 2;
        THROW;
    END CATCH
END
GO
