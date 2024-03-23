using StardewModdingAPI;
using System.Collections.Generic;

namespace NowPlaying
{
    class ModConfig
    {
        public string NowPlayingFormat = "Now playing: {0}";

        // song IDs to skip announcing, separated by commas
        // https://stardewvalleywiki.com/Modding:Audio#Track_list
        public string TracksToIgnore = "";

        // song titles to ignore in favor of ID, separated by commas
        public string TrackNamesToReplaceWithID = "Invalid";

        // ignore songs where title is same as ID
        public bool IgnoreUnnamedTracks = false;

        // override song titles, e.g. to match a custom music mod that replaces standard ones
        public Dictionary<string, string> TracksToRename = new();

        // after announcing a song, wait this many seconds before announcing it again
        public int RepeatDelay = 2;

        // option to temporarily disable all announcements
        public bool EnableAnnouncements = true;

        // key to toggle announcements off/on
        public SButton ToggleAnnouncementsKey = SButton.N;
    }
}
