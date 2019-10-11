using System;
using System.Collections.Generic;
using System.ComponentModel;

public class GameEvent
{
 public static EventHandlerList listEventDelegates = new EventHandlerList();

    public static readonly object CARD_PLAYED_KEY = new object();
    public static readonly object CARD_COMMITTED_KEY = new object();
    public static readonly object CHARACTER_TOOK_DAMAGE = new object();
    public static readonly object ACTIVATED_ACTION = new object();
    public static readonly object CARD_REVEALED = new object();
    public static readonly object CARD_REVEALED_ALLOW_COUNTER = new object();
    public static readonly object LOCATION_TRAVELED = new object();
    public static readonly object CARD_LEAVES_PLAY = new  object();
    public static readonly object SHADOW_CARD_DEALT = new  object();
    public static readonly object SHADOW_CARD_REVEALED = new  object();
    public static readonly object ENEMY_ATTACK_RESOLVED = new  object();
    public static readonly object ENEMY_ENGAGED = new  object();
    public static readonly object OPTION_1_PICKED = new  object();
    public static readonly object OPTION_2_PICKED = new  object();
    public static readonly object OPTION_3_PICKED = new  object();
    
    public static Dictionary<object, string> EVENT_DICT = new Dictionary<object, string>()
    {
        {CARD_PLAYED_KEY, "CARD_PLAYED_KEY"},
        {CARD_COMMITTED_KEY, "CARD_COMMITTED_KEY"},
        {CHARACTER_TOOK_DAMAGE, "CHARACTER_TOOK_DAMAGE"},
        {ACTIVATED_ACTION, "CARD_PLAYED_KEY"},
        {CARD_REVEALED, "CARD_REVEALED"},
        {CARD_REVEALED_ALLOW_COUNTER, "CARD_REVEALED_ALLOW_COUNTER"},
        {LOCATION_TRAVELED, "LOCATION_TRAVELED"},
        {CARD_LEAVES_PLAY, "CARD_LEAVES_PLAY"},
        {SHADOW_CARD_DEALT, "SHADOW_CARD_DEALT"},
        {SHADOW_CARD_REVEALED, "SHADOW_CARD_REVEALED"},
        {ENEMY_ATTACK_RESOLVED, "ENEMY_ATTACK_RESOLVED"},
        {ENEMY_ENGAGED, "ENEMY_ENGAGED"},
        {OPTION_1_PICKED, "OPTION_1_PICKED"},
        {OPTION_2_PICKED, "OPTION_2_PICKED"},
        {OPTION_3_PICKED, "OPTION_3_PICKED"},
    };
}




