# Changelog

## v1.8.2
*   **Bug Fixes:** Keys, Chain and Strict mode priorities fixes.

## v1.8.1
*   **Bug Fixes:** Compatibility with ToggleEverything mod

## v1.8.0
*   **New Feature:** **Progression Chain**. Define a custom unlock order.
*   **New Feature:** **Strict Chain Mode**. Forces the game to spawn *only* the current target item in the chain.
*   **Bug Fix:** Fixed an issue where UI elements would disappear when switching inventory tabs.
*   **UI Polish:** Updated Toggle Switches to a cleaner "Green/Black" visual style without text.
*   **Feature:** Added a "Safety Net" that forces a Key or Credit Card into a chest if the strict logic accidentally wipes a specific rarity pool completely (prevents the game from spawning fallback junk items).
*   **Logic Overhaul:** Rewrote the Loot Priority System to follow a strict hierarchy:
    1.  **Hard Limits:** (e.g., "10 Keys") always take precedence.
    2.  **Keys First Mode:** If enabled and keys < 10, bans everything except Keys, Legendaries, and Credit Cards.
    3.  **Strict Chain Mode:** If enabled, bans **EVERYTHING** (including Credit Cards & Legendaries) except the current Target Item and Keys.

## v1.5.2
*   **Logic Update:** Added **Credit Cards** (Green and Red) to the whitelist for "Keys First" mode. You can now farm damage and luck while hunting for keys.
*   **Feature:** Implemented "Keys First" logic. When enabled, non-essential items are banned from the loot pool until the Key limit is reached.
*   **Feature:** Added support for "Infinite" items (Limit -1) to bypass the Keys First lockdown.
*   **Bug Fix:** Fixed an issue where the UI would disappear when switching between Tabs (Items/Weapons/Tomes) due to object pooling.
*   **Bug Fix:** Fixed "Ghost UI" where input boxes would persist on empty slots.
*   **Safety:** Added strict type checking to ensure the UI never attaches to Weapons or Characters.
*   **New Feature:** Added **In-Game UI**. You can now edit item limits directly via textboxes on the item icons. Changes apply instantly without restarting.
*   **UI Polish:** Redesigned the in-game UI elements.
    *   Added double-layer borders (White/Black) to input boxes for better visibility on all backgrounds.
    *   Updated the "Keys First" toggle to a clean slider switch (Green/Black) without text.
*   **Refinement:** Textboxes now automatically resize text to fit perfectly within the 26x26 container.

## v1.0.5
*   Initial Release.
*   Dynamic config generation for all items.
*   Base logic for removing capped items from the loot pool.
