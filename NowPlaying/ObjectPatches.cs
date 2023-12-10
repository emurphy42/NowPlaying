using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NowPlaying
{
    internal class ObjectPatches
    {
        // initialized by ModEntry.cs
        public static IMonitor ModMonitor; // allow patches to call ModMonitor.Log()
        public static string NowPlayingFormat;
        public static List<string> TracksToIgnoreList = new();
        public static string LastTrackAnnounced = "none";
        public static bool IgnoreUnnamedTracks = false;

        public static void SetTracksToIgnore(string TracksToIgnore)
        {
            foreach (var track in TracksToIgnore.Split(','))
            {
                TracksToIgnoreList.Add(track.Trim());
            }
        }

        public static void Game1_UpdateRequestedMusicTrack_Postfix()
        {
            try
            {
                var songID = Game1.requestedMusicTrack; // e.g. "summer1"
                ModMonitor.Log($"[Now Playing] Song ID = {songID}", LogLevel.Trace);

                // Was this track already announced?
                if (songID == LastTrackAnnounced)
                {
                    ModMonitor.Log($"[Now Playing] Already announced this track", LogLevel.Trace);
                    return;
                }
                LastTrackAnnounced = songID;

                // Don't announce absence of a track
                if (songID == "none")
                {
                    ModMonitor.Log($"[Now Playing] Skipping announcement of silence", LogLevel.Trace);
                    return;
                }

                // Don't announce an ignored track
                if (TracksToIgnoreList.Contains(songID))
                {
                    ModMonitor.Log($"[Now Playing] Skipping announcement of {songID} by request", LogLevel.Debug);
                    return;
                }

                // Get track title
                var songTitle = Utility.getSongTitleFromCueName(Game1.requestedMusicTrack); // e.g. "Summer (Nature's Crescendo)"
                if (IgnoreUnnamedTracks && (songTitle == songID))
                {
                    ModMonitor.Log($"[Now Playing] Skipping announcement of {songID} because title is same as ID", LogLevel.Debug);
                    return;
                }

                // Announce new track
                ModMonitor.Log($"[Now Playing] Song ID = {songID}, title = {songTitle}", LogLevel.Debug);
                Game1.showGlobalMessage(string.Format(NowPlayingFormat, songTitle, songID));
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"[Now Playing] Game1_UpdateRequestedMusicTrack_Postfix: {ex.Message} - {ex.StackTrace}", LogLevel.Error);
            }
        }
    }
}
