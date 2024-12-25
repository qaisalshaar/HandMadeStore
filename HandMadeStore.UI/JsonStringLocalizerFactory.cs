using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace HandMadeStore.UI
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDistributedCache _cash;

        public JsonStringLocalizerFactory(IDistributedCache cash)
        {
            _cash = cash;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_cash);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_cash);
        }
    }
}
