USE [master]
GO

/****** Object:  Database [Lamp]    Script Date: 12/02/2016 15:00:47 ******/
CREATE DATABASE [Lamp] ON  PRIMARY 
( NAME = N'Lamp', FILENAME = N'D:\SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\Lamp.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Lamp_log', FILENAME = N'D:\SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\Lamp_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [Lamp] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Lamp].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [Lamp] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Lamp] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Lamp] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Lamp] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Lamp] SET ARITHABORT OFF 
GO

ALTER DATABASE [Lamp] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Lamp] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [Lamp] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Lamp] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Lamp] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Lamp] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Lamp] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Lamp] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Lamp] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Lamp] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Lamp] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Lamp] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Lamp] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Lamp] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Lamp] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Lamp] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Lamp] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Lamp] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Lamp] SET  READ_WRITE 
GO

ALTER DATABASE [Lamp] SET RECOVERY FULL 
GO

ALTER DATABASE [Lamp] SET  MULTI_USER 
GO

ALTER DATABASE [Lamp] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Lamp] SET DB_CHAINING OFF 
GO

