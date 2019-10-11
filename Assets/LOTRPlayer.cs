using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LOTRPlayer
{
    private List<LOTRHero> heroes;
    private List<PlayerCard> ally_cards;
    private List<PlayerCard> cards;
    private List<PlayerCard> cards_in_hand;
    private List<PlayerCard> discard_pile;
    private int threat;
    private List<EnemyCard> engaged_enemies;
    private List<LOTRAbility> abilities;

    public LOTRPlayer()
    {
        threat = 0;
        heroes = new List<LOTRHero>();
        cards = new List<PlayerCard>();
        engaged_enemies = new List<EnemyCard>();
        ally_cards = new List<PlayerCard>();
        cards_in_hand = new List<PlayerCard>();
        discard_pile = new List<PlayerCard>();
        cards = PlayerCard.LEADERSHIP_CARDS();
        Utils.Shuffle(cards);
        abilities = new List<LOTRAbility>();
    }

    public void add_ability(LOTRAbility ability)
    {
        abilities.Add(ability);
    }

    public void add_resource_token_to_all_heroes()
    {
        foreach (var hero in heroes)
        {
            hero.add_resource_token();
        }
    }

    public void discard_all_cards(string type)
    {
        foreach (var card in cards_in_hand)
        {
            if (card.get_type() == type)
            {
                discard_pile.Add(card);
            }
        }
        cards_in_hand.RemoveAll(x => x.get_type() == type);
    }
    
    public void discard_card_at_index(int index)
    {
        PlayerCard card_to_discard = cards_in_hand[index];
        discard_pile.Add(card_to_discard);
        cards_in_hand.RemoveAt(index);
    }
    
    public void ally_died(PlayerCard ally)
    {
        int card_index = -1;
        for (var i = 0; i < ally_cards.Count; i++)
        {
            PlayerCard card = ally_cards[i];
            if (card == ally)
            {
                discard_pile.Add(card);
                break;
            }
        }

        if (card_index != -1)
        {
            ally_cards.RemoveAt(card_index);
        }
    }

    public void draw_card()
    {
        PlayerCard card_to_draw = cards[0];
        cards_in_hand.Add(card_to_draw);
        card_to_draw.enters_the_game();
        Debug.Log(card_to_draw.get_name() + " AHS ENTERED THE GAME");
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

    public bool has_hero_of_sphere(LOTRGame.SPHERE_OF_INFLUENCE sphere)
    {
        foreach (var hero in heroes)
        {
            if (hero.get_sphere() == sphere)
            {
                return true;
            }
        }

        return false;
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

    public void discard_attachment(AttachmentCard attachment)
    {
        foreach (PlayerCard card in all_characters())
        {
            if(card.has_attachment(attachment))
            {
                card.discard_attachment(attachment);
                break;
            }
        }
    }

    public List<PlayerCard> all_characters()
    {
        List<PlayerCard> result = new List<PlayerCard>();
        result.AddRange(get_allies());
        foreach (PlayerCard hero in get_heroes())
        {
            result.Add(hero);
        }

        return result;
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

    public int get_willpower_committed(LOTRGame.GAMEPHASE cur_phase)
    {
        var result = 0;
        foreach (var h in heroes)
        {
            if (h.is_committed())
            {
                result += h.get_willpower();
            }
        }
        
        foreach (var h in ally_cards)
        {
            if (h.is_committed())
            {
                result += h.get_willpower();
            }
        }

        return result;
    }

    public void new_phase_started()
    {
        //sneak attack
        List<int> sneak_attacked_card_indices = new List<int>();
        for (var i = 0; i < ally_cards.Count; i++)
        {
            PlayerCard card = ally_cards[i];
            if (card.has_flag(LOTRAbility.ABILITY_FLAGS.SNEAK_ATTACK))
            {
                Debug.Log("SNEAKED");
                sneak_attacked_card_indices.Add(i);
                add_card_to_hand(card);
            }
        }
        foreach (var sneak_attack_index in sneak_attacked_card_indices)
        {
            ally_cards.RemoveAt(sneak_attack_index);
        }
        reset_all_flags();

    }

    private void reset_all_flags()
    {
        foreach (var card in heroes)
        {
            card.reset_flags();
        }
        foreach (var card in ally_cards)
        {
            card.reset_flags();
        }
        foreach (var card in cards_in_hand)
        {
            card.reset_flags();
        }
        
    }

    public List<AttachmentCard> get_all_attachments()
    {
        List<AttachmentCard> result = new List<AttachmentCard>();
        foreach (var character in all_characters())
        {
            result.AddRange(character.get_attachments());
        }

        return result;
    }
    public void add_card_to_hand(PlayerCard card)
    {
        cards_in_hand.Add(card);
        card.enters_the_game();
    }

    public bool has_ability(LOTRAbility ability)
    {
        return abilities.Contains(ability);
    }

    public bool can_play_actions(EventArgs args)
    {
        foreach (var card in cards_in_hand)
        {
            if (card.action_playable(args))
            {
                return true;
            }
        }
        foreach (var card in ally_cards)
        {
            if (card.action_playable(args))
            {
                return true;
            }
        }
        foreach (var card in heroes)
        {
            if (card.action_playable(args))
            {
                return true;
            }
        }

        return false;
    }

    public void clear_abilities_on_phase_change(LOTRGame.GAMEPHASE new_phase)
    {
        List<int> indicies_to_remove = new List<int>();
        for (var i = 0; i <  abilities.Count; i++)
        {
            var ability = abilities[i];
            if (ability.expires_on_phase_change() && !ability.active_on_phase(new_phase))
            {
                indicies_to_remove.Add(i);
            }
        }

        foreach (var index in indicies_to_remove)
        {
            abilities.RemoveAt(index);
        }
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
        hero.enters_the_game();
        this.threat += hero.get_threat_level();
    }

    public List<LOTRHero> get_heroes()
    {
        return this.heroes;
    }
}
