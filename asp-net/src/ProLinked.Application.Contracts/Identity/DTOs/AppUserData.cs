using ProLinked.Application.Contracts.Connections.DTOs;
using ProLinked.Application.Contracts.Jobs.DTOs;
using ProLinked.Application.Contracts.Posts.DTOs;
using ProLinked.Application.Contracts.Resumes.DTOs;

namespace ProLinked.Application.Contracts.Identity.DTOs;

public class AppUserData
{
    public InfoResponse? Details { get; set; }
    public List<ConnectionLookUpDto>? Network { get; set; }
    public List<AdvertisementDto>? Advertisements { get; set; }
    public List<ApplicationDto>? Applications { get; set; }
    public List<CommentDto>? Comments { get; set; }
    public List<PostLookUpDto>? Posts { get; set; }
    public List<ReactionDto>? Reactions { get; set; }
    public ResumeDto? Resume { get; set; }
}