using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using System.Reflection;
using BrowserInteractLabeler.Common;
using Serilog;


namespace BrowserInteractLabeler.Component
{
    public class DrawingConvasComponentModel : ComponentBase, IDisposable
    {
        [Inject] internal IMarkupControlService _markupControlService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] internal KeyPressImageGridHandler KeyPressImageGridHandler { get; set; }
        [Inject] internal KeyPressPaletteGridHandler KeyPressPaletteGridHandler { get; set; }

        [Parameter] public int Width { get; set; } = 0;
        [Parameter] public int Height { get; set; } = 0;

        private readonly ILogger _logger = Log.ForContext<Tools>();

        private string _currentNameDrawingImg = string.Empty;
        private PaletteData _currentPalette = new PaletteData();

        internal ElementReference ImageMap { get; set; }
        internal ElementReference DivCanvas { get; set; }
        internal BECanvasComponent CanvasReference { get; set; }

        private Canvas2DContext _currentCanvasContext;

        internal string Image64 { get; set; } = string.Empty;

        private const string _moqImg = "./Resource/error_1.png";

        protected override async Task OnInitializedAsync()
        {
            KeyPressImageGridHandler.OnChange += KeyPressIMagesHandlerOnChange;
            KeyPressPaletteGridHandler.OnChange += KeyPressPaletteGridOnChange;
        }

        private async void KeyPressPaletteGridOnChange()
        {
            await InvokeAsync(() =>
            {
                if (KeyPressPaletteGridHandler.CurrentPalette == _currentPalette)
                    return;

                _currentPalette = KeyPressPaletteGridHandler.CurrentPalette;
                Console.WriteLine($"[DrawingConvasComponentModel:KeyPressPaletteGridOnChange] {_currentPalette}");
            });
        }

        private async void KeyPressIMagesHandlerOnChange()
        {
            await InvokeAsync(() =>
            {
                if (KeyPressImageGridHandler.CurrentImages == _currentNameDrawingImg)
                    return;

                DrawingImg(KeyPressImageGridHandler.CurrentImages);
                _currentNameDrawingImg = KeyPressImageGridHandler.CurrentImages;
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _currentCanvasContext = await CanvasReference.CreateCanvas2DAsync();
                await DrawingMoqImg();
                StateHasChanged();
            }
        }

        private async Task DrawingMoqImg()
        {
            try
            {
                var img = await File.ReadAllBytesAsync(_moqImg);
                Image64 = "data:image/jpg;base64," + Convert.ToBase64String(img);
                StateHasChanged();
                await _currentCanvasContext.ClearRectAsync(0, 0, CanvasReference.Width, CanvasReference.Height);
                await _currentCanvasContext.DrawImageAsync(ImageMap, 0, 0, CanvasReference.Width,
                    CanvasReference.Height);
            }
            catch (FileNotFoundException ex)
            {
                await _currentCanvasContext.SetFillStyleAsync("Black");
                await _currentCanvasContext.SetFontAsync("44pt Ariel");
                await _currentCanvasContext.FillTextAsync(
                    "Images not Load", CanvasReference.Width * 0.4, CanvasReference.Height * 0.5);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed {MethodName}: {ErrorMessage}",
                    MethodBase.GetCurrentMethod(), ex.Message);
                return;
            }
        }

        private async void DrawingImg(string nameDrawingImg)
        {
            var img = await File.ReadAllBytesAsync(nameDrawingImg);
            Image64 = "data:image/jpg;base64," + Convert.ToBase64String(img);
            StateHasChanged();

            await _currentCanvasContext.ClearRectAsync(0, 0, CanvasReference.Width, CanvasReference.Height);
            await _currentCanvasContext.DrawImageAsync(ImageMap, 0, 0, CanvasReference.Width,
                CanvasReference.Height);

            var loadData = await _markupControlService.GetAllPointsAsync();
            var currentBlock = loadData.Where(p => p.FullImgName == _currentNameDrawingImg);
            var allPalette = await _markupControlService.GetPaletteAsync();
            foreach (var block in currentBlock)
            {
                var currentPalette = allPalette?.FirstOrDefault(p => p.ClassId == block.ClassID);
                if (currentPalette is null)
                {
                    _logger.Error("[DrawingConvasComponentModel:DrawingImg] Palette not Found Palette {IdPalette}",
                        block.ClassID);
                    continue;
                }

                foreach (var point in block.Points)
                {
                    DrawingPoint(point, _currentCanvasContext, currentPalette);
                }
            }
        }

        internal async void OnClick(MouseEventArgs eventArgs)
        {
            if (DivCanvas.Id?.Length < 0)
                return;

            var data = await JSRuntime.InvokeAsync<string>("getDivCanvasOffsets", new object[] {DivCanvas});

            var mousePositionData = JsonConvert.DeserializeObject<MousePositionData>(data);

            if (mousePositionData == null)
                return;

            var mouseX = eventArgs.ClientX - mousePositionData.ActiveRectangle.X;
            var mouseY = eventArgs.ClientY - mousePositionData.ActiveRectangle.Y;

            var typeDrawing = await _markupControlService.GetTypeMarkupAsync();

            switch (typeDrawing)
            {
                case TypeMarkup.None:
                    break;
                case TypeMarkup.PointMark:
                    var point = new Point(mouseX, mouseY);
                    DrawingPoint(point, _currentCanvasContext, _currentPalette);
                    await _markupControlService.SetPointAsync(_currentPalette.ClassId, point, typeDrawing,
                        _currentNameDrawingImg);
                    break;
                case TypeMarkup.Rectangle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            StateHasChanged();

            //SendAlert
            // await JSRuntime.InvokeAsync<string>("SendAlert", "Test Alert");
        }


        private async void DrawingPoint(Point point,
            Canvas2DContext currentCanvasContext,
            PaletteData palette)
        {
            await currentCanvasContext.BeginPathAsync();
            await currentCanvasContext.SetFillStyleAsync(palette.ColorClass);
            await currentCanvasContext.ArcAsync(point.X, point.Y, palette.Radius, 0, Math.PI * 2, false);
            await currentCanvasContext.FillAsync();
        }

        public void Dispose()
        {
            KeyPressImageGridHandler.OnChange -= KeyPressIMagesHandlerOnChange;
        }
    }
}