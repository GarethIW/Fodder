using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Fodder.GameState;
using Microsoft.Xna.Framework;

namespace Fodder.WindowsPhone.UX
{
    public class TouchObserver
    {
        private InputState _InputState;

        internal void Update(InputState input)
        {
            this._InputState = input;
        }

        public Single HorizontalScroll
        {
            get
            {
                if (this._InputState == null)
                    return 0.0f;
                if (this._InputState.Gestures.Count <= 0)
                    return 0.0f;

                var gesture = this._InputState.Gestures[0];
                if (gesture.GestureType == GestureType.HorizontalDrag)
                {
                    return gesture.Delta.X;
                }

                return 0.0f;
            }
        }

        public Single Zoom
        {
            get
            {

                if (this._InputState == null)
                    return 1.0f;
                if (this._InputState.Gestures.Count <= 0)
                    return 1.0f;

                var gesture = this._InputState.Gestures[0];
                if (gesture.GestureType == GestureType.Pinch)
                {
                    return this.GetScaleFactor(gesture.Position, gesture.Position2, gesture.Delta, gesture.Delta2);
                }

                return 1.0f;
            }
        }

        public Int32 X 
        { 
            get 
            {
                if (this._InputState == null)
                    return 0;
                if (this._InputState.Gestures.Count <= 0)
                    return 0;

                return this.GetSingleTouch(this._InputState.Gestures[0], TouchAxis.X); 
            } 
        }

        public Int32 Y 
        { 
            get
            {
                if (this._InputState == null)
                    return 0;
                if (this._InputState.Gestures.Count <= 0)
                    return 0;

                return this.GetSingleTouch(this._InputState.Gestures[0], TouchAxis.Y); 
            } 
        }

        private Int32 GetSingleTouch(GestureSample gesture, TouchAxis axis)
        {
            // fail fast
            if (gesture.GestureType != GestureType.Tap)
                return 0;

            var location = 0;

            if (axis == TouchAxis.X)
                location = (Int32)gesture.Position.X;
            else
                location = (Int32)gesture.Position.Y;

            return location;
        }

        float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 delta1, Vector2 delta2)
        {
            Vector2 oldPosition1 = position1 - delta1;
            Vector2 oldPosition2 = position2 - delta2;

            float distance = Vector2.Distance(position1, position2);
            float oldDistance = Vector2.Distance(oldPosition1, oldPosition2);

            if (oldDistance == 0 || distance == 0)
            {
                return 1.0f;
            }

            return (distance / oldDistance);
        }
    }
}
