using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lotus.Managers.Announcements.Helpers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Lotus.Managers.Announcements;

public class CustomAnnouncementManager
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(CustomAnnouncementManager));
    private List<Announcement> CustomAnnouncements;

    private static readonly IDeserializer AnnouncementDeserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
        .WithTypeConverter(new DateOnlyYamlConverter())  // Register custom DateOnly converter
        .Build();

    public CustomAnnouncementManager()
    {
        try
        {
            CustomAnnouncements = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(n => n.Contains("Lotus.assets.Announcements"))
                .Select(LoadTitleFromManifest)
                .Where(a => a != null)
                .ToList()!;
        }
        catch (Exception exception)
        {
            log.Exception("Error loading in manifest (global) announcements.", exception);
            CustomAnnouncements = new List<Announcement>();
        }
    }

    public void AddAnnouncements(IEnumerable<Announcement> announcements)
    {
        CustomAnnouncements.AddRange(announcements.Where(a => !string.IsNullOrEmpty(a.Title)));
    }

    public void AddAnnouncements(params Announcement[] announcements) => AddAnnouncements((IEnumerable<Announcement>)announcements);

    public void AddAnnouncements(DirectoryInfo announcementDirectory)
    {
        AddAnnouncements(
            announcementDirectory.GetFiles("*.yaml")
            .Select(f =>
            {
                try
                {
                    return LoadFromFileInfo(f);
                }
                catch (Exception exception)
                {
                    log.Exception($"Error loading title file: {f.Name}.", exception);
                    return new Announcement();
                }
            }));
    }

    internal List<Announcement> GetAnnouncements() => CustomAnnouncements.OrderBy(a => a.Date ?? DateOnly.MinValue).ToList();

    private static Announcement? LoadTitleFromManifest(string manifestResource)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestResource);
        return stream == null ? null : LoadFromStream(stream);
    }

    private static Announcement LoadFromFileInfo(FileInfo file) => LoadFromStream(file.Open(FileMode.Open));

    private static Announcement LoadFromStream(Stream stream)
    {
        string result;
        using (StreamReader reader = new(stream)) result = reader.ReadToEnd();

        return AnnouncementDeserializer.Deserialize<Announcement>(result);
    }
}