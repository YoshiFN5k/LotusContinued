namespace Lotus.Extensions;

public static class PlayerInfoExtensions
{
    public static string ColoredName(this NetworkedPlayerInfo playerInfo)
    {
        return playerInfo == null! ? "Unknown" : playerInfo.ColorName.Trim('(', ')');
    }
}