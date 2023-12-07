using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;

namespace NowPlaying
{
    internal class ObjectPatches
    {
        // initialized by ModEntry.cs
        public static IMonitor ModMonitor; // allow patches to call ModMonitor.Log()
        public static string NowPlayingFormat;

        public static void Game1_UpdateRequestedMusicTrack_Postfix()
        {
            try
            {
                if (Game1.requestedMusicTrack != "none")
                {
                    var songID = Game1.requestedMusicTrack; // e.g. "summer1"
                    ModMonitor.Log($"[Now Playing] Song ID = {songID}", LogLevel.Debug);
                    var songTitle = Utility.getSongTitleFromCueName(Game1.requestedMusicTrack); // e.g. "Summer (Nature's Crescendo)"
                    ModMonitor.Log($"[Now Playing] Song title = {songTitle}", LogLevel.Debug);
                    Game1.showGlobalMessage(string.Format(NowPlayingFormat, songTitle, songID));
                }
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"[Now Playing] Game1_UpdateRequestedMusicTrack_Postfix: {ex.Message} - {ex.StackTrace}", LogLevel.Error);
            }
        }
    }
}
