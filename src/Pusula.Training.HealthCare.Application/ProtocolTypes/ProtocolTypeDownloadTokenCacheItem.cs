using Pusula.Training.HealthCare.Shared;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypeDownloadTokenCacheItem : BaseTokenCacheItem
{
    public string Token { get; set; } = null!;
}