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
    class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool Active;
        public bool AffectedByGravity;
        public float Alpha;
        public double Life;
        public float RotationSpeed;
        public float Rotation;
        public Rectangle SourceRect;

        
    }

    class ParticleController
    {
        const int MAX_PARTICLES = 3000;

        public Particle[] Particles;
        public Random Rand = new Random();

        Texture2D _texParticles;

        public ParticleController()
        {
            Particles = new Particle[MAX_PARTICLES];
        }

        public void LoadContent(ContentManager content)
        {
            _texParticles = content.Load<Texture2D>("particles");

            for (int i = 0; i < MAX_PARTICLES; i++)
                Particles[i] = new Particle();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Particle p in Particles)
            {
                p.Life -= gameTime.ElapsedGameTime.TotalMilliseconds;
                p.Position += p.Velocity;
                p.Rotation += p.RotationSpeed;

                if (p.AffectedByGravity) p.Velocity += GameSession.Instance.Map.Gravity;

                if (GameSession.Instance.Map.TryGetPath((int)p.Position.X, 1000) <= p.Position.Y)
                {
                    p.Velocity = Vector2.Zero;
                    p.RotationSpeed = 0f;
                }

                if (p.Life <= 0)
                {
                    p.Alpha -= 0.01f;
                    if (p.Alpha < 0.05f) p.Active = false;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();

            foreach (Particle p in Particles)
            {
                sb.Draw(_texParticles, 
                         new Vector2(-GameSession.Instance.Map.ScrollPos.X, GameSession.Instance.Map.ScrollPos.Y + ((GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom) - (GameSession.Instance.Map.Height * GameSession.Instance.Map.Zoom))) + (p.Position * GameSession.Instance.Map.Zoom),
                        p.SourceRect, Color.White * p.Alpha, p.Rotation, new Vector2(p.SourceRect.Width / 2, p.SourceRect.Height / 2), GameSession.Instance.Map.Zoom, SpriteEffects.None, 1);
            }

            sb.End();
        }

        public void Add(Vector2 spawnPos, Vector2 velocity, float life, bool affectedbygravity, Rectangle sourcerect, float rot)
        {
            foreach (Particle p in Particles)
                if (!p.Active)
                {
                    p.Position = spawnPos;
                    p.Velocity = velocity;
                    p.Life = life;
                    p.AffectedByGravity = affectedbygravity;
                    p.SourceRect = sourcerect;
                    p.Alpha = 1f;
                    p.Active = true;
                    p.RotationSpeed = rot;
                    break;
                }
        }

        public void AddGSW(Vector2 pos, Vector2 velocity, bool shield)
        {
            Vector2 tempV = pos;
            float amount = 0f;
            while (amount<1f)
            {
                Add(tempV, ((velocity * (shield ? 0.2f : 1f))/5f) + new Vector2((float)(Rand.NextDouble()*2)-1f,(float)(Rand.NextDouble()*2)-1f), 5000, true, new Rectangle(0, 0, 5, 5), 0f);
                tempV = Vector2.Lerp(pos, pos+velocity, amount);
                amount += shield?0.2f:0.05f;
            }
        }

        public void AddExplosion(Vector2 pos)
        {
            pos.Y -= 2f;
            Vector2 tempV = pos;
            float amount = 0f;
            Vector2 velocity = new Vector2(0, -3f);
            while (amount < 1f)
            {
                Add(tempV, (velocity) + new Vector2((float)(Rand.NextDouble() * 10) - 5f, (float)(Rand.NextDouble() * 4) - 2f), 1000, true, new Rectangle(5, 0, 7, 7),0f);
                tempV = Vector2.Lerp(pos, pos + velocity, amount);
                amount += 0.01f;
            }
        }

        public void AddGibs(Vector2 hitpos, int team)
        {
            if (team == 0)
            {
                Add(hitpos + new Vector2(0, -10), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(23, 0, 15, 12), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(0, 0), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(28, 12, 5, 21), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(-10, 0), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(17, 16, 12, 6), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(10, 0), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(33, 16, 12, 6), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(-10, 10), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(16, 28, 12, 11), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(10, 10), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(33, 28, 12, 11), (float)(Rand.NextDouble() / 10) - 0.05f);
            }
            if (team == 1)
            {
                Add(hitpos + new Vector2(0, -10), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(63, 0, 15, 12), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(0, 0), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(68, 12, 5, 21), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(-10, 0), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(57, 16, 12, 6), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(10, 0), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(73, 16, 12, 6), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(-10, 10), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(56, 28, 12, 11), (float)(Rand.NextDouble() / 10) - 0.05f);
                Add(hitpos + new Vector2(10, 10), new Vector2((float)(Rand.NextDouble() * 4) - 2f, (float)(Rand.NextDouble() * 4) - 2f), 5000, true, new Rectangle(73, 28, 12, 11), (float)(Rand.NextDouble() / 10) - 0.05f);
            }
        }

        public void AddBoost(Vector2 dudePos)
        {
            Vector2 spawnPos = dudePos + new Vector2((float)(Rand.NextDouble() * 20) - 10, -(float)(Rand.NextDouble() * 10));
            Add(spawnPos, new Vector2(0, -0.1f), 100, false, new Rectangle(0, 10, 3, 3), (float)(Rand.NextDouble() / 10) - 0.05f);
        }

        internal void Reset()
        {
            foreach (Particle p in Particles)
            {
                p.Active = false;
            }
        }
    }
}
