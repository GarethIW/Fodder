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
    class Projectile
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool Active;
        public int Team;
        public float Size;
        public bool Explosive;
        public int Damage;
        public bool AffectedByGravity;

        Texture2D _texDude;
        
        public Vector2 _screenRelativePosition;
        Rectangle _sourceRect;

        public Projectile(Texture2D texture)
        {
            _texDude = texture;
        }

        public void Spawn(Vector2 spawnPos, Vector2 velocity, float size, bool affectedbygravity, bool explosive, int damage, int team)
        {
            Position = spawnPos;
            Velocity = velocity;
            Team = team;
            Active = true;
            Explosive = explosive;
            Size = size;
            Damage = damage;
            AffectedByGravity = affectedbygravity;

            _sourceRect = new Rectangle(107, 7, 6, 6);
        }

        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            if (Position.X > GameSession.Instance.Map.Width || Position.X < 0) { Active = false; return; }

            if (Position.Y >= GameSession.Instance.Map.TryGetPath((int)Position.X, (int)Position.Y))
            {
                if (Explosive) GameSession.Instance.ProjectileController.CalculateExplosion(this);
                Active = false;
            }

            Position += Velocity;

            if(AffectedByGravity)
                Velocity += GameSession.Instance.Map.Gravity;

        }

        public void Draw(SpriteBatch sb)
        {
            _screenRelativePosition = (new Vector2(-GameSession.Instance.Map.ScrollPos.X, GameSession.Instance.Map.ScrollPos.Y + ((GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom) - (GameSession.Instance.Map.Height * GameSession.Instance.Map.Zoom))) + (Position * GameSession.Instance.Map.Zoom));

            if (!Active) return;

            // We'll draw the dude with an origin of bottom middle, bcause his position will be fixed to the path
            sb.Draw(_texDude, _screenRelativePosition , _sourceRect, 
                    Color.White,
                    0f,
                    new Vector2(_sourceRect.Width/2, _sourceRect.Height/2),
                    Size * GameSession.Instance.Map.Zoom, 
                    SpriteEffects.None, 0);
        }
       
    }
}
