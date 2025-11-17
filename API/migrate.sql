Build started...
Build succeeded.
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231115120655_InitialCreate') THEN
    CREATE TABLE "Projects" (
        "Id" TEXT NOT NULL,
        "Name" TEXT,
        "ProjectOwner" TEXT,
        "Description" TEXT,
        "StartDate" TEXT NOT NULL,
        "EndDate" TEXT NOT NULL,
        CONSTRAINT "PK_Projects" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20231115120655_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20231115120655_InitialCreate', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240112161708_drop_date') THEN
    ALTER TABLE "Projects" DROP COLUMN "EndDate";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240112161708_drop_date') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240112161708_drop_date', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    ALTER TABLE "Projects" DROP COLUMN "EndDate";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE TABLE "AspNetRoles" (
        "Id" TEXT NOT NULL,
        "Name" TEXT,
        "NormalizedName" TEXT,
        "ConcurrencyStamp" TEXT,
        CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE TABLE "AspNetUsers" (
        "Id" TEXT NOT NULL,
        "DisplayName" TEXT,
        "JobTitle" TEXT,
        "UserName" TEXT,
        "NormalizedUserName" TEXT,
        "Email" TEXT,
        "NormalizedEmail" TEXT,
        "EmailConfirmed" INTEGER NOT NULL,
        "PasswordHash" TEXT,
        "SecurityStamp" TEXT,
        "ConcurrencyStamp" TEXT,
        "PhoneNumber" TEXT,
        "PhoneNumberConfirmed" INTEGER NOT NULL,
        "TwoFactorEnabled" INTEGER NOT NULL,
        "LockoutEnd" TEXT,
        "LockoutEnabled" INTEGER NOT NULL,
        "AccessFailedCount" INTEGER NOT NULL,
        CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE TABLE "AspNetRoleClaims" (
        "Id" INTEGER NOT NULL,
        "RoleId" TEXT NOT NULL,
        "ClaimType" TEXT,
        "ClaimValue" TEXT,
        CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE TABLE "AspNetUserClaims" (
        "Id" INTEGER NOT NULL,
        "UserId" TEXT NOT NULL,
        "ClaimType" TEXT,
        "ClaimValue" TEXT,
        CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE TABLE "AspNetUserLogins" (
        "LoginProvider" TEXT NOT NULL,
        "ProviderKey" TEXT NOT NULL,
        "ProviderDisplayName" TEXT,
        "UserId" TEXT NOT NULL,
        CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
        CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE TABLE "AspNetUserRoles" (
        "UserId" TEXT NOT NULL,
        "RoleId" TEXT NOT NULL,
        CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
        CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE TABLE "AspNetUserTokens" (
        "UserId" TEXT NOT NULL,
        "LoginProvider" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Value" TEXT,
        CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
        CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240123232754_IdentityAdded') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240123232754_IdentityAdded', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240216115332_ProjectParticipants') THEN
    ALTER TABLE "AspNetUsers" ADD "Bio" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240216115332_ProjectParticipants') THEN
    CREATE TABLE "ProjectParticipants" (
        "AppUserId" TEXT NOT NULL,
        "ProjectId" TEXT NOT NULL,
        "IsOwner" INTEGER NOT NULL,
        CONSTRAINT "PK_ProjectParticipants" PRIMARY KEY ("AppUserId", "ProjectId"),
        CONSTRAINT "FK_ProjectParticipants_AspNetUsers_AppUserId" FOREIGN KEY ("AppUserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ProjectParticipants_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240216115332_ProjectParticipants') THEN
    CREATE INDEX "IX_ProjectParticipants_ProjectId" ON "ProjectParticipants" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240216115332_ProjectParticipants') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240216115332_ProjectParticipants', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240220161535_addCancelledProperty') THEN
    ALTER TABLE "Projects" RENAME COLUMN "Name" TO "ProjectTitle";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240220161535_addCancelledProperty') THEN
    ALTER TABLE "Projects" ADD "IsCancelled" INTEGER NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240220161535_addCancelledProperty') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240220161535_addCancelledProperty', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240423105904_TicketEntityAdded') THEN
    CREATE TABLE "Tickets" (
        "Id" TEXT NOT NULL,
        "Title" TEXT,
        "Description" TEXT,
        "Submitter" TEXT,
        "Assigned" TEXT,
        priority TEXT,
        "Severity" TEXT,
        "Status" TEXT,
        "StartDate" TEXT NOT NULL,
        "Updated" TEXT NOT NULL,
        "ProjectId" INTEGER NOT NULL,
        "ProjectId1" TEXT,
        CONSTRAINT "PK_Tickets" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Tickets_Projects_ProjectId1" FOREIGN KEY ("ProjectId1") REFERENCES "Projects" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240423105904_TicketEntityAdded') THEN
    CREATE INDEX "IX_Tickets_ProjectId1" ON "Tickets" ("ProjectId1");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240423105904_TicketEntityAdded') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240423105904_TicketEntityAdded', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    ALTER TABLE "Tickets" DROP CONSTRAINT "FK_Tickets_Projects_ProjectId1";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    DROP INDEX "IX_Tickets_ProjectId1";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    ALTER TABLE "Tickets" DROP COLUMN "ProjectId1";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    ALTER TABLE "Tickets" RENAME COLUMN priority TO "Priority";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "ProjectId" TYPE TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    ALTER TABLE "Tickets" ADD "EndDate" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    CREATE INDEX "IX_Tickets_ProjectId" ON "Tickets" ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    ALTER TABLE "Tickets" ADD CONSTRAINT "FK_Tickets_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250731110718_ticket-entity-update') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250731110718_ticket-entity-update', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826183335_AddRoleToProjectParticipant') THEN
    ALTER TABLE "ProjectParticipants" ADD "Role" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826183335_AddRoleToProjectParticipant') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250826183335_AddRoleToProjectParticipant', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250929124641_AddCreatedAtToTicket') THEN
    ALTER TABLE "Tickets" ADD "CreatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250929124641_AddCreatedAtToTicket') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250929124641_AddCreatedAtToTicket', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006092111_AddGlobalRoleToAppUser') THEN
    ALTER TABLE "AspNetUsers" ADD "GlobalRole" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006092111_AddGlobalRoleToAppUser') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251006092111_AddGlobalRoleToAppUser', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021144351_AddSoftDeleteToUser') THEN
    ALTER TABLE "AspNetUsers" ADD "DeletedAt" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021144351_AddSoftDeleteToUser') THEN
    ALTER TABLE "AspNetUsers" ADD "IsDeleted" INTEGER NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021144351_AddSoftDeleteToUser') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021144351_AddSoftDeleteToUser', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021150309_AddJobTitleToAdminRegister') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021150309_AddJobTitleToAdminRegister', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022154313_AddIsDeletedToProject') THEN
    ALTER TABLE "Projects" ADD "IsDeleted" INTEGER NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022154313_AddIsDeletedToProject') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251022154313_AddIsDeletedToProject', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022180225_AddDeletedDateToProject') THEN
    ALTER TABLE "Projects" ADD "DeletedDate" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022180225_AddDeletedDateToProject') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251022180225_AddDeletedDateToProject', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103165325_AddClosedDateToTicket') THEN
    ALTER TABLE "Tickets" ADD "ClosedDate" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103165325_AddClosedDateToTicket') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251103165325_AddClosedDateToTicket', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105132028_AddCommentsAndAttachments') THEN
    CREATE TABLE "Comments" (
        "Id" TEXT NOT NULL,
        "Content" TEXT,
        "CreatedAt" TEXT NOT NULL,
        "UpdatedAt" TEXT,
        "TicketId" TEXT NOT NULL,
        "AuthorId" TEXT,
        CONSTRAINT "PK_Comments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Comments_AspNetUsers_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Comments_Tickets_TicketId" FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105132028_AddCommentsAndAttachments') THEN
    CREATE TABLE "CommentAttachments" (
        "Id" TEXT NOT NULL,
        "FileName" TEXT,
        "OriginalFileName" TEXT,
        "ContentType" TEXT,
        "FileSize" INTEGER NOT NULL,
        "FilePath" TEXT,
        "UploadedAt" TEXT NOT NULL,
        "CommentId" TEXT NOT NULL,
        "UploadedById" TEXT,
        CONSTRAINT "PK_CommentAttachments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_CommentAttachments_AspNetUsers_UploadedById" FOREIGN KEY ("UploadedById") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_CommentAttachments_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105132028_AddCommentsAndAttachments') THEN
    CREATE INDEX "IX_CommentAttachments_CommentId" ON "CommentAttachments" ("CommentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105132028_AddCommentsAndAttachments') THEN
    CREATE INDEX "IX_CommentAttachments_UploadedById" ON "CommentAttachments" ("UploadedById");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105132028_AddCommentsAndAttachments') THEN
    CREATE INDEX "IX_Comments_AuthorId" ON "Comments" ("AuthorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105132028_AddCommentsAndAttachments') THEN
    CREATE INDEX "IX_Comments_TicketId" ON "Comments" ("TicketId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105132028_AddCommentsAndAttachments') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251105132028_AddCommentsAndAttachments', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251111205741_AddSoftDeleteToTicket') THEN
    ALTER TABLE "Tickets" ADD "DeletedDate" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251111205741_AddSoftDeleteToTicket') THEN
    ALTER TABLE "Tickets" ADD "IsDeleted" INTEGER NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251111205741_AddSoftDeleteToTicket') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251111205741_AddSoftDeleteToTicket', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251112133128_AddJoinDateToUser') THEN
    ALTER TABLE "AspNetUsers" ADD "JoinDate" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251112133128_AddJoinDateToUser') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251112133128_AddJoinDateToUser', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251112135338_AddParentCommentId') THEN
    ALTER TABLE "Comments" ADD "ParentCommentId" TEXT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251112135338_AddParentCommentId') THEN
    CREATE INDEX "IX_Comments_ParentCommentId" ON "Comments" ("ParentCommentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251112135338_AddParentCommentId') THEN
    ALTER TABLE "Comments" ADD CONSTRAINT "FK_Comments_Comments_ParentCommentId" FOREIGN KEY ("ParentCommentId") REFERENCES "Comments" ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251112135338_AddParentCommentId') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251112135338_AddParentCommentId', '8.0.0');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Updated" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Title" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Submitter" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Status" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "StartDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Severity" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "ProjectId" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Priority" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "IsDeleted" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "EndDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Description" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "DeletedDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "CreatedAt" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "ClosedDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Assigned" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Tickets" ALTER COLUMN "Id" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "StartDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "ProjectTitle" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "ProjectOwner" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "IsDeleted" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "IsCancelled" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "Description" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "DeletedDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Projects" ALTER COLUMN "Id" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "ProjectParticipants" ALTER COLUMN "Role" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "ProjectParticipants" ALTER COLUMN "IsOwner" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "ProjectParticipants" ALTER COLUMN "ProjectId" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "ProjectParticipants" ALTER COLUMN "AppUserId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Comments" ALTER COLUMN "UpdatedAt" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Comments" ALTER COLUMN "TicketId" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Comments" ALTER COLUMN "ParentCommentId" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Comments" ALTER COLUMN "CreatedAt" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Comments" ALTER COLUMN "Content" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Comments" ALTER COLUMN "AuthorId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "Comments" ALTER COLUMN "Id" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "UploadedById" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "UploadedAt" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "OriginalFileName" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "FileSize" TYPE bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "FilePath" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "FileName" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "ContentType" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "CommentId" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "CommentAttachments" ALTER COLUMN "Id" TYPE uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserTokens" ALTER COLUMN "Value" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserTokens" ALTER COLUMN "Name" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserTokens" ALTER COLUMN "LoginProvider" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserTokens" ALTER COLUMN "UserId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "UserName" TYPE character varying(256);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "TwoFactorEnabled" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "SecurityStamp" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "PhoneNumberConfirmed" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "PhoneNumber" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "PasswordHash" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "NormalizedUserName" TYPE character varying(256);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "NormalizedEmail" TYPE character varying(256);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "LockoutEnd" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "LockoutEnabled" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "JoinDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "JobTitle" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "IsDeleted" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "GlobalRole" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "EmailConfirmed" TYPE boolean;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "Email" TYPE character varying(256);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "DisplayName" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "DeletedAt" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "ConcurrencyStamp" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "Bio" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "AccessFailedCount" TYPE integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUsers" ALTER COLUMN "Id" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserRoles" ALTER COLUMN "RoleId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserRoles" ALTER COLUMN "UserId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserLogins" ALTER COLUMN "UserId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserLogins" ALTER COLUMN "ProviderDisplayName" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserLogins" ALTER COLUMN "ProviderKey" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserLogins" ALTER COLUMN "LoginProvider" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserClaims" ALTER COLUMN "UserId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserClaims" ALTER COLUMN "ClaimValue" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserClaims" ALTER COLUMN "ClaimType" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetUserClaims" ALTER COLUMN "Id" TYPE integer;
    ALTER TABLE "AspNetUserClaims" ALTER COLUMN "Id" DROP DEFAULT;
    ALTER TABLE "AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoles" ALTER COLUMN "NormalizedName" TYPE character varying(256);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoles" ALTER COLUMN "Name" TYPE character varying(256);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoles" ALTER COLUMN "ConcurrencyStamp" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoles" ALTER COLUMN "Id" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoleClaims" ALTER COLUMN "RoleId" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoleClaims" ALTER COLUMN "ClaimValue" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoleClaims" ALTER COLUMN "ClaimType" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    ALTER TABLE "AspNetRoleClaims" ALTER COLUMN "Id" TYPE integer;
    ALTER TABLE "AspNetRoleClaims" ALTER COLUMN "Id" DROP DEFAULT;
    ALTER TABLE "AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251115195112_InitialCreatePostgreSQL') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251115195112_InitialCreatePostgreSQL', '8.0.0');
    END IF;
END $EF$;
COMMIT;


