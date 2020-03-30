using System;

namespace Leibit.Controls
{

    #region [eResizeMode]
    [Flags]
    public enum eResizeMode
    {
        NoResize = 0,
        ResizeWidth = 1,
        ResizeHeight = 2,
        ResizeAll = ResizeWidth | ResizeHeight,
    }
    #endregion

}
