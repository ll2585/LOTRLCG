using System;
using System.Collections.Generic;

public class QuestCard : EnemyCard
{
    private List<string> scenario_symbols;
    private string sequence;
    private bool won = false;
    private int progress_tokens_added;

    private QuestCard next_quest_stage;

    public QuestCard(string card_name, int quest_points, 
        string ability, string set_information, string scenario_title, List<string> scenario_symbols, string sequence) : base(card_name, engagement_cost: -1, threat_strength: -1, attack: -1,
        defense: -1, quest_points: quest_points, hp: -1,encounter_set: "", traits: null, ability: ability, shadow_effect_icon: "", card_type: "QUEST", set_information: set_information, scenario_title: scenario_title)
    {
        this.scenario_symbols = scenario_symbols;
        this.sequence = sequence;
        this.progress_tokens_added = 0;
        next_quest_stage = null;
    }

    public void set_next_quest_stage(QuestCard next_stage)
    {
        next_quest_stage = next_stage;
    }
    public QuestCard get_next_stage()
    {
        return next_quest_stage;
    }

    public void set_as_winning_stage()
    {
        this.won = true;
    }

    public int? get_progress()
    {
        return this.progress_tokens_added;
    }
    public void add_progress_token(int? i = 1)
    {
        progress_tokens_added += 1;
    }

    public bool is_won_quest()
    {
        return this.won;
    }
    
    public static QuestCard PASSAGE_THROUGH_MIRKWOOD_1A = new QuestCard("Passage Through Mirkwood 1A - Flies and Spiders",quest_points:-1, 
        ability: "Search the encounter deck for 1 copy of the Forest Spider blahgblah", set_information: "???", scenario_title: "PASSAGE THROUGH MIRWOOD", new List<string>() {"TREE", "SPIDER", "ORC"}, "1A" );
    public static QuestCard YOU_WIN()
    {
        QuestCard the_card = new QuestCard("YOU WIN",quest_points:10, 
            ability: "WINNER", set_information: "???", scenario_title: "PASSAGE THROUGH MIRKWOOD", new List<string>() {"TREE", "SPIDER", "ORC"}, "2B" );
        the_card.set_as_winning_stage();
        return the_card;
    }
    public static QuestCard PASSAGE_THROUGH_MIRKWOOD_3B_BEORNS_PATH()
    {
        QuestCard the_card = new QuestCard("Passage Through Mirkwood 3B - A Chosen Path - Beorn's Path ",quest_points:10, 
            ability: "Players cannot defeat this stage while Ungoliant's Spawn is in play. If players defeat this stage, they have won the game.", set_information: "???", scenario_title: "PASSAGE THROUGH MIRKWOOD", new List<string>() {"TREE", "SPIDER", "ORC"}, "2B" );
        the_card.set_next_quest_stage(YOU_WIN());
        return the_card;
    }
    public static QuestCard PASSAGE_THROUGH_MIRKWOOD_3B_DONT_LEAVE_THE_PATH()
    {
        QuestCard the_card = new QuestCard("Passage Through Mirkwood 3B - A Chosen Path - Don't Leave the Path!",quest_points:100000, 
            ability: "When Revealed: Each player must search the encounter deck and discard pile for 1 Spider card of his choice, and add it to the staging area. The players must find and defeat Ungoliant's Spawn to win this game.", set_information: "???", scenario_title: "PASSAGE THROUGH MIRKWOOD", new List<string>() {"TREE", "SPIDER", "ORC"}, "2B" );
        the_card.set_next_quest_stage(YOU_WIN());
        return the_card;
    }
    public static QuestCard PASSAGE_THROUGH_MIRKWOOD_2B()
    {
        QuestCard the_card = new QuestCard("Passage Through Mirkwood 2B - A Fork in the Road",quest_points:2, 
            ability: "Forced: When you defeat this stage, proceed to one of the 2 \"A Chosen Path\" stages, at random.", set_information: "???", scenario_title: "PASSAGE THROUGH MIRKWOOD", new List<string>() {"TREE", "SPIDER", "ORC"}, "2B" );
        Random random = new Random();
        if (random.Next(0, 2) == 0)
            the_card.set_next_quest_stage(PASSAGE_THROUGH_MIRKWOOD_3B_BEORNS_PATH());
        else
            the_card.set_next_quest_stage(PASSAGE_THROUGH_MIRKWOOD_3B_DONT_LEAVE_THE_PATH());
        return the_card;
    }
    public static QuestCard PASSAGE_THROUGH_MIRKWOOD_1B()
    {
        QuestCard the_card = new QuestCard("Passage Through Mirkwood 1B - Flies and Spiders",quest_points:8, 
            ability: "", set_information: "???", scenario_title: "PASSAGE THROUGH MIRWOOD", new List<string>() {"TREE", "SPIDER", "ORC"}, "1B" );
        the_card.set_next_quest_stage(PASSAGE_THROUGH_MIRKWOOD_2B());
        return the_card;
    }
}