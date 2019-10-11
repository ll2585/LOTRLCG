public class LOTRAbility
{
    private LOTRGame.GAMEPHASE relevant_phase;
    private bool expire_on_phase_change;

    public enum ABILITY_FLAGS
    {
        FARAMIR,
        SNEAK_ATTACK,
        FOR_GONDOR,
        UNGOLIANTS_SPAWN,
        CHIEFTAN_UFTHAK,
        PLUS_ONE_ATTACK_ENEMY,
        PLUS_THREE_ATTACK_ENEMY,
        PLUS_ONE_THREAT,
        FOREST_SPIDER,
        CELEBRIANS_STONE
    }
    public enum CLEAR_MARKERS
    {
        END_OF_ROUND,
        END_OF_PHASE
    }
    public LOTRAbility()
    {
        expire_on_phase_change = false;
    }
    

    public bool expires_on_phase_change()
    {
        return expire_on_phase_change;
    }

    public bool active_on_phase(LOTRGame.GAMEPHASE phase)
    {
        return relevant_phase == phase;
    }
}

