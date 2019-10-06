
using System;
using System.Collections.Generic;
using UnityEngine;

public class LOTRDispatcher
{
    private List<GameEvent> event_list;
    private List<LOTRDispatcherEvent> dispatched_events;

    public LOTRDispatcher()
    {
        event_list = new List<GameEvent>();
        dispatched_events = new List<LOTRDispatcherEvent>();
    }

    public void dispatch(GameEvent e, GameArgs data)
    {
        for (var i = 0; i < event_list.Count; i++)
        {
            GameEvent the_event = event_list[i];
            if (the_event.get_event_type() == e.get_event_type())
            {
                if (the_event.get_data() == null || the_event.get_data() == e.get_data())
                {
                    dispatched_events[i].fire(data);
                }
            }
        }
    }

    public void on(GameEvent e, Action<GameArgs> callback)
    {
        LOTRDispatcherEvent dispatched_event = null;
        for (var i = 0; i < event_list.Count; i++)
        {
            GameEvent the_event = event_list[i];
            if (the_event.get_event_type() == e.get_event_type())
            {
                if (the_event.get_data() == null || the_event.get_data() == e.get_data())
                {
                    dispatched_event = dispatched_events[i];
                    break;
                }
            }
        }

        if (dispatched_event == null)
        {
            this.event_list.Add(e);
            dispatched_event = new LOTRDispatcherEvent(e);
            dispatched_events.Add(dispatched_event);
        }
        dispatched_event.register_callback(callback);
    }

    public bool listening_to_event(GameEvent e)
    {
        for (var i = 0; i < event_list.Count; i++)
        {
            GameEvent the_event = event_list[i];
            if (the_event.get_event_type() == e.get_event_type())
            {
                if (the_event.get_data() == null || the_event.get_data() == e.get_data())
                {
                    return true;
                }
            }
        }

        return false;
    }
}


public class LOTRDispatcherEvent
{
    private GameEvent e;
    private List<Action<GameArgs>> callbacks;

    
        
    public LOTRDispatcherEvent(GameEvent e)
    {
        this.e = e;
        this.callbacks = new List<Action<GameArgs>>();
    }

    public void register_callback(Action<GameArgs> callback)
    {
        this.callbacks.Add(callback);
    }

    public void fire(GameArgs data)
    {
        foreach (var callback in callbacks)
        {
            if (data != null)
            {
                callback(data);
            }
            else
            {
                callback(null);
            }
        }
    }
}
public class GameArgs:EventArgs
{
    public readonly Card relevant_card;
    public readonly int? relevant_int;
    public readonly Action what_to_do_after_event_or_if_no_response;
        
    public GameArgs(Card c=null, int? i = null, Action a = null)
    {
        relevant_card = c;
        relevant_int = i;
        what_to_do_after_event_or_if_no_response = a;
    }
        
}
