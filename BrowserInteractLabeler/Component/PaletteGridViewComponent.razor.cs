using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blazor.Extensions;
using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Serilog;

namespace BrowserInteractLabeler.Component
{
    public class PaletteGridViewComponentModel : ComponentBase, IDisposable
    {
        [Parameter] public IEnumerable<PaletteData> PaletteData { get; set; }

        [Inject] internal KeyPressPaletteGridHandler KeyPressPaletteGridHandler { get; set; }
        [Inject] internal IJSRuntime _JSRuntime { get; set; }

        internal readonly ILogger _logger = Log.ForContext<PaletteGridViewComponentModel>();
        // internal BECanvasComponent CanvasReference { get; set; }
        //  private Canvas2DContext _currentCanvasContext;

        private List<BECanvasComponent> _canvasReference = new List<BECanvasComponent>();

        internal PaletteData _selectButton = new PaletteData();

        private const string _styleButtonActive = "btn-outline-info";
        private const string _styleButtonPassive = "btn-outline-dark";

        public void Dispose()
        {
            KeyPressPaletteGridHandler.OnChange -= KeyPressHandlerOnOnChange;
        }

        protected override async Task OnInitializedAsync()
        {
            KeyPressPaletteGridHandler.OnChange += KeyPressHandlerOnOnChange;
        }

        private async void KeyPressHandlerOnOnChange()
        {
            await InvokeAsync(() => { _selectButton = KeyPressPaletteGridHandler.CurrentPalette; });
        }

        internal Task ButtonClickAsync(PaletteData paletteData)
        {
            KeyPressPaletteGridHandler.SetCurrentPaletteButton(paletteData);
            return Task.CompletedTask;
        }

        internal string GetActualStyle(PaletteData itemPalette, PaletteData selectButton)
        {
            var retStyle = _styleButtonPassive;
            if (itemPalette.ClassId == selectButton.ClassId)
            {
                retStyle = _styleButtonActive;
            }

            return retStyle;
        }

        public async Task ForceClickAsync(PaletteData selectPalette)
        {
            var buttonId = $"{selectPalette.ClassId}{selectPalette?.NameClass}";
            await _JSRuntime.InvokeVoidAsync("FocusElement", buttonId);
            await _JSRuntime.InvokeVoidAsync("ClickElement", buttonId);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var palette = PaletteData.FirstOrDefault();
                if (palette is null)
                    return;
                await ForceClickAsync(palette);
            }
        }

    }
}