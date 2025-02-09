using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Lotus.API.Odyssey;
using Lotus.API.Player;
using Lotus.Chat.Commands;
using Lotus.Extensions;
using Lotus.Managers.History;
using Lotus.Managers.History.Events;
using Lotus.Roles;
using Lotus.Roles.Builtins;
using Lotus.Roles.Interfaces;
using Lotus.Victory;
using Lotus.Victory.Conditions;
using UnityEngine;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace Lotus.Patches.Client;

[HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
public class EndGameResultPatch
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(EndGameResultPatch));

    public static void Postfix(EndGameManager __instance)
    {
        var winnerText = Object.Instantiate(__instance.WinText);
        WinDelegate winDelegate = Game.GetWinDelegate();
        string winResult = new Optional<IWinCondition>(winDelegate.WinCondition()).Map(wc =>
        {
            string t;
            if (wc is IFactionWinCondition factionWin)
            {
                t = factionWin.Factions().Select(f => f.Color.Colorize(f.Name())).Distinct().Fuse();
                List<FrozenPlayer> additionalWinners = Game.MatchData.GameHistory.AdditionalWinners;
                if (additionalWinners.Count > 0)
                {
                    string awText = additionalWinners.Select(fp => new List<CustomRole> { fp.MainRole }.Concat(fp.Subroles).MaxBy(r => r.DisplayOrder)).Fuse();
                    t += $" + {awText}";
                }
            }
            else t = Game.MatchData.GameHistory.LastWinners.Select(lw => lw.MainRole.ColoredRoleName()).Fuse();

            if (Game.MatchData.GameHistory.LastWinners.Any()) __instance.BackgroundBar.material.color = Game.MatchData.GameHistory.LastWinners.First().MainRole.RoleColor;

            string? wcText = wc.GetWinReason().ReasonText;
            string reasonText = wcText == null ? "" : $"\n<size=1.5>{LastResultCommand.LRTranslations.WinReasonText.Formatted(wcText)}</size>";
            return $"<size=3>{LastResultCommand.LRTranslations.WinResultText.Formatted(t)}</size>{reasonText}";
        }).OrElse("").TrimStart('\n', '\r');
        {

            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            winnerText.transform.position = pos;
            winnerText.text = Color.white.Colorize(winResult);
        }

        var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));

        GameObject gameLog = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
        gameLog.transform.position = new Vector3(__instance.Navigation.PlayAgainButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
        gameLog.transform.localScale = new Vector3(1f, 1f, 1f);

        TMPro.TMP_Text gameLogTextMesh = gameLog.GetComponent<TMPro.TMP_Text>();
        gameLogTextMesh.alignment = TMPro.TextAlignmentOptions.TopRight;
        gameLogTextMesh.color = Color.white;
        gameLogTextMesh.fontSizeMin = 1.5f;
        gameLogTextMesh.fontSizeMax = 1.5f;
        gameLogTextMesh.fontSize = 1.5f;
        gameLogTextMesh.text = "Event Log:\n";

        var gameLogTextMeshRectTransform = gameLogTextMesh.GetComponent<RectTransform>();
        gameLogTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 7.2f, position.y - 0.1f);

        List<IHistoryEvent> historyEvents = Game.MatchData.GameHistory.Events;
        if (historyEvents.Any()) historyEvents.ForEach(history => gameLogTextMesh.text += history.GenerateMessage() + "\n");
        else gameLogTextMesh.text += "No events for this session.";

        GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
        roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
        roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

        TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
        roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
        roleSummaryTextMesh.color = Color.white;
        roleSummaryTextMesh.fontSizeMin = 1.5f;
        roleSummaryTextMesh.fontSizeMax = 1.5f;
        roleSummaryTextMesh.fontSize = 1.5f;

        var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
        roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);

        List<PlayerHistory>? playerHistory = Game.MatchData.GameHistory.PlayerHistory;
        if (playerHistory == null)
        {
            roleSummaryTextMesh.text = "Could not generate game history.";
            return;
        }
        roleSummaryTextMesh.text = "End Game Result:\n";


        const string indent = "  {0}";

        HashSet<byte> winners = Game.MatchData.GameHistory.LastWinners.Select(p => p.MyPlayer.PlayerId).ToHashSet();
        playerHistory
            .Where(ph => ph.MainRole is not GameMaster)
            .OrderBy(StatusOrder)
            .ForEach(history =>
            {
                bool isWinner = winners.Contains(history.PlayerId);

                string winnerPrefix = isWinner ? ModConstants.Palette.WinnerColor.Colorize("★ ") + "{0}" : indent;

                string statusText = history.Status is PlayerStatus.Dead ? history.CauseOfDeath?.SimpleName() ?? history.Status.ToString() : history.Status.ToString();
                string playerStatus = LastResultCommand.StatusColor(history.Status).Colorize(statusText);

                string statText = history.MainRole.Statistics().FirstOrOptional().Map(t => $" | {t.Name()}: {t.GetGenericValue(history.UniquePlayerId)}").OrElse("");

                int colorId = history.Outfit.ColorId;
                string coloredName = ((Color)Palette.PlayerColors[colorId]).Colorize(ModConstants.ColorNames[colorId]);
                string modifiers = history.Subroles.Count == 0 ? "" : $" {history.Subroles
                    .Where(sr => sr is ISubrole)
                    .Select(sr => sr.RoleColor.Colorize(((ISubrole)sr).Identifier() ?? sr.RoleName))
                    .Fuse("")}";

                roleSummaryTextMesh.text += winnerPrefix.Formatted($"{history.Name} : {coloredName} - {playerStatus} ({history.MainRole.ColoredRoleName()}{modifiers}){statText}\n");
            });

        float DeathTimeOrder(PlayerHistory ph) => ph.CauseOfDeath == null ? 0f : (7200f - (float)ph.CauseOfDeath.Timestamp().TimeSpan().TotalSeconds) / 7200f;
        float StatusOrder(PlayerHistory ph) => winners.Contains(ph.PlayerId) ? (float)ph.Status + DeathTimeOrder(ph) : (float)ph.Status + 99 + DeathTimeOrder(ph);
    }
}