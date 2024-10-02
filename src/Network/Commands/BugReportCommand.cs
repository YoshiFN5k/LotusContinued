using System;
using System.Collections.Generic;
using Lotus.API.Player;
using Lotus.Chat;
using Lotus.Logging;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using VentLib.Commands;
using VentLib.Commands.Attributes;
using VentLib.Commands.Interfaces;
using VentLib.Localization.Attributes;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using IEnumerator = System.Collections.IEnumerator;

namespace Lotus.Network.Commands;

[Localized("Commands.BugReports")]
[Command("report")]
public class BugReportCommand : ICommandReceiver
{
    [Localized(nameof(WaitingOnReport))] public static string WaitingOnReport = "This report has been voided as we are waiting on a response for your previous report.";
    [Localized(nameof(FailedReport))] public static string FailedReport = "An error occured while sending the request. Please try again in a minute.\nError: {0}";
    [Localized(nameof(ReportingBug))] public static string ReportingBug = "Your request has went through and we are now reporting your bug. Thanks!";
    [Localized(nameof(ReportedBug))] public static string ReportedBug = "Your bug has been successfully reported. Again, thank you!";
    private static readonly List<byte> reportingPlayerIds = new();
    public void Receive(PlayerControl source, CommandContext context)
    {
        if (reportingPlayerIds.Contains(source.PlayerId))
        {
            ChatHandlers.NotPermitted(WaitingOnReport).Send(source);
            return;
        }
        reportingPlayerIds.Add(source.PlayerId);
        ChatHandler.Of(ReportingBug).Send(source);

        Async.Execute(SendWebRequest(source, context));
    }

    private IEnumerator SendWebRequest(PlayerControl source, CommandContext context)
    {
        string message = string.Join(" ", context.Args);
        if (source == PlayerControl.LocalPlayer && message == "log")
        {
            LogManager.SendInGame("Uploading your log file to the forum is currently not supported. Please try again in another dev.");
            reportingPlayerIds.Remove(source.PlayerId);
            yield break;
        }
        if (string.IsNullOrEmpty(message) || message.Length < 7)
        {
            reportingPlayerIds.Remove(source.PlayerId);
            yield break;
        }
        FrozenPlayer frozenPlayer = new(source);
        string? errorMessage = null;

        // send report.

        string jsonData = $@"{{
            ""playerName"": ""{source.name}"",
            ""friendCode"": ""{source.FriendCode}"",
            ""message"": ""{message}""
        }}";
        byte[] postData = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Send POST request
        UnityWebRequest webRequest = new(NetConstants.Host + "reportbug", UnityWebRequest.kHttpVerbPOST)
        {
            uploadHandler = new UploadHandlerRaw(postData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                break;
            default:
                errorMessage = "Result: {0} - Error: {1} - ResponseCode: {2}".Formatted(webRequest.result.ToString(), webRequest.error, webRequest.responseCode);
                break;
        }

        if (errorMessage == null) ChatHandler.Of(ReportedBug).Send(source);
        else ChatHandler.Of(FailedReport.Formatted(errorMessage)).Send(source);
        webRequest.Dispose();

        reportingPlayerIds.Remove(frozenPlayer.PlayerId);
    }
}