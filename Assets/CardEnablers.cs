using System;
using System.Collections.Generic;
using UnityEngine;

public class CardEnablers
{
        
        private static LOTRGame game;
        
        public static void set_game(LOTRGame g)
        {
            game = g;
        }

        public static bool card_is_me(EventArgs args, Card me)
        {
            Card committed = ((GameArgs) args).relevant_card;
            return committed == me;
        }
        
        public static bool i_am_not_exhausted(EventArgs args, Card me)
        {
            return  !((PlayerCard) me).is_exhausted();
        }
        
        public static bool i_am_played(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            if (me != null)
            {
                //Debug.Log("CHECKING FOR ME " + me.get_name());
            }
            
            LOTRPlayer the_player = the_args.relevant_player;
            List<LOTRHero> heroes = the_player.get_heroes();
            List<PlayerCard> allies = the_player.get_allies();
            foreach(var hero in heroes)
            {
                //Debug.Log("IS " + hero.get_name() + hero.get_uuid() + " ME " + me.get_name() + me.get_uuid());
                if (hero == me)
                {
                    return true;
                }

                if (((PlayerCard) me).is_attachment())
                {
                    AttachmentCard attachment_me = (AttachmentCard) me;
                    if (hero.has_attachment(attachment_me))
                    {
                        return true;
                    }
                }
            }
            foreach(var ally in allies)
            {
                //Debug.Log("IS " + ally.get_name() + ally.get_uuid()  + " ME " + me.get_name()+ me.get_uuid());
                if (ally == me)
                {
                    return true;
                }
                if (((PlayerCard) me).is_attachment())
                {
                    AttachmentCard attachment_me = (AttachmentCard) me;
                    if (ally.has_attachment(attachment_me))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        
        public static bool i_am_in_hand(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_cards_in_hand())
            {
                if (card == me) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool has_exhausted_allies(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_allies())
            {
                if (card.is_exhausted()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool has_exhausted_characters(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_allies())
            {
                if (card.is_exhausted()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            foreach (var card in the_player.get_heroes())
            {
                if (card.is_exhausted()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        public static bool has_at_least_one_ready_hero(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_heroes())
            {
                if (!card.is_exhausted()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        public static bool has_at_least_one_ready_character(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_heroes())
            {
                if (!card.is_exhausted()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            foreach (var card in the_player.get_allies())
            {
                if (!card.is_exhausted()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        public static bool has_at_least_one_character_committed(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_heroes())
            {
                if (card.is_committed()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            foreach (var card in the_player.get_allies())
            {
                if (card.is_committed()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        public static bool has_ally_in_hand(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_cards_in_hand())
            {
                if (card.is_ally()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        public static bool has_exhausted_heroes(EventArgs args, Card me)
        {
            GameArgs the_args = (GameArgs) args;
            LOTRPlayer the_player = the_args.relevant_player;
            foreach (var card in the_player.get_heroes())
            {
                if (card.is_exhausted()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        
        public static bool enemy_in_staging(EventArgs args, Card me)
        {
            //TODO maybe do game check
            foreach (var card in game.get_staged_enemies())
            {
                if (card.is_enemy_card()) //TODO: or engaged with another player
                {
                    return true;
                }
            }
            return false;
        }
        
        public static List<Card> valid_targets_player_heroes()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_heroes())
            {
                valid_targets.Add(card);
            }
            return valid_targets;
        }
        public static List<Card> valid_targets_player_allies_in_hand()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_cards_in_hand())
            {
                if (card.is_ally())
                {
                    valid_targets.Add(card);
                }
            }
            return valid_targets;
        }
        public static List<Card> valid_targets_player_heroes_ready()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_heroes())
            {
                if (!card.is_exhausted())
                {
                    valid_targets.Add(card);
                }
                
            }
            return valid_targets;
        }
        public static List<Card> valid_targets_player_characters_ready()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_heroes())
            {
                if (!card.is_exhausted())
                {
                    valid_targets.Add(card);
                }
                
            }
            foreach (var card in game.get_cur_player().get_allies())
            {
                if (!card.is_exhausted())
                {
                    valid_targets.Add(card);
                }
                
            }
            return valid_targets;
        }
        public static List<Card> valid_targets_player_characters_committed()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_heroes())
            {
                if (card.is_committed())
                {
                    valid_targets.Add(card);
                }
                
            }
            foreach (var card in game.get_cur_player().get_allies())
            {
                if (card.is_committed())
                {
                    valid_targets.Add(card);
                }
                
            }
            return valid_targets;
        }
        public static List<Card> valid_targets_player_allies()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_allies())
            {
                valid_targets.Add(card);
            }
            return valid_targets;
        }
        
        public static List<Card> valid_targets_player_allies_exhausted()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_allies())
            {
                if (card.is_exhausted())
                {
                    valid_targets.Add(card);
                }
                
            }
            return valid_targets;
        }
        
        public static List<Card> valid_targets_player_characters_exhausted()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_cur_player().get_allies())
            {
                if (card.is_exhausted())
                {
                    valid_targets.Add(card);
                }
                
            }
            foreach (var card in game.get_cur_player().get_heroes())
            {
                if (card.is_exhausted())
                {
                    valid_targets.Add(card);
                }
                
            }
            return valid_targets;
        }

        

        public static List<Card> valid_targets_staged_enemies()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_staged_enemies())
            {
                if (card.is_enemy_card()) //TODO: or engaged with another player
                {
                    valid_targets.Add(card);
                }
            }
            return valid_targets;
        }
        
        public static List<Card> valid_targets_staged_locations_and_cur_location()
        {
            List<Card> valid_targets = new List<Card>();
            if (game.get_cur_location() != null)
            {
                valid_targets.Add(game.get_cur_location());
            }
        
            foreach (var card in game.get_staged_enemies())
            {
                if (card.is_location_card())
                {
                    valid_targets.Add(card);
                }
            }
            return valid_targets;
        }

        public static List<Card> LONGBEARD_ORC_SLAYER_staged_engaged_orcs()
        {
            List<Card> valid_targets = new List<Card>();
            foreach (var card in game.get_staged_enemies())
            {
                if (card.has_trait(LOTRGame.TRAITS.ORC)) //TODO: or engaged with another player
                {
                    valid_targets.Add(card);
                }
            }
            foreach (var card in game.get_cur_player().get_engaged_enemies())
            {
                if (card.has_trait(LOTRGame.TRAITS.ORC)) //TODO: or engaged with another player
                {
                    valid_targets.Add(card);
                }
            }

            return valid_targets;
        }
    
    }