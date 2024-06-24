using Lotus.Options.Client;
using VentLib.Utilities.Attributes;

namespace Lotus.Options;

[LoadStatic]
public class ClientOptions
{
    public static SoundOptions SoundOptions = new SoundOptions();

    public static VideoOptions VideoOptions = new VideoOptions();

    public static AdvancedOptions AdvancedOptions = new AdvancedOptions();


}