using System;
using BepInEx;
using BepInEx.Configuration;
using EFT.Communications;
using SPT.Reflection.Utils;

namespace MOAR.Helpers
{
    public class Methods
    {
        public static void DisplayMessage(
            string message,
            ENotificationIconType notificationType = ENotificationIconType.Quest
        )
        {
            var currentMessage = new GClass2269(
                message,
                ENotificationDurationType.Long,
                notificationType
            );

            NotificationManagerClass.DisplayNotification(currentMessage);
        }

        public static async void RefreshLocationInfo()
        {
            try
            {
                await PatchConstants.BackEndSession.GetLevelSettings();
                // await PatchConstants.BackEndSession.GetWeatherAndTime();
            }
            catch (NullReferenceException)
            {
                Plugin.LogSource.LogError("[MOAR] Unable to refresh location info. User is most likely in menu.");
            }
        }

        public static bool IsKeyPressed(KeyboardShortcut key)
        {
            return UnityInput.Current.GetKeyDown(key.MainKey);
        }
    }
}
