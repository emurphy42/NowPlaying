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
        public static List<string> TrackNamesToReplaceWithIDList = new();
        public static string LastTrackAnnounced = "none";
        public static bool IgnoreUnnamedTracks = false;
        public static Dictionary<string, DateTime> WhenToReannounceTrack = new();
        public static Dictionary<string, string> TracksToRename = new();
        public static int RepeatDelay;

        public static void SetTracksToIgnore(string TracksToIgnore)
        {
            foreach (var track in TracksToIgnore.Split(','))
            {
                TracksToIgnoreList.Add(track.Trim());
            }
        }

        public static void SetTrackNamesToReplaceWithID(string TrackNamesToReplaceWithID)
        {
            foreach (var trackName in TrackNamesToReplaceWithID.Split(','))
            {
                TrackNamesToReplaceWithIDList.Add(trackName.Trim());
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
                if (songID == "none" || songID == "silence")
                {
                    ModMonitor.Log($"[Now Playing] Skipping announcement of silence", LogLevel.Trace);
                    return;
                }

                // Don't announce a track too soon after it was previously announced
                // (occasionally the game requests e.g. both an ambient track and a music track multiple times within a short period of time,
                // in which case the LastTrackAnnounced check doesn't catch it)
                if (WhenToReannounceTrack.ContainsKey(songID) && DateTime.Now < WhenToReannounceTrack[songID])
                {
                    ModMonitor.Log($"[Now Playing] Skipping announcement of {songID} too soon after it was previously announced", LogLevel.Trace);
                    return;
                }

                // Don't announce an ignored track
                if (TracksToIgnoreList.Contains(songID))
                {
                    ModMonitor.Log($"[Now Playing] Skipping announcement of {songID} by request", LogLevel.Debug);
                    return;
                }

                // Get track title (custom or standard)
                var songTitle = (TracksToRename.ContainsKey(songID))
                    ? TracksToRename[songID]
                    : Utility.getSongTitleFromCueName(Game1.requestedMusicTrack); // e.g. "Summer (Nature's Crescendo)"
                if (TrackNamesToReplaceWithIDList.Contains(songTitle))
                {
                    ModMonitor.Log($"[Now Playing] TODO {songTitle} by request", LogLevel.Debug);
                    songTitle = songID;
                }
                if (IgnoreUnnamedTracks && (songTitle == songID))
                {
                    ModMonitor.Log($"[Now Playing] Skipping announcement of {songID} because title is same as ID", LogLevel.Debug);
                    return;
                }


                // Announce new track
                ModMonitor.Log($"[Now Playing] Song ID = {songID}, title = {songTitle}", LogLevel.Debug);
                Game1.showGlobalMessage(string.Format(NowPlayingFormat, songTitle, songID));

                // Update cooldown timer for next announcement of same track
                WhenToReannounceTrack[songID] = DateTime.Now.AddSeconds(RepeatDelay);
            }
            catch (Exception ex)
            {
                ModMonitor.Log($"[Now Playing] Game1_UpdateRequestedMusicTrack_Postfix: {ex.Message} - {ex.StackTrace}", LogLevel.Error);
            }
        }
    }
}
