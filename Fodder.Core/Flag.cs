using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Fodder.Core.Weapons;

namespace Fodder.Core
{
    public class Flag
    {
        public Vector2 Position;
        public int Team;

        public int RaisedHeight;
        public int NumLowering = 0;

        public Vector2 _screenRelativePosition;

        Texture2D _texFlag;

        Rectangle _sourceRectPole;
        Rectangle _sourceRectFlag;

        double _lowerTimer = 0;
        double _lowerTargetTime = 1000;
        double _raiseTargetTime = 3000;

        public Flag(Texture2D texture, Vector2 pos, int team)
        {
            _texFlag = texture;
            Position = pos;
            RaisedHeight = 100;
            Team = team;

            _sourceRectPole = new Rectangle(22, 0, 6, 100);
            _sourceRectFlag = new Rectangle((Team == 0 ? 29 : 0), 0, 21, 16);
        }

        public void Update(GameTime gameTime)
        {
            if (NumLowering > 0)
            {
                _lowerTimer += gameTime.ElapsedGameTime.TotalMilliseconds * (double)NumLowering;

                if (_lowerTimer >= _lowerTargetTime)
                {
                    _lowerTimer = 0;
                    if (RaisedHeight > 16) RaisedHeight--;
                }
            }
            else
            {
                _lowerTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_lowerTimer >= _raiseTargetTime)
                {
                    _lowerTimer = 0;
                    if (RaisedHeight < 100) RaisedHeight++;
                }
            }

            // Reset the lowering counter for the next loop
            NumLowering = 0;

            
        }

        public void Draw(SpriteBatch sb)
        {
            _screenRelativePosition = (new Vector2(-GameSession.Instance.Map.ScrollPos.X, GameSession.Instance.Map.ScrollPos.Y + ((GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom) - (GameSession.Instance.Map.Height * GameSession.Instance.Map.Zoom))) + (Position * GameSession.Instance.Map.Zoom));

            sb.Draw(_texFlag, _screenRelativePosition  , _sourceRectPole, 
                    Color.White,
                    0f,
                    new Vector2(_sourceRectPole.Width/2, _sourceRectPole.Height),
                    GameSession.Instance.Map.Zoom, 
                    SpriteEffects.None, 0);
            sb.Draw(_texFlag, _screenRelativePosition + (new Vector2(0,-RaisedHeight) * GameSession.Instance.Map.Zoom), _sourceRectFlag,
                    Color.White,
                    0f,
                    new Vector2((Team==1?_sourceRectFlag.Width:0), 0),
                    GameSession.Instance.Map.Zoom,
                    SpriteEffects.None, 0);
        }
       
    }
}
