use bigmall
go
ALTER DATABASE [BigMall] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BigMall].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BigMall] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BigMall] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BigMall] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BigMall] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BigMall] SET ARITHABORT OFF 
GO
ALTER DATABASE [BigMall] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BigMall] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BigMall] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BigMall] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BigMall] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BigMall] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BigMall] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BigMall] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BigMall] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BigMall] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BigMall] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BigMall] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BigMall] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BigMall] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BigMall] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BigMall] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BigMall] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BigMall] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BigMall] SET  MULTI_USER 
GO
ALTER DATABASE [BigMall] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BigMall] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BigMall] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BigMall] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [BigMall] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'BigMall', N'ON'
GO
USE [BigMall]
GO
/****** Object:  UserDefinedFunction [dbo].[CategoryFullName]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* CREATE FUNCTION [dbo].[CategoryFullName]
(
	@CategoryCode NVARCHAR(20)
)
RETURNS NVARCHAR(256)
AS
BEGIN
	DECLARE @FullName NVARCHAR(250);
	WITH cte AS
	(
		SELECT ParentCode,name,Level FROM Category WHERE code=@CategoryCode
		UNION ALL
		SELECT t.ParentCode,t.name,t.Level FROM Category t,cte WHERE t.Code=cte.ParentCode
	)
	SELECT @FullName=STUFF((SELECT '-'+cte.name FROM cte ORDER BY cte.Level ASC FOR XML PATH('')),1,1,'');
	RETURN @FullName;
END */

