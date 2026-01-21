# Changelog

## v1.5.2
*   **UI Polish:** Redesigned the in-game UI elements.
    *   Added double-layer borders (White/Black) to input boxes for better visibility on all backgrounds.
    *   Updated the "Keys First" toggle to a clean slider switch (Green/Black) without text.
*   **Refinement:** Textboxes now automatically resize text to fit perfectly within the 26x26 container.

## v1.5.1
*   **Logic Update:** Added **Credit Cards** (Green and Red) to the whitelist for "Keys First" mode. You can now farm economy while hunting for keys.

## v1.5.0
*   **Feature:** Implemented "Keys First" logic. When enabled, non-essential items are banned from the loot pool until the Key limit is reached.
*   **Feature:** Added support for "Infinite" items (Limit -1) to bypass the Keys First lockdown.

## v1.4.2
*   **Bug Fix:** Fixed an issue where the UI would disappear when switching between Tabs (Items/Weapons/Tomes) due to object pooling.
*   **Bug Fix:** Fixed "Ghost UI" where input boxes would persist on empty slots.
*   **Safety:** Added strict type checking to ensure the UI never attaches to Weapons or Characters.

## v1.2.0
*   **New Feature:** Added **In-Game UI**. You can now edit item limits directly via textboxes on the item icons. Changes apply instantly without restarting.

## v1.0.0
*   Initial Release.
*   Dynamic config generation for all items.
*   Base logic for removing capped items from the loot pool.