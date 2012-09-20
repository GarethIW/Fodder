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

namespace Fodder.Core
{
    class HUD
    {
        Texture2D _texHud;
        SpriteFont _font;

        Vector2 _team1Bar;
        Vector2 _team2Bar;

        int _barWidth;
        int _barHeight;

        public HUD() { }

        public void LoadContent(ContentManager content)
        {
            _texHud = content.Load<Texture2D>("hud");
            _font = content.Load<SpriteFont>("font");

            _barWidth = (GameSession.Instance.Viewport.Width - 660) /2;
            _barHeight = 60;

            _team1Bar = new Vector2(0, GameSession.Instance.Viewport.Height-60);
            _team2Bar = new Vector2(GameSession.Instance.Viewport.Width - _barWidth, GameSession.Instance.Viewport.Height - 60);
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();

            int reinforcementswidth = (int)((_barWidth / (float)GameSession.Instance.Team1StartReinforcements) * (float)GameSession.Instance.Team1Reinforcements);
            int activewidth = (int)((_barWidth / (float)GameSession.Instance.Team1StartReinforcements) * (float)GameSession.Instance.Team1ActiveCount);
            if (GameSession.Instance.Team1DeadCount == 0 && (reinforcementswidth + activewidth < _barWidth)) activewidth += 1;
            sb.Draw(_texHud, _team1Bar, new Rectangle(0, 0, _barWidth, _barHeight), Color.White);
            sb.Draw(_texHud, _team1Bar + new Vector2(reinforcementswidth, 0), new Rectangle(reinforcementswidth, _barHeight, activewidth, _barHeight), Color.White);
            sb.Draw(_texHud, _team1Bar, new Rectangle(0, _barHeight * 2, reinforcementswidth, _barHeight), Color.White);

            reinforcementswidth = (int)((_barWidth / (float)GameSession.Instance.Team2StartReinforcements) * (float)GameSession.Instance.Team2Reinforcements);
            activewidth = (int)((_barWidth / (float)GameSession.Instance.Team2StartReinforcements) * (float)GameSession.Instance.Team2ActiveCount);
            if (GameSession.Instance.Team2DeadCount == 0 && (reinforcementswidth + activewidth < _barWidth)) activewidth += 1;
            sb.Draw(_texHud, _team2Bar, new Rectangle(0, 0, _barWidth, _barHeight), Color.White);
            sb.Draw(_texHud, _team2Bar + new Vector2(_barWidth - reinforcementswidth - activewidth, 0), new Rectangle(_barWidth - reinforcementswidth - activewidth, _barHeight, activewidth, _barHeight), Color.White);
            sb.Draw(_texHud, _team2Bar + new Vector2(_barWidth - reinforcementswidth, 0), new Rectangle(_barWidth - reinforcementswidth, _barHeight * 2, reinforcementswidth, _barHeight), Color.White);

            //if (GameSession.Instance.Team1Win && GameSession.Instance.Team2Win) sb.DrawString(_font, "Nobody wins", new Vector2(GameSession.Instance.Viewport.Width, GameSession.Instance.Viewport.Height) / 2, Color.White, 0f, _font.MeasureString("Nobody wins") / 2, 1f, SpriteEffects.None, 1);
            //else
            //{
            //    if (GameSession.Instance.Team1Win) sb.DrawString(_font, "Reds Win", new Vector2(GameSession.Instance.Viewport.Width, GameSession.Instance.Viewport.Height) / 2, Color.White, 0f, _font.MeasureString("Reds Win") / 2, 1f, SpriteEffects.None, 1);
            //    if (GameSession.Instance.Team2Win) sb.DrawString(_font, "Blues Win", new Vector2(GameSession.Instance.Viewport.Width, GameSession.Instance.Viewport.Height) / 2, Color.White, 0f, _font.MeasureString("Blues Win") / 2, 1f, SpriteEffects.None, 1);
            //}

            sb.End();
        }


        
    }
}
