using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fodder.Core.UX;
using Microsoft.Xna.Framework.Input;

namespace Fodder.Windows.UX
{
    /// <summary>
    /// Provides a concrete implementation for the <see cref="IHumanPlayerControls"/> interface
    /// that is a dependency of the Fodder game engine that allows a human player to play the 
    /// game.
    /// </summary>
    public class WindowsPlayerControls : IHumanPlayerControls
    {
        #region Fields

        private MouseObserver _MouseObserver;
        private KeyboardObserver _KeyboardObserver;
        private Int32 _LastKnownMouseScrollWheelValue;

        #endregion

        /// <summary>
        /// Constructs a new instance of the <see cref="WindowsPlayerControls"/> class.
        /// </summary>
        /// <param name="mouseObserver">Requires a <see cref="MouseObserver"/> object.</param>
        /// <param name="keyboardObserver">Requires a <see cref="KeyboardObserver"/> object.</param>
        public WindowsPlayerControls(MouseObserver mouseObserver, KeyboardObserver keyboardObserver)
        {
            if (mouseObserver == null)
                throw new ArgumentException("Cannot construct a WindowsPlayerControls without a MouseObserver");
            if (keyboardObserver == null)
                throw new ArgumentException("Cannot construct a WindowsPlayerControls without a KeyboardObserver");

            this._MouseObserver = mouseObserver;
            this._KeyboardObserver = keyboardObserver;

            this._LastKnownMouseScrollWheelValue = this._MouseObserver.MouseScrollWheelValue;
        }

        #region IHumanPlayerControls members

        public ZoomDirection Zoom
        {
            get
            {
                var zoomDirection = ZoomDirection.None;

                if (this._MouseObserver.MouseScrollWheelValue > this._LastKnownMouseScrollWheelValue)
                    zoomDirection = ZoomDirection.In;
                if (this._MouseObserver.MouseScrollWheelValue < this._LastKnownMouseScrollWheelValue)
                    zoomDirection = ZoomDirection.Out;

                this._LastKnownMouseScrollWheelValue = this._MouseObserver.MouseScrollWheelValue;

                return zoomDirection;
            }
        }

        public ScrollDirection Scroll
        {
            get
            {
                var scrollDirection = ScrollDirection.None;

                if (this._KeyboardObserver.IsKeyPressed(Keys.A))
                    scrollDirection = ScrollDirection.Left;
                if (this._KeyboardObserver.IsKeyPressed(Keys.D))
                    scrollDirection = ScrollDirection.Right;

                return scrollDirection;
            }
        }

        public Int32 X
        {
            get { return this._MouseObserver.X; }
        }

        public Int32 Y
        {
            get { return this._MouseObserver.Y; }
        }

        public Boolean Select
        {
            get { return this._MouseObserver.LeftButtonClicked; }
        }

        public Boolean Reset
        {
            get { return this._KeyboardObserver.IsKeyPressed(Keys.F12); }
        }

        public Boolean IsButtonShortcutKeyPressed(Keys key)
        {
            return this._KeyboardObserver.IsKeyPressed(key);
        }

        public Boolean IsPhone { get { return false; } }

        #endregion
    }
}
