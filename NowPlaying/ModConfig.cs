using StardewModdingAPI;

namespace NowPlaying
{
    class ModConfig
    {
        public string NowPlayingFormat = "Now playing: {0}";

        // song IDs to skip announcing, separated by commas
        // https://stardewvalleywiki.com/Modding:Audio#Track_list
        public string TracksToIgnore = "";
    }
}
