using System.Text;
using TMPro;
using UnityEngine;

public class StatsMenu : MonoBehaviour
{
    public TMP_Text statText;

    void Start()
    {
        var text = new StringBuilder();
        text.AppendLine("High Score: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreHighScore()));
        text.AppendLine("Total Normal Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalNormalBerries()));
        text.AppendLine("Total Poison Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalPoisonBerries()));
        text.AppendLine("Total Slow Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalSlowBerries()));
        text.AppendLine("Total Ultra Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalUltraBerries()));
        text.AppendLine("Total Speedy Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalSpeedyBerries()));
        text.AppendLine("Total Time Slow Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalTimeSlowBerries()));
        text.AppendLine("Total Purple Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalPurpleBerries()));
        text.AppendLine("Total Time Freeze Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalTimeFreezeBerries()));
        text.AppendLine("Total Evil Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalEvilBerries()));
        text.AppendLine("Total Attempts: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalAttepts()));
        statText.text = text.ToString();
    }
}
