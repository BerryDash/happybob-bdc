using UnityEngine.Scripting;
using Newtonsoft.Json;
using System.Numerics;

[Preserve]
public class ChatroomMessage {
  [Preserve]
  [JsonProperty("username")]
  public string Username { get; set; }

  [Preserve]
  [JsonProperty("userid")]
  public BigInteger UserID { get; set; }

  [Preserve]
  [JsonProperty("content")]
  public string Content { get; set; }

  [Preserve]
  [JsonProperty("id")]
  public BigInteger ID { get; set; }

  [Preserve]
  [JsonProperty("icon")]
  public int Icon { get; set; }

  [Preserve]
  [JsonProperty("overlay")]
  public int Overlay { get; set; }

  [Preserve]
  [JsonProperty("birdColor")]
  public int[] BirdColor { get; set; }

  [Preserve]
  [JsonProperty("overlayColor")]
  public int[] OverlayColor { get; set; }

  [Preserve]
  [JsonProperty("deleted")]
  public bool Deleted { get; set; }

  [Preserve]
  [JsonProperty("customIcon")]
  public string CustomIcon { get; set; }
}