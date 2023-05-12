using VentLib.Commands;
using VentLib.Commands.Attributes;
using VentLib.Commands.Interfaces;

namespace TOHTOR.Chat.Commands;

[Command(CommandFlag.HostOnly, "test")]
public class TestCommand: ICommandReceiver
{
    public void Receive(PlayerControl source, CommandContext context)
    {
        //Utils.SendMessage("asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[asddddddddddddddddddgsdfggdfpsgfdpgfddpsfgldgpsf[");
        ChatHandler.Of("Test Message").Send(source);
    }
}