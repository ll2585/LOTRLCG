public class LOTRAbility
{
    private bool does_activate_on_taking_damage;
    private LOTRGame game;
    private LOTRHero hero;
    private LOTRPlayer player;

    public LOTRAbility(bool activates_on_taking_damage = false)
    {
        this.does_activate_on_taking_damage = activates_on_taking_damage;
    }

    public bool activates_on_taking_damage()
    {
        return this.does_activate_on_taking_damage;
    }

    public void set_game(LOTRGame game)
    {
        this.game = game;
    }
    public void set_hero(LOTRHero hero)
    {
        this.hero = hero;
    }
    public void set_player(LOTRPlayer player)
    {
        this.player = player;
    }
    
    //TODO: make factories? make singletons?
    
    public LOTRAbility GLOIN_ABILITY = new LOTRAbility(activates_on_taking_damage: true);
}
