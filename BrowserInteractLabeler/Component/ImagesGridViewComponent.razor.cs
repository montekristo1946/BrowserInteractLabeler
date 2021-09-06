using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Serilog;

namespace BrowserInteractLabeler.Component
{
    public class ImagesGridViewComponentModel : ComponentBase,IDisposable
    {
        internal readonly ILogger _logger = Log.ForContext<ImagesGridViewComponentModel>();
        [Parameter] public IEnumerable<string> Images { get; set; }
        
        [Inject] internal KeyPressImageGridHandler KeyPressImageGridHandler { get; set; }

        [Inject] internal IJSRuntime _JSRuntime { get; set; }
        
        private const string _styleButtonActive = "btn-outline-danger";
        private const string _styleButtonPassive = "btn-outline-dark";

        internal string _selectButton { get; set; } = string.Empty;

        public async Task ForceClickAsync(string selectImages)
        {
            var idElement = Path.GetFileName(selectImages);
            if (idElement is null)
            {
                _logger.Error("[ImagesGridViewComponentModel:ForceClickAsync] bad imagesName {ImageName}",selectImages);
                return;
            }
            await _JSRuntime.InvokeVoidAsync("FocusElement", idElement);
            await _JSRuntime.InvokeVoidAsync("ClickElement", idElement);
        }
        
        protected override async Task OnInitializedAsync()
        {
            KeyPressImageGridHandler.OnChange += KeyPressHandlerOnOnChange;
        }

        
        private async void KeyPressHandlerOnOnChange()
        {
            await InvokeAsync(() =>
            {
                _selectButton = KeyPressImageGridHandler.CurrentImages;
            });
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
        }

        internal Task ButtonClickAsync(string selectImages)
        {
           KeyPressImageGridHandler.SetCurrentImagesButton(selectImages);
            return Task.CompletedTask;
        }
        
        internal string GetActualStyle(string itemImages, string selectButton)
        {
            var retStyle = _styleButtonPassive;
            if (itemImages == selectButton)
            {
                retStyle = _styleButtonActive;
            }

            return retStyle;
        }


        public void Dispose()
        {
            KeyPressImageGridHandler.OnChange -= KeyPressHandlerOnOnChange;
        }
    }
}