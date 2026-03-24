using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamPriceCheckerConsoleApp
{
    internal class Program
    {
        private static string AppVersion = "v1.1.0";
        private readonly static HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "SteamPriceChecker-App");

            Console.WriteLine($"--- Steam Adatlekérő {AppVersion} ---");

            await CheckForUpdates();

            string appId = "2807960";
            await GetSteamGameDetails(appId);

            Console.WriteLine("\nNyomj meg egy gombot a kilépéshez...");
            await Task.Delay(-1);
        }

        static async Task CheckForUpdates()
        {
            try
            {
                Console.WriteLine("Frissítések keresése...");

                string url = "https://api.github.com/repos/Riptir3/SteamPriceCheckerConsoleApp/releases/latest";

                var response = await _httpClient.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(response);

                string latestVersion = doc.RootElement.GetProperty("tag_name").GetString()!;

                if (latestVersion != AppVersion)
                {
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine($"[!] ÚJ VERZIÓ ELÉRHETŐ: {latestVersion}");
                    Console.WriteLine("[!] A Watchtower hamarosan frissíti a konténert.");
                    Console.WriteLine("--------------------------------------------------\n");
                }
                else
                {
                    Console.WriteLine("[OK] Az alkalmazás naprakész.\n");
                }
            }
            catch
            {
                Console.WriteLine("Frissítés-ellenőrzés átugorva (nincs publikált Release).\n");
            }
        }

        static async Task GetSteamGameDetails(string appId)
        {
            string url = $"https://store.steampowered.com/api/appdetails?appids={appId}&cc=hu&l=hungarian";

            try
            {
                Console.WriteLine("Adatok lekérése a Steamről......");
                var response = await _httpClient.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(response);
                JsonElement root = doc.RootElement.GetProperty(appId);

                if (root.GetProperty("success").GetBoolean())
                {
                    var data = root.GetProperty("data");
                    string name = data.GetProperty("name").GetString()!;

                    string price = "Ingyenes";
                    if (data.TryGetProperty("price_overview", out JsonElement priceElem))
                    {
                        price = priceElem.GetProperty("final_formatted").GetString()!;
                    }

                    string description = data.GetProperty("short_description").GetString()!;

                    Console.WriteLine("\n---- Játék adatai ----");
                    Console.WriteLine($"Név: {name}");
                    Console.WriteLine($"Ár:  {price}");
                    Console.WriteLine($"Leírás: {(description.Length > 100 ? description.Substring(0, 100) + "..." : description)}");
                }
                else
                {
                    Console.WriteLine("Hiba: A játék nem található.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a Steam lekérésnél: {ex.Message}");
            }
        }
    }
}