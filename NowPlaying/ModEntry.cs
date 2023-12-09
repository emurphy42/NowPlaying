using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System.Linq;

namespace NowPlaying
{
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            ObjectPatches.ModMonitor = this.Monitor;
            ObjectPatches.NowPlayingFormat = Config.NowPlayingFormat;
            ObjectPatches.SetTracksToIgnore(Config.TracksToIgnore);

            var harmony = new Harmony(this.ModManifest.UniqueID);
            // detect when music changes
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.UpdateRequestedMusicTrack)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Game1_UpdateRequestedMusicTrack_Postfix))
            );
        }
    }
    }
