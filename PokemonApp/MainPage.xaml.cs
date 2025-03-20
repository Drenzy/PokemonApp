


namespace PokemonApp
{
    public partial class MainPage : ContentPage
    {
        // HttpClient instance to handle API requests.
        private readonly HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        // Fetch all Pokémon from Generations 1 to 9 concurrently.
        private async void OnFetchMultipleGenerationsClicked(object sender, EventArgs e)
        {
            ResultLabel.Text = "Henter Pokémoner...";

            try
            {
                var generations = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                var tasks = generations.Select(gen => FetchGenerationAsync(gen)).ToArray();

                // Await all tasks concurrently
                var results = await Task.WhenAll(tasks);

                // Combine all fetched results into one formatted string.
                var combinedResults = string.Join(Environment.NewLine, results);

                // Display the combined Pokémon data in the editor.
                PokemonEditor.Text = combinedResults;

                ResultLabel.Text = "Hentede Generation 1-9. Du kan redigere dem nu!";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Error: {ex.Message}";
            }
        }

        // Fetch Pokémon data for a specific generation asynchronously.
        private async Task<string> FetchGenerationAsync(int generationNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/generation/{generationNumber}/");

                // IF API Returns an error (f.eks. 404 or 500)
                if (!response.IsSuccessStatusCode)
                {
                    return $"Fejl ved hentning af Generation {generationNumber}: {response.StatusCode}";
                }

                var json = await response.Content.ReadAsStringAsync();
                var generationData = JsonSerializer.Deserialize<GenerationData>(json);

                var pokemonList = generationData?.pokemon_species?
                    .Select(pokemon =>
                    {
                        var segments = pokemon.url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                        int id = int.Parse(segments[^1]);
                        return new { id, pokemon.name };
                    })
                    .OrderBy(p => p.id)
                    .ToList();

                var sb = new StringBuilder();
                sb.AppendLine($"--- Generation {generationNumber} ---");

                foreach (var pokemon in pokemonList!)
                {
                    sb.AppendLine($"{pokemon.id} - {pokemon.name}");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Fejl ved hentning af Generation {generationNumber}: {ex.Message}";
            }
        }



        // Save the edited Pokémon data to the files app download folder on the device.
        private async void OnSaveEditedDataClicked(object sender, EventArgs e)
        {
            try
            {
                var editedData = PokemonEditor.Text;
                string filePath = "Ukendt placering"; // Default message if platform is not Android

#if ANDROID
        var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        filePath = Path.Combine(downloadsPath, "PokemonListEdited.txt");
        await File.WriteAllTextAsync(filePath, editedData);
#endif

#if WINDOWS
var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
filePath = Path.Combine(downloadsPath, "PokemonListEdited.txt");
await File.WriteAllTextAsync(filePath, editedData);
#endif

                ResultLabel.Text = $"Fil gemt: {filePath}";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Fejl: {ex.Message}";
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
