using AutoMapper;
using ProLinked.Application.DTOs.Chats;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Jobs;
using ProLinked.Application.DTOs.Posts;
using ProLinked.Domain.DTOs.Chats;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Entities.Jobs;

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

        /* Comments */
        CreateMap<ReactionLookUp, ReactionDto>();
        CreateMap<CommentLookUp, CommentDto>();
        CreateMap<PostLookUp, PostLookUpDto>();
        CreateMap<PostWithDetails, PostWithDetailsDto>();
    }
}