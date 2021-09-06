using System;
using System.Threading.Tasks;
using BrowserInteractLabeler.Common;

namespace BrowserInteractLabeler.Infrastructure
{
    public class KeyPressPaletteGridHandler
    {
        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        private static readonly object _lockerCurrentPalette = new object();

        private PaletteData _currentPalette;
        
        
        public PaletteData CurrentPalette
        {
            get
            {
                lock (_lockerCurrentPalette)
                {
                    return _currentPalette;
                }
            }
            private set
            {
                lock (_lockerCurrentPalette)
                {
                    _currentPalette = value;
                }
            }
        }

        
        public void SetCurrentPaletteButton(PaletteData paletteData)
        {
            lock (_lockerCurrentPalette)
            {
                if (paletteData == _currentPalette)
                {
                    return;
                }

                _currentPalette = paletteData;
                NotifyStateChanged();
            }
        }
        
    }
}