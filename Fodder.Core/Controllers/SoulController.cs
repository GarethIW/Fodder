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
    class Soul
    {
        public Vector2 Position = new Vector2(-50,-50);
        public int Team;
        public float Alpha;
        public bool Active;

        public Vector2 ScreenRelativePosition;
    }

    class SoulController
    {
        const int MAX_SOULS = 1000;

        public static Soul[] Souls;

        static Texture2D _texDude;

        public SoulController()
        {
            Souls = new Soul[MAX_SOULS];
        }

        public void LoadContent(ContentManager content)
        {
            _texDude = content.Load<Texture2D>("dude");

            for (int i = 0; i < MAX_SOULS; i++)
                Souls[i] = new Soul();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Soul s in Souls)
            {
                if (!s.Active) continue;

                if (s.Position.Y > -50)
                {
                    s.Position.Y -= 1f;
                }
                else
                {
                   s.Active = false;
                   if(s.Team==0) GameSession.Instance.Team1SoulCount++;
                   if (s.Team == 1) GameSession.Instance.Team2SoulCount++; 
                }

                s.ScreenRelativePosition = -GameSession.Instance.Map.ScrollPos + (new Vector2(0, (GameSession.Instance.Viewport.Height- GameSession.Instance.ScreenBottom) - (GameSession.Instance.Map.Height * GameSession.Instance.Map.Zoom)) + (s.Position * GameSession.Instance.Map.Zoom));

                if (s.Position.Y < 0)
                {
                    s.Alpha -= 0.01f;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();

            foreach (Soul s in Souls)
            {
                if (!s.Active) continue;

                Rectangle sourceRect = new Rectangle(s.Team * 40, 0, 40, 40);
                sb.Draw(_texDude, s.ScreenRelativePosition, sourceRect,
                    Color.White * s.Alpha,
                    0f,
                    new Vector2(sourceRect.Width / 2, sourceRect.Height),
                    GameSession.Instance.Map.Zoom,
                    SpriteEffects.None, 0);
            }

            sb.End();
        }

        public void Add(Vector2 spawnPos, int team)
        {
            foreach(Soul s in Souls)
                if (!s.Active)
                {
                    s.Position = spawnPos;
                    s.Team = team;
                    s.Alpha = 0.4f;
                    s.Active = true;
                    break;
                }
        }


        public void Reset()
        {
            foreach (Soul s in Souls) s.Active = false;
        }
    }
}
