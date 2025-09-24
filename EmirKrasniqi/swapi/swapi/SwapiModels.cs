using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swapi
{
    public class FilmResult
    {
        public List<Film> results { get; set; }
    }

    public class Film
    {
        public string title { get; set; }
        public string opening_crawl { get; set; }
        public string director { get; set; }
        public string producer { get; set; }
        public string release_date { get; set; }
        public List<string> characters { get; set; }
    }

    public class CharacterResult
    {
        public List<Character> results { get; set; }
    }

    public class Character
    {
        public string name { get; set; }
        public List<string> films { get; set; }
        public List<string> starships { get; set; }
    }

    public class PlanetResult
    {
        public List<Planet> results { get; set; }
    }

    public class Planet
    {
        public string name { get; set; }
        public string population { get; set; }
    }

    public class StarshipResult
    {
        public List<Starship> results { get; set; }
    }

    public class Starship
    {
        public string name { get; set; }
        public string cost_in_credits { get; set; }
        public string starship_class { get; set; }
        public string max_atmosphering_speed { get; set; }
        public string hyperdrive_rating { get; set; }
        public string length { get; set; }
        public List<string> films { get; set; }
    }
}
