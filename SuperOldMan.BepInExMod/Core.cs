using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppSystem;
using Il2CppInterop.Runtime;

// BepInEx Interop for Megabonk uses 'Assets.Scripts' as the root namespace
using Assets.Scripts._Data.Tomes;
using Assets.Scripts.Actors.Player;
using Assets.Scripts.Inventory__Items__Pickups;
using Assets.Scripts.Inventory__Items__Pickups.Stats;
using Assets.Scripts.Menu.Shop;

namespace SuperOldMan.BepInExMod
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Core : BasePlugin
    {
        private const string MyGUID = "Slimaeus.SuperOldMan";
        private const string PluginName = "SuperOldMan";
        private const string VersionString = "1.0.1";

        public override void Load()
        {
            // Register Harmony Patches
            Harmony.CreateAndPatchAll(typeof(PlayerPatches));
            Log.LogInfo($"Plugin {PluginName} {VersionString} is loaded!");
        }

        [HarmonyPatch(typeof(MyPlayer))]
        public static class PlayerPatches
        {
            [HarmonyPatch(nameof(MyPlayer.Spawn))]
            [HarmonyPostfix]
            public static void Spawn_Postfix(MyPlayer __instance)
            {
                var xpTome = DataManager.Instance.tomeData[ETome.Xp];

                // Use Il2CppSystem.Collections.Generic.List explicitly for game lists
                var xpStatModifiers = new Il2CppSystem.Collections.Generic.List<StatModifier>();

                xpStatModifiers.Add(new StatModifier
                {
                    stat = EStat.XpIncreaseMultiplier,
                    modification = 9,
                    modifyType = EStatModifyType.Flat
                });

                __instance.inventory.tomeInventory.AddTome(xpTome, xpStatModifiers, ERarity.New);

                for (int i = 0; i < 98; i++)
                {
                    __instance.inventory.tomeInventory.AddTome(xpTome, new Il2CppSystem.Collections.Generic.List<StatModifier>(), ERarity.Legendary);
                }
            }
        }
    }
}