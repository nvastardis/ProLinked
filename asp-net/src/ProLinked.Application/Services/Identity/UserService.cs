using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ProLinked.Application.Contracts.Connections.DTOs;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.Contracts.Identity.DTOs;
using ProLinked.Application.Contracts.Jobs.DTOs;
using ProLinked.Application.Contracts.Posts.DTOs;
using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.DTOs.Resumes;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Utils;
using System.Xml;
using System.Xml.Serialization;
using InfoResponse = ProLinked.Application.Contracts.Identity.DTOs.InfoResponse;

namespace ProLinked.Application.Services.Identity;

public class UserService: ProLinkedServiceBase, IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IBlobManager _blobManager;
    private readonly IConnectionRepository _connectionRepository;
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IRepository<Reaction,Guid> _reactionRepository;
    private readonly IResumeRepository _resumeRepository;
    private readonly EmailAddressAttribute _emailAddressAttribute = new();
    private readonly PhoneAttribute _phoneAttribute = new();

    public UserService(
        IMapper objectMapper,
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore,
        IBlobManager blobManager,
        IConnectionRepository connectionRepository,
        IAdvertisementRepository advertisementRepository,
        IApplicationRepository applicationRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IRepository<Reaction, Guid> reactionRepository,
        IResumeRepository resumeRepository) :
        base(objectMapper)
    {
        _userManager = userManager;
        _userStore = userStore;
        _blobManager = blobManager;
        _connectionRepository = connectionRepository;
        _advertisementRepository = advertisementRepository;
        _applicationRepository = applicationRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _reactionRepository = reactionRepository;
        _resumeRepository = resumeRepository;
    }

    public async Task<Results<Ok<InfoResponse>, NotFound>> InfoAsync(
        ClaimsPrincipal claimsPrincipal)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(await ResponseGenerator.CreateInfoResponseAsync(user, _userManager));
    }

    public async Task<Results<Ok<InfoResponse>, NotFound>> InfoAsync(
        Guid userId)
    {
        if (await _userManager.FindByIdAsync(userId.ToString()) is not { } user)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(await ResponseGenerator.CreateInfoResponseAsync(user, _userManager));
    }

    public async Task<Results<FileStreamHttpResult, NotFound>> DownloadInfoAsync(
        Guid userId,
        bool inXml = false)
    {
        if (await _userManager.FindByIdAsync(userId.ToString()) is not { } user)
        {
            return TypedResults.NotFound();
        }

        var dataObject = new AppUserData
        {
            Details = await ResponseGenerator.CreateInfoResponseAsync(user, _userManager)
        };

        var network = await _connectionRepository.GetListByUserAsync(userId);
        dataObject.Network = ObjectMapper.Map<List<ConnectionLookUp>, List<ConnectionLookUpDto>>(network);

        var advertisements = await _advertisementRepository.GetListByUserAsync(userId);
        dataObject.Advertisements = ObjectMapper.Map<List<Advertisement>, List<AdvertisementDto>>(advertisements);

        var applications = await _applicationRepository.GetListByUserAsync(userId);
        dataObject.Applications = ObjectMapper.Map<List<Domain.Entities.Jobs.Application>, List<ApplicationDto>>(applications);

        var posts = await _postRepository.GetLookUpListAsync(userId: userId);
        dataObject.Posts = ObjectMapper.Map<List<PostLookUp>, List<PostLookUpDto>>(posts);

        var comments = await _commentRepository.GetLookUpListAsync(userId: userId);
        dataObject.Comments = ObjectMapper.Map<List<CommentLookUp>, List<CommentDto>>(comments);

        var reactions = await _reactionRepository.FindManyAsync(e => e.CreatorId == userId);
        if (reactions is not null)
        {
            dataObject.Reactions = ObjectMapper.Map<List<Reaction>, List<ReactionDto>>(reactions.ToList());
        }

        var resume = await _resumeRepository.FindAsync(e => e.UserId == userId);
        if (resume is not null)
        {
            var resumeWithData = await _resumeRepository.GetWithDetailsAsync(resume.Id);
            dataObject.Resume = ObjectMapper.Map<ResumeWithDetails, ResumeDto>(resumeWithData);
        }

        var data = inXml ? await SerializeToXml(dataObject) : await SerializeToJson(dataObject);
        var fileName = $"{user.Name}_{user.Surname}_data" + (inXml ? ".xml" : ".json");

        return TypedResults.File(data, "application/octet-stream", fileName);
    }

    public async Task<Results<Ok<InfoResponse[]>, NotFound>> FindAsync(
        string name)
    {
        var tasks = _userManager.
            Users.
            Where(e => (e.Name+e.Surname).Contains(name, StringComparison.CurrentCultureIgnoreCase)).
            Select(e => ResponseGenerator.CreateInfoResponseAsync(e, _userManager)).
            ToList();

        var result = await Task.WhenAll(tasks);
        return TypedResults.Ok(result);
    }

    public async Task<Results<NoContent, ValidationProblem, NotFound>> UpdateAsync(
        ClaimsPrincipal claimsPrincipal,
        UpdateIdentityDto input,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (!input.NewEmail.IsNullOrWhiteSpace() && !_emailAddressAttribute.IsValid(input.NewEmail))
        {
            return ResponseGenerator.CreateValidationProblem(
                IdentityResult.Failed(_userManager.ErrorDescriber.InvalidEmail(input.NewEmail)));
        }

        if (!input.NewPhoneNumber.IsNullOrWhiteSpace())
        {
            if (!_phoneAttribute.IsValid(input.NewPhoneNumber))
            {
                return ResponseGenerator.CreateValidationProblem(
                    IdentityResult.Failed(_userManager.ErrorDescriber.InvalidPhone(input.NewPhoneNumber)));
            }
            await _userStore.SetCompanyAsync(user, input.NewCompany, cancellationToken);
        }

        if (!input.NewName.IsNullOrWhiteSpace())
        {
            await _userStore.SetNameAsync(user, input.NewName, cancellationToken);
        }

        if (!input.NewSurname.IsNullOrWhiteSpace())
        {
            await _userStore.SetSurnameAsync(user, input.NewSurname, cancellationToken);
        }

        if (!input.NewSummary.IsNullOrWhiteSpace())
        {
            await _userStore.SetSummaryAsync(user, input.NewSummary, cancellationToken);
        }

        if (!input.NewJobTitle.IsNullOrWhiteSpace())
        {
            await _userStore.SetJobTitleAsync(user, input.NewJobTitle, cancellationToken);
        }

        if (!input.NewCompany.IsNullOrWhiteSpace())
        {
            await _userStore.SetCompanyAsync(user, input.NewCompany, cancellationToken);
        }

        if (!input.NewCity.IsNullOrWhiteSpace())
        {
            await _userStore.SetCityAsync(user, input.NewCity, cancellationToken);
        }

        if (input.NewDateOfBirth is not null)
        {
            await _userStore.SetDateOfBirthAsync(user, input.NewDateOfBirth, cancellationToken);
        }

        if (!string.IsNullOrEmpty(input.NewPassword))
        {
            if (string.IsNullOrEmpty(input.OldPassword))
            {
                return ResponseGenerator.CreateValidationProblem("OldPasswordRequired",
                    "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
            }

            var changePasswordResult =
                await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return ResponseGenerator.CreateValidationProblem(changePasswordResult);
            }
        }

        return TypedResults.NoContent();
    }

    public async Task<Results<NoContent, NotFound>> UpdatePhotographAsync(
        ClaimsPrincipal claimsPrincipal,
        IFormFile photograph,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (user.PhotographId is not null)
        {
            var oldBlob = await _blobManager.GetAsync(user.PhotographId.Value, cancellationToken);
            await _blobManager.DeleteAsync(oldBlob.Info, cancellationToken);
        }

        var data = await photograph.OpenReadStream().GetAllBytesAsync(cancellationToken);
        var updatedBlob = await _blobManager.SaveAsync(
            user.Id,
            photograph.FileName,
            data,
            cancellationToken);

        await _userStore.SetPhotographIdAsync(user, updatedBlob.Id, cancellationToken);
        await _userManager.UpdateAsync(user);
        return TypedResults.NoContent();
    }

    public async Task<Results<NoContent, NotFound>> UpdateCurriculumVitaeAsync(
        ClaimsPrincipal claimsPrincipal,
        IFormFile curriculumVitae,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (user.CurriculumVitaeId is not null)
        {
            var oldBlob = await _blobManager.GetAsync(user.CurriculumVitaeId.Value, cancellationToken);
            await _blobManager.DeleteAsync(oldBlob.Info, cancellationToken);
        }

        var data = await curriculumVitae.OpenReadStream().GetAllBytesAsync(cancellationToken);
        var updatedBlob = await _blobManager.SaveAsync(
            user.Id,
            curriculumVitae.FileName,
            data,
            cancellationToken);

        await _userStore.SetCurriculumVitaeIdAsync(user, updatedBlob.Id, cancellationToken);
        await _userManager.UpdateAsync(user);

        return TypedResults.NoContent();
    }

    private async Task<Stream> SerializeToXml<T>(T item)
        where T:class
    {
        var xsSubmit = new XmlSerializer(typeof(T));
        await using var sww = new StringWriter();
        await using XmlWriter writer = XmlWriter.Create(sww, new XmlWriterSettings() {Async = true});
        xsSubmit.Serialize(writer, item);
        var xml = sww.ToString();
        return await GenerateStreamFromStringAsync(xml);
    }


    private async Task<Stream> SerializeToJson<T>(T info)
    {
        var json = JsonConvert.SerializeObject(info);
        return await GenerateStreamFromStringAsync(json);
    }

    private async Task<Stream> GenerateStreamFromStringAsync(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(s);
        await writer.FlushAsync();
        stream.Position = 0;
        return stream;
    }
}