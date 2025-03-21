﻿using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.AzureStorage.Blobs;
using ProLinked.Domain.Chats;
using ProLinked.Domain.Connections;
using ProLinked.Domain.Identity;
using ProLinked.Domain.Jobs;
using ProLinked.Domain.Notifications;
using ProLinked.Domain.Posts;
using ProLinked.Domain.Resumes;
using ProLinked.Domain.Resumes.Education;
using ProLinked.Domain.Resumes.Experience;
using ProLinked.Domain.Resumes.Skills;
using ProLinked.Shared;
using ProLinked.Shared.Chats;
using ProLinked.Shared.Jobs;
using ProLinked.Shared.Posts;
using ProLinked.Shared.Resumes;
using ProLinked.Shared.Utils;

namespace ProLinked.Data;

public static class ProLinkedDbContextModelBuilderExtensions
{
    public static void ConfigureBlobs(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        builder.Entity<Blob>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Blobs", ProLinkedConsts.DbSchema);


            b.HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<AppUser>("PhotographId")
                .IsRequired(false);

            b.HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<AppUser>("CurriculumVitaeId")
                .IsRequired(false);

            b.HasIndex(x => x.Id);
        });
    }

    public static void ConfigurePostEntities(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        builder.Entity<Post>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Posts", ProLinkedConsts.DbSchema);

            b.Property(x => x.Text).
                IsUnicode().
                HasMaxLength(PostConsts.MaxContentLength);

            b.HasOne<AppUser>().
                WithMany().
                HasForeignKey(x => x.CreatorId).
                IsRequired().
                OnDelete(DeleteBehavior.NoAction);

            b.HasMany(x => x.Comments)
                .WithOne()
                .IsRequired()
                .HasForeignKey(x => x.PostId);

            b.HasMany(x => x.Reactions)
                .WithOne()
                .IsRequired()
                .HasForeignKey(x => x.PostId);

            b.HasMany(x => x.Media)
                .WithOne()
                .HasForeignKey(x => x.PostId);

            b.HasIndex(x => x.Id);
        });

        builder.Entity<PostReaction>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "PostReactions", ProLinkedConsts.DbSchema);


            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(x => x.Id);
        });

        builder.Entity<PostBlob>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "PostBlobs", ProLinkedConsts.DbSchema);
            b.HasKey(x => new { x.PostId, x.BlobId });

            b.HasOne<Blob>()
                .WithOne()
                .HasForeignKey<PostBlob>(x => x.BlobId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(x => new {x.PostId, x.BlobId});
        });

        builder.Entity<Comment>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Comments", ProLinkedConsts.DbSchema);

            b.Property(x => x.Text)
                .IsUnicode()
                .HasMaxLength(CommentConsts.MaxContentLength);

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            b.HasMany(x => x.Reactions)
                .WithOne()
                .IsRequired()
                .HasForeignKey(x => x.CommentId);

            b.HasOne(x => x.Parent)
                .WithOne()
                .IsRequired(false);

            b.HasIndex(x => x.Id);
        });

        builder.Entity<CommentReaction>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "CommentReactions", ProLinkedConsts.DbSchema);


            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(x => x.Id);
        });
    }

    public static void ConfigureChatEntities(this ModelBuilder builder)
    {
        builder.Entity<Chat>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Chats", ProLinkedConsts.DbSchema);

            b.Property(x => x.Title).
                IsUnicode().
                HasMaxLength(ChatConsts.MaxTitleLength);

            b.HasMany(x => x.Members)
                .WithOne()
                .HasForeignKey(x => x.ChatId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Messages)
                .WithOne()
                .IsRequired()
                .HasForeignKey(x => x.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.Id);
        });

        builder.Entity<ChatMembership>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "ChatMemberships", ProLinkedConsts.DbSchema);

            b.HasKey(x => new { x.ChatId, x.UserId });
            b.HasIndex(x => new { x.ChatId, x.UserId });

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        });

        builder.Entity<Message>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Messages", ProLinkedConsts.DbSchema);

            b.Property(x => x.Text)
                .IsUnicode()
                .HasMaxLength(MessageConsts.MaxContentLength);

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(x => x.Parent)
                .WithOne()
                .IsRequired(false);

            b.HasIndex(x => x.Id);
        });
    }

    public static void ConfigureResumeEntities(this ModelBuilder builder)
    {
         builder.Entity<Skill>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Skills", ProLinkedConsts.DbSchema);

            b.Property(x => x.Title)
                .IsUnicode()
                .HasMaxLength(SkillConsts.MaxTitleLength);
            b.HasMany<ResumeSkill>()
                .WithOne()
                .HasForeignKey(e => e.SkillId);
            b.HasIndex(x => x.Id);
        });

        builder.Entity<Resume>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Resumes", ProLinkedConsts.DbSchema);

            b.HasIndex(x => x.Id);
            b.HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<Resume>(x => x.UserId)
                .IsRequired();

            b.HasMany(x => x.Education)
                .WithOne()
                .HasForeignKey(x => x.ResumeId);

            b.HasMany(x => x.Experience)
                .WithOne()
                .HasForeignKey(x => x.ResumeId);

            b.HasMany(x => x.ResumeSkills)
                .WithOne()
                .HasForeignKey(x => x.ResumeId);
        });

        builder.Entity<ResumeSkill>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "ResumeSkills", ProLinkedConsts.DbSchema);

            b.HasKey(x => new {x.ResumeId, x.SkillId});
            b.HasIndex(x => new {x.ResumeId, x.SkillId});
        });

        builder.Entity<EducationStep>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "EducationSteps", ProLinkedConsts.DbSchema);

            b.HasMany(e => e.RelatedSkills)
                .WithOne()
                .HasForeignKey(e => e.EducationStepId);
            b.HasIndex(x => x.Id);
        });

        builder.Entity<EducationStepSkill>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "EducationStepSkills", ProLinkedConsts.DbSchema);

            b.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(e => e.SkillId);
            b.HasKey(x => new { x.EducationStepId, x.SkillId} );
            b.HasIndex(x => new { x.EducationStepId, x.SkillId });
        });

        builder.Entity<ExperienceStep>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "ExperienceSteps", ProLinkedConsts.DbSchema);

            b.HasMany(e => e.RelatedSkills)
                .WithOne()
                .HasForeignKey(e => e.ExperienceStepId);
            b.HasIndex(x => x.Id);
        });

        builder.Entity<ExperienceStepSkill>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "ExperienceStepSkills", ProLinkedConsts.DbSchema);

            b.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(e => e.SkillId);
            b.HasKey(x => new {x.ExperienceStepId, x.SkillId});
            b.HasIndex(x => new { x.ExperienceStepId, x.SkillId} );
        });
    }

    public static void ConfigureConnectionEntities(this ModelBuilder builder)
    {
        builder.Entity<ConnectionRequest>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "ConnectionRequests", ProLinkedConsts.DbSchema);

            b.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(x => x.Target)
                .WithMany()
                .HasForeignKey(x => x.TargetId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(x => x.Id);
        });

        builder.Entity<Connection>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Connections", ProLinkedConsts.DbSchema);

            b.HasOne(x => x.UserA)
                .WithMany()
                .HasForeignKey(x => x.UserAId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(x => x.UserB)
                .WithMany()
                .HasForeignKey(x => x.UserBId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(x => x.Id);
        });

    }

    public static void ConfigureJobEntities(this ModelBuilder builder)
    {
        builder.Entity<Advertisement>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "JobAdvertisements", ProLinkedConsts.DbSchema);


            b.Property(x => x.Title)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(AdvertisementConsts.MaxTitleLength);

            b.Property(x => x.Description)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(AdvertisementConsts.MaxDescriptionLength);

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .IsRequired();

            b.HasMany(x => x.Applications)
                .WithOne()
                .HasForeignKey(x => x.AdvertisementId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.Id);
        });

        builder.Entity<Application>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "JobApplications", ProLinkedConsts.DbSchema);


            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(false);

            b.HasIndex(x => x.Id);
        });
    }

    public static void ConfigureNotifications(this ModelBuilder builder)
    {
        builder.Entity<Notification>(b =>
        {
            b.ToTable(ProLinkedConsts.DbTablePrefix + "Notifications", ProLinkedConsts.DbSchema);


            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.TargetUserId);

            b.HasIndex(x => x.Id);
        });
    }
}
