
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/10/2016 14:32:48
-- Generated from EDMX file: C:\Users\Sonny\Source\Repos\makeyourtournament\makeyourtournament\DAL\Database.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [aspnet-makeyourtournament-20161118020358];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserTournament_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTournament] DROP CONSTRAINT [FK_UserTournament_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTournament_Tournament]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTournament] DROP CONSTRAINT [FK_UserTournament_Tournament];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentModus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TournamentSet] DROP CONSTRAINT [FK_TournamentModus];
GO
IF OBJECT_ID(N'[dbo].[FK_MatchingRound]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MatchingSet] DROP CONSTRAINT [FK_MatchingRound];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTeam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamSet] DROP CONSTRAINT [FK_UserTeam];
GO
IF OBJECT_ID(N'[dbo].[FK_StatisticUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSet] DROP CONSTRAINT [FK_StatisticUser];
GO
IF OBJECT_ID(N'[dbo].[FK_StatisticTeam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamSet] DROP CONSTRAINT [FK_StatisticTeam];
GO
IF OBJECT_ID(N'[dbo].[FK_StatisticGroupCard]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupCardSet] DROP CONSTRAINT [FK_StatisticGroupCard];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamGroupCard]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupCardSet] DROP CONSTRAINT [FK_TeamGroupCard];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentGroupCard]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupCardSet] DROP CONSTRAINT [FK_TournamentGroupCard];
GO
IF OBJECT_ID(N'[dbo].[FK_TournamentMatching]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MatchingSet] DROP CONSTRAINT [FK_TournamentMatching];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUser_has_friends]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User_has_friendsSet] DROP CONSTRAINT [FK_UserUser_has_friends];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[StatisticSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StatisticSet];
GO
IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[TournamentSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TournamentSet];
GO
IF OBJECT_ID(N'[dbo].[TeamSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamSet];
GO
IF OBJECT_ID(N'[dbo].[GroupCardSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupCardSet];
GO
IF OBJECT_ID(N'[dbo].[ModusSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ModusSet];
GO
IF OBJECT_ID(N'[dbo].[MatchingSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MatchingSet];
GO
IF OBJECT_ID(N'[dbo].[RoundSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RoundSet];
GO
IF OBJECT_ID(N'[dbo].[User_has_friendsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_has_friendsSet];
GO
IF OBJECT_ID(N'[dbo].[UserTournament]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserTournament];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'StatisticSet'
CREATE TABLE [dbo].[StatisticSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [goals] int  NOT NULL,
    [owngoals] int  NOT NULL,
    [points] int  NOT NULL,
    [wins] int  NOT NULL,
    [loses] int  NOT NULL,
    [draws] int  NOT NULL,
    [totalGames] int  NOT NULL
);
GO

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [email] nvarchar(max)  NOT NULL,
    [friend_key] nvarchar(max)  NOT NULL,
    [Statistic_Id] int  NOT NULL
);
GO

-- Creating table 'TournamentSet'
CREATE TABLE [dbo].[TournamentSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [winner] int  NOT NULL,
    [countTeamsForUser] int  NOT NULL,
    [owner] int  NOT NULL,
    [countUser] int  NOT NULL,
    [countGroups] int  NOT NULL,
    [status] int  NOT NULL,
    [Modus_Id] int  NOT NULL
);
GO

-- Creating table 'TeamSet'
CREATE TABLE [dbo].[TeamSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [Statistic_Id] int  NOT NULL
);
GO

-- Creating table 'GroupCardSet'
CREATE TABLE [dbo].[GroupCardSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [groupNumber] int  NOT NULL,
    [TeamId] int  NULL,
    [TournamentId] int  NULL,
    [Statistic_Id] int  NOT NULL
);
GO

-- Creating table 'ModusSet'
CREATE TABLE [dbo].[ModusSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'MatchingSet'
CREATE TABLE [dbo].[MatchingSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [teamA] int  NOT NULL,
    [teamB] int  NOT NULL,
    [goalA] int  NOT NULL,
    [goalB] int  NOT NULL,
    [TournamentId] int  NULL,
    [Round_Id] int  NOT NULL
);
GO

-- Creating table 'RoundSet'
CREATE TABLE [dbo].[RoundSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'User_has_friendsSet'
CREATE TABLE [dbo].[User_has_friendsSet] (
    [User_Id] int  NOT NULL,
    [friend_id] int  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'UserTournament'
CREATE TABLE [dbo].[UserTournament] (
    [User_Id] int  NOT NULL,
    [Tournament_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'StatisticSet'
ALTER TABLE [dbo].[StatisticSet]
ADD CONSTRAINT [PK_StatisticSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TournamentSet'
ALTER TABLE [dbo].[TournamentSet]
ADD CONSTRAINT [PK_TournamentSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TeamSet'
ALTER TABLE [dbo].[TeamSet]
ADD CONSTRAINT [PK_TeamSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GroupCardSet'
ALTER TABLE [dbo].[GroupCardSet]
ADD CONSTRAINT [PK_GroupCardSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ModusSet'
ALTER TABLE [dbo].[ModusSet]
ADD CONSTRAINT [PK_ModusSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MatchingSet'
ALTER TABLE [dbo].[MatchingSet]
ADD CONSTRAINT [PK_MatchingSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RoundSet'
ALTER TABLE [dbo].[RoundSet]
ADD CONSTRAINT [PK_RoundSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [friend_id], [User_Id] in table 'User_has_friendsSet'
ALTER TABLE [dbo].[User_has_friendsSet]
ADD CONSTRAINT [PK_User_has_friendsSet]
    PRIMARY KEY CLUSTERED ([friend_id], [User_Id] ASC);
GO

-- Creating primary key on [User_Id], [Tournament_Id] in table 'UserTournament'
ALTER TABLE [dbo].[UserTournament]
ADD CONSTRAINT [PK_UserTournament]
    PRIMARY KEY CLUSTERED ([User_Id], [Tournament_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'UserTournament'
ALTER TABLE [dbo].[UserTournament]
ADD CONSTRAINT [FK_UserTournament_User]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Tournament_Id] in table 'UserTournament'
ALTER TABLE [dbo].[UserTournament]
ADD CONSTRAINT [FK_UserTournament_Tournament]
    FOREIGN KEY ([Tournament_Id])
    REFERENCES [dbo].[TournamentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTournament_Tournament'
CREATE INDEX [IX_FK_UserTournament_Tournament]
ON [dbo].[UserTournament]
    ([Tournament_Id]);
GO

-- Creating foreign key on [Modus_Id] in table 'TournamentSet'
ALTER TABLE [dbo].[TournamentSet]
ADD CONSTRAINT [FK_TournamentModus]
    FOREIGN KEY ([Modus_Id])
    REFERENCES [dbo].[ModusSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentModus'
CREATE INDEX [IX_FK_TournamentModus]
ON [dbo].[TournamentSet]
    ([Modus_Id]);
GO

-- Creating foreign key on [Round_Id] in table 'MatchingSet'
ALTER TABLE [dbo].[MatchingSet]
ADD CONSTRAINT [FK_MatchingRound]
    FOREIGN KEY ([Round_Id])
    REFERENCES [dbo].[RoundSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MatchingRound'
CREATE INDEX [IX_FK_MatchingRound]
ON [dbo].[MatchingSet]
    ([Round_Id]);
GO

-- Creating foreign key on [UserId] in table 'TeamSet'
ALTER TABLE [dbo].[TeamSet]
ADD CONSTRAINT [FK_UserTeam]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTeam'
CREATE INDEX [IX_FK_UserTeam]
ON [dbo].[TeamSet]
    ([UserId]);
GO

-- Creating foreign key on [Statistic_Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [FK_StatisticUser]
    FOREIGN KEY ([Statistic_Id])
    REFERENCES [dbo].[StatisticSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StatisticUser'
CREATE INDEX [IX_FK_StatisticUser]
ON [dbo].[UserSet]
    ([Statistic_Id]);
GO

-- Creating foreign key on [Statistic_Id] in table 'TeamSet'
ALTER TABLE [dbo].[TeamSet]
ADD CONSTRAINT [FK_StatisticTeam]
    FOREIGN KEY ([Statistic_Id])
    REFERENCES [dbo].[StatisticSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StatisticTeam'
CREATE INDEX [IX_FK_StatisticTeam]
ON [dbo].[TeamSet]
    ([Statistic_Id]);
GO

-- Creating foreign key on [Statistic_Id] in table 'GroupCardSet'
ALTER TABLE [dbo].[GroupCardSet]
ADD CONSTRAINT [FK_StatisticGroupCard]
    FOREIGN KEY ([Statistic_Id])
    REFERENCES [dbo].[StatisticSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StatisticGroupCard'
CREATE INDEX [IX_FK_StatisticGroupCard]
ON [dbo].[GroupCardSet]
    ([Statistic_Id]);
GO

-- Creating foreign key on [TeamId] in table 'GroupCardSet'
ALTER TABLE [dbo].[GroupCardSet]
ADD CONSTRAINT [FK_TeamGroupCard]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[TeamSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamGroupCard'
CREATE INDEX [IX_FK_TeamGroupCard]
ON [dbo].[GroupCardSet]
    ([TeamId]);
GO

-- Creating foreign key on [TournamentId] in table 'GroupCardSet'
ALTER TABLE [dbo].[GroupCardSet]
ADD CONSTRAINT [FK_TournamentGroupCard]
    FOREIGN KEY ([TournamentId])
    REFERENCES [dbo].[TournamentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentGroupCard'
CREATE INDEX [IX_FK_TournamentGroupCard]
ON [dbo].[GroupCardSet]
    ([TournamentId]);
GO

-- Creating foreign key on [TournamentId] in table 'MatchingSet'
ALTER TABLE [dbo].[MatchingSet]
ADD CONSTRAINT [FK_TournamentMatching]
    FOREIGN KEY ([TournamentId])
    REFERENCES [dbo].[TournamentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TournamentMatching'
CREATE INDEX [IX_FK_TournamentMatching]
ON [dbo].[MatchingSet]
    ([TournamentId]);
GO

-- Creating foreign key on [UserId] in table 'User_has_friendsSet'
ALTER TABLE [dbo].[User_has_friendsSet]
ADD CONSTRAINT [FK_UserUser_has_friends]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUser_has_friends'
CREATE INDEX [IX_FK_UserUser_has_friends]
ON [dbo].[User_has_friendsSet]
    ([UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------