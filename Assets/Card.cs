
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Card
    {
        private string name;
        private bool valid_response_target;
        private List<object> events_to_respond_to;
        private List<EventHandler> callbacks;
        private string uuid;
        protected List<LOTRAbility.ABILITY_FLAGS> my_flags;
        private bool entered_game;
        private bool face_down;
        
        public Card(string name)
        {
            this.name = name;
            valid_response_target = false;
            events_to_respond_to = new List<object>();
            callbacks = new List<EventHandler>();
            this.uuid = Guid.NewGuid().ToString();
            entered_game = false;
            my_flags= new List<LOTRAbility.ABILITY_FLAGS>();
            this.face_down = false;
        }

        public void set_face_down()
        {
            this.face_down = true;
        }

        public void set_face_up()
        {
            this.face_down = false;
        }

        public bool is_face_down()
        {
            return this.face_down;
        }
        public string get_name()
        {
            return this.name;
        }
        
        public void reset_response_target()
        {
            valid_response_target = false;
        }

        public void set_response_target_true()
        {
            valid_response_target = true;
        }
        
        public void add_flag(LOTRAbility.ABILITY_FLAGS flag)
        {
            my_flags.Add(flag);
        }

        public bool has_flag(LOTRAbility.ABILITY_FLAGS flag)
        {
            return my_flags.Contains(flag);
        }

        public void remove_flag(LOTRAbility.ABILITY_FLAGS flag, bool all = false)
        {
            if (has_flag(flag))
            {
                List<LOTRAbility.ABILITY_FLAGS> new_flags = new List<LOTRAbility.ABILITY_FLAGS>();
                bool removed = false;
                foreach (var f in my_flags)
                {
                    if (f != flag)
                    {
                        new_flags.Add(f);
                       
                    }
                    else if(!removed)
                    {
                        removed = true;
                    }
                    else if(!all)
                    {
                        new_flags.Add(f);
                    }
                }
                my_flags = new_flags;
            }

            
        }

        public void reset_flags()
        {
            my_flags= new List<LOTRAbility.ABILITY_FLAGS>();
        }
        
        public bool is_valid_response_target()
        {
            return valid_response_target;
        }
        
        public string get_uuid()
        {
            return this.uuid;
        }
        
        public void respond_to_event(object event_name, EventHandler e)
        {
            events_to_respond_to.Add(event_name);
            callbacks.Add(e);
        }

        public void enters_the_game()
        {
            if (!entered_game)
            {
                for (var i = 0; i < events_to_respond_to.Count; i++)
                {
                    LOTRGameEventHandler.add_handler_to_event_name(events_to_respond_to[i], callbacks[i]);
                }

                entered_game = true;
            }
            
        }
        
        

    }