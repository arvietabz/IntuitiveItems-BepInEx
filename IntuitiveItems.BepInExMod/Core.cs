using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Il2CppInterop.Runtime;
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
        public const string VERSION = "1.5.2"; // Double Border UI Update

        public static Core Instance;
        private Harmony _harmony;

        public ConfigEntry<bool> _keysFirstConfig;

        public System.Collections.Generic.Dictionary<EItem, ConfigEntry<int>> _itemLimits
            = new System.Collections.Generic.Dictionary<EItem, ConfigEntry<int>>();

        public System.Collections.Generic.Dictionary<EItem, ItemData> _itemDataCache
            = new System.Collections.Generic.Dictionary<EItem, ItemData>();

        private System.Collections.Generic.HashSet<EItem> _allowedItems
            = new System.Collections.Generic.HashSet<EItem>();

        public bool _isConfigInitialized = false;

        public override void Load()
        {
            Instance = this;
            _harmony = new Harmony(GUID);

            ClassInjector.RegisterTypeInIl2Cpp<IntuitiveItemsBehavior>();
            ClassInjector.RegisterTypeInIl2Cpp<ConfigInputBehavior>();

            var obj = new GameObject("IntuitiveItemsBehavior");
            GameObject.DontDestroyOnLoad(obj);
            obj.AddComponent<IntuitiveItemsBehavior>();

            _harmony.PatchAll();

            Log.LogInfo("IntuitiveItems Loaded.");
        }

        [HarmonyPatch(typeof(UnlockContainer), nameof(UnlockContainer.OnEnable))]
        public static class UnlockContainerPatch
        {
            [HarmonyPostfix]
            public static void Postfix(UnlockContainer __instance)
            {
                if (__instance.GetComponent<ConfigInputBehavior>() != null) return;
                __instance.gameObject.AddComponent<ConfigInputBehavior>();
            }
        }

        public void TryInitializeConfig()
        {
            var allAssets = Resources.FindObjectsOfTypeAll(Il2CppType.Of<ItemData>());
            if (allAssets == null || allAssets.Length == 0) return;

            _keysFirstConfig = Config.Bind("-- General --", "Keys First", false,
                "If set to true: Only Keys, Legendaries, and Credit Cards will spawn until you reach your Key limit.");

            var uniqueItems = new System.Collections.Generic.List<ItemData>();
            var processedEnums = new System.Collections.Generic.HashSet<EItem>();

            foreach (var obj in allAssets)
            {
                ItemData item = obj.TryCast<ItemData>();
                if (item == null) continue;
                if (processedEnums.Contains(item.eItem)) continue;

                uniqueItems.Add(item);
                processedEnums.Add(item.eItem);
                if (!_itemDataCache.ContainsKey(item.eItem)) _itemDataCache[item.eItem] = item;
            }

            var sortedItems = uniqueItems.ToArray().OrderBy(x => (int)x.rarity).ThenBy(x => x.eItem.ToString());

            foreach (var item in sortedItems)
            {
                string itemName = item.eItem.ToString();
                string sectionName = $"{(int)item.rarity}. {item.rarity}";
                int defaultVal = -1;

                switch (itemName)
                {
                    case "Borgar": defaultVal = 1; break;
                    case "BossBuster": defaultVal = 1; break;
                    case "Battery": defaultVal = 1; break;
                    case "Cactus": defaultVal = 1; break;
                    case "Clover": defaultVal = 1; break;
                    case "CreditCardGreen": defaultVal = 20; break;
                    case "CreditCardRed": defaultVal = 20; break;
                    case "CursedDoll": defaultVal = 1; break;
                    case "ForbiddenJuice": defaultVal = 1; break;
                    case "Ghost": defaultVal = 1; break;
                    case "GoldenGlove": defaultVal = 1; break;
                    case "GymSauce": defaultVal = 1; break;
                    case "IceCrystal": defaultVal = 1; break;
                    case "Key": defaultVal = 10; break;
                    case "Medkit": defaultVal = 1; break;
                    case "MoldyCheese": defaultVal = 1; break;
                    case "Oats": defaultVal = 1; break;
                    case "OldMask": defaultVal = 1; break;
                    case "Skuleg": defaultVal = 1; break;
                    case "SlipperyRing": defaultVal = 1; break;
                    case "TacticalGlasses": defaultVal = 1; break;
                    case "TimeBracelet": defaultVal = 1; break;
                    case "TurboSocks": defaultVal = 1; break;
                    case "Wrench": defaultVal = 1; break;
                    case "Backpack": defaultVal = 1; break;
                    case "Beacon": defaultVal = 1; break;
                    case "Beer": defaultVal = 1; break;
                    case "BrassKnuckles": defaultVal = 5; break;
                    case "Campfire": defaultVal = 1; break;
                    case "CowardsCloak": defaultVal = 1; break;
                    case "DemonBlade": defaultVal = 1; break;
                    case "DemonicBlood": defaultVal = 5; break;
                    case "EchoShard": defaultVal = 1; break;
                    case "ElectricPlug": defaultVal = 1; break;
                    case "Feathers": defaultVal = 1; break;
                    case "GloveLightning": defaultVal = 1; break;
                    case "GlovePoison": defaultVal = 1; break;
                    case "GoldenShield": defaultVal = 1; break;
                    case "GoldenSneakers": defaultVal = 1; break;
                    case "IdleJuice": defaultVal = 5; break;
                    case "LeechingCrystal": defaultVal = 1; break;
                    case "PhantomShroud": defaultVal = 1; break;
                    case "Pumpkin": defaultVal = 1; break;
                    case "UnstableTransfusion": defaultVal = 1; break;
                    case "BeefyRing": defaultVal = 5; break;
                    case "BobDead": defaultVal = 1; break;
                    case "BobsLantern": defaultVal = 5; break;
                    case "DemonicSoul": defaultVal = 5; break;
                    case "EagleClaw": defaultVal = 1; break;
                    case "GamerGoggles": defaultVal = 1; break;
                    case "Gasmask": defaultVal = 1; break;
                    case "GloveBlood": defaultVal = 1; break;
                    case "GloveCurse": defaultVal = 1; break;
                    case "GrandmasSecretTonic": defaultVal = 2; break;
                    case "Kevin": defaultVal = 1; break;
                    case "Mirror": defaultVal = 1; break;
                    case "QuinsMask": defaultVal = 1; break;
                    case "Rollerblades": defaultVal = 1; break;
                    case "Scarf": defaultVal = 1; break;
                    case "ShatteredWisdom": defaultVal = 1; break;
                    case "SluttyCannon": defaultVal = 5; break;
                    case "SpikyShield": defaultVal = 1; break;
                    case "ToxicBarrel": defaultVal = 1; break;
                    case "Anvil": defaultVal = 3; break;
                    case "BloodyCleaver": defaultVal = 2; break;
                    case "Bonker": defaultVal = 5; break;
                    case "Chonkplate": defaultVal = 5; break;
                    case "Dragonfire": defaultVal = 5; break;
                    case "EnergyCore": defaultVal = 1; break;
                    case "GiantFork": defaultVal = 5; break;
                    case "GlovePower": defaultVal = 1; break;
                    case "GoldenRing": defaultVal = 1; break;
                    case "HolyBook": defaultVal = 2; break;
                    case "IceCube": defaultVal = 1; break;
                    case "JoesDagger": defaultVal = 5; break;
                    case "LightningOrb": defaultVal = 5; break;
                    case "OverpoweredLamp": defaultVal = 5; break;
                    case "Snek": defaultVal = 5; break;
                    case "SoulHarvester": defaultVal = 1; break;
                    case "SpeedBoi": defaultVal = 1; break;
                    case "SpicyMeatball": defaultVal = 1; break;
                    case "SuckyMagnet": defaultVal = 2; break;
                    case "ZaWarudo": defaultVal = 1; break;
                }

                var entry = Config.Bind(sectionName, itemName, defaultVal, $"Max amount of {itemName} to hold.");
                _itemLimits[item.eItem] = entry;
            }
            Config.Save();
            _isConfigInitialized = true;
        }

        public void UpdateAllowedItemsList()
        {
            if (RunUnlockables.availableItems == null) return;
            foreach (var rarityObj in System.Enum.GetValues(typeof(EItemRarity)))
            {
                EItemRarity rarity = (EItemRarity)rarityObj;
                if (RunUnlockables.availableItems.TryGetValue(rarity, out var list) && list != null)
                {
                    foreach (var item in list) _allowedItems.Add(item.eItem);
                }
            }
        }

        public void ApplySmartLootLogic()
        {
            if (!_isConfigInitialized) { TryInitializeConfig(); return; }

            var player = MyPlayer.Instance;
            if (player == null || player.inventory == null || player.inventory.itemInventory == null) return;
            if (RunUnlockables.availableItems == null) return;

            UpdateAllowedItemsList();

            bool keysFirstEnabled = _keysFirstConfig.Value;
            int currentKeys = player.inventory.itemInventory.GetAmount(EItem.Key);
            int keyTarget = 10;
            if (_itemLimits.ContainsKey(EItem.Key)) keyTarget = _itemLimits[EItem.Key].Value;

            if (keysFirstEnabled && currentKeys < keyTarget)
            {
                // PHASE 1: KEYS FIRST MODE
                foreach (var kvp in _itemDataCache)
                {
                    ItemData itemData = kvp.Value;
                    EItemRarity rarity = itemData.rarity;
                    List<ItemData> lootList = null;
                    if (!RunUnlockables.availableItems.TryGetValue(rarity, out lootList) || lootList == null) continue;
                    if (!_allowedItems.Contains(itemData.eItem)) continue;

                    bool isKey = (itemData.eItem == EItem.Key);
                    bool isLegendary = (rarity == EItemRarity.Legendary);
                    bool isCreditCard = (itemData.eItem == EItem.CreditCardGreen || itemData.eItem == EItem.CreditCardRed);

                    bool isInPool = lootList.Contains(itemData);

                    // ALLOW: Key, Legendaries, Credit Cards
                    if (isKey || isLegendary || isCreditCard)
                    {
                        if (!isInPool) lootList.Add(itemData);
                    }
                    else
                    {
                        // BAN: Everything else
                        if (isInPool) lootList.Remove(itemData);
                    }
                }
            }
            else
            {
                // PHASE 2: STANDARD MODE
                foreach (var kvp in _itemDataCache)
                {
                    EItem itemEnum = kvp.Key;
                    ItemData itemData = kvp.Value;
                    EItemRarity rarity = itemData.rarity;
                    List<ItemData> lootList = null;
                    if (!RunUnlockables.availableItems.TryGetValue(rarity, out lootList) || lootList == null) continue;
                    if (!_allowedItems.Contains(itemEnum)) continue;

                    int limit = -1;
                    if (_itemLimits.ContainsKey(itemEnum)) limit = _itemLimits[itemEnum].Value;
                    bool isInPool = lootList.Contains(itemData);

                    if (limit < 0) { if (!isInPool) lootList.Add(itemData); continue; }
                    int currentAmount = player.inventory.itemInventory.GetAmount(itemEnum);
                    if (currentAmount >= limit) { if (isInPool) lootList.Remove(itemData); }
                    else { if (!isInPool) lootList.Add(itemData); }
                }
            }
        }
    }

    // === UI COMPONENT ===
    public class ConfigInputBehavior : MonoBehaviour
    {
        private UnlockContainer _container;

        private GameObject _limitRoot;
        private InputField _inputField;
        private GameObject _toggleRoot;
        private Image _toggleBg;
        private RectTransform _toggleHandle;

        private ItemData _currentItem;

        public ConfigInputBehavior(IntPtr ptr) : base(ptr) { }

        public void Start()
        {
            _container = GetComponent<UnlockContainer>();
            CreateLimitUI();
            CreateToggleUI();
            HideAllUI();
        }

        public void OnEnable()
        {
            _currentItem = null;
            if (_limitRoot != null) HideAllUI();
        }

        public void Update()
        {
            if (_container == null || _container.unlockable == null)
            {
                if (_currentItem != null) { _currentItem = null; HideAllUI(); }
                return;
            }

            var item = _container.unlockable.TryCast<ItemData>();
            if (item == null)
            {
                if (_currentItem != null) { _currentItem = null; HideAllUI(); }
                return;
            }

            if (!Core.Instance._itemLimits.ContainsKey(item.eItem))
            {
                if (_currentItem != null) { _currentItem = null; HideAllUI(); }
                return;
            }

            if (item != _currentItem)
            {
                _currentItem = item;
                RefreshUI();
            }
        }

        private void HideAllUI()
        {
            if (_limitRoot != null) _limitRoot.SetActive(false);
            if (_toggleRoot != null) _toggleRoot.SetActive(false);
        }

        private void CreateLimitUI()
        {
            _limitRoot = new GameObject("LimitBorder");
            _limitRoot.transform.SetParent(this.transform, false);
            _limitRoot.SetActive(false);

            RectTransform borderRect = _limitRoot.AddComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(1, 0); // Bottom Right
            borderRect.anchorMax = new Vector2(1, 0);
            borderRect.pivot = new Vector2(1, 0);
            borderRect.sizeDelta = new Vector2(26, 26);
            borderRect.anchoredPosition = new Vector2(-5, 5);

            Image borderImg = _limitRoot.AddComponent<Image>();
            borderImg.color = Color.white;

            GameObject inputObj = new GameObject("LimitInput");
            inputObj.transform.SetParent(_limitRoot.transform, false);

            RectTransform inputRect = inputObj.AddComponent<RectTransform>();
            inputRect.anchorMin = Vector2.zero;
            inputRect.anchorMax = Vector2.one;
            inputRect.sizeDelta = Vector2.zero;
            inputRect.offsetMin = new Vector2(1, 1);
            inputRect.offsetMax = new Vector2(-1, -1);

            Image inputImg = inputObj.AddComponent<Image>();
            inputImg.color = Color.black;

            _inputField = inputObj.AddComponent<InputField>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(inputObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 8;
            text.resizeTextMaxSize = 16;

            _inputField.textComponent = text;
            _inputField.onEndEdit.AddListener((UnityAction<string>)OnValueChanged);
        }

        private void CreateToggleUI()
        {
            // 1. OUTER WHITE BORDER (Root)
            _toggleRoot = new GameObject("KeysFirstToggle");
            _toggleRoot.transform.SetParent(this.transform, false);
            _toggleRoot.SetActive(false);

            RectTransform rect = _toggleRoot.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0); // Bottom Left
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            // Increased size to accommodate borders (30+4, 16+4)
            rect.sizeDelta = new Vector2(34, 20);
            rect.anchoredPosition = new Vector2(5, 5);

            Button btn = _toggleRoot.AddComponent<Button>();
            btn.onClick.AddListener((UnityAction)OnToggleClicked);

            Image outerImg = _toggleRoot.AddComponent<Image>();
            outerImg.color = Color.white; // OUTER BORDER

            // 2. INNER BLACK BORDER
            GameObject blackBorder = new GameObject("InnerBlack");
            blackBorder.transform.SetParent(_toggleRoot.transform, false);
            RectTransform blackRect = blackBorder.AddComponent<RectTransform>();
            blackRect.anchorMin = Vector2.zero;
            blackRect.anchorMax = Vector2.one;
            blackRect.sizeDelta = Vector2.zero;
            blackRect.offsetMin = new Vector2(1, 1); // 1px padding
            blackRect.offsetMax = new Vector2(-1, -1);

            Image blackImg = blackBorder.AddComponent<Image>();
            blackImg.color = Color.black;

            // 3. COLORED CONTENT
            GameObject contentObj = new GameObject("ToggleContent");
            contentObj.transform.SetParent(blackBorder.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;
            contentRect.offsetMin = new Vector2(1, 1); // 1px padding
            contentRect.offsetMax = new Vector2(-1, -1);

            _toggleBg = contentObj.AddComponent<Image>();
            _toggleBg.color = Color.black; // Default Off

            // 4. HANDLE (White Square)
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(contentObj.transform, false); // Parent to Content

            _toggleHandle = handleObj.AddComponent<RectTransform>();
            _toggleHandle.sizeDelta = new Vector2(12, 12);
            _toggleHandle.anchorMin = new Vector2(0, 0.5f);
            _toggleHandle.anchorMax = new Vector2(0, 0.5f);
            _toggleHandle.pivot = new Vector2(0, 0.5f);
            _toggleHandle.anchoredPosition = new Vector2(2, 0);

            Image handleImg = handleObj.AddComponent<Image>();
            handleImg.color = Color.white;
        }

        private void RefreshUI()
        {
            if (_inputField == null) return;

            if (_currentItem == null)
            {
                HideAllUI();
                return;
            }

            if (Core.Instance._itemLimits.TryGetValue(_currentItem.eItem, out var entry))
            {
                _limitRoot.SetActive(true);
                if (entry.Value == -1) _inputField.text = "∞";
                else _inputField.text = entry.Value.ToString();
            }
            else
            {
                _limitRoot.SetActive(false);
            }

            if (_currentItem.eItem == EItem.Key)
            {
                _toggleRoot.SetActive(true);
                bool isOn = Core.Instance._keysFirstConfig.Value;
                UpdateToggleVisuals(isOn);
            }
            else
            {
                _toggleRoot.SetActive(false);
            }
        }

        private void UpdateToggleVisuals(bool isOn)
        {
            if (isOn)
            {
                _toggleBg.color = new Color(0, 0.8f, 0); // Green
                _toggleHandle.anchorMin = new Vector2(1, 0.5f);
                _toggleHandle.anchorMax = new Vector2(1, 0.5f);
                _toggleHandle.pivot = new Vector2(1, 0.5f);
                _toggleHandle.anchoredPosition = new Vector2(-2, 0);
            }
            else
            {
                _toggleBg.color = Color.black; // Black
                _toggleHandle.anchorMin = new Vector2(0, 0.5f);
                _toggleHandle.anchorMax = new Vector2(0, 0.5f);
                _toggleHandle.pivot = new Vector2(0, 0.5f);
                _toggleHandle.anchoredPosition = new Vector2(2, 0);
            }
        }

        private void OnToggleClicked()
        {
            bool current = Core.Instance._keysFirstConfig.Value;
            Core.Instance._keysFirstConfig.Value = !current;
            Core.Instance.Config.Save();
            UpdateToggleVisuals(!current);
        }

        private void OnValueChanged(string val)
        {
            if (_currentItem == null) return;

            int newValue = -1;

            if (string.IsNullOrEmpty(val) || val == "∞")
            {
                newValue = -1;
            }
            else
            {
                if (!int.TryParse(val, out newValue))
                {
                    RefreshUI();
                    return;
                }
            }

            if (Core.Instance._itemLimits.TryGetValue(_currentItem.eItem, out var entry))
            {
                entry.Value = newValue;
                Core.Instance.Config.Save();
                RefreshUI();
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
                if (Core.Instance != null) Core.Instance.ApplySmartLootLogic();
            }
        }
    }
}