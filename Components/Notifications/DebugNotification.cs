using EFT.Communications;
using Newtonsoft.Json;

namespace MOAR.Components.Notifications;

public class DebugNotification : NotificationAbstractClass
{
    public override ENotificationIconType Icon => ENotificationIconType.EntryPoint;

    public override string Description => Notification;

    [JsonProperty("notificationIcon")]
    public ENotificationIconType NotificationIcon;
    
    [JsonProperty("notification")]
    public string Notification;
}