using System.Text;
using System.Text.Json;

namespace PokemonApp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void OnFetchPokemonClicked(object sender, EventArgs e)
        {
            ResultLabel.Text = "Fetching Pokémon...";

            try
            {
                var response = await _httpClient.GetAsync("https://pokeapi.co/api/v2/generation/1/");
                if (!response.IsSuccessStatusCode)
                {
                    ResultLabel.Text = "Failed to fetch data.";
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var generationData = JsonSerializer.Deserialize<GenerationData>(json);
                var sb = new StringBuilder();

                foreach (var pokemon in generationData.pokemon_species)
                {
                    var segments = pokemon.url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    var id = segments[^1]; // get last segment
                    sb.AppendLine($"{id} - {pokemon.name}");
                }

                // Save the file to Android Downloads folder
    #if ANDROID
                var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                var filePath = Path.Combine(downloadsPath, "PokemonList.txt");
                await File.WriteAllTextAsync(filePath, sb.ToString());
    #endif

                ResultLabel.Text = $"Successfully saved Pokémon data to Downloads/PokemonList.txt!";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Error: {ex.Message}";
            }
        }

        // JSON models
        public class GenerationData
        {
            public PokemonSpecies[] pokemon_species { get; set; }
        }

        public class PokemonSpecies
        {
            public string name { get; set; }
            public string url { get; set; }
        }
    }
}
