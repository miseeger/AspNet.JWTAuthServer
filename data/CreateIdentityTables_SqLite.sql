CREATE TABLE [Claim] (
    [Id] VARCHAR(64) NOT NULL PRIMARY KEY,
    [ClientId] VARCHAR(64) NULL,
    [ClaimType] VARCHAR(256) NOT NULL,
    [ClaimValue] VARCHAR(256) NOT NULL
);

CREATE TABLE [Client] (
    [Id] VARCHAR(64) NOT NULL PRIMARY KEY,
    [Name] VARCHAR(128) NOT NULL,
    [Base64Secret] VARCHAR(256) NOT NULL,
	[AllowedOrigin] VARCHAR(256) NOT NULL,
    [IsActive] INTEGER, 
);

CREATE TABLE [Role] (
    [Id] VARCHAR(64)  NOT NULL PRIMARY KEY,
    [ClientId] VARCHAR(64) NULL,
    [Name] VARCHAR(256)  NOT NULL,
);

CREATE TABLE [User] (
	[Id] VARCHAR(64)  PRIMARY KEY NULL,
	[ClientId] VARCHAR(64)  NULL,
	[Email] VARCHAR(256)  NULL,
	[EmailConfirmed] BOOL  NULL,
	[PasswordHash] VARCHAR(512)  NULL,
	[SecurityStamp] VARCHAR(512)  NULL,
	[PhoneNumber] VARCHAR(256)  NULL,
	[PhoneNumberConfirmed] BOOL  NULL,
	[TwoFactorEnabled] BOOL  NULL,
	[LockoutEndDateUtc] DATETIME  NULL,
	[LockoutEnabled] BOOL  NULL,
	[AccessFailedCount] INT DEFAULT '0' NULL,
	[UserName] VARCHAR(256)  NULL,
	[Firstname] VARCHAR(64)  NULL,
	[Lastname] VARCHAR(64)  NULL,
	[JoinDate] DATE  NULL,
	[Level] INTEGER DEFAULT '0' NULL
);

CREATE TABLE [UserClaim] (
    [UserId] VARCHAR(64) NOT NULL,
    [ClaimId] VARCHAR(64) NOT NULL,
    PRIMARY KEY ([UserId],[ClaimId])
);

CREATE TABLE [UserRole] (
    [UserId] VARCHAR(64) NOT NULL,
    [RoleId] VARCHAR(64) NOT NULL,
    PRIMARY KEY ([UserId], [RoleId])
);

CREATE TABLE [UserLogin] (
    [Id] VARCHAR(64) NOT NULL PRIMARY KEY,
    [UserId] VARCHAR(64)  NOT NULL,
    [ClientId] VARCHAR(64) NOT NULL,
    [LoginProvider] VARCHAR(128) NOT NULL,
    [ProviderKey] VARCHAR(128) NOT NULL
);

