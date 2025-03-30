using System;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace MOAR.Helpers
{
    /// <summary>
    /// Utility extension methods for KeyboardShortcut input checks with modifier support.
    /// </summary>
    public static class UIUtils
    {
        /// <summary>
        /// Returns true while the key is held down with all modifiers.
        /// </summary>
        public static bool BetterIsPressed(this KeyboardShortcut key) =>
            key.MainKey != KeyCode.None && Input.GetKey(key.MainKey) && ModifiersHeld(key);

        /// <summary>
        /// Returns true only on the frame the key was pressed, with all modifiers.
        /// </summary>
        public static bool BetterIsDown(this KeyboardShortcut key) =>
            key.MainKey != KeyCode.None && Input.GetKeyDown(key.MainKey) && ModifiersHeld(key);

        /// <summary>
        /// Checks if all modifier keys are currently held.
        /// </summary>
        private static bool ModifiersHeld(KeyboardShortcut key)
        {
            if (key.Modifiers == null || !key.Modifiers.Any())
                return true;

            foreach (KeyCode mod in key.Modifiers)
            {
                if (!Input.GetKey(mod))
                    return false;
            }

            return true;
        }

    }
}
