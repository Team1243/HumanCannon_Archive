using System;

public enum EventState
{
    Subscribe,
    Unsubscribe,
    Publish
}

[Serializable]
public struct EventSubscribeLog
{
    public string Sender;
    public string EventMethodName;
    public string EventType;
    public string State;
    public string Time;
    
    public EventSubscribeLog(object sender, string eventMethodName, EventState eventState, GameEventType eventType)
    {
        Sender = sender.GetType().Name;
        EventMethodName = eventMethodName; 
        EventType = eventType.ToString();
        State = eventState.ToString();
        Time = DateTime.Now.ToString(("HH:mm:ss"));
    }

    public string GetLogString()
    {
        string message = $"{Time} : {Sender} - {State} {EventType}";
        return message;
    }
}
