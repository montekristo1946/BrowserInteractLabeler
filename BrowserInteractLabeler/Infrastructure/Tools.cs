using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace BrowserInteractLabeler.Infrastructure
{
    public class Tools
    {
        private readonly ILogger _logger = Log.ForContext<Tools>();
        
        public string[] SearchFiles(string pathRoot)
        {
            _logger.Debug("[Tools:SearchFiles] Call");
            if (string.IsNullOrEmpty(pathRoot) || !Directory.Exists(pathRoot))
            {
                _logger.Error("Bad pathRoot");
                return Array.Empty<string>();
            }

            string[] extensions = { "jpg", "jpeg", "png", "bmp" };

            var files = Directory.GetFiles(pathRoot, "*.*",SearchOption.TopDirectoryOnly)
                .Where(f => extensions.Contains(f.Split('.').Last().ToLower())).ToArray();

            return files;
        }

        public string GetNextElement(IEnumerable<string> allImg, string currentImg)
        {
            //_logger.Debug("[Tools:SearchFiles] GetNextElement");
            try
            {
                var allImgList = allImg.ToList();
                var index = allImgList.FindIndex(c => c == currentImg);
                if (index>=0 && index < allImg.Count() - 1)
                {
                    return allImgList[index + 1];
                }
                
                if (index == allImg.Count() - 1)
                    return allImgList[0];
                
                return currentImg;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed {MethodName}: {ErrorMessage}",
                    MethodBase.GetCurrentMethod(), ex.Message);
            }

            return currentImg;
        }
    }
}