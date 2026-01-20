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
        public const string VERSION = "1.0.5"; // Final Polish Version

        public static Core Instance;

        // Config Entries
        public ConfigEntry<bool> _keysFirstConfig;

        public System.Collections.Generic.Dictionary<EItem, ConfigEntry<int>> _itemLimits
            = new System.Collections.Generic.Dictionary<EItem, ConfigEntry<int>>();

        // Caches
        public System.Collections.Generic.Dictionary<EItem, ItemData> _itemDataCache
            = new System.Collections.Generic.Dictionary<EItem, ItemData>();

        // Tracks which items are genuinely unlocked by the player to prevent spoilers
        private System.Collections.Generic.HashSet<EItem> _allowedItems
            = new System.Collections.Generic.HashSet<EItem>();

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
            var allAssets = Resources.FindObjectsOfTypeAll(Il2CppType.Of<ItemData>());
            if (allAssets == null || allAssets.Length == 0) return;

            // 1. Bind General Config
            // CHANGED SECTION NAME: "-- General --" sorts before "0. Common"
            _keysFirstConfig = Config.Bind("-- General --", "Keys First", false,
                "If set to true: Only Keys and Legendary items will spawn until you reach your Key limit. Once reached, other unlocked items will spawn.");

            // 2. Process Items
            var uniqueItems = new System.Collections.Generic.List<ItemData>();
            var processedEnums = new System.Collections.Generic.HashSet<EItem>();

            foreach (var obj in allAssets)
            {
                ItemData item = obj.TryCast<ItemData>();
                if (item == null) continue;
                if (processedEnums.Contains(item.eItem)) continue;

                uniqueItems.Add(item);
                processedEnums.Add(item.eItem);

                if (!_itemDataCache.ContainsKey(item.eItem))
                {
                    _itemDataCache[item.eItem] = item;
                }
            }

            var sortedItems = uniqueItems.ToArray().OrderBy(x => (int)x.rarity).ThenBy(x => x.eItem.ToString());

            foreach (var item in sortedItems)
            {
                string itemName = item.eItem.ToString();
                string rarityName = item.rarity.ToString();
                string sectionName = $"{(int)item.rarity}. {rarityName}";

                int defaultVal = -1; // Default is Infinite

                // CUSTOM DEFAULTS
                switch (itemName)
                {
                    case "Borgar": defaultVal = 1; break;
                    case "BossBuster": defaultVal = 7; break;
                    case "CursedDoll": defaultVal = 1; break;
                    case "Ghost": defaultVal = 1; break;
                    case "Key": defaultVal = 10; break;
                    case "Medkit": defaultVal = 3; break;
                    case "MoldyCheese": defaultVal = 3; break;
                    case "TacticalGlasses": defaultVal = 5; break;
                    case "Beer": defaultVal = 10; break;
                    case "BrassKnuckles": defaultVal = 4; break;
                    case "Campfire": defaultVal = 1; break;
                    case "CowardsCloak": defaultVal = 1; break;
                    case "CreditCardRed": defaultVal = 10; break;
                    case "DemonBlade": defaultVal = 4; break;
                    case "ElectricPlug": defaultVal = 1; break;
                    case "GoldenShield": defaultVal = 1; break;
                    case "UnstableTransfusion": defaultVal = 3; break;
                    case "BobsLantern": defaultVal = 1; break;
                    case "CreditCardGreen": defaultVal = 10; break;
                    case "DemonicSoul": defaultVal = 10; break;
                    case "Gasmask": defaultVal = 1; break;
                    case "GloveBlood": defaultVal = 1; break;
                    case "GrandmasSecretTonic": defaultVal = 2; break;
                    case "Kevin": defaultVal = 1; break;
                    case "Mirror": defaultVal = 1; break;
                    case "SluttyCannon": defaultVal = 5; break;
                    case "ToxicBarrel": defaultVal = 1; break;
                    case "Anvil": defaultVal = 3; break;
                    case "BloodyCleaver": defaultVal = 2; break;
                    case "Bonker": defaultVal = 5; break;
                    case "Chonkplate": defaultVal = 10; break;
                    case "Dragonfire": defaultVal = 7; break;
                    case "EnergyCore": defaultVal = 1; break;
                    case "GiantFork": defaultVal = 7; break;
                    case "GlovePower": defaultVal = 1; break;
                    case "GoldenRing": defaultVal = 1; break;
                    case "HolyBook": defaultVal = 2; break;
                    case "IceCube": defaultVal = 5; break;
                    case "JoesDagger": defaultVal = 10; break;
                    case "LightningOrb": defaultVal = 4; break;
                    case "OverpoweredLamp": defaultVal = 5; break;
                    case "Snek": defaultVal = 10; break;
                    case "SoulHarvester": defaultVal = 1; break;
                    case "SpeedBoi": defaultVal = 1; break;
                    case "SpicyMeatball": defaultVal = 4; break;
                    case "SuckyMagnet": defaultVal = 2; break;
                    case "ZaWarudo": defaultVal = 1; break;
                }

                var entry = Config.Bind(sectionName, itemName, defaultVal, $"Max amount of {itemName} to hold.");
                _itemLimits[item.eItem] = entry;
            }

            Config.Save();
            _isConfigInitialized = true;
            Log.LogInfo("Sorted Config Generation Complete!");
        }

        // Helper to scan for unlocked items
        public void UpdateAllowedItemsList()
        {
            if (RunUnlockables.availableItems == null) return;

            foreach (var rarityObj in System.Enum.GetValues(typeof(EItemRarity)))
            {
                EItemRarity rarity = (EItemRarity)rarityObj;
                if (RunUnlockables.availableItems.TryGetValue(rarity, out var list) && list != null)
                {
                    foreach (var item in list)
                    {
                        _allowedItems.Add(item.eItem);
                    }
                }
            }
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

            // Record what is genuinely unlocked
            UpdateAllowedItemsList();

            bool keysFirstEnabled = _keysFirstConfig.Value;
            int currentKeys = player.inventory.itemInventory.GetAmount(EItem.Key);

            int keyTarget = 10;
            if (_itemLimits.ContainsKey(EItem.Key)) keyTarget = _itemLimits[EItem.Key].Value;

            // =========================================================
            // PHASE 1: LOCKDOWN (Keys First)
            // =========================================================
            if (keysFirstEnabled && currentKeys < keyTarget)
            {
                foreach (var kvp in _itemDataCache)
                {
                    ItemData itemData = kvp.Value;
                    EItemRarity rarity = itemData.rarity;

                    List<ItemData> lootList = null;
                    if (!RunUnlockables.availableItems.TryGetValue(rarity, out lootList) || lootList == null) continue;

                    bool isKey = (itemData.eItem == EItem.Key);
                    bool isLegendary = (rarity == EItemRarity.Legendary);
                    bool isInPool = lootList.Contains(itemData);

                    // Skip locked items
                    if (!_allowedItems.Contains(itemData.eItem)) continue;

                    if (isKey || isLegendary)
                    {
                        if (!isInPool) lootList.Add(itemData);
                    }
                    else
                    {
                        if (isInPool) lootList.Remove(itemData);
                    }
                }
            }
            // =========================================================
            // PHASE 2: STANDARD (Limits)
            // =========================================================
            else
            {
                foreach (var kvp in _itemDataCache)
                {
                    EItem itemEnum = kvp.Key;
                    ItemData itemData = kvp.Value;
                    EItemRarity rarity = itemData.rarity;

                    List<ItemData> lootList = null;
                    if (!RunUnlockables.availableItems.TryGetValue(rarity, out lootList) || lootList == null) continue;

                    // Skip locked items
                    if (!_allowedItems.Contains(itemEnum)) continue;

                    int limit = -1;
                    if (_itemLimits.ContainsKey(itemEnum)) limit = _itemLimits[itemEnum].Value;

                    bool isInPool = lootList.Contains(itemData);

                    // Infinite Limit (-1)
                    if (limit < 0)
                    {
                        if (!isInPool) lootList.Add(itemData);
                        continue;
                    }

                    // Numbered Limit
                    int currentAmount = player.inventory.itemInventory.GetAmount(itemEnum);

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