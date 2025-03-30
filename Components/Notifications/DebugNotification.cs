using System;
using EFT.Communications;
using Newtonsoft.Json;
using MOAR.Helpers;
#if FIKA
using Fika.Networking;
using Fika.Networking.Models;
#endif

namespace MOAR.Components.Notifications
{
    /// <summary>
    /// Custom debug notification used to display in-game messages during development.
    /// Now multiplayer-safe and can be synchronized across FIKA sessions.
    /// </summary>
    public class DebugNotification : NotificationAbstractClass
    {
        [JsonProperty("notificationIcon")]
        public ENotificationIconType NotificationIcon;

        [JsonProperty("notification")]
        public string Notification;

        public override ENotificationIconType Icon => NotificationIcon;

        public override string Description => Notification;

        /// <summary>
        /// Displays this notification on the local client.
        /// </summary>
        public void Display()
        {
            NotificationManagerClass.DisplayNotification(this);
        }

        /// <summary>
        /// Sends this notification to all clients in a FIKA multiplayer session.
        /// </summary>
        public void BroadcastToClients()
        {
#if FIKA
            if (!Settings.IsFika) return;
            var payload = JsonConvert.SerializeObject(this);
            FikaNetwork.SendToAll("MOAR:Notification", payload);
#endif
        }

        /// <summary>
        /// Serializes the notification to a JSON string.
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Deserializes a notification from a JSON string.
        /// </summary>
        public static DebugNotification FromJson(string json)
        {
            return JsonConvert.DeserializeObject<DebugNotification>(json);
        }

        /// <summary>
        /// Registers the packet handler for receiving remote notifications.
        /// </summary>
        public static void RegisterNetworkHandler()
        {
#if FIKA
            if (!Settings.IsFika) return;
            FikaNetwork.On("MOAR:Notification", (data) =>
            {
                try
                {
                    var notification = FromJson(data);
                    notification.Display();
                }
                catch (Exception ex)
                {
                    Plugin.LogSource.LogError($"[DebugNotification] Failed to handle remote packet: {ex.Message}");
                }
            });
#endif
        }
    }
}
