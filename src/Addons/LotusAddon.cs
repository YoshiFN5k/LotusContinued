using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lotus.Factions.Interfaces;
using Lotus.Roles;
using Lotus.Extensions;
using Lotus.GameModes;
using Lotus.GameModes.Standard;
using VentLib.Utilities.Extensions;
using Version = VentLib.Version.Version;

namespace Lotus.Addons;

public abstract class LotusAddon
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LotusAddon));

    internal Dictionary<AbstractBaseRole, HashSet<IGameMode>> ExportedDefinitions { get; } = new();

    internal readonly List<IFaction> Factions = new();
    internal readonly List<IGameMode> Gamemodes = new();

    internal readonly Assembly BundledAssembly = Assembly.GetCallingAssembly();
    internal readonly ulong UUID;

    public abstract string Name { get; }
    public abstract Version Version { get; }

    public LotusAddon()
    {
        UUID = (ulong)HashCode.Combine(BundledAssembly.GetIdentity(false)?.SemiConsistentHash() ?? 0ul, Name.SemiConsistentHash());
    }

    /// <summary>
    /// Returns the name of this addon.
    /// </summary>
    /// <param name="fullName">Whether or not to return te name with the Assembly and version.</param>
    /// <returns>Name of Addon (string)</returns>
    internal string GetName(bool fullName = false) => !fullName
        ? Name
        : $"{BundledAssembly.FullName}::{Name}-{Version.ToSimpleName()}";

    /// <summary>
    /// First function called after loading the plugin. Your plugin code should go here.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// This runs after <b>all</b> plugins were loaded. You can disable stuff if your addon conflicts with another one.
    /// </summary>
    /// <param name="addons">List of all Addons that were loaded.</param>
    public virtual void PostInitialize(List<LotusAddon> addons)
    {
    }

    /// <summary>
    /// Whether or not to disable rpcs if the host/client do not have this addon. If this is set to false, the host/client will essentially not even know that you have this addon.
    /// </summary>
    /// <returns>Boolean</returns>
    public virtual bool DisableRPC() => true;
    // You might say that this is kind of stupid idea, but there are a lot of other ways to achieve this without even using an addon.

    /// <summary>
    /// Export your custom roles.
    /// </summary>
    /// <param name="roleDefinitions">List of roles to export.</param>
    /// <param name="baseGameModes">The gamemodes to export these roles in. Make sure they have been registered first.</param>
    public void ExportCustomRoles(IEnumerable<CustomRole> roleDefinitions, params Type[] baseGameModes)
    {
        if (baseGameModes.Length == 0) ExportCustomRoles(roleDefinitions, StandardGameMode.Instance);
        else ExportCustomRoles(roleDefinitions, baseGameModes.Select(gm => ProjectLotus.GameModeManager.GetGameMode(gm) ?? StandardGameMode.Instance).ToArray());
    }

    public void ExportCustomRoles(IEnumerable<CustomRole> roleDefinitions, params IGameMode[] baseGameModes)
    {
        IGameMode[] targetGameModes = ProjectLotus.GameModeManager.GameModes.Where(gm => baseGameModes.Any(bgm => bgm.GetType().IsInstanceOfType(gm))).ToArray();
        roleDefinitions.ForEach(r =>
        {
            r.Addon = this;
            HashSet<IGameMode> iGameMode = ExportedDefinitions.GetOrCompute(r, () => new HashSet<IGameMode>());
            targetGameModes.All(x => iGameMode.Add(x));
        });
    }

    /// <summary>
    /// Export your custom gamemodes.
    /// </summary>
    /// <param name="gamemodes">List of gamemodes to export.</param>
    public void ExportGameModes(IEnumerable<IGameMode> gamemodes)
    {
        foreach (IGameMode gamemode in gamemodes)
        {
            log.Trace($"Exporting GameMode: {gamemode.Name}", "ExportGameModes");
            // ProjectLotus.GameModeManager.AddGamemodeSettingToOptions(gamemode.MainTab().GetOptions());
            ProjectLotus.GameModeManager.GameModes.Add(gamemode);
        }
    }

    public void ExportGameModes(params IGameMode[] gamemodes) => ExportGameModes((IEnumerable<IGameMode>)gamemodes);

    public override string ToString() => GetName(true);
}

