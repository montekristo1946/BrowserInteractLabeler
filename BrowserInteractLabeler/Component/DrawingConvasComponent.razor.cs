using System;
using System.IO;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
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

        
        private readonly ILogger _logger = Log.ForContext<Tools>();

        private string _currentNameDrawingImg = string.Empty;
        private PaletteData _currenIdPalette = new PaletteData();

        internal ElementReference ImageMap { get; set; }
        internal ElementReference DivCanvas { get; set; }
        internal BECanvasComponent CanvasReference { get; set; }
        
        private Canvas2DContext _currentCanvasContext;

        internal string Image64 { get; set; } = string.Empty;

        internal Size WindowsImgSize { get; set; } = new Size(1550, 800);//TODO: move to Config page


        protected override async Task OnInitializedAsync()
        {
            KeyPressImageGridHandler.OnChange += KeyPressIMagesHandlerOnChange;
            KeyPressPaletteGridHandler.OnChange += KeyPressPaletteGridOnChange;
        }

        private async void KeyPressPaletteGridOnChange()
        {
            await InvokeAsync(() =>
            {
                if (KeyPressPaletteGridHandler.CurrentPalette == _currenIdPalette)
                    return;

                _currenIdPalette = KeyPressPaletteGridHandler.CurrentPalette;
                Console.WriteLine($"[DrawingConvasComponentModel:KeyPressPaletteGridOnChange] {_currenIdPalette}");
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
                var moqImg = "./Resource/error_1.png";
                var img = await File.ReadAllBytesAsync(moqImg);
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


          //  Console.WriteLine($"x:{mouseX},Y:{mouseY}; ClientX:{eventArgs.ClientX} ClientY:{eventArgs.ClientY}  " +
          //                    $"x_rect:{mousePositionData.ActiveRectangle.X}; y_rect:{mousePositionData.ActiveRectangle.Y}");
            //Console.WriteLine($"OnClick {_keyPressHandler.CurrentImages}");
            var typeDrawing = await _markupControlService.GetTypeMarkupAsync();

            switch (typeDrawing)
            {
                case TypeMarkup.None:
                    break;
                case TypeMarkup.PointMark:
                    DrawingPoint(mouseX, mouseY,_currentCanvasContext,_currenIdPalette);
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

        private async void DrawingPoint(double mouseX, double mouseY, Canvas2DContext currentCanvasContext, PaletteData palette)
        {
            await _currentCanvasContext.BeginPathAsync();
            await _currentCanvasContext.SetFillStyleAsync(palette.ColorClass);
            await _currentCanvasContext.ArcAsync(mouseX, mouseY, palette.Radius, 0, Math.PI * 2, false);
            await _currentCanvasContext.FillAsync();
        }

        public void Dispose()
        {
            KeyPressImageGridHandler.OnChange -= KeyPressIMagesHandlerOnChange;
        }
    }
}