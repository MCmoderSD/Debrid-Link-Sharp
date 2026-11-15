using DebridLinkSharp.Models.Downloader;

namespace DebridLinkSharp.Internal;

// The names that the API uses for the hosts, and the Hoster for each of them.
internal static class HosterNames
{
    public static readonly (string Wire, Hoster Value)[] Table =
    [
        ("_other_", Hoster.Other),
        ("clicknupload", Hoster.ClickNUpload),
        ("dailyuploads", Hoster.DailyUploads),
        ("ddl", Hoster.Ddl),
        ("downup", Hoster.DownUp),
        ("dropbox", Hoster.Dropbox),
        ("elitefile", Hoster.EliteFile),
        ("fileal", Hoster.FileAl),
        ("fileaxa", Hoster.FileAxa),
        ("filecat", Hoster.FileCat),
        ("filedot", Hoster.FileDot),
        ("filespace", Hoster.FileSpace),
        ("filestore", Hoster.FileStore),
        ("filextras", Hoster.FileXtras),
        ("filezz", Hoster.FileZz),
        ("gigapeta", Hoster.GigaPeta),
        ("gofile", Hoster.GoFile),
        ("goloady", Hoster.GoLoady),
        ("googledrive", Hoster.GoogleDrive),
        ("hitfile", Hoster.HitFile),
        ("hulkshare", Hoster.HulkShare),
        ("iclouddrive", Hoster.ICloudDrive),
        ("isracloud", Hoster.IsraCloud),
        ("katfile", Hoster.KatFile),
        ("kshared", Hoster.KShared),
        ("mediafire", Hoster.MediaFire),
        ("mega", Hoster.Mega),
        ("megaload", Hoster.MegaLoad),
        ("mixdrop", Hoster.MixDrop),
        ("1fichier", Hoster.OneFichier),
        ("pcloud", Hoster.PCloud),
        ("pixeldrain", Hoster.PixelDrain),
        ("prefiles", Hoster.PreFiles),
        ("rapidgator", Hoster.RapidGator),
        ("scribd", Hoster.Scribd),
        ("silkfiles", Hoster.SilkFiles),
        ("terabytez", Hoster.TeraBytez),
        ("tezfiles", Hoster.TezFiles),
        ("turbobit", Hoster.TurboBit),
        ("uploady", Hoster.Uploady),
        ("upvid", Hoster.UpVid),
        ("wdupload", Hoster.WdUpload),
        ("workupload", Hoster.WorkUpload),
        ("worldfiles", Hoster.WorldFiles)
    ];

    // A name is compared without regard to upper and lower case. A name that is not in the table is Unknown.
    public static Hoster Parse(string name)
    {
        foreach (var (wire, hoster) in Table)
        {
            if (string.Equals(wire, name, StringComparison.OrdinalIgnoreCase))
            {
                return hoster;
            }
        }

        return Hoster.Unknown;
    }
}

internal sealed class HosterConverter() : WireEnumConverter<Hoster>(HosterNames.Table);
