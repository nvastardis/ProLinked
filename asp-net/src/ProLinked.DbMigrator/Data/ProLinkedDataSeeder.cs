using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProLinked.Domain;
using ProLinked.Domain.Entities;
using ProLinked.Domain.Entities.Identity;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ProLinked.DbMigrator.Data;

public class ProLinkedDataSeeder
{

    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public ProLinkedDataSeeder(
        IServiceProvider serviceProvider,
        ILogger<ProLinkedDataSeeder> logger,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
        _serviceProvider = serviceProvider;
    }

    public async Task SeedEntitiesIfNewAsync<TSerializableEntity>(
        string entityName,
        string jsonFileName,
        Func<TSerializableEntity, Expression<Func<TSerializableEntity,bool>>> searchPredicate)
        where TSerializableEntity: Entity
    {
        var jsonFileNameClean =
            Path.GetFileNameWithoutExtension(jsonFileName).Replace('.', Path.DirectorySeparatorChar);
        var repository = _serviceProvider.GetRequiredService<IRepository<TSerializableEntity>>();
        var seedObjects = await LoadDataAsync<TSerializableEntity>(entityName, jsonFileNameClean+".json");
        foreach (var seedObj in seedObjects)
        {
            if (await repository.FindAsync(searchPredicate(seedObj), false) is null)
            {
                await repository.InsertAsync(seedObj, autoSave:true);
            }
        }
        _logger.LogInformation($"Succeeded Seeding Entity: {entityName}");
    }

    public async Task SeedUsersIfNewAsync(
        string entityName,
        string jsonFileName)
    {
        var jsonFileNameClean =
            Path.GetFileNameWithoutExtension(jsonFileName).Replace('.', Path.DirectorySeparatorChar);
        var seedObjects = await LoadDataAsync<AppUser>(entityName, jsonFileNameClean + ".json");
        foreach (var seedObj in seedObjects)
        {
            if (await _userManager.FindByIdAsync(seedObj.Id.ToString()) is null)
            {
                await _userManager.CreateAsync(seedObj);
                await _userManager.AddPasswordAsync(seedObj, "1q2w3E*");
            }
        }
        _logger.LogInformation($"Succeeded Seeding Entity: {entityName}");
    }

    public async Task SeedRolesIfNewAsync(
        string entityName,
        string jsonFileName)
    {
        var jsonFileNameClean =
            Path.GetFileNameWithoutExtension(jsonFileName).Replace('.', Path.DirectorySeparatorChar);
        var seedObjects = await LoadDataAsync<IdentityRole<Guid>>(entityName, jsonFileNameClean + ".json");
        foreach (var seedObj in seedObjects)
        {
            if (await _roleManager.FindByIdAsync(seedObj.Id.ToString()) is null)
            {
                await _roleManager.CreateAsync(seedObj);
            }
        }
        _logger.LogInformation($"Succeeded Seeding Entity: {entityName}");
    }

    private async Task<List<T>> LoadDataAsync<T>(
        string entityName,
        string jsonFileName)
    {
        var dataAsJson = await File.ReadAllTextAsync(jsonFileName);
        var seedObjects = JsonConvert.DeserializeObject<List<T>>(dataAsJson) ??
                          throw new SerializationException($"Unable to Deserialize JSON line to {entityName}");
        return seedObjects;
    }

}