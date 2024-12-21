using System;
using Pusula.Training.HealthCare.Shared;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolDownloadTokenCacheItem : BaseTokenCacheItem
{
    public string Token { get; set; } = null!;
}