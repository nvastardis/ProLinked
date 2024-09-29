using AutoMapper;
using ProLinked.Application.Contracts.Chats.DTOs;
using ProLinked.Application.Contracts.Connections.DTOs;
using ProLinked.Application.Contracts.Jobs.DTOs;
using ProLinked.Application.Contracts.Notifications.DTOs;
using ProLinked.Application.Contracts.Posts.DTOs;
using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Domain.DTOs.Chats;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.DTOs.Notifications;
using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.DTOs.Resumes;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Application;

public class ProLinkedAutoMapperProfile: Profile
{
    public ProLinkedAutoMapperProfile()
    {
        /* Chat Entity Mappings */
        CreateMap<Chat, ChatWithDetailsDto>()
            .ForMember(
                e => e.ImageId,
                s => s.MapFrom(
                x => (Guid?)(x.Image == null ? null : x.Image.Id))
            );

        CreateMap<ChatLookUp, ChatLookUpDto>();
        CreateMap<MessageWithDetails, MessageLookUpDto>();
        CreateMap<ChatMembershipLookUp, ChatMembershipLookUpDto>();

        /* Connection Entity Mappings */
        CreateMap<ConnectionLookUp, ConnectionLookUpDto>();
        CreateMap<ConnectionRequestLookUp, ConnectionRequestLookUpDto>();

        /* Jobs Entity Mappings */
        CreateMap<Advertisement, AdvertisementDto>();
        CreateMap<Domain.Entities.Jobs.Application, ApplicationDto>();

        /* Post Entity Mappings */
        CreateMap<ReactionLookUp, ReactionDto>();
        CreateMap<CommentLookUp, CommentDto>();
        CreateMap<PostLookUp, PostLookUpDto>();
        CreateMap<PostWithDetails, PostWithDetailsDto>();

        /* Resume Entity Mappings */
        CreateMap<Skill, SkillDto>();
        CreateMap<ResumeWithDetails, ResumeDto>();
        CreateMap<EducationStep, EducationStepDto>();
        CreateMap<ExperienceStep, ExperienceStepDto>();

        /* Notifications */
        CreateMap<NotificationLookUp, NotificationLookUpDto>();
    }
}