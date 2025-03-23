using Comfort.Common;
using EFT;
using EFT.Communications;
using MOAR.Components.Notifications;
using SPT.Reflection.Utils;

namespace MOAR.Helpers
{
    public class Methods
    {
        public static void DisplayMessage(
            string message,
            ENotificationIconType notificationIcon = ENotificationIconType.Quest
        )
        {
            DebugNotification currentMessage = new()
            {
                Duration = ENotificationDurationType.Long,
                Notification = message,
                NotificationIcon = notificationIcon
            };

            NotificationManagerClass.DisplayNotification(currentMessage);
        }

        public static async void RefreshLocationInfo()
        {
            if (PatchConstants.BackEndSession != null)
                await PatchConstants.BackEndSession.GetLevelSettings();
            // await PatchConstants.BackEndSession.GetWeatherAndTime();
        }

        public static AddSpawnRequest GetPlayersCoordinatesAndLevel()
        {
            var position = Singleton<GameWorld>.Instance.MainPlayer.Position;
            var location = Singleton<GameWorld>.Instance.MainPlayer.Location;

            return new AddSpawnRequest
            {
                map = location,
                position = new Ixyz
                {
                    x = position.x,
                    y = position.y,
                    z = position.z,
                },
            };
        }
    }
}
