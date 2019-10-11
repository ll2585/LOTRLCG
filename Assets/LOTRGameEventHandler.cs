
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LOTRGameEventHandler
{
    private LOTRGame game;
    public Action callback;
    private List<GameEventBundle> events_to_fire;

    public LOTRGameEventHandler(LOTRGame game)
    {
        this.game = game;
        events_to_fire = new List<GameEventBundle>();
    }


    public void fire_game_event(object key, GameArgs args)
    {
        
        Debug.Log("FIRIGN " + GameEvent.EVENT_DICT[key]);
        
        EventHandler e =
            (EventHandler)GameEvent.listEventDelegates[key];
        GameEventBundle this_bundle = new GameEventBundle(key, args, e, args.what_to_do_after_event_or_if_no_response, false);
        events_to_fire.Add(this_bundle);
        execute_event(this_bundle);

    }

    private void execute_event(GameEventBundle bundle, bool is_resuming = false)
    {
        GameEventBundle this_bundle = bundle;
        object key = bundle.key;
        EventHandler e = bundle.e;
        GameArgs args = bundle.args;
        if (game.is_waiting_for_player_response())
        {
            Debug.Log("We are waiting for a response before " + GameEvent.EVENT_DICT[key]);
            Debug.Log("Adding to queue or stack");
        }
        else
        {
            Debug.Log("not waiting for response, so go ahead.");
            if (e != null && !this_bundle.event_fired_already)
            {
                Debug.Log("SOMEONE is responding" + GameEvent.EVENT_DICT[key]);
                e(this, args);
                this_bundle.event_fired_already = true;
            }
            else
            {
                Debug.Log("event handler is null meaning no one responded." + GameEvent.EVENT_DICT[key]);
            }

            if (game.is_waiting_for_player_response() && !is_resuming)
            {
                Debug.Log("Now we are waiting for a response after " + GameEvent.EVENT_DICT[key]);
            }
            else
            {
                fire_callback(key, args.what_to_do_after_event_or_if_no_response, this_bundle);
                if (is_resuming)
                {
                    resume_handling();
                }
            }
            Debug.Log("Finished firing event " + GameEvent.EVENT_DICT[key]);
        }
        Debug.Log("Bundle for " + GameEvent.EVENT_DICT[key] + " " + args.relevant_card.get_name());
        Debug.Log("==============================");
        foreach (GameEventBundle b in events_to_fire)
        {
            Debug.Log(b.ToString());
            
        }
        Debug.Log("==============================");
        
    }

    public void resume_handling()
    {
        if (events_to_fire.Count > 0)
        {
            execute_event(events_to_fire[events_to_fire.Count - 1], is_resuming:true);//stack
        }
    }

    private void fire_callback(object key, Action callback, GameEventBundle the_bundle)
    {
        Debug.Log("Firing callback for " + GameEvent.EVENT_DICT[key] + " " + the_bundle.args.relevant_card.get_name());
        Debug.Log(events_to_fire.Contains(the_bundle));
        events_to_fire.Remove(the_bundle);
        if (callback != null)
        {
            Debug.Log("Running the callback");
            callback();
        }
        
    }

    


    public static void add_handler_to_event_name(object event_key, EventHandler e)
    {
        EventHandler event_delegate =
            (EventHandler)GameEvent.listEventDelegates[event_key];
        GameEvent.listEventDelegates.AddHandler(event_key, e);
    }
    
    public class GameEventBundle
    {
        public object key;
        public GameArgs args;
        public EventHandler e;
        public Action callback;
        public bool event_fired_already;
        public GameEventBundle(object key, GameArgs args, EventHandler e, Action callback, bool event_fired_already)
        {
            this.key = key;
            this.args = args;
            this.e = e;
            this.callback = callback;
            this.event_fired_already = event_fired_already;
        }

        public override string ToString()
        {
            string result = "";
            Debug.Log(args.relevant_card);
            result += "event " + GameEvent.EVENT_DICT[key] + " with args-card" + args.relevant_card.get_name();
            return result;
        }  
    }


}

public class GameArgs:EventArgs
{
    public LOTRGame g { get; set; }
    public Card relevant_card { get; set; }
    public readonly int? relevant_int;
    public Action what_to_do_after_event_or_if_no_response { get; set;  }
    public LOTRPlayer relevant_player { get; set; }
    public bool? attack_undefended { get; set; } //only used for shadow
    public Card secondary_card { get; set;  }
    
    public GameArgs(Card c = null, int? i = null, Action a = null, LOTRGame game = null,
        LOTRPlayer p = null, bool? attack_undefended = null, Card secondary_card = null)
    {
        relevant_card = c;
        relevant_int = i;
        what_to_do_after_event_or_if_no_response = a;
        g = game;
        relevant_player = p;
        this.attack_undefended = attack_undefended;
        this.secondary_card = secondary_card;
    }
}