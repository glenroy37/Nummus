CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory`
(
    `MigrationId`    varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;

CREATE TABLE `AspNetRoles`
(
    `Id`               varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Name`             varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedName`   varchar(256) CHARACTER SET utf8mb4 NULL,
    `ConcurrencyStamp` longtext CHARACTER SET utf8mb4     NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetUsers`
(
    `Id`                   varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `UserName`             varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedUserName`   varchar(256) CHARACTER SET utf8mb4 NULL,
    `Email`                varchar(256) CHARACTER SET utf8mb4 NULL,
    `NormalizedEmail`      varchar(256) CHARACTER SET utf8mb4 NULL,
    `EmailConfirmed`       tinyint(1)                         NOT NULL,
    `PasswordHash`         longtext CHARACTER SET utf8mb4     NULL,
    `SecurityStamp`        longtext CHARACTER SET utf8mb4     NULL,
    `ConcurrencyStamp`     longtext CHARACTER SET utf8mb4     NULL,
    `PhoneNumber`          longtext CHARACTER SET utf8mb4     NULL,
    `PhoneNumberConfirmed` tinyint(1)                         NOT NULL,
    `TwoFactorEnabled`     tinyint(1)                         NOT NULL,
    `LockoutEnd`           datetime(6)                        NULL,
    `LockoutEnabled`       tinyint(1)                         NOT NULL,
    `AccessFailedCount`    int                                NOT NULL,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
);

CREATE TABLE `Categories`
(
    `Id`           int                            NOT NULL AUTO_INCREMENT,
    `Description`  longtext CHARACTER SET utf8mb4 NOT NULL,
    `CategoryType` int                            NOT NULL,
    CONSTRAINT `PK_Categories` PRIMARY KEY (`Id`)
);

CREATE TABLE `NummusUsers`
(
    `Id`        int                            NOT NULL AUTO_INCREMENT,
    `UserEmail` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_NummusUsers` PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetRoleClaims`
(
    `Id`         int                                NOT NULL AUTO_INCREMENT,
    `RoleId`     varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType`  longtext CHARACTER SET utf8mb4     NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4     NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserClaims`
(
    `Id`         int                                NOT NULL AUTO_INCREMENT,
    `UserId`     varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClaimType`  longtext CHARACTER SET utf8mb4     NULL,
    `ClaimValue` longtext CHARACTER SET utf8mb4     NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserLogins`
(
    `LoginProvider`       varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderKey`         varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `ProviderDisplayName` longtext CHARACTER SET utf8mb4     NULL,
    `UserId`              varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserRoles`
(
    `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserTokens`
(
    `UserId`        varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Name`          varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Value`         longtext CHARACTER SET utf8mb4     NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Accounts`
(
    `Id`           int                            NOT NULL AUTO_INCREMENT,
    `Name`         longtext CHARACTER SET utf8mb4 NOT NULL,
    `NummusUserId` int                            NOT NULL,
    CONSTRAINT `PK_Accounts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Accounts_NummusUsers_NummusUserId` FOREIGN KEY (`NummusUserId`) REFERENCES `NummusUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AccountStatements`
(
    `Id`              int           NOT NULL AUTO_INCREMENT,
    `BookingDate`     datetime(6)   NOT NULL,
    `ClosingSum`      decimal(9, 2) NOT NULL,
    `AccountId`       int           NOT NULL,
    `lastStatementId` int           NULL,
    CONSTRAINT `PK_AccountStatements` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AccountStatements_Accounts_AccountId` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AccountStatements_AccountStatements_lastStatementId` FOREIGN KEY (`lastStatementId`) REFERENCES `AccountStatements` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BookingLines`
(
    `Id`                   int                            NOT NULL AUTO_INCREMENT,
    `BookingText`          longtext CHARACTER SET utf8mb4 NOT NULL,
    `Amount`               decimal(9, 2)                  NOT NULL,
    `BookingTime`          datetime(6)                    NOT NULL,
    `AccountId`            int                            NULL,
    `CategoryId`           int                            NULL,
    `AccountStatementId`   int                            NULL,
    `RelatedBookingLineId` int                            NULL,
    CONSTRAINT `PK_BookingLines` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BookingLines_Accounts_AccountId` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BookingLines_AccountStatements_AccountStatementId` FOREIGN KEY (`AccountStatementId`) REFERENCES `AccountStatements` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BookingLines_BookingLines_RelatedBookingLineId` FOREIGN KEY (`RelatedBookingLineId`) REFERENCES `BookingLines` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BookingLines_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_Accounts_NummusUserId` ON `Accounts` (`NummusUserId`);

CREATE INDEX `IX_AccountStatements_AccountId` ON `AccountStatements` (`AccountId`);

CREATE INDEX `IX_AccountStatements_lastStatementId` ON `AccountStatements` (`lastStatementId`);

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

CREATE INDEX `IX_BookingLines_AccountId` ON `BookingLines` (`AccountId`);

CREATE INDEX `IX_BookingLines_AccountStatementId` ON `BookingLines` (`AccountStatementId`);

CREATE INDEX `IX_BookingLines_CategoryId` ON `BookingLines` (`CategoryId`);

CREATE INDEX `IX_BookingLines_RelatedBookingLineId` ON `BookingLines` (`RelatedBookingLineId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201206221548_Init', '5.0.0');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201207220245_AllowUncategorisedBookingLines', '5.0.0');

COMMIT;

START TRANSACTION;

ALTER TABLE `BookingLines`
    DROP FOREIGN KEY `FK_BookingLines_Accounts_AccountId`;

ALTER TABLE `Categories`
    ADD `NummusUserId` int NULL;

ALTER TABLE `BookingLines`
    MODIFY COLUMN `AccountId` int NOT NULL DEFAULT 0;

CREATE INDEX `IX_Categories_NummusUserId` ON `Categories` (`NummusUserId`);

ALTER TABLE `BookingLines`
    ADD CONSTRAINT `FK_BookingLines_Accounts_AccountId` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Categories`
    ADD CONSTRAINT `FK_Categories_NummusUsers_NummusUserId` FOREIGN KEY (`NummusUserId`) REFERENCES `NummusUsers` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201224210519_AddCategoryUserRelation', '5.0.0');

COMMIT;