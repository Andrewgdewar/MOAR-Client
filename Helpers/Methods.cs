using System;
using System.Linq;
using System.Threading.Tasks;
using Comfort.Common;
using EFT;
using EFT.Communications;
using MOAR.Components.Notifications;
using SPT.Reflection.Utils;
using UnityEngine;

namespace MOAR.Helpers
{
    /// <summary>
    /// Common utility methods for UI messaging, location sync, and player coordinate capture.
    /// </summary>
    public static class Methods
    {
        /// <summary>
        /// Displays an in-game notification and safely broadcasts it in multiplayer if FIKA is present.
        /// </summary>
        public static void DisplayMessage(string message, ENotificationIconType icon = ENotificationIconType.Quest)
        {
            var notification = new DebugNotification
            {
                Notification = message,
                NotificationIcon = icon
            };

            notification.Display();
            notification.BroadcastToClients();
        }

        /// <summary>
        /// Triggers a location info refresh by querying the backend session.
        /// </summary>
        public static async Task RefreshLocationInfo()
        {
            try
            {
                if (PatchConstants.BackEndSession != null)
                    await PatchConstants.BackEndSession.GetLevelSettings();
            }
            catch (Exception ex)
            {
                Plugin.LogSource.LogError($"[RefreshLocationInfo] {ex.Message}");
            }
        }

        /// <summary>
        /// Returns current player position and location for use in spawn requests.
        /// </summary>
        public static AddSpawnRequest GetPlayersCoordinatesAndLevel()
        {
            var player = Singleton<GameWorld>.Instance?.MainPlayer;

            if (player == null)
            {
                Plugin.LogSource.LogWarning("[GetPlayersCoordinatesAndLevel] MainPlayer is null");
                return new AddSpawnRequest { map = "Unknown", position = new Ixyz() };
            }

            Vector3 pos = player.Position;
            return new AddSpawnRequest
            {
                map = player.Location ?? "Unknown",
                position = new Ixyz
                {
                    x = pos.x,
                    y = pos.y,
                    z = pos.z
                }
            };
        }

        /// <summary>
        /// Checks if the announce key is pressed and displays the current preset name.
        /// </summary>
        public static void CheckAnnounceKey()
        {
            if (Settings.AnnounceKey?.Value.BetterIsDown() == true)
            {
                Settings.AnnounceManually();
            }
        }
    }
}
