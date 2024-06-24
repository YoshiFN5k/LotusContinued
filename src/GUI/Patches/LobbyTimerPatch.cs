using HarmonyLib;
using UnityEngine;
using VentLib.Utilities;

namespace Lotus.GUI.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
public class LobbyTimerPatch
{
    static void Postfix(GameStartManager __instance)
    {
        if (!AmongUsClient.Instance.AmHost || !GameData.Instance || AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame) return; // Not host or no instance or LocalGame
        HudManager.Instance.ShowLobbyTimer(600);
        HudManager.Instance.LobbyTimerExtensionUI.timerText.transform.parent.transform.Find("LabelBackground").gameObject.SetActive(false);
        HudManager.Instance.LobbyTimerExtensionUI.timerText.transform.parent.transform.Find("Icon").gameObject.SetActive(false);
    }
}

[HarmonyPatch(typeof(TimerTextTMP), nameof(TimerTextTMP.UpdateText))]
public class TimerTextStringPatch
{
    static bool Prefix(TimerTextTMP __instance)
    {
        if (__instance.name != "WarningText") return true;
        int timer = __instance.GetSecondsRemaining();
        int minutes = (int)timer / 60;
        int seconds = (int)timer % 60;
        string suffix = $" {minutes:00}:{seconds:00}";
        if (timer <= 60) suffix = Color.red.Colorize(suffix);
        __instance.text.text = string.Format(__instance.format, suffix);
        return false;
    }
}