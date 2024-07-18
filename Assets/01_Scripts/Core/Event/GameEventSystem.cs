using System.Collections.Generic;
using System.Reflection;
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

        WriteLog(sender, listener.GetMethodInfo().Name, EventState.Unsubscribe, eventType);
    }
    
    // 이벤트 구독 취소
    public void Unsubscribe(object sender, GameEventType eventType, Action listener) 
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent -= listener;
            eventDictionary[eventType] = thisEvent;

            WriteLog(sender, listener.GetMethodInfo().Name, EventState.Unsubscribe, eventType);
        }
    }

    // 이벤트 발행
    public void PublishEvent(object sender, string where, GameEventType eventType)
    {
        Action thisEvent;

        if (eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent?.Invoke();

            WriteLog(sender, where, EventState.Publish, eventType);
        }
    }

    // 로그 생성
    private void WriteLog(object sender, string eventMeshodName, EventState eventState, GameEventType eventType)
    {
        var log = new EventSubscribeLog(sender, eventMeshodName, eventState, eventType);
        eventSubscribeLogs.Add(log);
    }

}
