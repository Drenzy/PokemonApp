using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonApp.Services
{

    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    namespace PokemonApp.Services
    {
        public class GenerationData
        {
            // The PokéAPI returns an array of "pokemon_species" objects for each generation
            [JsonPropertyName("pokemon_species")]
            public List<PokemonSpecies> PokemonSpecies { get; set; }
        }

        public class PokemonSpecies
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }
        }
    }
}
