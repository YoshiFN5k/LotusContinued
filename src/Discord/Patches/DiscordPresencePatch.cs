using HarmonyLib;
using Discord;

namespace Lotus.Discord.Patches;

[HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
public class DiscordPatch
{
    private static string DiscordMessage = "Project Lotus " + (ProjectLotus.DevVersion ? ProjectLotus.DevVersionStr : "v" + ProjectLotus.VisibleVersion);
    public static void Prefix(ref Activity activity)
    {
        if (activity.Assets == null)
        {
            activity.Assets = new ActivityAssets();
        }
        activity.Assets.SmallImage = activity.Assets.LargeImage;
        activity.Assets.LargeImage = "https://github.com/Lotus-AU/LotusContinued/blob/main/Images/lotus_image2.png";
        activity.Assets.SmallText = "Among Us";
        activity.Assets.LargeText = "Project Lotus";
        if (activity.Details == "" || activity.Details == null) activity.Details = DiscordMessage;
        else activity.Details += $" ({DiscordMessage})";
    }
}