using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TOHTOR.API.Vanilla.Meetings;
using TOHTOR.Extensions;
using TOHTOR.Utilities;
using UnityEngine;
using VentLib.Logging;
using VentLib.Networking.RPC;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Harmony.Attributes;

namespace TOHTOR.Chat.Patches;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public class ChatUpdatePatch
{
    public static bool DoBlockChat = false;
    public static Queue<(string, byte, string, bool)> MessagesToSend = new();
    
    public static void Postfix(ChatController __instance)
    {
        __instance.TextArea.AllowPaste = true;
        __instance.chatBubPool.Prefab.Cast<ChatBubble>().TextArea.overrideColorTags = false;
        __instance.TimeSinceLastMessage = 3f;
        if (!AmongUsClient.Instance.AmHost) return;
        if (DateTime.Now.Subtract(MeetingPrep.MeetingCalledTime).TotalSeconds < NetUtils.DeriveDelay(3.5f)) return;
        /*if (!AmongUsClient.Instance.AmHost || TOHPlugin.MessagesToSend.Count < 1 || (TOHPlugin.MessagesToSend[0].Item2 == byte.MaxValue && TOHPlugin.MessageWait.Value > __instance.TimeSinceLastMessage)) return;*/
        if (MessagesToSend.Count == 0) return;
        if (DoBlockChat) return;
        var player = PlayerControl.AllPlayerControls.ToArray().OrderBy(x => x.PlayerId).FirstOrDefault(x => !x.Data.IsDead);
        if (player == null) return;
        (string msg, byte sendTo, string title, bool leftAlign) = MessagesToSend.Dequeue();
        if (leftAlign) ChatBubblePatch.SetLeftQueue.Enqueue(0);
        int clientId = sendTo == byte.MaxValue ? -1 : Utils.GetPlayerById(sendTo).GetClientId();
        var name = player.name;

        if (clientId == -1)
        {
            player.SetName(title);
            OnChatPatch.UtilsSentList.Add(player.PlayerId);
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, msg);
            player.SetName(name);
        }
        else if (clientId == PlayerControl.LocalPlayer.GetClientId()) OnChatPatch.UtilsSentList.Add(player.PlayerId);

        PlayerControl.AllPlayerControls.ToArray().Where(p => clientId == -1 || p.GetClientId() == clientId).ForEach(p =>
        {
            if (p.AmOwner && clientId == -1) return;

            string message = p.IsModded() ? msg : msg.RemoveHtmlTags();

            RpcV3.Mass()
                .Start(player.NetId, RpcCalls.SetName)
                    .Write(title)
                    .End()
                .Start(player.NetId, RpcCalls.SendChat)
                    .Write(message)
                    .End()
                .Start(player.NetId, RpcCalls.SetName)
                    .Write(name)
                    .End()
                .Send(p.GetClientId());
        });

        __instance.TimeSinceLastMessage = 0f;
    }


    [QuickPostfix(typeof(ChatController), nameof(ChatController.UpdateCharCount))]
    public static void UpdateCharCount(ChatController __instance)
    {
        int length = __instance.TextArea.text.Length;
        __instance.CharCount.text = $"{length}/{__instance.TextArea.characterLimit}";
        if (length < (AmongUsClient.Instance.AmHost ? 1750 : 250))
            __instance.CharCount.color = Color.black;
        else if (length < (AmongUsClient.Instance.AmHost ? 2000 : 300))
            __instance.CharCount.color = new Color(1f, 1f, 0.0f, 1f);
        else
            __instance.CharCount.color = Color.red;
    }
}