using System;
using System.Collections.Generic;
using System.Linq;
using Lotus.Roles;
using System.Reflection;
using Lotus.Logging;
using Lotus.Options;
using Lotus.Roles.Builtins;
using Lotus.Roles.Debugger;
using Lotus.Roles.Internals;
using Lotus.Roles.RoleGroups.Crew;
using Lotus.Roles.RoleGroups.Impostors;
using Lotus.Roles.RoleGroups.Madmates.Roles;
using Lotus.Roles.RoleGroups.Neutral;
using Lotus.Roles.RoleGroups.NeutralKilling;
using Lotus.Roles.RoleGroups.Stock;
using Lotus.Roles.RoleGroups.Undead.Roles;
using Lotus.Roles.RoleGroups.Vanilla;
using Lotus.Roles.Subroles;
using Lotus.Roles.Subroles.Romantics;
using Lotus.Roles.Managers.Interfaces;
using VentLib.Options;
using VentLib.Options.Game;
using VentLib.Utilities.Attributes;
using VentLib.Utilities.Extensions;
using Medium = Lotus.Roles.RoleGroups.Crew.Medium;
using Pirate = Lotus.Roles.RoleGroups.Neutral.Pirate;

namespace Lotus.GameModes.Standard;
public class StandardRoles : RoleHolder
{
    public override List<Action> FinishedCallbacks() => Callbacks;

    public List<CustomRole> MainRoles { get; set; }

    public List<CustomRole> ModifierRoles { get; set; }

    public List<CustomRole> SpecialRoles { get; set; }

    public List<CustomRole> AllRoles { get; set; }

    public List<Action> Callbacks { get; set; } = new List<Action>();

    public StaticRoles Static;
    public Modifiers Mods;
    public ExtraRoles Special;

    public StandardRoles() : base()
    {
        AllRoles = new List<CustomRole>();
        Static = new StaticRoles();
        Mods = new Modifiers();
        Special = new ExtraRoles();

        MainRoles = Static.GetType()
            .GetFields()
            .Select(f => (CustomRole)f.GetValue(Static)!)
            .ToList();

        ModifierRoles = Mods.GetType()
            .GetFields()
            .Select(f => (CustomRole)f.GetValue(Mods)!)
            .ToList();

        SpecialRoles = Special.GetType()
            .GetFields()
            .Select(f => (CustomRole)f.GetValue(Special)!)
            .ToList();

        List<CustomRole> realAllRoleList = MainRoles;
        realAllRoleList.AddRange(ModifierRoles);
        realAllRoleList.AddRange(SpecialRoles);
        realAllRoleList.AddRange(AllRoles);
        AllRoles = realAllRoleList;
        AllRoles.ForEach(r => r.Solidify());

        this.Intialized = true;
    }

    public class StaticRoles
    {
        //Impostors

        public CustomRole LOAD_IMPOSTOR_OPTIONS = new EnforceFunctionOrderingRole(RoleOptions.ImpostorOptions.Start);
        //assassin
        //bomber
        public Assassin Assassin = new Assassin();
        public Blackmailer Blackmailer = new Blackmailer();
        public BountyHunter BountyHunter = new BountyHunter();
        public Camouflager Camouflager = new Camouflager();
        public Consort Consort = new Consort();
        public Creeper Creeper = new Creeper();
        public Disperser Disperser = new Disperser();
        public Escapist Escapist = new Escapist();
        public FireWorks FireWorks = new FireWorks();
        public Freezer Freezer = new Freezer();
        public Grenadier Grenadier = new Grenadier();
        public IdentityThief IdentityThief = new IdentityThief();
        public Impostor Impostor = new Impostor();
        public Janitor Janitor = new Janitor();
        public Mafioso Mafioso = new Mafioso();
        public Mare Mare = new Mare();
        public Mastermind Mastermind = new Mastermind();
        public Miner Miner = new Miner();
        public Morphling Morphling = new Morphling();
        public Ninja Ninja = new Ninja();
        public PickPocket PickPocket = new PickPocket();
        public Puppeteer Puppeteer = new Puppeteer();
        //sidekick madmate
        //silencer
        public SerialKiller SerialKiller = new SerialKiller();
        public Sniper Sniper = new Sniper();
        public Swooper Swooper = new Swooper();
        public TimeThief TimeThief = new TimeThief();
        //traitor
        public Vampire Vampire = new Vampire();
        public Warlock Warlock = new Warlock();
        public Witch Witch = new Witch();
        public YinYanger YinYanger = new YinYanger();

        public CustomRole MADMATE_TITLE = new EnforceFunctionOrderingRole(RoleOptions.MadmateOptions.Start);

        public CrewPostor CrewPostor = new CrewPostor();
        public Madmate Madmate = new Madmate();
        public MadGuardian MadGuardian = new MadGuardian();
        public MadSnitch MadSnitch = new MadSnitch();
        public Parasite Parasite = new Parasite();

        //Crewmates
        public CustomRole LOAD_CREW_OPTIONS = new EnforceFunctionOrderingRole(RoleOptions.CrewmateOptions.Start);

        public Alchemist Alchemist = new Alchemist();
        public Bastion Bastion = new Bastion();
        public Bodyguard Bodyguard = new Bodyguard();
        public Chameleon Chameleon = new Chameleon();
        public Charmer Charmer = new Charmer();
        public Crewmate Crewmate = new Crewmate();
        public Crusader Crusader = new Crusader();
        public Demolitionist Demolitionist = new Demolitionist();
        public Dictator Dictator = new Dictator();
        public Doctor Doctor = new Doctor();

