# Intuitive Items for Megabonk (BepInExMod)

An intelligent inventory manager that lets you define a maximum carry limit for any item in the game. Say goodbye to finding useless dupes once you've already hit your stack goal!

### ⚠️ WARNING
**This mod affects gameplay mechanics (RNG manipulation).** It is recommended to **disable leaderboard uploading** in the game settings if you intend to use this mod, as it provides a distinct advantage.

---

### Overview

**IntuitiveItems** dynamically reads game data to generate a comprehensive configuration for every item tier. It injects a small UI overlay onto your inventory screen, giving you complete control over how many of each item you want to carry before they stop appearing in the loot pool.

### Core Features

*   **Smart Loot Filter:** Once you reach your configured limit for an item (e.g., 3 Moldy Cheese), the mod temporarily removes it from the drop table. This naturally boosts the odds of rolling items you actually need.
*   **Dynamic Restoration:** If your inventory count drops below the limit (for example, after using a Key or microwaving an item), the mod immediately detects the change and adds the item back into rotation.
*   **In-Game Configuration:** No need to edit text files! Small input boxes appear directly on your item icons in the menu.
*   **"Keys First" Mode:** A special toggle that forces the game to prioritize Keys, Credit Cards, and Legendaries early in the run.

### How to Use

#### 1. Setting Item Limits
Go to the **Unlocks -> Items** menu in the game.
*   You will see a small **Black Box** at the bottom-right of every item icon.
*   **Click the box** and type a number (e.g., `5`).
*   The mod will now stop spawning that item once you have 5 of them.
*   Type `-1` (or delete the text) to set it to **∞** (Infinite/No Limit).

#### 2. "Keys First" Mode
On the **Key** item icon, there is a toggle switch at the bottom-left.
*   **OFF (Black):** Standard gameplay.
*   **ON (Green):** The game enters a "Priority Phase."
    *   **Effect:** The game will **ONLY** drop Keys, Legendaries, and Credit Cards.
    *   **Goal:** This forces you to reach your Key Cap (default 10) quickly.
    *   **Completion:** Once you have 10 Keys, the restriction lifts automatically, and all other unlocked items begin spawning normally.

---

## Installation (BepInEx)

1.  Ensure **BepInEx 6.0 (IL2CPP)** is installed.
2.  Download the latest `IntuitiveItems.dll`.
3.  Drop the file into your game directory at: `(Game Root)/BepInEx/plugins`.
4.  Launch the game!

## Compatibility

*   **Progression Safe:** The mod adheres to your current save file. It will never force locked items to spawn or show UI for items you haven't discovered yet.
*   **Future Proof:** If a game update introduces new items, the mod automatically detects them and adds them to the configuration system on the next launch.

---

### Credits
Based on the concept of **IntuitiveKeys** by Twinzet.

### BUILDING

- You should create `Directory.Build.props` in the root of the solution with the following content:
```xml
<Project>
	<PropertyGroup>
		<MEGABONK_DIR>X:\Your\Path\TO\Megabonk</MEGABONK_DIR>
		<MEGABONK_PROFILE>X:\Your\Path\TO\Megabonk\BepInEx</MEGABONK_PROFILE>
	</PropertyGroup>
</Project>
```
