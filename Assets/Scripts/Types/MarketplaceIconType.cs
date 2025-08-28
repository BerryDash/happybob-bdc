using UnityEngine.Scripting;
using Newtonsoft.Json;
using System.Numerics;

[Preserve]
public class MarketplaceIconType {
  [Preserve]
  [JsonProperty("username")]
  public string CreatorUsername { get; set; }

  [Preserve]
  [JsonProperty("userid")]
  public BigInteger CreatorUserID { get; set; }

  [Preserve]
  [JsonProperty("data")]
  public string Data { get; set; }

  [Preserve]
  [JsonProperty("uuid")]
  public string UUID { get; set; }

  [Preserve]
  [JsonProperty("price")]
  public BigInteger Price { get; set; }

  [Preserve]
  [JsonProperty("name")]
  public string Name { get; set; }
}