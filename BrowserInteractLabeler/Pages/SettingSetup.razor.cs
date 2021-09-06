using System;
using System.Threading;
using System.Threading.Tasks;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BrowserInteractLabeler.Pages
{
    public class SettingSetupComponent : ComponentBase
    {
        [Inject] internal IMarkupControlService _markupControlService { get; set; }

        [Inject] internal NavigationManager NavigationManager { get; set; }

        internal string _rootPathImgDir = string.Empty;

        internal readonly ILogger _logger = Log.ForContext<SettingSetupComponent>();


        internal async Task GoToNextPage()
        {
            if (!string.IsNullOrEmpty(_rootPathImgDir))
            {
                await _markupControlService.SetPathRootFolderImagesAsync(_rootPathImgDir);
                  //  Thread.Sleep(10000);
                 _markupControlService.SearchAllImagesAsync().Wait();
                NavigationManager.NavigateTo("MarkupMain", true);
            }
        }

        internal async Task PreviousPage()
        {
            NavigationManager.NavigateTo("ChoosingMarkupType", true);
        }

        protected override async Task OnInitializedAsync()
        {
            _logger.Debug("[SettingSetupComponent:OnInitializedAsync] Init");
            _rootPathImgDir = await _markupControlService.GetPathRootFolderImagesAsync();
        }
    }
}