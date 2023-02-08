
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/25/2023 21:37:43
-- Generated from EDMX file: E:\HK3\LT TQ\HotelManagementApp\HotelManagementApp\Model\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [QuanLyKhachSan2];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__Accounts__IDStaf__2EDAF651]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK__Accounts__IDStaf__2EDAF651];
GO
IF OBJECT_ID(N'[dbo].[FK__Orders__IDBillDe__2BFE89A6]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK__Orders__IDBillDe__2BFE89A6];
GO
IF OBJECT_ID(N'[dbo].[FK__RoomsRese__IDBil__625A9A57]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoomsReservations] DROP CONSTRAINT [FK__RoomsRese__IDBil__625A9A57];
GO
IF OBJECT_ID(N'[dbo].[FK_BillDetail_Customer_CustomerID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BillDetails] DROP CONSTRAINT [FK_BillDetail_Customer_CustomerID];
GO
IF OBJECT_ID(N'[dbo].[FK_BillDetail_Staff_StaffID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BillDetails] DROP CONSTRAINT [FK_BillDetail_Staff_StaffID];
GO
IF OBJECT_ID(N'[dbo].[FK__Orders__IDFoodsA__2B0A656D]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK__Orders__IDFoodsA__2B0A656D];
GO
IF OBJECT_ID(N'[dbo].[FK__RoomsRese__IDRoo__634EBE90]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RoomsReservations] DROP CONSTRAINT [FK__RoomsRese__IDRoo__634EBE90];
GO
IF OBJECT_ID(N'[dbo].[FK_Room_RoomType_RoomTypeID]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Rooms] DROP CONSTRAINT [FK_Room_RoomType_RoomTypeID];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Accounts];
GO
IF OBJECT_ID(N'[dbo].[BillDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BillDetails];
GO
IF OBJECT_ID(N'[dbo].[Customers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customers];
GO
IF OBJECT_ID(N'[dbo].[FoodsAndServices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FoodsAndServices];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO
IF OBJECT_ID(N'[dbo].[Rooms]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Rooms];
GO
IF OBJECT_ID(N'[dbo].[RoomsReservations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoomsReservations];
GO
IF OBJECT_ID(N'[dbo].[RoomTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoomTypes];
GO
IF OBJECT_ID(N'[dbo].[Staffs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Staffs];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Accounts'
CREATE TABLE [dbo].[Accounts] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [IDStaff] int  NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'BillDetails'
CREATE TABLE [dbo].[BillDetails] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [IDStaff] int  NOT NULL,
    [IDCustomer] int  NOT NULL,
    [TotalMoney] decimal(19,4)  NULL,
    [BillDate] datetime  NULL,
    [Status] nvarchar(max)  NULL,
    [Deleted] bit  NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Sex] nvarchar(max)  NOT NULL,
    [CCCD] nvarchar(max)  NOT NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NULL,
    [Nationality] nvarchar(max)  NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'FoodsAndServices'
CREATE TABLE [dbo].[FoodsAndServices] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Unit] nvarchar(max)  NULL,
    [Price] decimal(19,4)  NULL,
    [ImageData] nvarchar(max)  NULL,
    [Type] nvarchar(max)  NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [IDFoodsAndServices] int  NULL,
    [IDBillDetail] int  NULL,
    [Quantity] int  NULL,
    [TotalPrice] decimal(19,4)  NULL,
    [Time] datetime  NULL,
    [Deleted] bit  NULL
);
GO

-- Creating table 'Rooms'
CREATE TABLE [dbo].[Rooms] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [RoomNum] nvarchar(max)  NOT NULL,
    [IDRoomType] int  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [ImageData] nvarchar(max)  NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'RoomsReservations'
CREATE TABLE [dbo].[RoomsReservations] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [IDBillDetail] int  NULL,
    [IDRoom] int  NULL,
    [CheckInTime] datetime  NULL,
    [CheckOutTime] datetime  NULL,
    [Deleted] bit  NULL
);
GO

-- Creating table 'RoomTypes'
CREATE TABLE [dbo].[RoomTypes] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Price] decimal(19,4)  NULL,
    [Deleted] bit  NULL
);
GO

-- Creating table 'Staffs'
CREATE TABLE [dbo].[Staffs] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Sex] nvarchar(max)  NOT NULL,
    [CCCD] nvarchar(max)  NOT NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [Role] nvarchar(max)  NULL,
    [ImageData] nvarchar(max)  NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'Consts'
CREATE TABLE [dbo].[Consts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [PK_Accounts]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'BillDetails'
ALTER TABLE [dbo].[BillDetails]
ADD CONSTRAINT [PK_BillDetails]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'FoodsAndServices'
ALTER TABLE [dbo].[FoodsAndServices]
ADD CONSTRAINT [PK_FoodsAndServices]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Rooms'
ALTER TABLE [dbo].[Rooms]
ADD CONSTRAINT [PK_Rooms]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'RoomsReservations'
ALTER TABLE [dbo].[RoomsReservations]
ADD CONSTRAINT [PK_RoomsReservations]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'RoomTypes'
ALTER TABLE [dbo].[RoomTypes]
ADD CONSTRAINT [PK_RoomTypes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Staffs'
ALTER TABLE [dbo].[Staffs]
ADD CONSTRAINT [PK_Staffs]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'Consts'
ALTER TABLE [dbo].[Consts]
ADD CONSTRAINT [PK_Consts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [IDStaff] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [FK__Accounts__IDStaf__2EDAF651]
    FOREIGN KEY ([IDStaff])
    REFERENCES [dbo].[Staffs]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Accounts__IDStaf__2EDAF651'
CREATE INDEX [IX_FK__Accounts__IDStaf__2EDAF651]
ON [dbo].[Accounts]
    ([IDStaff]);
GO

-- Creating foreign key on [IDBillDetail] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK__Orders__IDBillDe__2BFE89A6]
    FOREIGN KEY ([IDBillDetail])
    REFERENCES [dbo].[BillDetails]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Orders__IDBillDe__2BFE89A6'
CREATE INDEX [IX_FK__Orders__IDBillDe__2BFE89A6]
ON [dbo].[Orders]
    ([IDBillDetail]);
GO

-- Creating foreign key on [IDBillDetail] in table 'RoomsReservations'
ALTER TABLE [dbo].[RoomsReservations]
ADD CONSTRAINT [FK__RoomsRese__IDBil__625A9A57]
    FOREIGN KEY ([IDBillDetail])
    REFERENCES [dbo].[BillDetails]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__RoomsRese__IDBil__625A9A57'
CREATE INDEX [IX_FK__RoomsRese__IDBil__625A9A57]
ON [dbo].[RoomsReservations]
    ([IDBillDetail]);
GO

-- Creating foreign key on [IDCustomer] in table 'BillDetails'
ALTER TABLE [dbo].[BillDetails]
ADD CONSTRAINT [FK_BillDetail_Customer_CustomerID]
    FOREIGN KEY ([IDCustomer])
    REFERENCES [dbo].[Customers]
        ([ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BillDetail_Customer_CustomerID'
CREATE INDEX [IX_FK_BillDetail_Customer_CustomerID]
ON [dbo].[BillDetails]
    ([IDCustomer]);
GO

-- Creating foreign key on [IDStaff] in table 'BillDetails'
ALTER TABLE [dbo].[BillDetails]
ADD CONSTRAINT [FK_BillDetail_Staff_StaffID]
    FOREIGN KEY ([IDStaff])
    REFERENCES [dbo].[Staffs]
        ([ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BillDetail_Staff_StaffID'
CREATE INDEX [IX_FK_BillDetail_Staff_StaffID]
ON [dbo].[BillDetails]
    ([IDStaff]);
GO

-- Creating foreign key on [IDFoodsAndServices] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK__Orders__IDFoodsA__2B0A656D]
    FOREIGN KEY ([IDFoodsAndServices])
    REFERENCES [dbo].[FoodsAndServices]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Orders__IDFoodsA__2B0A656D'
CREATE INDEX [IX_FK__Orders__IDFoodsA__2B0A656D]
ON [dbo].[Orders]
    ([IDFoodsAndServices]);
GO

-- Creating foreign key on [IDRoom] in table 'RoomsReservations'
ALTER TABLE [dbo].[RoomsReservations]
ADD CONSTRAINT [FK__RoomsRese__IDRoo__634EBE90]
    FOREIGN KEY ([IDRoom])
    REFERENCES [dbo].[Rooms]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__RoomsRese__IDRoo__634EBE90'
CREATE INDEX [IX_FK__RoomsRese__IDRoo__634EBE90]
ON [dbo].[RoomsReservations]
    ([IDRoom]);
GO

-- Creating foreign key on [IDRoomType] in table 'Rooms'
ALTER TABLE [dbo].[Rooms]
ADD CONSTRAINT [FK_Room_RoomType_RoomTypeID]
    FOREIGN KEY ([IDRoomType])
    REFERENCES [dbo].[RoomTypes]
        ([ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Room_RoomType_RoomTypeID'
CREATE INDEX [IX_FK_Room_RoomType_RoomTypeID]
ON [dbo].[Rooms]
    ([IDRoomType]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------