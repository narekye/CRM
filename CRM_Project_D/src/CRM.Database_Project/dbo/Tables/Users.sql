CREATE TABLE [dbo].[Users] (
    [Id]             NVARCHAR (128) NOT NULL,
    [Email]          NVARCHAR (256) NULL,
    [EmailConfirmed] BIT            NOT NULL,
    [PasswordHash]   NVARCHAR (MAX) NULL,
    [PhoneNumber]    NVARCHAR (MAX) NULL,
    [UserName]       NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[Users]([UserName] ASC);

