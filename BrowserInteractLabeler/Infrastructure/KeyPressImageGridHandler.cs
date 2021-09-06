using System;
using System.Threading.Tasks;

namespace BrowserInteractLabeler.Infrastructure
{
    public class KeyPressImageGridHandler
    {
        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        private static readonly object _lockerCurrentFileName = new object();

        private string _currentImages = string.Empty;
        
        public string CurrentImages
        {
            get
            {
                lock (_lockerCurrentFileName)
                {
                    return _currentImages;
                }
            }
            private set
            {
                lock (_lockerCurrentFileName)
                {
                    _currentImages = value;
                }
            }
        }


        public void SetCurrentImagesButton(string imgName)
        {
            lock (_lockerCurrentFileName)
            {
                if (imgName == _currentImages)
                {
                    return;
                }

                _currentImages = imgName;
                NotifyStateChanged();
            }
        }
        
    }
}