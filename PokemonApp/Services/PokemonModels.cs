namespace PokemonApp.Services
{

    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    namespace PokemonApp.Services
    {
        // Represents the JSON response for a Pokémon generation from the PokéAPI.
        public class GenerationData
        {
            // The PokéAPI returns an array of "pokemon_species" objects for each generation
            [JsonPropertyName("pokemon_species")]
            public List<PokemonSpecies> PokemonSpecies { get; set; }
        }

        // Represents an individual Pokémon species entry from the API response.
        public class PokemonSpecies
        {
            // The name of the Pokémon species (e.g., "bulbasaur").
            [JsonPropertyName("name")]
            public string Name { get; set; }

            // The URL where more details about this Pokémon species can be retrieved.
            // Example: "https://pokeapi.co/api/v2/pokemon-species/1/"
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }
    }
}
