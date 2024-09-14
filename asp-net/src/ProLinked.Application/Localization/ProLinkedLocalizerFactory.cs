using Microsoft.Extensions.Localization;

namespace ProLinked.Application.Localization;

public class ProLinkedLocalizerFactory: IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource)
    {
        return new ProLinkedLocalizer();
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new ProLinkedLocalizer();
    }
}
