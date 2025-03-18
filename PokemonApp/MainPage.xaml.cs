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

        // Fetch generations 1, 2, and 3 simultaneously
        private async void OnFetchMultipleGenerationsClicked(object sender, EventArgs e)
        {
            ResultLabel.Text = "Fetching Pokémon (Generations 1-3)...";

            try
            {
                var generations = new[] { 1, 2, 3 };
                var tasks = generations.Select(gen => FetchGenerationAsync(gen)).ToArray();

                // Await all tasks concurrently
                var results = await Task.WhenAll(tasks);

                // Combine results
                var combinedResults = string.Join(Environment.NewLine, results);

                // Show the combined Pokémon data in the editor
                PokemonEditor.Text = combinedResults;

                ResultLabel.Text = "Fetched Generations 1-3. You can edit now!";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Error: {ex.Message}";
            }
        }

        // Helper method to fetch a single generation's Pokémon data
        private async Task<string> FetchGenerationAsync(int generationNumber)
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/generation/{generationNumber}/");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var generationData = JsonSerializer.Deserialize<GenerationData>(json);

            var sb = new StringBuilder();
            sb.AppendLine($"--- Generation {generationNumber} ---");
            foreach (var pokemon in generationData.pokemon_species)
            {
                var segments = pokemon.url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var id = segments[^1];
                sb.AppendLine($"{id} - {pokemon.name}");
            }

            return sb.ToString();
        }


        // Save edited content to file
        private async void OnSaveEditedDataClicked(object sender, EventArgs e)
        {
            try
            {
                var editedData = PokemonEditor.Text;

#if ANDROID
                var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                var filePath = Path.Combine(downloadsPath, "PokemonListEdited.txt");
                await File.WriteAllTextAsync(filePath, editedData);
#endif

                ResultLabel.Text = "Edited Pokémon data saved successfully!";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Error: {ex.Message}";
            }
        }

        // JSON Models (fix nullable warnings)
        public class GenerationData
        {
            public PokemonSpecies[]? pokemon_species { get; set; }
        }

        public class PokemonSpecies
        {
            public string? name { get; set; }
            public string? url { get; set; }
        }
    }
}
