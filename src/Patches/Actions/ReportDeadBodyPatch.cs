using HarmonyLib;
using Lotus.API.Odyssey;
using Lotus.API.Vanilla.Meetings;
using Lotus.Roles.Internals;
using Lotus.Extensions;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Operations;
using Lotus.Utilities;
using UnityEngine;
using System.Linq;
using VentLib.Utilities.Optionals;

namespace Lotus.Patches.Actions;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
public class ReportDeadBodyPatch
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(ReportDeadBodyPatch));

    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo? target)
    {
        log.Trace($"{__instance.GetNameWithRole()} => {target?.GetNameWithRole() ?? "(Meeting)"}", "ReportDeadBody");
        if (!AmongUsClient.Instance.AmHost) return true;
        if (__instance.Data.IsDead) return false;
        if (Game.State != GameState.Roaming) return false;
        if (AmongUsClient.Instance.IsGameOver) return false;
        if (target == null && Enumerable.Any<PlayerTask>(PlayerControl.LocalPlayer.myTasks.ToArray(), new System.Func<PlayerTask, bool>(PlayerTask.TaskIsEmergency))) // stops meetings with sabotage
        {
            return false;
        }
        // MeetingRoomManager.Instance.AssignSelf(__instance, target);
        if (GameManager.Instance.CheckTaskCompletion())
        {
            return false;
        }

        ActionHandle handle = ActionHandle.NoInit();

        if (target != null)
        {
            if (__instance.PlayerId == target.PlayerId) return false;
            if (Game.MatchData.UnreportableBodies.Contains(target.PlayerId)) return false;
            if (!Object.FindObjectsOfType<DeadBody>().Any(db => db.ParentId == target.PlayerId)) return false; // trying to report a local or non-existent body.

            log.Trace($"Triggering ReportBody with parameters: {__instance}, {handle}, {target}");
            RoleOperations.Current.Trigger(LotusActionType.ReportBody, __instance, handle, Optional<NetworkedPlayerInfo>.NonNull(target));
            if (handle.IsCanceled)
            {
                log.Trace("Not Reporting Body - Cancelled by Any Report Action", "ReportDeadBody");
                return false;
            }
        }
        else
        {
            if (Game.MatchData.UnreportableBodies.Contains(255)) return false; // meeting ID
            log.Trace($"Triggering ReportBody (meeting) with parameters: {__instance}, {handle}");
            RoleOperations.Current.Trigger(LotusActionType.ReportBody, __instance, handle, Optional<NetworkedPlayerInfo>.Null());
            if (handle.IsCanceled)
            {
                log.Trace("Not Calling meeting - Cancelled by Any Report Action", "ReportDeadBody");
                return false;
            }
        }

        MeetingPrep.Reported = target;
        MeetingPrep.PrepMeeting(__instance, target);
        return false;
    }
}
