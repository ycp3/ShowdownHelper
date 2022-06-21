using System.Collections.Generic;

namespace ShowdownHelper
{
    public class Pokedex
    {
        public class BaseStats
        {
            public int hp { get; set; }
            public int atk { get; set; }
            public int def { get; set; }
            public int spa { get; set; }
            public int spd { get; set; }
            public int spe { get; set; }
        }
        
        public class Pokemon
        {
            public int num { get; set; }
            public string name { get; set; }
            public List<string> types { get; set; }
            public BaseStats baseStats { get; set; }
            public Dictionary<string, string> abilities { get; set; }
            public double heightm { get; set; }
            public float weightkg { get; set; }
            public string color { get; set; }
            public string prevo { get; set; }
            public int evoLevel { get; set; }
            public List<string> eggGroups { get; set; }
            public string tier { get; set; }
            public string isNonstandard { get; set; }
        }
    }
}