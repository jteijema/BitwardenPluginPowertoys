using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.BitwardenPlugin
{
    public class Main : IPlugin, ISettingProvider, IDisposable
    {        
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "1C638DC8E8564E3194B323BA041B4A05";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "BitwardenPlugin";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "Interactions with the Bitwarden API to fetch login data";

        /// <summary>
        /// Additional options for the plugin.
        /// </summary>
        public IEnumerable<PluginAdditionalOption> AdditionalOptions => new[]
        {
            new PluginAdditionalOption
            {
                Key = nameof(ApiKey),
                DisplayLabel = "API Key",
                DisplayDescription = "Enter your API key",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = ApiKey,
            }
        };

        public List<Result> Query(Query query)
        {
            var results = new List<Result>();

            // Check if the query matches the command to copy the API key
            if (query.Search.Equals("copyapikey", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(new Result
                {
                    Title = "Copy API Key",
                    SubTitle = "Copy the Bitwarden API key to the clipboard",
                    IcoPath = IconPath,
                    Action = context =>
                    {
                        return CopyApiKeyToClipboard();
                    },
                });
            }

            return results;
        }

        private Result CopyApiKeyToClipboard()
        {
            if (CopyToClipboard(ApiKey))
            {
                return new Result
                {
                    Title = "API Key copied to clipboard",
                    IcoPath = IconPath,
                };
            }

            return new Result
            {
                Title = "Failed to copy API Key to clipboard",
                IcoPath = IconPath,
            };
        }

        private string? IconPath { get; set; }

        private PluginInitContext? Context { get; set; }

        private string? ApiKey { get; set; }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            Log.Info("Init", GetType());

            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());

            ApiKey = context.API?.GetAdditionalOptionValue(PluginID, nameof(ApiKey));
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? Context?.CurrentPluginMetadata.IcoPathLight : Context?.CurrentPluginMetadata.IcoPathDark;

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);

        /// <summary>
        /// Creates setting panel.
        /// </summary>
        /// <returns>The control.</returns>
        /// <exception cref="NotImplementedException">method is not implemented.</exception>
        public Control CreateSettingPanel() => throw new NotImplementedException();

        /// <summary>
        /// Updates settings.
        /// </summary>
        /// <param name="settings">The plugin settings.</param>
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            Log.Info("UpdateSettings", GetType());

            ApiKey = settings.AdditionalOptions?.FirstOrDefault(x => x.Key == nameof(ApiKey))?.Value;
        }

        private static bool CopyToClipboard(string? value)
        {
            if (value != null)
            {
                Clipboard.SetText(value);
            }

            return true;
        }

        private bool Disposed { get; set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Log.Info("Dispose", GetType());

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            Disposed = true;
        }
    }
}