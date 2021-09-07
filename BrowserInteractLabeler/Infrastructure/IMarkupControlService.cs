using System.Collections.Generic;
using System.Threading.Tasks;
using BrowserInteractLabeler.Common;

namespace BrowserInteractLabeler.Infrastructure
{
    public interface IMarkupControlService
    {
        // Task SetCurrentFileNameAsync(string fileName);
        // Task<string> GetCurrentFileNameAsync();

        Task<IEnumerable<string>> GetAllFileNamesAsync();
        Task SetTypeMarkupAsync(TypeMarkup typeMarkup);
        Task <TypeMarkup> GetTypeMarkupAsync();
        Task SetPathRootFolderImagesAsync(string path);
        Task<string> GetPathRootFolderImagesAsync();
        Task SearchAllImagesAsync();
        Task<IEnumerable<PaletteData>> GetPaletteAsync();
        Task SetPointAsync(int classId, Point point,TypeMarkup typeDrawing, string fullImgName);
        Task<ExportData[]> GetAllPointsAsync();
    }
}