        public Escort Escort = new Escort();
        public ExConvict ExConvict = new ExConvict();
        public Herbalist Herbalist = new Herbalist();
        public Investigator Investigator = new Investigator();
        public Mayor Mayor = new Mayor();
        public Mechanic Mechanic = new Mechanic();
        public Medic Medic = new Medic();
        public Medium Medium = new Roles.RoleGroups.Crew.Medium();
        public Mystic Mystic = new Mystic();
        public Observer Observer = new Observer();
        public Oracle Oracle = new Oracle();
        public Physicist Physicist = new Physicist();
        public Psychic Psychic = new Psychic();
        public Repairman Repairman = new Repairman();
        public Sheriff Sheriff = new Sheriff();
        public Snitch Snitch = new Snitch();
        public Speedrunner Speedrunner = new Speedrunner();
        public Swapper Swapper = new Swapper();
        public Roles.RoleGroups.Crew.Tracker Tracker = new Roles.RoleGroups.Crew.Tracker();
        public Transporter Transporter = new Transporter();
        public Trapster Trapster = new Trapster();
        public Veteran Veteran = new Veteran();
        public Vigilante Vigilante = new Vigilante();


        //Neutrals

        // ReSharper disable once InconsistentNaming
        public CustomRole LOAD_NEUTRAL_OPTIONS = new EnforceFunctionOrderingRole(RoleOptions.NeutralOptions.Start);

        public CustomRole NEUTRAL_KILLING_TITLE = new EnforceFunctionOrderingRole(() => new GameOptionTitleBuilder().Title("<size=2.3>★ Neutral Killing ★</size>").Color(ModConstants.Palette.KillingColor).Tab(DefaultTabs.NeutralTab).Build());

        public AgiTater AgiTater = new AgiTater();
        public Arsonist Arsonist = new Arsonist();
        public BloodKnight BloodKnight = new BloodKnight();
        public Demon Demon = new Demon();
        public Egoist Egoist = new Egoist();
        public Hitman Hitman = new Hitman();
        public Jackal Jackal = new Jackal();
        public Juggernaut Juggernaut = new Juggernaut();
        public Marksman Marksman = new Marksman();
        public Necromancer Necromancer = new Necromancer();
        public Occultist Occultist = new Occultist();
        public Pelican Pelican = new Pelican();
        public PlagueBearer PlagueBearer = new PlagueBearer();
        public Retributionist Retributionist = new Retributionist();
        public Glitch Glitch = new Glitch();
        public Werewolf Werewolf = new Werewolf();
        public CustomRole NEUTRAL_PASSIVE_TITLE = new EnforceFunctionOrderingRole(() => new GameOptionTitleBuilder().Title("<size=2.3>❀ Neutral Passive ❀</size>").Color(ModConstants.Palette.PassiveColor).Tab(DefaultTabs.NeutralTab).Build());

        public Amnesiac Amnesiac = new Amnesiac();
        public Archangel Archangel = new Archangel();
        public Copycat Copycat = new Copycat();
        public Executioner Executioner = new Executioner();
        public Hacker Hacker = new Hacker();
        public Jester Jester = new Jester();
        public Opportunist Opportunist = new Opportunist();
        public Roles.RoleGroups.Neutral.Phantom Phantom = new Roles.RoleGroups.Neutral.Phantom();
        public Pirate Pirate = new Pirate();
        public Postman Postman = new Postman();
        public SchrodingersCat SchrodingersCat = new SchrodingersCat();
        public Survivor Survivor = new Survivor();
        public Terrorist Terrorist = new Terrorist();
        public Vulture Vulture = new Vulture();

        public Guesser Guesser = new Guesser();
        public CustomRole LOAD_MODIFIER_OPTIONS = new EnforceFunctionOrderingRole(RoleOptions.SubroleOptions.Start);
    }

    public class Modifiers
    {
        public Bait Bait = new Bait();
        public Bewilder Bewilder = new Bewilder();
        public Bloodlust Bloodlust = new Bloodlust();
        public Deadly Deadly = new Deadly();
        public Diseased Diseased = new Diseased();
        public Flash Flash = new Flash();
        public Honed Honed = new Honed();
        public Nimble Nimble = new Nimble();
        public Oblivious Oblivious = new Oblivious();
        public Romantic Romantic = new Romantic();
        public Sleuth Sleuth = new Sleuth();
        public TieBreaker TieBreaker = new TieBreaker();
        public Torch Torch = new Torch();
        public Unstoppable Unstoppable = new Unstoppable();
        public Watcher Watcher = new Watcher();
        public Workhorse Workhorse = new Workhorse();
    }

    public class ExtraRoles
    {
        public IllegalRole IllegalRole = new IllegalRole();
        public GameMaster GM = new GameMaster();
        // public Debugger Debugger = new Debugger();

        // public CrewGuesser CrewGuesser = new CrewGuesser();
        // public ImpGuesser ImpGuesser = new ImpGuesser();
        // public NeutralKillerGuesser NeutralKillerGuesser = new NeutralKillerGuesser();
        // public NeutralGuesser NeutralGuesser = new NeutralGuesser();

        //double shot
        //flash
        //oblivious
        //obvious
        //sleuth
        //torch
        //watcher
    }
}

