using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lotus.Utilities;
using VentLib.Commands.Attributes;

namespace Lotus.Chat.Commands.Other;

// These are commands that are in other mods, so people expect them here.
// BUT WE DONT HAVE THEM.
// So as a result, we just tell them it is not a command in Project Lotus.
public class EmptyCommands : CommandTranslations
{
    [Command(VentLib.Commands.CommandFlag.InGameOnly, "kc", "kcount", "killercount")]
    public static void KillerCount(PlayerControl source)
    {
        ChatHandlers.InvalidCmdUsage("/kc is not a command in Project Lotus. You can ask the host to add the alive killer count into their meeting template if you really want it.").Send(source);
    }
}