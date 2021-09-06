using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Component;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Serilog;

namespace BrowserInteractLabeler.Pages
{
    public class MarkupMainComponent: ComponentBase
    {
        internal readonly ILogger _logger = Log.ForContext<Tools>();
        
        [Inject]
        internal IMarkupControlService _markupControlService { get; set; }

        [Inject]
        internal Tools _tools { get; set; }
        
        [Inject]
        internal KeyPressImageGridHandler KeyPressImageGridHandler { get; set; }
        
        [Inject] internal IJSRuntime _JSRuntime { get; set; }

        internal ElementReference refMainPanel;
        internal ImagesGridViewComponent _refImagesGridViewComponent;
        internal PaletteGridViewComponent _refPaletteGridViewComponent;

        internal Size SizeImgForm = new Size(1560, 950);
     
        internal void HandleKeyDown(KeyboardEventArgs e)
        {
            switch (e.Key.ToLower())
            {
                case "enter":
                    NextImages();
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    SelectPalette(e.Key.ToLower());
                    break;
            }
        }

        private async Task SelectPalette(string key)
        {
            var idPalette = int.Parse(key);
            var palettes = await _markupControlService.GetPaletteAsync();
            if (palettes is null || !palettes.Any())
            {
                _logger.Error("[MarkupMainComponent:SelectPalette] Not load Palettes");
                return;
            }
            var currentPalette = palettes.FirstOrDefault(p => p.ClassId == idPalette);
            if (currentPalette is null)
            {
                _logger.Error("[MarkupMainComponent:SelectPalette] Not Find palette:{PaletteId}",key);
                currentPalette = palettes.FirstOrDefault();
            }
            _refPaletteGridViewComponent.ForceClickAsync(currentPalette);
        }

        private async Task NextImages()
        {
           
            var allImg =await _markupControlService.GetAllFileNamesAsync();
            if (allImg is null || !allImg.Any())
            {
                _logger.Error("[MarkupMainComponent:NextImages] Not load all img");
                return;
            }
            var currentImg = KeyPressImageGridHandler.CurrentImages;
            var nexImages= _tools.GetNextElement(allImg, currentImg);

            if (string.IsNullOrEmpty(nexImages))
            {
                _logger.Error("[MarkupMainComponent:SelectPalette] Not init new img");
                nexImages = allImg.FirstOrDefault();
            }
            _refImagesGridViewComponent.ForceClickAsync(nexImages);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await refMainPanel.FocusAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
        }

       
    }
}