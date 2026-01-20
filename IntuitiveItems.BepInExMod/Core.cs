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

                int defaultVal = (itemName == "Key") ? 10 : -1;

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