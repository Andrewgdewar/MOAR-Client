using System;
using EFT.Communications;
using Newtonsoft.Json;

namespace MOAR.Components.Notifications
{
    /// <summary>
    /// Custom debug notification used to display in-game messages during development.
    /// </summary>
    public class DebugNotification : NotificationAbstractClass
    {
        [JsonProperty("notificationIcon")]
        public ENotificationIconType NotificationIcon;

        [JsonProperty("notification")]
        public string Notification;

        public override ENotificationIconType Icon => ENotificationIconType.EntryPoint;

        public override string Description => Notification;
    }
}
