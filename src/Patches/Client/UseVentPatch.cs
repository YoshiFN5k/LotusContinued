using HarmonyLib;
using Lotus.Roles;
using Lotus.Extensions;

namespace Lotus.Patches.Client;

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
class UseVentPatch
{
    public static bool Prefix(Vent __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] ref bool canUse, [HarmonyArgument(1)] ref bool couldUse)
    {
        if (pc.Object == null) return true;
        CustomRole role = pc.Object.PrimaryRole();

        if (role.CanVent()) return couldUse = true;
        return canUse = couldUse = false;
    }
}