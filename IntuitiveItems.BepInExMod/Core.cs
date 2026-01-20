using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using UnityEngine;
using Il2CppInterop.Runtime; // Required for Il2CppType
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using Assets.Scripts.Actors.Player;
using Assets.Scripts.Inventory__Items__Pickups;
using Assets.Scripts.Inventory__Items__Pickups.Items;

namespace IntuitiveItems
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Core : BasePlugin
    {
        public const string GUID = "com.arvietabz.intuitiveitems";
        public const string NAME = "IntuitiveItems";
        public const string VERSION = "1.0.1"; // Bumped version

        public static Core Instance;

        public System.Collections.Generic.Dictionary<EItem, ConfigEntry<int>> _itemLimits
            = new System.Collections.Generic.Dictionary<EItem, ConfigEntry<int>>();

        public System.Collections.Generic.Dictionary<EItem, ItemData> _itemDataCache
            = new System.Collections.Generic.Dictionary<EItem, ItemData>();

        public bool _isConfigInitialized = false;

        public override void Load()
        {
            Instance = this;
            ClassInjector.RegisterTypeInIl2Cpp<IntuitiveItemsBehavior>();
            var obj = new GameObject("IntuitiveItemsBehavior");
            GameObject.DontDestroyOnLoad(obj);
            obj.AddComponent<IntuitiveItemsBehavior>();

            Log.LogInfo("IntuitiveItems Loaded.");
        }

        public void TryInitializeConfig()
        {
            // NEW METHOD: Find ALL ItemData assets in the entire game memory
            // This bypasses the "Available Items" list and gets everything.
            var allAssets = Resources.FindObjectsOfTypeAll(Il2CppType.Of<ItemData>());

            if (allAssets == null || allAssets.Length == 0) return;

            // Log how many raw assets we found to debug
            Log.LogInfo($"Found {allAssets.Length} raw ItemData assets in memory.");

            var uniqueItems = new System.Collections.Generic.List<ItemData>();
            var processedEnums = new System.Collections.Generic.HashSet<EItem>();

            foreach (var obj in allAssets)
            {
                ItemData item = obj.TryCast<ItemData>();
                if (item == null) continue;

                // Deduplicate: If we already processed this Enum ID, skip it.
                if (processedEnums.Contains(item.eItem)) continue;

                uniqueItems.Add(item);
                processedEnums.Add(item.eItem);

                // Add to cache for logic later
                if (!_itemDataCache.ContainsKey(item.eItem))
                {
                    _itemDataCache[item.eItem] = item;
                }
            }

            Log.LogInfo($"Identified {uniqueItems.Count} unique items for Config.");

            // Sort by Rarity -> Name
            var sortedItems = uniqueItems.ToArray().OrderBy(x => (int)x.rarity).ThenBy(x => x.eItem.ToString());

            foreach (var item in sortedItems)
            {
                string itemName = item.eItem.ToString();
                string rarityName = item.rarity.ToString();
                string sectionName = $"{(int)item.rarity}. {rarityName}";

                int defaultVal = -1; // -1 means "No Limit" (the default for everything else)

                switch (itemName)
                {
                    case "Borgar":
                        defaultVal = 1;
                        break;

                    case "BossBuster":
                        defaultVal = 7;
                        break;

                    case "CursedDoll":
                        defaultVal = 1;
                        break;

                    case "Ghost":
                        defaultVal = 1;
                        break;

                    case "Key":
                        defaultVal = 10;
                        break;

                    case "Medkit":
                        defaultVal = 3;
                        break;

                    case "MoldyCheese":
                        defaultVal = 3;
                        break;

                    case "TacticalGlasses":
                        defaultVal = 5;
                        break;

                    case "Beer":
                        defaultVal = 10;
                        break;

                    case "BrassKnuckles":
                        defaultVal = 4;
                        break;

                    case "Campfire":
                        defaultVal = 1;
                        break;

                    case "CowardsCloak":
                        defaultVal = 1;
                        break;

                    case "CreditCardRed":
                        defaultVal = 10;
                        break;

                    case "DemonBlade":
                        defaultVal = 4;
                        break;

                    case "ElectricPlug":
                        defaultVal = 1;
                        break;

                    case "GoldenShield":
                        defaultVal = 1;
                        break;

                    case "UnstableTransfusion":
                        defaultVal = 3;
                        break;

                    case "BobsLantern":
                        defaultVal = 1;
                        break;

                    case "CreditCardGreen":
                        defaultVal = 10;
                        break;

                    case "DemonicSoul":
                        defaultVal = 10;
                        break;

                    case "Gasmask":
                        defaultVal = 1;
                        break;

                    case "GloveBlood":
                        defaultVal = 1;
                        break;

                    case "GrandmasSecretTonic":
                        defaultVal = 2;
                        break;

                    case "Kevin":
                        defaultVal = 1;
                        break;

                    case "Mirror":
                        defaultVal = 1;
                        break;

                    case "SluttyCannon":
                        defaultVal = 5;
                        break;

                    case "ToxicBarrel":
                        defaultVal = 1;
                        break;

                    case "Anvil":
                        defaultVal = 3;
                        break;

                    case "BloodyCleaver":
                        defaultVal = 2;
                        break;

                    case "Bonker":
                        defaultVal = 5;
                        break;

                    case "Chonkplate":
                        defaultVal = 10;
                        break;

                    case "Dragonfire":
                        defaultVal = 7;
                        break;

                    case "EnergyCore":
                        defaultVal = 1;
                        break;

                    case "GiantFork":
                        defaultVal = 7;
                        break;

                    case "GlovePower":
                        defaultVal = 1;
                        break;

                    case "GoldenRing":
                        defaultVal = 1;
                        break;

                    case "HolyBook":
                        defaultVal = 2;
                        break;

                    case "IceCube":
                        defaultVal = 5;
                        break;

                    case "JoesDagger":
                        defaultVal = 10;
                        break;

                    case "LightningOrb":
                        defaultVal = 4;
                        break;

                    case "OverpoweredLamp":
                        defaultVal = 5;
                        break;

                    case "Snek":
                        defaultVal = 10;
                        break;

                    case "SoulHarvester":
                        defaultVal = 1;
                        break;

                    case "SpeedBoi":
                        defaultVal = 1;
                        break;

                    case "SpicyMeatball":
                        defaultVal = 4;
                        break;

                    case "SuckyMagnet":
                        defaultVal = 2;
                        break;

                    case "ZaWarudo":
                        defaultVal = 1;
                        break;

                }

                var entry = Config.Bind(sectionName, itemName, defaultVal,
                    $"Max amount of {itemName} to hold.");

                _itemLimits[item.eItem] = entry;
            }

            Config.Save();
            _isConfigInitialized = true;
            Log.LogInfo("Sorted Config Generation Complete!");
        }

        public void ApplySmartLootLogic()
        {
            if (!_isConfigInitialized)
            {
                TryInitializeConfig();
                return;
            }

            var player = MyPlayer.Instance;
            if (player == null || player.inventory == null || player.inventory.itemInventory == null) return;
            if (RunUnlockables.availableItems == null) return;

            foreach (var kvp in _itemLimits)
            {
                EItem itemEnum = kvp.Key;
                int limit = kvp.Value.Value;

                if (limit < 0) continue;
                if (!_itemDataCache.ContainsKey(itemEnum)) continue;

                ItemData itemData = _itemDataCache[itemEnum];
                EItemRarity rarity = itemData.rarity;
                int currentAmount = player.inventory.itemInventory.GetAmount(itemEnum);

                List<ItemData> lootList = null;
                // We still check availableItems here because we can only remove items 
                // that are actually IN the loot pool for this run.
                if (!RunUnlockables.availableItems.TryGetValue(rarity, out lootList) || lootList == null) continue;

                bool isInPool = lootList.Contains(itemData);

                if (currentAmount >= limit)
                {
                    if (isInPool) lootList.Remove(itemData);
                }
                else
                {
                    if (!isInPool) lootList.Add(itemData);
                }
            }
        }
    }

    public class IntuitiveItemsBehavior : MonoBehaviour
    {
        private float _checkTimer = 0f;
        private const float CHECK_INTERVAL = 1.0f;

        public IntuitiveItemsBehavior(IntPtr ptr) : base(ptr) { }

        public void Update()
        {
            _checkTimer += Time.deltaTime;
            float currentInterval = Core.Instance._isConfigInitialized ? CHECK_INTERVAL : 1.0f;

            if (_checkTimer > currentInterval)
            {
                _checkTimer = 0f;
                if (Core.Instance != null)
                {
                    Core.Instance.ApplySmartLootLogic();
                }
            }
        }
    }
}