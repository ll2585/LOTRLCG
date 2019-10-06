using System.Collections.Generic;

public class GameEvent
{
    private GAME_EVENT_TYPE event_type;
    private GameArgs data;

    public enum GAME_EVENT_TYPE
    {
        COMMITTED,
        TAKE_DAMAGE,
        CARD_ENTERS_PLAY,
        CARD_LEAVES_PLAY
    }
    private GameEvent(GAME_EVENT_TYPE event_type, GameArgs data = null)
    {
        this.event_type = event_type;
        this.data = data;
    }
    public static List<GameEvent> instances = new List<GameEvent>();

    public static GameEvent get_instance(GAME_EVENT_TYPE event_type, GameArgs data = null)
    {
        foreach (GameEvent instance in instances)
        {
            if (instance.event_type == event_type && instance.data == data)
            {
                return instance;
            }
        }

        GameEvent new_instance = new GameEvent(event_type, data);
        instances.Add(new_instance);
        return new_instance;
    }

    public GAME_EVENT_TYPE get_event_type()
    {
        return event_type;
    }
    
    public object get_data()
    {
        return data;
    }
}
