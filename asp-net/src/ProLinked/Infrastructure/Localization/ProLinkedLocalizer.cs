using System.Globalization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace ProLinked.Infrastructure.Localization;

public class ProLinkedLocalizer: IStringLocalizer
{
    private List<LocalizationResource> LocalizationList { get; init; } = new();

    public ProLinkedLocalizer(string[]? filePaths = null)
    {
        var paths = filePaths ?? ["Infrastructure\\Localization\\ProLinked\\en.json"];
        foreach (var filePath in paths)
        {
            var jsonAsString = File.ReadAllText(filePath);
            var localizationFile = JsonConvert.DeserializeObject<LocalizationResource>(jsonAsString);
            if (localizationFile is null)
            {
                continue;
            }

            LocalizationList.Add(localizationFile);
        }
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = GetString(name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value, format == null);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var localizationResource = LocalizationList.FirstOrDefault(
            _ =>
                LocalizationList.Any(lv => lv.Culture == CultureInfo.CurrentCulture.Name)
        );
        if (localizationResource is null)
        {
            return new List<LocalizedString>();
        }

        return
            localizationResource.
                Dictionary.
                Select(
                    e => new LocalizedString(
                    name:e.Key,
                    value:e.Value,
                    resourceNotFound:true
                    )
                );
    }

    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        return new ProLinkedLocalizer();
    }

    private string? GetString(string name)
    {
        var resource = LocalizationList.
            FirstOrDefault(l => l.Culture == CultureInfo.CurrentCulture.Name);
        if (resource?.Dictionary is null || !resource.Dictionary.TryGetValue(name, out var result))
        {
            return null;
        }

        return result;
    }
}
