using System;
using System.Reflection;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Serilog;

namespace BrowserInteractLabeler.Component
{
    public class BrashColorComponentModel : ComponentBase
    {
        internal readonly ILogger _logger = Log.ForContext<PaletteGridViewComponentModel>();
        [Parameter] public string ColorComponent { get; set; } = String.Empty;
        [Parameter] public int Width { get; set; } = 0;
        [Parameter] public int Height { get; set; } = 0;
        
        internal BECanvasComponent CanvasReference { get; set; }
        private Canvas2DContext _currentCanvasContext;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _currentCanvasContext = await CanvasReference.CreateCanvas2DAsync();
                StateHasChanged();
                await DrawingColor();
            }
        }


        internal async Task DrawingColor()
        {
            if (CanvasReference is null || _currentCanvasContext is null)
                return;

            try
            {
                await _currentCanvasContext.ClearRectAsync(0, 0, 10, 10);
                await _currentCanvasContext.SetFillStyleAsync(ColorComponent);
                await _currentCanvasContext.FillRectAsync(0, 0, 10, 10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed {MethodName}: {ErrorMessage}",
                    MethodBase.GetCurrentMethod(), ex.Message);
            }
        }
    }
}