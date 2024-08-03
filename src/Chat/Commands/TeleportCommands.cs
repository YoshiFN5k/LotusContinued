using System;
using Lotus.Managers;
using Lotus.Options;
using Lotus.Utilities;
using VentLib.Commands;
using VentLib.Commands.Attributes;
using VentLib.Utilities.Extensions;

namespace Lotus.Chat.Commands;

public class TeleportCommands
{
    public static bool IsAllowedToTeleport(PlayerControl source)
    {
        int allowedUsers = GeneralOptions.MiscellaneousOptions.AllowTeleportInLobby;
        bool permitted = allowedUsers switch
        {
            0 => source.IsHost(),
            1 => source.IsHost() || PluginDataManager.FriendManager.IsFriend(source),
            2 => true,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (!permitted)
        {
            ChatHandlers.NotPermitted().Send(source);
        }
        return permitted;
    }
    [Command(CommandFlag.LobbyOnly, "tpout")]
    public static void TeleportOutOfLobby(PlayerControl source)
    {
        if (!IsAllowedToTeleport(source)) return;
        Utils.Teleport(source.NetTransform, new UnityEngine.Vector2(0.1f, 3.8f));
    }

    [Command(CommandFlag.LobbyOnly, "tpin")]
    public static void TeleportIntoLobby(PlayerControl source)
    {
        if (!IsAllowedToTeleport(source)) return;
        Utils.Teleport(source.NetTransform, new UnityEngine.Vector2(-0.2f, 1.3f));
    }
}