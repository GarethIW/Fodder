using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fodder.Core.UX;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Fodder.GameState;

namespace Fodder.WindowsPhone.UX
{
    public class WindowsPhonePlayerControls : IHumanPlayerControls
    {
        #region Fields

        private TouchObserver _TouchObserver;
        private ButtonObserver _ButtonObserver;

        #endregion

        public WindowsPhonePlayerControls(TouchObserver touchObserver, ButtonObserver buttonObserver)
        {
            if (touchObserver == null)
                throw new ArgumentException("Cannot construct a WindowsPhonePlayerControls without a TouchObserver");
            if (buttonObserver == null)
                throw new ArgumentException("Cannot construct a WindowsPhonePlayerControls without a ButtonObserver");

            this._TouchObserver = touchObserver;
            this._ButtonObserver = buttonObserver;
        }

        internal void Update(InputState input)
        {
            this._TouchObserver.Update(input);
        }

        #region IHumanPlayerControls members

        public ZoomDirection Zoom
        {
            get
            {
                var zoom = this._TouchObserver.Zoom;
                if (zoom > 1.0f) return ZoomDirection.Out;
                if (zoom < 1.0f) return ZoomDirection.In;

                return ZoomDirection.None;
            }
        }

        public ScrollDirection Scroll
        {
            get
            {
                var zoom = this._TouchObserver.HorizontalScroll;
                if (zoom > 0.0f) 
                    return ScrollDirection.Right;
                if (zoom < 0.0f) 
                    return ScrollDirection.Left;

                return ScrollDirection.None;
            }
        }

        public Int32 X { get { return this._TouchObserver.X; } }

        public Int32 Y { get { return this._TouchObserver.Y; } }

        public Boolean Select
        {
            get { return true; }
        }

        public Boolean Reset
        {
            get { return false; }
        }

        public Boolean IsButtonShortcutKeyPressed(Keys key)
        {
            return false;
        }

        public Boolean IsPhone { get { return true; } }

        #endregion
    }
}
