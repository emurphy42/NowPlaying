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

        // ignore songs where title is same as ID
        public bool IgnoreUnnamedTracks = false;

        // override song titles, e.g. to match a custom music mod that replaces standard ones
        public Dictionary<string, string> TracksToRename = new();
    }
}
