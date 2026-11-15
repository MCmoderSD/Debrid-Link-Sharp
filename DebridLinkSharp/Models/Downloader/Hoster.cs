namespace DebridLinkSharp.Models.Downloader;

public enum Hoster
{
    // No value, or a host that this version of the library does not know yet.
    Unknown,

    // The limits that apply to every host without an entry of its own. The API calls it _other_. It is not a host.
    Other,

    ClickNUpload,

    DailyUploads,

    Ddl,

    DownUp,

    Dropbox,

    EliteFile,

    FileAl,

    FileAxa,

    FileCat,

    FileDot,

    FileSpace,

    FileStore,

    FileXtras,

    FileZz,

    GigaPeta,

    GoFile,

    GoLoady,

    GoogleDrive,

    HitFile,

    HulkShare,

    ICloudDrive,

    IsraCloud,

    KatFile,

    KShared,

    MediaFire,

    Mega,

    MegaLoad,

    MixDrop,

    OneFichier,

    PCloud,

    PixelDrain,

    PreFiles,

    RapidGator,

    Scribd,

    SilkFiles,

    TeraBytez,

    TezFiles,

    TurboBit,

    Uploady,

    UpVid,

    WdUpload,

    WorkUpload,

    WorldFiles
}