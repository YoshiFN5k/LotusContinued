using System;

namespace Lotus.Managers.Announcements;

public class Announcement
{
    public string Title { get; set; } = null!;
    public string ShortTitle { get; set; } = null!;
    public string Subtitle { get; set; } = null!;
    public string BodyText { get; set; } = null!;
    public bool DevOnly { get; set; } = false;
    public DateOnly? Date { get; set; }

    public string GetFormattedDate() => Date?.ToString("M/d/yy") ?? DateOnly.MinValue.ToString("M/d/yy");
}