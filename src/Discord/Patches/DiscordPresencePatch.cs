using HarmonyLib;
using Discord;

namespace Lotus.Discord.Patches;

[HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
public class DiscordPatch
{
    public static void Prefix(ref Activity activity) => activity.Details += " Project Lotus " + (ProjectLotus.DevVersion ? ProjectLotus.DevVersionStr : ProjectLotus.VisibleVersion);
}