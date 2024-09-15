using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley;
using StardewValley.GameData.FishPonds;

namespace ColorfulFishPonds.Patches;
internal class FishPondPatch {

    static IMonitor? Monitor;
    static ModConfig Config = new();

    public static void Init(IMonitor monitor, ModConfig config) {
        Monitor = monitor;
        Config = config;
    }

    public static void DoFishSpecificWaterColoring_Postfix(FishPond __instance) {
        try {
            FishPondData data = __instance.GetFishPondData();

            if (data.WaterColor != null) return;

            if (__instance.currentOccupants.Value >= Config.RequiredPopulation) {
                var color = ItemContextTagManager.GetColorFromTags(ItemRegistry.Create(ItemRegistry.type_object + __instance.fishType.Value));
                if (color != null) {
                    __instance.overrideWaterColor.Value = color.Value;
                }
            }
        }
        catch (Exception ex) {
            Monitor!.LogOnce(
                $"Harmony patch {nameof(DoFishSpecificWaterColoring_Postfix)} encountered an error. Custom fish pond colors might not be applied.\n {ex}",
                LogLevel.Error);
            return;
        }
    }

}