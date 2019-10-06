using System.Collections.Generic;
using UnityEngine;

public class LOTRPlayer
{
    private List<LOTRHero> heroes;
    private List<PlayerCard> ally_cards;
    private List<PlayerCard> cards;
    private List<PlayerCard> cards_in_hand;
    private int threat;
    private List<EnemyCard> engaged_enemies;

    public LOTRPlayer()
    {
        threat = 0;
        heroes = new List<LOTRHero>();
        cards = new List<PlayerCard>();
        engaged_enemies = new List<EnemyCard>();
        ally_cards = new List<PlayerCard>();
        cards_in_hand = new List<PlayerCard>();
        cards = PlayerCard.LEADERSHIP_CARDS();
    }

    public void add_resource_token_to_all_heroes()
    {
        foreach (var hero in heroes)
        {
            hero.add_resource_token();
        }
    }

    public void draw_card()
    {
        cards_in_hand.Add(cards[0]);
        cards.RemoveAt(0);
    }

    public List<PlayerCard> get_cards_in_hand()
    {
        return cards_in_hand;
    }

    public void increase_threat(int amt_to_increase)
    {
        this.threat += amt_to_increase;
    }

    public List<EnemyCard> get_engaged_enemies()
    {
        return this.engaged_enemies;
    }
    public void engage_enemy(EnemyCard enemy)
    {
        engaged_enemies.Add(enemy);
    }

    public void reset_engaged_enemy_resolutions()
    {
        foreach (EnemyCard enemy in engaged_enemies)
        {
            enemy.reset_for_combat_phase();
        }
    }

    public List<PlayerCard> get_allies()
    {
        return this.ally_cards;
    }

    public void play_card(PlayerCard card)
    {
        if (card.is_ally())
        {
            ally_cards.Add(card);
        }//TODO: events and also attachments
        for(var i = 0; i < cards_in_hand.Count; i++)
        {
            var the_card = cards_in_hand[i];
            if (the_card == card)
            {
                cards_in_hand.RemoveAt(i);
            }
        }
    }

    public void ready_all_exhausted_cards()
    {
        foreach (var hero in heroes)
        {
            hero.ready();
        }
        foreach(var card in ally_cards)
        {
            card.ready();
        }
    }

    public void enemy_attack_resolved(EnemyCard enemy)
    {
        foreach (EnemyCard e in engaged_enemies)
        {
            if (enemy == e)
            {
                e.resolve_attack();
            }
        }
    }

    public bool has_engaged_enemies()
    {
        foreach (EnemyCard enemy in engaged_enemies)
        {
            if (!enemy.is_attack_resolved())
            {
                return true;
            }
        }

        return false;
    }

    public bool has_enemies_to_attack()
    {
        return engaged_enemies.Count > 0;
    }

    public void enemy_defeated(EnemyCard enemy)
    {
        for (var i = 0; i < engaged_enemies.Count; i++)
        {
            EnemyCard e = engaged_enemies[i];
            if (e == enemy)
            {
                engaged_enemies.RemoveAt(i);
            }
        }
    }

    public int get_threat_level()
    {
        return this.threat;
    }

    public int get_willpower_committed()
    {
        var result = 0;
        foreach (var h in heroes)
        {
            if (h.is_committed())
            {
                result += h.get_willpower();
            }
        }

        return result;
    }

    public bool has_responses(GameEvent e)
    {
    List<PlayerCard> all_cards = new List<PlayerCard>();
    all_cards.AddRange(heroes);
    all_cards.AddRange(ally_cards);
    all_cards.AddRange(cards);
        foreach (var card in all_cards)
        {
            if (card.responds_to_event(e))
            {
                return true;
            }
        }

        return false;
    }

    public void debug()
    {
        foreach (var hero in heroes)
        {
            if (hero.get_name() == "Gloin")
            {
                hero.set_resources(3);
            }
            else
            {
                hero.set_resources(2);
            }
        }
        
    }

    public Dictionary<LOTRHero, int>  get_resources()
    {
        Dictionary<LOTRHero, int> result = new Dictionary<LOTRHero, int>();
        //TODO: check to make sure this player has this hero
        foreach (var hero in heroes)
        {
            result[hero] = hero.get_resources();
        }
        return result;
    }

    public void add_hero(LOTRHero hero)
    {
        heroes.Add(hero);
        this.threat += hero.get_threat_level();
    }

    public List<LOTRHero> get_heroes()
    {
        return this.heroes;
    }
}
