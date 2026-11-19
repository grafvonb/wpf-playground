-- Creates the CalculatorDb database when it does not exist. Safe to re-run.
IF DB_ID(N'CalculatorDb') IS NULL
BEGIN
    CREATE DATABASE CalculatorDb;
END
GO
