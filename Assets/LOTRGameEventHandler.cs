
using System;
using System.Collections.Generic;
using UnityEngine;

public class LOTRGameEventHandler
{
    private LOTRGame game;
    private List<LOTRDispatcher> card_observers;

    public LOTRGameEventHandler(LOTRGame game)
    {
        this.game = game;
        card_observers = new List<LOTRDispatcher>();
    }

    public void register_cards(List<PlayerCard> cards)
    {
        foreach(var card in cards)
        {
            if (card.has_card_observer() && !card_observers.Contains(card.get_card_observer()))
            {
                card_observers.Add(card.get_card_observer());
            }
        }
    }

    public void fire_event(GameEvent e, GameArgs data)
    {
        foreach (var observer in card_observers)
        {
            observer.dispatch(e, data);
        }
    }

    
}

