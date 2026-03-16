using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus: Singleton<EventBus>
{
    // Ham luu tru cac su kien dang ky
    private readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();


    // Dang ky nghe

    public void Subscribe<T>(Action<T> listener) where T:IEvent
    {
        // Lay kieu du lieu truyen di
        Type eventType = typeof(T);
        if (_events.ContainsKey(eventType))
        {
            // Gan su kien lang nghe
            _events[eventType] = Delegate.Combine(_events[eventType], listener);
        }

        else
        {
            // Them su kien lang nghe
            _events.Add(eventType, listener);
        }
    }

    //Huy dang ky

    public void UnSubscribe<T>(Action<T> listener) where T : IEvent
    {
        Type eventType = typeof(T);
        if (_events.ContainsKey(eventType))
        {
            var currentDelegate = _events[eventType];
            currentDelegate = Delegate.Remove(currentDelegate, listener);
            if(currentDelegate == null)
            {
                _events.Remove(eventType);
            }
            else
            {
                _events[eventType] = currentDelegate;
            }
        }
    }

    // Phat song

    public void Publish<T>(T eventData) where T: IEvent
    {
        Type eventType = typeof(T);

        if (_events.TryGetValue(eventType, out var currentDelegate))
        {
            // kich hoat cac ham dang lang nghe eventType
            (currentDelegate as Action<T>)?.Invoke(eventData);
        }
    }
}
