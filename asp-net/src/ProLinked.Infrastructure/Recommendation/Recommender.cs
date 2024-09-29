using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Recommendations;

namespace ProLinked.Infrastructure.Recommendation;

public class Recommender
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IResumeRepository _resumeRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly IPostRepository _postRepository;
    private readonly IRepository<JobRecommendation> _jobRecommendationRepository;
    private readonly IRepository<PostRecommendation> _postRecommendationRepository;
    private readonly IAdvertisementRepository _advertisementRepository;

    public Recommender(
        UserManager<AppUser> userManager,
        IPostRepository postRepository,
        IAdvertisementRepository advertisementRepository,
        IConnectionRepository connectionRepository,
        IRepository<PostRecommendation> postRecommendationRepository,
        IRepository<JobRecommendation> jobRecommendationRepository,
        IResumeRepository resumeRepository)
    {
        _postRepository = postRepository;
        _advertisementRepository = advertisementRepository;
        _connectionRepository = connectionRepository;
        _postRecommendationRepository = postRecommendationRepository;
        _jobRecommendationRepository = jobRecommendationRepository;
        _resumeRepository = resumeRepository;
        _userManager = userManager;
    }

    public async Task SetPostRecommendationsAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.ToArrayAsync(cancellationToken);
        var posts = await (await _postRepository.WithDetailsAsync(cancellationToken)).ToArrayAsync(cancellationToken);

        if (!users.Any() || !posts.Any())
        {
            return;
        }

        var userCount = users.Length;
        var postCount = posts.Length;

        // Reaction -> 3 point
        // Comment -> 2 points
        // Connection Reaction/Comment -> 1 points
        // Max -> 5
        // Default -> -1
        var matrix = new double[userCount, postCount];
        for (var i = 0; i < userCount; i++)
        {
            for (var j = 0; j < postCount; j++)
            {
                matrix[i, j] = -1;
                if (posts[j].Reactions.Any(e => e.CreatorId == users[i].Id))
                {
                    matrix[i, j] += 3;
                }

                if (posts[j].Comments.Any(e => e.CreatorId == users[i].Id))
                {
                    matrix[i, j] += 2;
                }

                var connections = await _connectionRepository.GetListByUserAsync(
                    users[i].Id, cancellationToken: cancellationToken);

                if (posts[j].Reactions.Any(e => connections.Any(c => c.UserId == e.CreatorId)))
                {
                    matrix[i, j] += 1;
                }

                if (Math.Abs(matrix[i, j] - 5.0) < 0.00001)
                {
                    continue;
                }

                if (posts[j].Comments.Any(e => connections.Any(c => c.UserId == e.CreatorId)))
                {
                    matrix[i, j] += 1;
                }
            }
        }

        var recommendationMatrix = Algorithms.CalculateMatrixFactorization(
            matrix,
            2,
            5000,
            0.0005,
            0.0);

        for (var i = 0; i < userCount; i++)
        {
            var userId = users[i].Id;
            var pairs = new PairWithValue[postCount];
            for (var j = 0; j < postCount; j++)
            {
                pairs[j] = new PairWithValue()
                {
                    UserId = userId,
                    ItemId = posts[j].Id,
                    Value = recommendationMatrix[i,j]
                };
            }

            var orderedPairs=
                pairs.
                    OrderByDescending(e => e.Value).
                    Select(e => new PostRecommendation(e.UserId, e.ItemId));
            await _postRecommendationRepository.InsertManyAsync(
                orderedPairs,
                autoSave: true,
                cancellationToken);
        }
    }

    public async Task SetJobRecommendationsAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.ToArrayAsync(cancellationToken);
        var jobs = await (await _advertisementRepository.WithDetailsAsync(cancellationToken)).ToArrayAsync(cancellationToken);

        if (!users.Any() || !jobs.Any())
        {
            return;
        }

        var userCount = users.Length;
        var jobCount = jobs.Length;

        // Skill -> 1 point
        // "Friend" -> 1 points
        // Max -> 5
        // Default -> -1
        var matrix = new double[userCount, jobCount];
        for (var i = 0; i < userCount; i++)
        {
            for (var j = 0; j < jobCount; j++)
            {
                matrix[i, j] = -1;
                var resume = await _resumeRepository.FindAsync(
                    e => e.UserId == users[i].Id,
                    true,
                    cancellationToken);
                if (resume is not null)
                {
                    var resumeSkills = await _resumeRepository.GetListResumeSkillAsync(
                        resume.Id,
                        cancellationToken);

                    matrix[i, j] += resumeSkills.Count(e =>
                        jobs[j].Description.Contains(e.Title, StringComparison.InvariantCultureIgnoreCase));
                }

                if (matrix[i, j] >= 5)
                {
                    matrix[i, j] = 5;
                    continue;
                }

                var connections = await _connectionRepository.GetListByUserAsync(
                    users[i].Id,
                    cancellationToken: cancellationToken);

                if (connections.Any(e => e.Id == jobs[j].CreatorId))
                {
                    matrix[i, j] += 1;
                }
            }
        }

        var recommendationMatrix = Algorithms.CalculateMatrixFactorization(
            matrix,
            2,
            5000,
            0.0005,
            0.0);

        for (var i = 0; i < userCount; i++)
        {
            var userId = users[i].Id;
            var pairs = new PairWithValue[jobCount];
            for (var j = 0; j < jobCount; j++)
            {
                pairs[j] = new PairWithValue()
                {
                    UserId = userId,
                    ItemId = jobs[j].Id,
                    Value = recommendationMatrix[i,j]
                };
            }

            var orderedPairs=
                pairs.
                    OrderByDescending(e => e.Value).
                    Select(e => new JobRecommendation(e.UserId, e.ItemId));
            await _jobRecommendationRepository.InsertManyAsync(
                orderedPairs,
                autoSave: true,
                cancellationToken);
        }
    }
}