namespace WeatherAlertsBot.DAL.Entities;

public class SubscriberCommands
{
    public long Id { get; set; }
    public long SubscriberId { get; set; }
    public long CommandId { get; set; }

    public Subscriber Subscriber { get; set; }
    public Command Command { get; set; }
}