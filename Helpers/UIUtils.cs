using System;
using BepInEx.Configuration;
using UnityEngine;

namespace MOAR.Helpers
{
    /// <summary>
    /// Utility extension methods for KeyboardShortcut input checks with modifier support.
    /// </summary>
    public static class UIUtils
    {
        public static bool BetterIsPressed(this KeyboardShortcut key) =>
            Input.GetKey(key.MainKey) && AllModifiersHeld(key);

        public static bool BetterIsDown(this KeyboardShortcut key) =>
            Input.GetKeyDown(key.MainKey) && AllModifiersHeld(key);

        private static bool AllModifiersHeld(KeyboardShortcut key)
        {
            foreach (KeyCode modifier in key.Modifiers)
            {
                if (!Input.GetKey(modifier)) return false;
            }
            return true;
        }
    }
}
