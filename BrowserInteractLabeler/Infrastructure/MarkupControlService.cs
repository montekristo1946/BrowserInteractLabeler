using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BrowserInteractLabeler.Common;
using Serilog;

namespace BrowserInteractLabeler.Infrastructure
{
    public class MarkupControlService : IMarkupControlService
    {
        private readonly Tools _tools;
        
        private readonly ILogger _logger = Log.ForContext<Tools>();
        private IEnumerable<string> _allPathFiles = Array.Empty<string>();
        private TypeMarkup _typeMarkup = TypeMarkup.PointMark;
        
        private static readonly object _lockerDataChanges = new object();

        private string _rootDirImages =
         //   "/mnt/Disk_1/TMP/IMG";   
          "/mnt/Disk_D/TMP/01.09.2021/Img/";
           // "/mnt/Disk_D/Jupyter/Torch/vae.pytorch/data/celeba/images/";
           
           internal IEnumerable<PaletteData> _palettesData = new[]
           {
               new PaletteData() {ClassId = 0, ColorClass = "#ffffff", NameClass = "fon"},
               new PaletteData() {ClassId = 1, ColorClass = "#ff0000",NameClass = "Kolodka"},
               new PaletteData() {ClassId = 2, ColorClass = "#00ff00",NameClass = "Long name Class"},
               new PaletteData() {ClassId = 3, ColorClass = "#0000ff",NameClass = "Русскими_буквами_класс_ооочень_длинное_название"},
               new PaletteData() {ClassId = 4, ColorClass = "#deb887",NameClass = "Двухстрочник проверка переноса символов"},
               new PaletteData() {ClassId = 5, ColorClass = "#ff00ff",NameClass = "Riska"}
           };
           
        public MarkupControlService(Tools tools)
        {
            _tools = tools ?? throw new ArgumentException(nameof(tools));
        }

        
        public Task<IEnumerable<string>> GetAllFileNamesAsync()
        {
            return Task.Run(()=>
            {
                lock (_lockerDataChanges)
                {
                    return _allPathFiles;
                }
            });
            
        }

        public Task SetTypeMarkupAsync(TypeMarkup typeMarkup)
        {
            return Task.Run(()=>
            {
                lock(_lockerDataChanges)
                {
                    _typeMarkup = typeMarkup;
                }
            });
        }

        public Task<TypeMarkup> GetTypeMarkupAsync()
        {
            return Task.Run(()=>
            {
                lock (_lockerDataChanges)
                {
                    return _typeMarkup;
                }
            });
        }

        public Task SetPathRootFolderImagesAsync(string path)
        {
            return Task.Run(()=>
            {
                lock(_lockerDataChanges)
                {
                    _rootDirImages = path;
                }
            });
        }
        
        public Task<string> GetPathRootFolderImagesAsync()
        {
            return Task.Run(()=>
            {
                lock (_lockerDataChanges)
                {
                    return _rootDirImages;
                }
            });
        }

        public Task SearchAllImagesAsync()
        {
            return Task.Run(()=>
            {
                var files = _tools.SearchFiles(_rootDirImages);
                if (files is null || !files.Any())
                {
                    _logger.Error("Not Find files {Path}",_rootDirImages);
                    return;
                }

                lock (_lockerDataChanges)
                {
                    _allPathFiles = files;
                }
            });
         
            
        }
        public Task<IEnumerable<PaletteData>> GetPaletteAsync()
        {
            return Task.Run(()=>
            {
                lock (_lockerDataChanges)
                {
                    return _palettesData;
                }
            });
        }
    }
}