GO
/****** Object:  Table [dbo].[Account]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](64) NULL,
	[Password] [nvarchar](64) NULL,
	[NickName] [nvarchar](64) NULL,
	[RoleId] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[Status] [int] NULL,
	[LoginErrorCount] [int] NULL,
	[LastUpdateDate] [datetime] NULL,
	[Phone] [nvarchar](50) NULL,
 CONSTRAINT [PK_ACCOUNT] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AccountLoginHistory]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountLoginHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NULL,
	[UserName] [nvarchar](64) NULL,
	[CreatedOn] [datetime] NULL,
	[IPAddress] [nvarchar](64) NULL,
	[LoginStatus] [int] NULL,
 CONSTRAINT [PK_ACCOUNTLOGINHISTORY] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Area]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Area](
	[Id] [char](6) NOT NULL,
	[Name] [nvarchar](64) NULL,
	[ShowName] [nvarchar](64) NULL,
	[FullName] [nvarchar](256) NULL,
	[Level] [int] NULL,
 CONSTRAINT [PK_AREA] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BillSequence]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BillSequence](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GuidCode] [nvarchar](32) NULL,
 CONSTRAINT [PK_BILLSEQUENCE] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Brand]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brand](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](20) NULL,
 CONSTRAINT [PK_BRAND] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BrandRechargeVoucher]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BrandRechargeVoucher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NULL,
	[StoreId] [int] NULL,
	[StoreCode] [nvarchar](20) NULL,
	[StoreName] [nvarchar](200) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[BrandId] [int] NULL,
	[BrandCode] [nvarchar](20) NULL,
	[BrandName] [nvarchar](20) NULL,
	[Amount] [decimal](20, 2) NULL,
	[Reduced] [decimal](20, 2) NULL,
	[Balance] [decimal](20, 2) NULL,
	[Limit] [decimal](10, 2) NULL,
	[Status] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[CreatedByName] [nvarchar](50) NULL,
	[AuditOn] [datetime] NULL,
	[AuditBy] [int] NULL,
	[AuditByName] [nvarchar](50) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_BrandRechargeVoucher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Category]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[ParentCode] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[Level] [int] NULL,
	[FullName] [nvarchar](256) NULL,
	[Description] [nvarchar](512) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CategoryCard]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CategoryCard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NULL,
	[CardNumber] [varchar](50) NULL,
 CONSTRAINT [PK_CategoryCard] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CategoryRechargeVoucher]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CategoryRechargeVoucher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NULL,
	[StoreId] [int] NULL,
	[StoreCode] [nvarchar](20) NULL,
	[StoreName] [nvarchar](200) NULL,
	[CardNumber] [varchar](50) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Amount] [decimal](20, 2) NULL,
	[Reduced] [decimal](20, 2) NULL,
	[Balance] [decimal](20, 2) NULL,
	[Limit] [decimal](10, 2) NULL,
	[Status] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[CreatedByName] [nvarchar](50) NULL,
	[AuditOn] [datetime] NULL,
	[AuditBy] [int] NULL,
	[AuditByName] [nvarchar](50) NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_CategoryRechargeVoucher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerCode] [nvarchar](20) NULL,
	[CustomerName] [nvarchar](100) NULL,
	[CommercialActivities] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExceptProduct]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExceptProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NULL,
	[CategoryRechargeVoucherId] [int] NULL,
	[BrandRechargeVoucherId] [int] NULL,
	[ProductId] [int] NULL,
	[ProductCode] [nvarchar](50) NULL,
	[ProductName] [nvarchar](50) NULL,
 CONSTRAINT [PK_ExceptProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Menu]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[Name] [nvarchar](64) NULL,
	[Url] [nvarchar](256) NULL,
	[Icon] [nvarchar](64) NULL,
	[DisplayOrder] [int] NULL,
	[UrlType] [int] NULL,
 CONSTRAINT [PK_MENU] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ParticipantCategory]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParticipantCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NULL,
	[CategoryRechargeVoucherId] [int] NULL,
	[BrandRechargeVoucherId] [int] NULL,
	[CategoryId] [int] NULL,
	[CategoryCode] [nvarchar](50) NULL,
	[CategoryName] [nvarchar](50) NULL,
 CONSTRAINT [PK_ParticipantCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessHistory]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcessHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedByName] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[Status] [int] NULL,
	[FormId] [int] NULL,
	[FormType] [nvarchar](64) NULL,
	[Remark] [nvarchar](1000) NULL,
 CONSTRAINT [PK_PROCESSHISTORY] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Product]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Spec] [nvarchar](200) NULL,
	[Unit] [nvarchar](10) NULL,
	[CategoryId] [int] NULL,
	[BrandId] [int] NULL,
	[SecondSpec] [nvarchar](20) NULL,
	[HasSNCode] [bit] NULL CONSTRAINT [DF__Product__HasSNCc__4DD47EBD]  DEFAULT ((0)),
	[TaxRate] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Product_1] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductPrice]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[StoreId] [int] NULL,
	[SalePrice] [decimal](18, 4) NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[SapIdentity] [nvarchar](10) NULL,
	[PriceType] [int] NULL,
	[Status] [int] NULL CONSTRAINT [DF_ProductPrice_Status]  DEFAULT ((1)),
	[SapPriceType] [nvarchar](20) NULL,
	[SapSalePrice] [decimal](18, 4) NULL,
	[SapPriceUnit] [int] NULL,
	[ProductCode] [nvarchar](20) NULL,
	[CustomerCode] [nvarchar](20) NULL,
	[CustomerId] [int] NULL,
	[Slevel] [int] NULL,
 CONSTRAINT [PK_ProducLimitedPrice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductPurchasePrice]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPurchasePrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[StoreId] [int] NULL,
	[PurchasePrice] [decimal](18, 4) NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[SapIdentity] [nvarchar](10) NULL,
	[Status] [int] NULL CONSTRAINT [DF_ProductPurchasePrice_Status]  DEFAULT ((1)),
	[SapSalePrice] [decimal](18, 4) NULL,
	[SapPriceUnit] [int] NULL,
	[ProductCode] [nvarchar](20) NULL,
	[SupplierCode] [nvarchar](20) NULL,
	[SupplierId] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductSupplier]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSupplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[ProductCode] [nvarchar](50) NULL,
	[SupplierId] [int] NULL,
	[SupplierCode] [nvarchar](50) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_ProductSupplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseGroup]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](20) NULL,
 CONSTRAINT [PK_PURCHASEGROUP] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[StoreId] [int] NULL,
	[BillType] [int] NULL,
	[SupplierCode] [nvarchar](20) NULL,
	[SupplierId] [int] NULL,
	[Status] [int] NULL,
	[CompanyCode] [nvarchar](20) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[SapCode] [nvarchar](20) NULL,
	[PurchaseGroupId] [int] NULL,
	[PurchaseTime] [date] NULL,
	[Amount] [decimal](14, 4) NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[Remark] [nvarchar](1024) NULL,
	[OrderType] [int] NULL,
	[StoreCode] [nvarchar](20) NULL,
	[IsPushSap] [bit] NULL CONSTRAINT [DF__PurchaseO__IsPus__60E75331]  DEFAULT ((0)),
 CONSTRAINT [PK_PURCHASEORDER] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrderItem]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderId] [int] NULL,
	[ProductId] [int] NULL,
	[Quantity] [int] NULL,
	[CostPrice] [decimal](14, 4) NULL,
	[ActualQuantity] [int] NULL,
	[SapRow] [nvarchar](10) NULL,
	[Unit] [nvarchar](10) NULL,
	[SNCodes] [nvarchar](1024) NULL,
	[BatchNo] [bigint] NULL,
	[SNQuantity] [int] NULL,
	[IsSnCode] [bit] NULL,
	[ProductCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_PURCHASEORDERITEM] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RechargeVoucherHistory]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RechargeVoucherHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VoucherType] [int] NULL,
	[StoreId] [int] NULL,
	[BillCode] [nvarchar](20) NULL,
	[BillType] [int] NULL,
	[ProductId] [int] NULL,
	[ProductCode] [nvarchar](50) NULL,
	[SNCode] [varchar](50) NULL,
	[VoucherId] [int] NULL,
	[VoucherCode] [varchar](20) NULL,
	[Quantity] [int] NULL,
	[BalanceBeforeChange] [decimal](20, 2) NULL,
	[ChangeAmount] [decimal](20, 2) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_RechargeVoucherHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Role]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NULL,
	[Description] [nvarchar](1024) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleMenu]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleMenu](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NULL,
	[MenuId] [int] NULL,
 CONSTRAINT [PK_ROLEMENU] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SaleOrder]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SaleOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[StoreId] [int] NULL,
	[PosId] [int] NULL,
	[OrderType] [int] NULL,
	[RefundAccount] [varchar](60) NULL,
	[Status] [int] NULL,
	[OrderAmount] [decimal](12, 2) NULL,
	[PayAmount] [decimal](12, 2) NULL,
	[OnlinePayAmount] [decimal](12, 2) NULL,
	[PaymentWay] [int] NULL,
	[PaidDate] [datetime] NULL,
	[Hour] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[WorkScheduleCode] [varchar](32) NULL,
	[OrderLevel] [int] NULL,
	[AreaId] [char](6) NULL,
	[Address] [nvarchar](128) NULL,
	[Phone] [nvarchar](32) NULL,
	[Buyer] [nvarchar](16) NULL,
	[Remark] [nvarchar](500) NULL,
	[SourceSaleOrderCode] [nvarchar](20) NULL,
	[RoStatus] [int] NULL,
	[ParentCode] [nvarchar](20) NULL,
	[SapCode] [nvarchar](20) NULL,
	[AuditedBy] [int] NULL,
	[AuditedOn] [datetime] NULL,
	[AuditedRemark] [nvarchar](500) NULL,
	[StoreCode] [nvarchar](20) NULL,
	[BillType] [int] NULL,
	[SourceSapCode] [nvarchar](20) NULL,
	[IsPushSap] [bit] NULL CONSTRAINT [DF__SaleOrder__IsPus__61DB776A]  DEFAULT ((0)),
	[StoreIdGift] [int] NULL,
	[StoreCodeGift] [varchar](20) NULL,
	[CustomerCode] [varchar](20) NULL,
 CONSTRAINT [PK__SaleOrde__3214EC0654F2FC19] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SaleOrderItem]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SaleOrderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SaleOrderId] [int] NULL,
	[ProductId] [int] NULL,
	[ProductCode] [nvarchar](20) NULL,
	[ProductName] [nvarchar](50) NULL,
	[AvgCostPrice] [decimal](12, 2) NULL,
	[SalePrice] [decimal](12, 2) NULL,
	[RealPrice] [decimal](12, 2) NULL,
	[MinSalePrice] [decimal](12, 2) NULL,
	[Quantity] [int] NULL,
	[ActualQuantity] [int] NULL,
	[SupplierId] [int] NULL,
	[ParentProductId] [int] NULL CONSTRAINT [DF_SaleOrderItem_ParentId]  DEFAULT ((0)),
	[GiftType] [int] NULL,
	[SNCode] [nvarchar](max) NULL,
	[FJCode] [varchar](80) NULL,
	[SapRow] [varchar](80) NULL,
	[Unit] [nvarchar](20) NULL,
	[SourceSaleOrderRow] [int] NULL,
	[SourceSapRow] [nvarchar](20) NULL,
	[SaleClerkOne] [nvarchar](20) NULL,
	[SaleClerkTwo] [nvarchar](20) NULL,
	[CategoryCardNumber] [varchar](50) NULL,
	[CategoryPreferential] [decimal](12, 2) NULL,
	[BrandPreferential] [decimal](12, 2) NULL,
 CONSTRAINT [PK__SaleOrde__3214EC06B8F1E76A] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Stocktaking]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Stocktaking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StocktakingPlanId] [int] NULL,
	[Code] [nvarchar](20) NULL,
	[StocktakingType] [int] NULL,
	[ShelfCode] [nvarchar](20) NULL,
	[CreatedBy] [int] NULL,
	[CreatedByName] [nvarchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[Status] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedByName] [varchar](50) NULL,
	[StoreId] [int] NULL,
	[Note] [nvarchar](1000) NULL,
 CONSTRAINT [PK_STOCKTAKING] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StocktakingItem]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StocktakingItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StocktakingId] [int] NULL,
	[ProductId] [nvarchar](50) NULL,
	[CostPrice] [decimal](8, 4) NULL,
	[SalePrice] [decimal](8, 2) NULL,
	[Quantity] [int] NULL,
	[FirstQuantity] [int] NULL,
	[ComplexQuantity] [int] NULL,
	[CorectReason] [nvarchar](500) NULL,
	[Note] [nvarchar](500) NULL,
	[CheckQuantity] [int] NULL,
 CONSTRAINT [PK_STOCKTAKINGITEM] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StocktakingPlan]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StocktakingPlan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[CreatedBy] [int] NULL,
	[CreatedByName] [nvarchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedByName] [nvarchar](50) NULL,
	[UpdatedOn] [datetime] NULL,
	[Method] [int] NULL,
	[Status] [int] NULL,
	[StoreId] [int] NULL,
	[Note] [nvarchar](1000) NULL,
	[StocktakingDate] [datetime] NULL,
	[ComplexDate] [datetime] NULL,
	[StoreCode] [nvarchar](20) NULL,
	[IsPushSap] [bit] NULL,
 CONSTRAINT [PK_STOCKTAKINGPLAN] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StocktakingPlanItem]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StocktakingPlanItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StocktakingPlanId] [int] NULL,
	[ProductId] [int] NULL,
	[IsSNProduct] [bit] NULL,
	[CostPrice] [decimal](14, 4) NULL,
	[SalePrice] [decimal](12, 2) NULL,
	[Quantity] [int] NULL,
	[FirstQuantity] [int] NULL,
	[ComplexQuantity] [int] NULL,
	[CheckQuantity] [int] NULL,
	[SurplusSNCode] [varchar](max) NULL,
	[MissingSNCode] [varchar](max) NULL,
	[ProductCode] [nvarchar](20) NULL,
	[Unit] [nvarchar](20) NULL,
 CONSTRAINT [PK_STOCKTAKINGPLANITEM] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Store]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Store](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Number] [int] NULL,
	[Name] [nvarchar](200) NULL,
	[AreaId] [char](6) NULL CONSTRAINT [DF__Store__AreaId__642DD430]  DEFAULT ('500000'),
	[Address] [nvarchar](500) NULL,
	[Phone] [nvarchar](50) NULL,
	[SourceKey] [nvarchar](32) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[Contact] [nvarchar](32) NULL,
	[Province] [nvarchar](64) NULL,
	[City] [nvarchar](64) NULL,
	[District] [nvarchar](64) NULL,
	[Type] [int] NULL DEFAULT ((1)),
	[CommercialActivities] [nvarchar](50) NULL,
 CONSTRAINT [PK_Store_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StoreAccountMapping]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreAccountMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NULL,
	[AccountId] [int] NULL,
 CONSTRAINT [PK_StoreAccountMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreCustomerMap]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreCustomerMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerCode] [nvarchar](32) NOT NULL,
	[StoreCode] [nvarchar](32) NOT NULL,
	[Description] [nvarchar](50) NULL,
	[StoreId] [int] NULL,
	[SyncPrice] [bit] NULL DEFAULT ((0)),
 CONSTRAINT [PK__StoreCus__3214EC07862B1E7D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreInventory]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreInventory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NULL,
	[ProductId] [int] NULL,
	[SaleQuantity] [int] NULL,
	[OrderQuantity] [int] NULL,
	[Quantity] [int] NULL,
	[AvgCostPrice] [decimal](14, 4) NULL,
	[WarnQuantity] [int] NULL,
	[IsQuit] [bit] NULL,
	[LastCostPrice] [decimal](14, 4) NULL,
	[StoreSalePrice] [decimal](12, 2) NULL,
	[Status] [int] NULL,
	[LockedQuantity] [int] NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_STOREINVENTORY] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreInventoryBatch]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreInventoryBatch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[StoreId] [int] NULL,
	[SupplierId] [int] NULL,
	[Quantity] [int] NULL,
	[ProductionDate] [datetime] NULL,
	[ShelfLife] [int] NULL,
	[ContractPrice] [decimal](14, 4) NULL,
	[Price] [decimal](14, 4) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[BatchNo] [bigint] NULL,
	[PurchaseQuantity] [int] NULL,
	[RefundableQuantity] [int] NULL,
	[LockedQuantity] [int] NULL,
	[RowVersion] [timestamp] NOT NULL,
	[SNCode] [nvarchar](80) NULL CONSTRAINT [DF_StoreInventoryBatch_SNCode]  DEFAULT (''),
 CONSTRAINT [PK_STOREINVENTORYBATCH] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreInventoryHistory]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StoreInventoryHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NULL,
	[StoreId] [int] NULL,
	[Quantity] [int] NULL,
	[ChangeQuantity] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[BillId] [int] NULL,
	[BillCode] [varchar](20) NULL,
	[BillType] [int] NULL,
	[BatchNo] [bigint] NULL,
	[Price] [decimal](14, 4) NULL,
	[SupplierId] [int] NULL,
	[RealPrice] [decimal](12, 2) NULL,
	[CategoryPreferential] [decimal](12, 2) NULL,
	[BrandPreferential] [decimal](12, 2) NULL,
	[SNCode] [nvarchar](80) NULL,
	[StoreInventoryBatchId] [int] NULL,
 CONSTRAINT [PK_STOREINVENTORYHISTORY] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StoreInventoryHistorySAP]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreInventoryHistorySAP](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](64) NOT NULL,
	[Type] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[ProductCode] [nvarchar](20) NOT NULL,
	[StoreId] [int] NOT NULL,
	[StoreCode] [nvarchar](20) NOT NULL,
	[Quantity] [int] NOT NULL,
	[SNCodes] [nvarchar](max) NULL,
	[BillCode] [nvarchar](20) NOT NULL,
	[BillType] [int] NOT NULL,
	[SAPCode] [nvarchar](20) NULL,
	[SAPRow] [nvarchar](10) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[BillItemId] [int] NULL,
	[Unit] [nvarchar](20) NULL,
	[BillSapCode] [nvarchar](20) NULL,
	[BillSapRow] [nvarchar](20) NULL,
 CONSTRAINT [PK_StoreInventoryHistorySAP] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](100) NULL,
	[Type] [int] NULL,
	[ShortName] [nvarchar](50) NULL,
	[Contact] [nvarchar](300) NULL,
	[Phone] [nvarchar](300) NULL,
	[QQ] [nvarchar](300) NULL,
	[Address] [nvarchar](100) NULL,
	[Bank] [nvarchar](50) NULL,
	[BankAccount] [nvarchar](50) NULL,
	[BankAccountName] [varchar](50) NULL,
	[TaxNo] [nvarchar](50) NULL,
	[LicenseNo] [nvarchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
 CONSTRAINT [PK_SUPPLIER] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SystemSetup]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemSetup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Value] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
 CONSTRAINT [PK_SystemSetup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_SystemSetup] UNIQUE NONCLUSTERED 
(
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[temp_customer_replace]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[temp_customer_replace](
	[OldCustomerCode] [nvarchar](30) NULL,
	[OldCustomerName] [nvarchar](50) NULL,
	[NewCustomerCode] [nvarchar](30) NULL,
	[NewCustomerName] [nvarchar](50) NULL,
	[CommercialActivities] [nvarchar](50) NULL,
	[StoreCode1] [nvarchar](30) NULL,
	[StoreCode2] [nvarchar](30) NULL,
	[StoreCode3] [nvarchar](30) NULL,
	[StoreCode4] [nvarchar](30) NULL,
	[StoreCode5] [nvarchar](30) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[temp_customer_store]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[temp_customer_store](
	[客户编码] [float] NULL,
	[客户名称] [nvarchar](255) NULL,
	[F3] [nvarchar](255) NULL,
	[F4] [nvarchar](255) NULL,
	[F5] [nvarchar](255) NULL,
	[F6] [nvarchar](255) NULL,
	[F7] [nvarchar](255) NULL,
	[仓位1] [nvarchar](255) NULL,
	[仓位2] [nvarchar](255) NULL,
	[仓位3] [nvarchar](255) NULL,
	[仓位4] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[temp_customer_store_map]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[temp_customer_store_map](
	[客户编码] [float] NULL,
	[客户名称] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[temp_customer_store_map1]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[temp_customer_store_map1](
	[客户编码] [float] NULL,
	[客户名称] [nvarchar](255) NULL,
	[F3] [nvarchar](255) NOT NULL,
	[F4] [nvarchar](255) NOT NULL,
	[F5] [nvarchar](255) NOT NULL,
	[F6] [nvarchar](255) NOT NULL,
	[F7] [nvarchar](255) NOT NULL,
	[仓位1] [nvarchar](255) NULL,
	[仓位2] [nvarchar](255) NULL,
	[仓位3] [nvarchar](255) NULL,
	[仓位4] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TransferOrder]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransferOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NULL,
	[SapCode] [varchar](20) NULL,
	[Type] [int] NULL,
	[FromStoreId] [int] NULL,
	[FromStoreName] [char](50) NULL,
	[ToStoreName] [char](50) NULL,
	[ToStoreId] [int] NULL,
	[BatchNo] [bigint] NULL,
	[Status] [int] NULL,
	[Remark] [nvarchar](200) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[CreatedByName] [varchar](30) NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedByName] [varchar](30) NULL,
	[AuditOn] [datetime] NULL,
	[AuditBy] [int] NULL,
	[AuditByName] [varchar](30) NULL,
	[AuditRemark] [nvarchar](50) NULL,
	[FromStoreCode] [nvarchar](20) NULL,
	[ToStoreCode] [nvarchar](20) NULL,
	[IsPushSap] [bit] NULL CONSTRAINT [DF_TransferOrder_IsPushSap]  DEFAULT ((0)),
 CONSTRAINT [PK_TRANSFERORDER] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransferOrderItem]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransferOrderItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransferOrderId] [int] NULL,
	[ProductId] [int] NULL,
	[Quantity] [int] NULL CONSTRAINT [DF_TransferOrderItem2_Quantity]  DEFAULT ((0)),
	[ActualShipmentQuantity] [int] NULL CONSTRAINT [DF_TransferOrderItem_ActualShipmentQuantity_1]  DEFAULT ((0)),
	[ActualReceivedQuantity] [int] NULL CONSTRAINT [DF_TransferOrderItem_ActualReceivedQuantity_1]  DEFAULT ((0)),
	[Price] [decimal](14, 4) NULL,
	[ProductCode] [nvarchar](20) NULL,
	[Unit] [nvarchar](20) NULL,
	[SapRow] [nvarchar](20) NULL,
	[SNCodes] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransferOrderItem2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[GetCategoriesTree]    Script Date: 2019/1/29 9:58:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*************************************
* 获取品类树。
**************************************/
/* CREATE FUNCTION [dbo].[GetCategoriesTree]
(
	@CategoryId INT
)
RETURNS TABLE
AS
RETURN 
(
	WITH 
		parents AS
		(
			SELECT Id,Code,name,ParentCode,Level FROM MT_Category WHERE Id=@CategoryId
			UNION ALL
			SELECT c.Id,c.Code,c.name,c.ParentCode,c.Level FROM MT_Category AS c,parents p WHERE c.Code=p.ParentCode
		),
		childs AS
		(
			SELECT Id,Code,name,ParentCode,Level FROM MT_Category WHERE Id=@CategoryId
			UNION ALL
			SELECT c.Id,c.Code,c.name,c.ParentCode,c.Level FROM MT_Category AS c,childs t WHERE t.Code=c.ParentCode
		)
		SELECT * FROM parents
		UNION
		SELECT * FROM childs
) */

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_account_username]    Script Date: 2019/1/29 9:58:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_account_username] ON [dbo].[Account]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IDX_Category_Code]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [IDX_Category_Code] ON [dbo].[Category]
(
	[Code] ASC,
	[ParentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [<Name of Missing Index, sysname,>]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>] ON [dbo].[Product]
(
	[Id] ASC
)
INCLUDE ( 	[Name],
	[Spec],
	[CategoryId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IDX_SaleOrder_1]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [IDX_SaleOrder_1] ON [dbo].[SaleOrder]
(
	[PaidDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_saleorder_code]    Script Date: 2019/1/29 9:58:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_saleorder_code] ON [dbo].[SaleOrder]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_saleorder_StoreIdAndupdatedOn]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [idx_saleorder_StoreIdAndupdatedOn] ON [dbo].[SaleOrder]
(
	[StoreId] ASC,
	[Status] ASC,
	[UpdatedOn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_saleorder_updatedOn]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [idx_saleorder_updatedOn] ON [dbo].[SaleOrder]
(
	[UpdatedOn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_saleorderitem_productid]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [idx_saleorderitem_productid] ON [dbo].[SaleOrderItem]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [idx_saleorderitem_saleorderid]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [idx_saleorderitem_saleorderid] ON [dbo].[SaleOrderItem]
(
	[SaleOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IDX_StoreInventoryHistory_1]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [IDX_StoreInventoryHistory_1] ON [dbo].[StoreInventoryHistory]
(
	[ProductId] ASC,
	[BillCode] ASC
)
INCLUDE ( 	[BillType],
	[Price],
	[SupplierId],
	[SNCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IDX_StoreInventoryHistory_2]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [IDX_StoreInventoryHistory_2] ON [dbo].[StoreInventoryHistory]
(
	[StoreId] ASC,
	[BillId] ASC,
	[BillCode] ASC
)
INCLUDE ( 	[ProductId],
	[Price],
	[SNCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IDX_StoreInventoryHistory_3]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [IDX_StoreInventoryHistory_3] ON [dbo].[StoreInventoryHistory]
(
	[BillCode] ASC
)
INCLUDE ( 	[ProductId],
	[Price],
	[SNCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [idx_transaferOrder_code]    Script Date: 2019/1/29 9:58:42 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_transaferOrder_code] ON [dbo].[TransferOrder]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [<Name of Missing Index, sysname,>]    Script Date: 2019/1/29 9:58:42 ******/
CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>] ON [dbo].[TransferOrderItem]
(
	[TransferOrderId] ASC
)
INCLUDE ( 	[ProductId],
	[Quantity],
	[ActualShipmentQuantity],
	[ActualReceivedQuantity],
	[ProductCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'账户名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'Password'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'昵称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'NickName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'RoleId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登录错误次数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'LoginErrorCount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account', @level2type=N'COLUMN',@level2name=N'LastUpdateDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'后台管理账户表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Account'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AccountLoginHistory', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'账号id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AccountLoginHistory', @level2type=N'COLUMN',@level2name=N'AccountId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登录账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AccountLoginHistory', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登录时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AccountLoginHistory', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'IP地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AccountLoginHistory', @level2type=N'COLUMN',@level2name=N'IPAddress'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'登录状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AccountLoginHistory', @level2type=N'COLUMN',@level2name=N'LoginStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'账号登录历史' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AccountLoginHistory'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Area', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区域名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Area', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Area', @level2type=N'COLUMN',@level2name=N'ShowName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区域全民' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Area', @level2type=N'COLUMN',@level2name=N'FullName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'层级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Area', @level2type=N'COLUMN',@level2name=N'Level'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区域表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Area'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'充值券编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'StoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'StoreCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'StoreName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品牌' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'BrandId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Amount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'已用金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Reduced'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'余额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Balance'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单品限额%' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Limit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'充值券状态（0=待审、1=可用、2=驳回、3=中止）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BrandRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryCard', @level2type=N'COLUMN',@level2name=N'StoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类卡号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryCard', @level2type=N'COLUMN',@level2name=N'CardNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'充值券编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'StoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'StoreCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'StoreName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类卡号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'CardNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'总金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Amount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'已用金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Reduced'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'余额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Balance'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单品限额%' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Limit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'充值券状态（0=待审、1=可用、2=驳回、3=中止）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类充值券' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryRechargeVoucher'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型（1=品类充值券、2=品牌充值券）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ExceptProduct', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类充值券ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ExceptProduct', @level2type=N'COLUMN',@level2name=N'CategoryRechargeVoucherId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品牌充值券ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ExceptProduct', @level2type=N'COLUMN',@level2name=N'BrandRechargeVoucherId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ExceptProduct', @level2type=N'COLUMN',@level2name=N'ProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ExceptProduct', @level2type=N'COLUMN',@level2name=N'ProductCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ExceptProduct', @level2type=N'COLUMN',@level2name=N'ProductName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'充值券不包括的商品' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ExceptProduct'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu', @level2type=N'COLUMN',@level2name=N'ParentId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'连接' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu', @level2type=N'COLUMN',@level2name=N'Url'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu', @level2type=N'COLUMN',@level2name=N'Icon'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示顺序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu', @level2type=N'COLUMN',@level2name=N'DisplayOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'连接类型' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu', @level2type=N'COLUMN',@level2name=N'UrlType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统菜单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Menu'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'类型（1=品类充值券、2=品牌充值券）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParticipantCategory', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类充值券Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParticipantCategory', @level2type=N'COLUMN',@level2name=N'CategoryRechargeVoucherId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品牌充值券ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParticipantCategory', @level2type=N'COLUMN',@level2name=N'BrandRechargeVoucherId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类充值券参与的品类' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ParticipantCategory'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建议零售价或限价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProductPrice', @level2type=N'COLUMN',@level2name=N'SalePrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'价格类型（1=建议零售价、2=限价）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProductPrice', @level2type=N'COLUMN',@level2name=N'PriceType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'(1=有效、0=无效)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ProductSupplier', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据类型（1=门店采购单、2=总仓采购单）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PurchaseOrder', @level2type=N'COLUMN',@level2name=N'BillType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据状态（-1=作废、1=初始、2=驳回、3=已审、4=已出库、5=已入库、9=已关闭、10=已完成）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PurchaseOrder', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'订单类型（1=采购单、2=采购退单、3=采购换机单）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PurchaseOrder', @level2type=N'COLUMN',@level2name=N'OrderType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'充值券类型（1=品类充值券、2=品牌充值券）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RechargeVoucherHistory', @level2type=N'COLUMN',@level2name=N'VoucherType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RechargeVoucherHistory', @level2type=N'COLUMN',@level2name=N'StoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RechargeVoucherHistory', @level2type=N'COLUMN',@level2name=N'BillCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据类型（11=零售、15=预售）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RechargeVoucherHistory', @level2type=N'COLUMN',@level2name=N'BillType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品数量（必须大于0）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RechargeVoucherHistory', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'扣减前剩余总额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RechargeVoucherHistory', @level2type=N'COLUMN',@level2name=N'BalanceBeforeChange'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'本次增加或扣减的总额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RechargeVoucherHistory', @level2type=N'COLUMN',@level2name=N'ChangeAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Role', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Role', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Role', @level2type=N'COLUMN',@level2name=N'Description'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'账户角色表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Role'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RoleMenu', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RoleMenu', @level2type=N'COLUMN',@level2name=N'RoleId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'菜单编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RoleMenu', @level2type=N'COLUMN',@level2name=N'MenuId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色菜单对应表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RoleMenu'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'StoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pos机Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'PosId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'订单类型（1=订单、2=退单）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'OrderType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'退款账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'RefundAccount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'订单状态（-2=已退货、-1=作废、2=待审核、3=已审核、4=待出库、5=已出库、6=预售转正）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'订单金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'OrderAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'现金支付金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'PayAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'在线支付金额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'OnlinePayAmount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'支付方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'PaymentWay'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'支付时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'PaidDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'时段' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'Hour'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'UpdatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'UpdatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'班次代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'WorkScheduleCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'订单级别：1 普通订单，2 Vip订单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'OrderLevel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收货区域ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'AreaId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收货地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'Address'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'收货电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'Phone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'购买人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'Buyer'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'源订单编码(用于退货)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'SourceSaleOrderCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'退单状态（-1=作废、1=初始、2=待审核、3=已审核、4=待入库、5=已入库）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'RoStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据类型（1=零售单、2=批发单、3=预售单、4=换机单）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder', @level2type=N'COLUMN',@level2name=N'BillType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售订单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'SaleOrderId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'ProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'ProductCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'ProductName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'平均成本价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'AvgCostPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'SalePrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实际售价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'RealPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'限价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'MinSalePrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实发数量（批发单）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'ActualQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售明细唯一码，用于赠品与明细关联。' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'ParentProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SMT原单行号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'SourceSaleOrderRow'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类卡号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'CategoryCardNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品类优惠总额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'CategoryPreferential'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'品牌优惠总额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SaleOrderItem', @level2type=N'COLUMN',@level2name=N'BrandPreferential'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点计划编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'StocktakingPlanId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点表类型1 盘点空表，2 盘点修正表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'StocktakingType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'货架码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'ShelfCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'CreatedByName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态（待审，已审）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'UpdatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'UpdatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'UpdatedByName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'StoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Stocktaking'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'ProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨成本价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'CostPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'SalePrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点锁定库存数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'初盘数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'FirstQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'复盘数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'ComplexQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修正原因' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'CorectReason'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem', @level2type=N'COLUMN',@level2name=N'CheckQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点明细' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点代码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'CreatedByName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'UpdatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新人名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'UpdatedByName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'UpdatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点方式（大盘：小盘）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'Method'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点状态（待盘，初盘，复盘，终盘）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'StoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'Note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan', @level2type=N'COLUMN',@level2name=N'StocktakingDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点计划' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlan'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点计划编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'StocktakingPlanId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系统编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'ProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨成本价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'CostPrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'销售价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'SalePrice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'库存数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'初盘点数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'FirstQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'复盘点数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'ComplexQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘盈串码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'SurplusSNCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘亏串码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem', @level2type=N'COLUMN',@level2name=N'MissingSNCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'盘点计划明细' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StocktakingPlanItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Store', @level2type=N'COLUMN',@level2name=N'Number'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'门店唯一码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Store', @level2type=N'COLUMN',@level2name=N'SourceKey'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Store', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Store', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Store', @level2type=N'COLUMN',@level2name=N'Contact'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更前的总库存' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistory', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'变更数量（正=入、负=出）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistory', @level2type=N'COLUMN',@level2name=N'ChangeQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'11=销售订单、12=销售退单、13=批发订单、14=批发退单、15=预售订单、16=预售退单、17=换机单、20=总仓采购订单、21=门店采购订单、23=仓库采购退单、24=门店采购退单、25=采购换单、60=调拨单、61=残损单、53=门店盘点、54=盘点单、55=盘点修正单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistory', @level2type=N'COLUMN',@level2name=N'BillType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前批次的品类优惠总额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistory', @level2type=N'COLUMN',@level2name=N'CategoryPreferential'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'当前批次的品牌优惠总额' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistory', @level2type=N'COLUMN',@level2name=N'BrandPreferential'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SMT出入库单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'单据类型（1=出库、2=入库）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SMT原单号（销售单号、调拨单号、采购单号...）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'BillCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'原单据类型，用于区分零售、调拨、采购等' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'BillType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SAP出入库单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'SAPCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SAP出入库行号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'SAPRow'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SMT原单关联的SAP单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'BillSapCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SMT原单关联的SAP行号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StoreInventoryHistorySAP', @level2type=N'COLUMN',@level2name=N'BillSapRow'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商类别' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'简称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'ShortName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'Contact'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'联系电话' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'Phone'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开户行' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'Bank'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开户行账号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'BankAccount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'开户名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'BankAccountName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'税号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'TaxNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'执照号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'LicenseNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'UpdatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier', @level2type=N'COLUMN',@level2name=N'UpdatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'供应商' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Supplier'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'temp_customer_replace', @level2type=N'COLUMN',@level2name=N'CommercialActivities'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'良品仓' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'temp_customer_replace', @level2type=N'COLUMN',@level2name=N'StoreCode1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'微商城仓' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'temp_customer_replace', @level2type=N'COLUMN',@level2name=N'StoreCode5'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨单号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'Code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨单类型（1=总仓分配、2=门店申请、3=门店调拨）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'从门店' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'FromStoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'从门店名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'FromStoreName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到门店名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'ToStoreName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到门店' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'ToStoreId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'根据调拨单生成的批次号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'BatchNo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'状态（-2=驳回、-1=作废、0=初始、1=待审、2=已审、3=待发货、4=已发货、5=待收货、6=已收货、7=已完成）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'Status'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'Remark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建人名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'CreatedByName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'UpdatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'UpdatedBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'UpdatedByName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审核时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'AuditOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审核人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'AuditBy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审核人姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'AuditByName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'审核意见' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'AuditRemark'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨单当前状态是否推送给了SAP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder', @level2type=N'COLUMN',@level2name=N'IsPushSap'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'编号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨单ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'TransferOrderId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SKU编码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'ProductId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'数量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'Quantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实发数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'ActualShipmentQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'实收数' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'ActualReceivedQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'成本价' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'Price'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'商品串码，逗号分隔' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem', @level2type=N'COLUMN',@level2name=N'SNCodes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'调拨项目' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TransferOrderItem'
GO
USE [master]
GO
ALTER DATABASE [BigMall] SET  READ_WRITE 
GO
