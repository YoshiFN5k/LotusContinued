using HarmonyLib;
using UnityEngine;

namespace Lotus.Chat.Patches;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatControllerUpdatePatch
{
    public static void Postfix(ChatController __instance)
    {
        if (!__instance.freeChatField.textArea.hasFocus) return;
        __instance.freeChatField.textArea.AllowPaste = true;
        __instance.freeChatField.textArea.AllowSymbols = true;
        __instance.freeChatField.textArea.AllowEmail = true;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendFreeChat))]
public static class SendFreeChatPatch
{
    public static bool Prefix(ChatController __instance)
    {
        string text = __instance.freeChatField.Text;
        ChatController.Logger.Debug("SendFreeChat () :: Sending message: '" + text + "'", null);
        PlayerControl.LocalPlayer.RpcSendChat(text);
        return false;
    }
}
