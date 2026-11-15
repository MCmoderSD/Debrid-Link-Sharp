using System.Diagnostics.CodeAnalysis;

namespace DebridLinkSharp.Models.Downloader;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum Hoster
{
    // No value, or a host that this version of the library does not know yet.
    Unknown,

    // The limits that apply to every host without an entry of its own. The API calls it _other_. It is not a host.
    Other,

    // Hoster of the type Host: the file hosts
    ClickNUpload,

    DailyUploads,

    Ddl,

    Depositfiles,

    DownUp,

    Dropbox,

    EliteFile,

    FileAl,

    FileAxa,

    FileCat,

    FileDot,

    Filefactory,

    FileSpace,

    FileStore,

    FileXtras,

    FileZz,

    GigaPeta,

    GoFile,

    GoLoady,

    GoogleDrive,

    HitFile,

    Hot4share,

    Hotlink,

    HulkShare,

    ICloudDrive,

    IsraCloud,

    KatFile,

    Keep2share,

    KShared,

    MediaFire,

    Mega,

    MegaLoad,

    MixDrop,

    Nitroflare,

    Onedrive,

    OneFichier,

    PCloud,

    PixelDrain,

    PreFiles,

    RapidGator,

    Scribd,

    SilkFiles,

    Streamtape,

    TeraBytez,

    TezFiles,

    TurboBit,

    Uploady,

    UpVid,

    Uqload,

    Vidoza,

    Voe,

    WdUpload,

    WorkUpload,

    WorldFiles,

    // Hoster of the type Stream: the streaming sites
    Academicearth,

    Acast,

    AddAnime,

    Airmozilla,

    Allocine,

    Alphaporno,

    Anysex,

    Aparat,

    Arte,

    Audimediacenter,

    Audioboom,

    Audiomack,

    Beeg,

    Camdemy,

    Chilloutzone,

    Clubic,

    Clyp,

    Daclips,

    Dailymail,

    Dailymotion,

    Democracynow,

    Discovery,

    Dotsub,

    Ebaumsworld,

    Eitb,

    Ellentv,

    Flipagram,

    Footyroom,

    Formula1,

    FourTube,

    Foxnews,

    Franceculture,

    Gameinformer,

    Gamersyde,

    Gorillavid,

    Hbo,

    Hellporno,

    HentaiAnimestigma,

    Hornbunny,

    Imdb,

    Instagram,

    ItarTass,

    Jamendo,

    Jove,

    Keek,

    Keezmovies,

    Khanacademy,

    Kickstarter,

    Krasview,

    La7,

    Lci,

    Lcp,

    Libsyn,

    Liveleak,

    Livestream,

    M6,

    Metacritic,

    Mgoon,

    Mixcloud,

    Mojvideo,

    Movieclips,

    Movpod,

    Musicplayon,

    Myspass,

    Myvidster,

    Odatv,

    Onionstudios,

    Ora,

    Playfm,

    Plays,

    Playvid,

    Pornhd,

    Pornhub,

    Pyvideo,

    Redtube,

    Reverbnation,

    Revision3,

    Rts,

    Rtve,

    Sapovideo,

    Sbs,

    Screencast,

    Skysports,

    Slutload,

    Soundcloud,

    Soundgasm,

    Steam,

    Streamable,

    Streamcloud,

    StreamCz,

    Sunporno,

    Teachertube,

    Teamcoco,

    Ted,

    Tfo,

    Thescene,

    Thesixtyone,

    Tnaflix,

    Trutv,

    Turbo,

    TuTv,

    Tweakers,

    Ustream,

    Vbox7,

    Veehd,

    Veoh,

    Videodetective,

    Vimeo,

    Wat,

    Wimp,

    Xtube,

    Yahoo,

    Youporn
}