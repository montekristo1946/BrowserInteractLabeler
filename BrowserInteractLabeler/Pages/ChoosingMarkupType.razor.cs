using BrowserInteractLabeler.Common;
using BrowserInteractLabeler.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace BrowserInteractLabeler.Pages
{
    public class ChoosingMarkupTypeComponent : ComponentBase
    {
        [Inject]
        IMarkupControlService _markupControlService { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; }

        internal void RedirectToSettingSetup( TypeMarkup typeMarkup)
        {
            _markupControlService.SetTypeMarkupAsync(typeMarkup);
            NavigationManager.NavigateTo("SettingSetup", true);
        }
    }
}