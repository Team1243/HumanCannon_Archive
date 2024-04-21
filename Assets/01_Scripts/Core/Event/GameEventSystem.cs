using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEventSystem : MonoSingleton<GameEventSystem>
{
    private IDictionary<GameEventType, Action> eventDictionary = new Dictionary<GameEventType, Action>();

    [SerializeField] private List<EventSubscribeLog> eventSubscribeLogs = new ();

    // 이벤트 구독
    public void Subscribe(object sender, GameEventType eventType, Action listener)
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent -= listener;
            thisEvent += listener;
            eventDictionary[eventType] = thisEvent;
        }
        else
        {
            eventDictionary.Add(eventType, listener);
        }

        WriteLog(sender, EventState.Subscribe, eventType);
    }
    
    // 이벤트 구독 취소
    public void Unsubscribe(object sender, GameEventType eventType, Action listener) 
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventType] = thisEvent;

            WriteLog(sender, EventState.Unsubscribe, eventType);
        }
    }

    // 이벤트 발생
    public void PublishEvent(object sender, GameEventType eventType)
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent?.Invoke();

            WriteLog(sender, EventState.Publish, eventType);
        }
    }

    // 로그 작성 (어디서 보내주는 것이고, 어떤 행동을 할 것이며, 어느 상태에 그 행동을 할 것인지)
    private void WriteLog(object sender, EventState eventState, GameEventType eventType)
    {
        var log = new EventSubscribeLog(sender, eventState, eventType);
        eventSubscribeLogs.Add(log);
    }
}
