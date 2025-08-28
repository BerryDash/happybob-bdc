using System;
using System.Numerics;
using Newtonsoft.Json;

public class MarketplaceIconStorageType {
  [JsonProperty("selected")]
  public string Selected { get; set; } = null;
  [JsonProperty("balance")]
  public BigInteger Balance { get; set; } = 0;
  [JsonProperty("data")]
  public MarketplaceIconType[] Data { get; set; } = Array.Empty<MarketplaceIconType>();
}