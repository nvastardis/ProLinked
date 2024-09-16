using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLinked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetBlobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FullFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetBlobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetSkills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetChats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastMessageDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetChats_AspNetBlobs_ImageId",
                        column: x => x.ImageId,
                        principalTable: "AspNetBlobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Company = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    PhotographId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurriculumVitaeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RefreshTokenExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetBlobs_CurriculumVitaeId",
                        column: x => x.CurriculumVitaeId,
                        principalTable: "AspNetBlobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetBlobs_PhotographId",
                        column: x => x.PhotographId,
                        principalTable: "AspNetBlobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetChatMemberships",
                columns: table => new
                {
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetChatMemberships", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AspNetChatMemberships_AspNetChats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "AspNetChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetChatMemberships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetConnectionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetConnectionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetConnectionRequests_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetConnectionRequests_AspNetUsers_TargetId",
                        column: x => x.TargetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserBId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetConnections_AspNetUsers_UserAId",
                        column: x => x.UserAId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetConnections_AspNetUsers_UserBId",
                        column: x => x.UserBId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetJobAdvertisements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    WorkArrangement = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetJobAdvertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetJobAdvertisements_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetMessages_AspNetBlobs_MediaId",
                        column: x => x.MediaId,
                        principalTable: "AspNetBlobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetMessages_AspNetChats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "AspNetChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetMessages_AspNetMessages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AspNetMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    IsShown = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetNotifications_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PostVisibility = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetPosts_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetResumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetResumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetResumes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetJobApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvertisementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetJobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetJobApplications_AspNetJobAdvertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "AspNetJobAdvertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetJobApplications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MediaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetComments_AspNetBlobs_MediaId",
                        column: x => x.MediaId,
                        principalTable: "AspNetBlobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetComments_AspNetComments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AspNetComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetComments_AspNetPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "AspNetPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetComments_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetPostBlobs",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetPostBlobs", x => new { x.PostId, x.BlobId });
                    table.ForeignKey(
                        name: "FK_AspNetPostBlobs_AspNetBlobs_BlobId",
                        column: x => x.BlobId,
                        principalTable: "AspNetBlobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetPostBlobs_AspNetPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "AspNetPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetPostReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetPostReactions_AspNetPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "AspNetPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetPostReactions_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetEducationSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    School = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetEducationSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetEducationSteps_AspNetResumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "AspNetResumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetExperienceSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    IsEmployed = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkArrangement = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetExperienceSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetExperienceSteps_AspNetResumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "AspNetResumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetResumeSkills",
                columns: table => new
                {
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFollowingSkill = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetResumeSkills", x => new { x.ResumeId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_AspNetResumeSkills_AspNetResumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "AspNetResumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetResumeSkills_AspNetSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AspNetSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetCommentReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetCommentReactions_AspNetComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "AspNetComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetCommentReactions_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetEducationStepSkills",
                columns: table => new
                {
                    EducationStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetEducationStepSkills", x => new { x.EducationStepId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_AspNetEducationStepSkills_AspNetEducationSteps_EducationStepId",
                        column: x => x.EducationStepId,
                        principalTable: "AspNetEducationSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetEducationStepSkills_AspNetSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AspNetSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetExperienceStepSkills",
                columns: table => new
                {
                    ExperienceStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetExperienceStepSkills", x => new { x.ExperienceStepId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_AspNetExperienceStepSkills_AspNetExperienceSteps_ExperienceStepId",
                        column: x => x.ExperienceStepId,
                        principalTable: "AspNetExperienceSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetExperienceStepSkills_AspNetSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AspNetSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetBlobs_Id",
                table: "AspNetBlobs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetChatMemberships_ChatId_UserId",
                table: "AspNetChatMemberships",
                columns: new[] { "ChatId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetChatMemberships_UserId",
                table: "AspNetChatMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetChats_Id",
                table: "AspNetChats",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetChats_ImageId",
                table: "AspNetChats",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetCommentReactions_CommentId",
                table: "AspNetCommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetCommentReactions_CreatorId",
                table: "AspNetCommentReactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetCommentReactions_Id",
                table: "AspNetCommentReactions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetComments_CreatorId",
                table: "AspNetComments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetComments_Id",
                table: "AspNetComments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetComments_MediaId",
                table: "AspNetComments",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetComments_ParentId",
                table: "AspNetComments",
                column: "ParentId",
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetComments_PostId",
                table: "AspNetComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetConnectionRequests_Id",
                table: "AspNetConnectionRequests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetConnectionRequests_SenderId",
                table: "AspNetConnectionRequests",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetConnectionRequests_TargetId",
                table: "AspNetConnectionRequests",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetConnections_Id",
                table: "AspNetConnections",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetConnections_UserAId",
                table: "AspNetConnections",
                column: "UserAId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetConnections_UserBId",
                table: "AspNetConnections",
                column: "UserBId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetEducationSteps_Id",
                table: "AspNetEducationSteps",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetEducationSteps_ResumeId",
                table: "AspNetEducationSteps",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetEducationStepSkills_EducationStepId_SkillId",
                table: "AspNetEducationStepSkills",
                columns: new[] { "EducationStepId", "SkillId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetEducationStepSkills_SkillId",
                table: "AspNetEducationStepSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetExperienceSteps_Id",
                table: "AspNetExperienceSteps",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetExperienceSteps_ResumeId",
                table: "AspNetExperienceSteps",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetExperienceStepSkills_ExperienceStepId_SkillId",
                table: "AspNetExperienceStepSkills",
                columns: new[] { "ExperienceStepId", "SkillId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetExperienceStepSkills_SkillId",
                table: "AspNetExperienceStepSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetJobAdvertisements_CreatorId",
                table: "AspNetJobAdvertisements",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetJobAdvertisements_Id",
                table: "AspNetJobAdvertisements",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetJobApplications_AdvertisementId",
                table: "AspNetJobApplications",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetJobApplications_Id",
                table: "AspNetJobApplications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetJobApplications_UserId",
                table: "AspNetJobApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetMessages_ChatId",
                table: "AspNetMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetMessages_Id",
                table: "AspNetMessages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetMessages_MediaId",
                table: "AspNetMessages",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetMessages_ParentId",
                table: "AspNetMessages",
                column: "ParentId",
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetMessages_SenderId",
                table: "AspNetMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetNotifications_Id",
                table: "AspNetNotifications",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetNotifications_TargetUserId",
                table: "AspNetNotifications",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostBlobs_BlobId",
                table: "AspNetPostBlobs",
                column: "BlobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostBlobs_PostId_BlobId",
                table: "AspNetPostBlobs",
                columns: new[] { "PostId", "BlobId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostReactions_CreatorId",
                table: "AspNetPostReactions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostReactions_Id",
                table: "AspNetPostReactions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPostReactions_PostId",
                table: "AspNetPostReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPosts_CreatorId",
                table: "AspNetPosts",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetPosts_Id",
                table: "AspNetPosts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetResumes_Id",
                table: "AspNetResumes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetResumes_UserId",
                table: "AspNetResumes",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetResumeSkills_ResumeId_SkillId",
                table: "AspNetResumeSkills",
                columns: new[] { "ResumeId", "SkillId" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetResumeSkills_SkillId",
                table: "AspNetResumeSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetSkills_Id",
                table: "AspNetSkills",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CurriculumVitaeId",
                table: "AspNetUsers",
                column: "CurriculumVitaeId",
                unique: true,
                filter: "[CurriculumVitaeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhotographId",
                table: "AspNetUsers",
                column: "PhotographId",
                unique: true,
                filter: "[PhotographId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetChatMemberships");

            migrationBuilder.DropTable(
                name: "AspNetCommentReactions");

            migrationBuilder.DropTable(
                name: "AspNetConnectionRequests");

            migrationBuilder.DropTable(
                name: "AspNetConnections");

            migrationBuilder.DropTable(
                name: "AspNetEducationStepSkills");

            migrationBuilder.DropTable(
                name: "AspNetExperienceStepSkills");

            migrationBuilder.DropTable(
                name: "AspNetJobApplications");

            migrationBuilder.DropTable(
                name: "AspNetMessages");

            migrationBuilder.DropTable(
                name: "AspNetNotifications");

            migrationBuilder.DropTable(
                name: "AspNetPostBlobs");

            migrationBuilder.DropTable(
                name: "AspNetPostReactions");

            migrationBuilder.DropTable(
                name: "AspNetResumeSkills");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetComments");

            migrationBuilder.DropTable(
                name: "AspNetEducationSteps");

            migrationBuilder.DropTable(
                name: "AspNetExperienceSteps");

            migrationBuilder.DropTable(
                name: "AspNetJobAdvertisements");

            migrationBuilder.DropTable(
                name: "AspNetChats");

            migrationBuilder.DropTable(
                name: "AspNetSkills");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetPosts");

            migrationBuilder.DropTable(
                name: "AspNetResumes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AspNetBlobs");
        }
    }
}
