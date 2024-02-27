using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wox.Plugin;
using Microsoft.PowerToys.Settings.UI.Library;
using ManagedCommon;

namespace Community.PowerToys.Run.Plugin.Bitwarden
{
    public class Main : IPlugin, IContextMenu, ISettingProvider, IDisposable
    {
        private PluginInitContext Context { get; set; }
        private HttpClient HttpClient { get; set; }

        public string Name => "Bitwarden Login Data";
        public string Description => "Retrieve login data from Bitwarden";

        // Replace with your Bitwarden API token
        private string ApiToken = "your_bitwarden_api_token";

        public void Init(PluginInitContext context)
        {
            this.Context = context;
            this.HttpClient = new HttpClient();
            // Configure HttpClient for Bitwarden API
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
        }

        public List<Result> Query(Query query)
        {
            var searchResults = Task.Run(() => SearchBitwarden(query.Search)).Result;
            return searchResults.Select(item => new Result
            {
                Title = item.Name,
                SubTitle = "Username: " + item.Username,
                IcoPath = "Images\\bitwarden.png", // Ensure you have an appropriate icon
                Action = context =>
                {
                    // Copy the password to the clipboard and show a message
                    Clipboard.SetText(item.Password);
                    MessageBox.Show("Password copied to clipboard", "Bitwarden", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
            }).ToList();
        }

        private async Task<List<BitwardenItem>> SearchBitwarden(string query)
        {
            var response = await HttpClient.GetAsync($"https://api.bitwarden.com/items?search={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<List<BitwardenItem>>(stream);
            return items ?? new List<BitwardenItem>();
        }

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }

    public class BitwardenItem
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
