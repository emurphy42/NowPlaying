using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework.Media;
using StardewModdingAPI;
using StardewModdingAPI.Events;
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
            InjectOptions();

            var harmony = new Harmony(this.ModManifest.UniqueID);
            // detect when music changes
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.UpdateRequestedMusicTrack)),
               postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Game1_UpdateRequestedMusicTrack_Postfix))
            );

            Helper.Events.GameLoop.GameLaunched += (e, a) => OnGameLaunched(e, a);
            Helper.Events.Input.ButtonPressed += (e, a) => OnButtonPressed(e, a);
        }

        /// <summary>Update static object's attributes to match current options</summary>
        private void InjectOptions()
        {
            ObjectPatches.NowPlayingFormat = Config.NowPlayingFormat;
            ObjectPatches.SetTracksToIgnore(Config.TracksToIgnore);
            ObjectPatches.SetTrackNamesToReplaceWithID(Config.TrackNamesToReplaceWithID);
            ObjectPatches.IgnoreUnnamedTracks = Config.IgnoreUnnamedTracks;
            ObjectPatches.TracksToRename = Config.TracksToRename;
            ObjectPatches.RepeatDelay = Config.RepeatDelay;
            ObjectPatches.EnableAnnouncements = Config.EnableAnnouncements;
            ObjectPatches.ToggleAnnouncementsKey = Config.ToggleAnnouncementsKey;
        }

        /// <summary>Re-inject options after they're changed</summary>
        private void OnFieldChanged(string fieldID, object fieldValue)
        {
            InjectOptions();
        }

        /// <summary>Add to Generic Mod Config Menu</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // re-inject options after they're changed
            configMenu.OnFieldChanged(
                mod: this.ModManifest,
                onChange: (fieldID, fieldValue) => this.OnFieldChanged(fieldID, fieldValue)
            );

            // add config options
            // Future improvement: increase width of "Message format" input box
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Message format",
                getValue: () => this.Config.NowPlayingFormat,
                setValue: value => this.Config.NowPlayingFormat = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Tracks to ignore",
                tooltip: () => "Track IDs separated by commas",
                getValue: () => this.Config.TracksToIgnore,
                setValue: value => this.Config.TracksToIgnore = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Track names to replace with ID",
                tooltip: () => "Track names separated by commas",
                getValue: () => this.Config.TrackNamesToReplaceWithID,
                setValue: value => this.Config.TrackNamesToReplaceWithID = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Ignore unnamed tracks",
                tooltip: () => "Skip announcing track if name is same as ID",
                getValue: () => this.Config.IgnoreUnnamedTracks,
                setValue: value => this.Config.IgnoreUnnamedTracks = value
            );
            // Future improvement: TracksToRename
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Repeat delay",
                tooltip: () => "Wait this many seconds before re-announcing a track",
                getValue: () => this.Config.RepeatDelay,
                setValue: value => this.Config.RepeatDelay = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable announcements",
                getValue: () => this.Config.EnableAnnouncements,
                setValue: value => this.Config.EnableAnnouncements = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Toggle announcements",
                tooltip: () => "Enables or disables announcements",
                getValue: () => this.Config.ToggleAnnouncementsKey,
                setValue: value => this.Config.ToggleAnnouncementsKey = value
            );
        }

        /// <summary>React to toggle key</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.ToggleAnnouncementsKey)
            {
                this.Config.EnableAnnouncements = !this.Config.EnableAnnouncements;
                InjectOptions();

                var enabledDescription = this.Config.EnableAnnouncements ? "enabled" : "disabled";
                this.Monitor.Log($"[Now Playing] Announcements are now {enabledDescription}", LogLevel.Debug);
            }
        }

    }
}
