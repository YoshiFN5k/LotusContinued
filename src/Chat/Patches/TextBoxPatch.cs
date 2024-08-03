using HarmonyLib;
using UnityEngine;
using VentLib.Utilities.Harmony.Attributes;

namespace Lotus.Chat.Patches;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public class TextBoxPatch
{
    public static void Postfix(TextBoxTMP __instance, char i, ref bool __result)
    {
        __result = __result || (i >= 31 && i <= 126);
    }

    [QuickPrefix(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
    public static void ModifyCharacterLimit(TextBoxTMP __instance) => __instance.characterLimit = AmongUsClient.Instance.AmHost ? 2000 : 300;
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class IsCharAllowedPatch
{
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        __result = !(i == '\b');    // Bugfix: '\b' messing with chat message
        return false;
    }

    public static void Postfix(TextBoxTMP __instance)
    {
        __instance.allowAllCharacters = true; // not used by game's code, but I include it anyway
        __instance.AllowEmail = true;
        __instance.AllowPaste = true;
        __instance.AllowSymbols = true;
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
public static class TextBoxTMPUpdatePatch
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!__instance.hasFocus) { return; }

        // If player presses Ctrl + C, copy the text from the chatbox to the clipboard
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
        {
            ClipboardHelper.PutClipboardString(__instance.text);
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
        {
            __instance.text += ClipboardHelper.GetClipboardString().ToString();
        }
    }
}