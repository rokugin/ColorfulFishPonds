using StardewModdingAPI;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley;
using System.Diagnostics;
using ColorfulFishPonds.Patches;

namespace ColorfulFishPonds {
    internal class ModEntry : Mod {

        public static ModConfig Config = new();

        public override void Entry(IModHelper helper) {
            Config = helper.ReadConfig<ModConfig>();
            FishPondPatch.Init(Monitor, Config);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(FishPond), "doFishSpecificWaterColoring"),
                postfix: new HarmonyMethod(typeof(FishPondPatch), nameof(FishPondPatch.DoFishSpecificWaterColoring_Postfix))
            );
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            SetUpGMCM();
        }

        private void SetUpGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("required-population.label"),
                tooltip: () => Helper.Translation.Get("required-population.tooltip"),
                getValue: () => Config.RequiredPopulation,
                setValue: value => Config.RequiredPopulation = value,
                min: 1,
                max: 10
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("debug-logging.label"),
                tooltip: () => Helper.Translation.Get("debug-logging.tooltip"),
                getValue: () => Config.DebugLogging,
                setValue: value => Config.DebugLogging = value
            );
        }

    }
}
