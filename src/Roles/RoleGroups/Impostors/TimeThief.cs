using HarmonyLib;
using TOHTOR.API;
using TOHTOR.API.Odyssey;
using TOHTOR.Extensions;
using TOHTOR.Roles.Internals;
using TOHTOR.Roles.Internals.Attributes;
using TOHTOR.Roles.Overrides;
using UnityEngine;
using VentLib.Logging;
using VentLib.Options.Game;

namespace TOHTOR.Roles.RoleGroups.Impostors;

public class TimeThief : Vanilla.Impostor
{
    private int kills;
    private int meetingTimeSubtractor;
    private int minimumVotingTime;
    private bool returnTimeAfterDeath;

    [RoleAction(RoleActionType.Attack)]
    public override bool TryKill(PlayerControl target)
    {
        var flag = base.TryKill(target);
        if (flag)
            kills++;
        return flag;
    }

    [RoleAction(RoleActionType.RoundEnd)]
    private void TimeThiefSubtractMeetingTime()
    {
        if (!MyPlayer.IsAlive() && returnTimeAfterDeath) return;
        int discussionTime = AUSettings.DiscussionTime();
        int votingTime = AUSettings.VotingTime();

        int totalStolenTime = meetingTimeSubtractor * kills;

        // Total Meeting Time - Stolen Time = Remaining Meeting Time
        int modifiedDiscussionTime = discussionTime - totalStolenTime;

        if (modifiedDiscussionTime < 0)
        {
            totalStolenTime = -modifiedDiscussionTime;
            modifiedDiscussionTime = 1;
        }

        int modifiedVotingTime = Mathf.Clamp(votingTime - totalStolenTime, minimumVotingTime, votingTime);

        VentLogger.Debug($"{MyPlayer.name} | Time Thief | Meeting Time: {modifiedDiscussionTime} | Voting Time: {modifiedVotingTime}", "TimeThiefStolen");
        GameOptionOverride[] overrides = { new(Override.DiscussionTime, modifiedDiscussionTime), new(Override.VotingTime, modifiedVotingTime) };
        Game.GetAllPlayers().Do(p => p.GetCustomRole().SyncOptions(overrides));
    }

    protected override GameOptionBuilder RegisterOptions(GameOptionBuilder optionStream) =>
        base.RegisterOptions(optionStream)
            .SubOption(sub => sub
                .Name("Meeting Time Stolen")
                .Bind(v => meetingTimeSubtractor = (int)v)
                .AddIntRange(5, 120, 5, 4, "s")
                .Build())
            .SubOption(sub => sub
                .Name("Minimum Voting Time")
                .Bind(v => minimumVotingTime = (int)v)
                .AddIntRange(5, 120, 5, 1, "s")
                .Build())
            .SubOption(sub => sub
                .Name("Return Stolen Time After Death")
                .Bind(v => returnTimeAfterDeath = (bool)v)
                .AddOnOffValues()
                .Build());
}