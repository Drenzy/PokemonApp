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

        // Fetch Pokémon data and display in Editor
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
                var generationData = JsonSerializer.Deserialize<GenerationData>(json);
                var sb = new StringBuilder();

                foreach (var pokemon in generationData.pokemon_species)
                {
                    var segments = pokemon.url.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    var id = segments[^1];
                    sb.AppendLine($"{id} - {pokemon.name}");
                }

                PokemonEditor.Text = sb.ToString();
                ResultLabel.Text = "Pokémon data fetched. You can edit it now.";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = $"Error: {ex.Message}";
            }
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
