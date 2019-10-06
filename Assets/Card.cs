
    public class Card
    {
        private string name;
        private bool valid_response_target;
        
        public Card(string name)
        {
            this.name = name;
            valid_response_target = false;
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
        
        public bool is_valid_response_target()
        {
            return valid_response_target;
        }
    }