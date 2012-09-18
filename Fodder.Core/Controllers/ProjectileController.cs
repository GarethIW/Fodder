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
    class ProjectileController
    {
        const int MAX_PROJECTILES = 1000;

        public static Projectile[] Projectiles;
        public static Random Rand = new Random();

        static Texture2D _texDude;

        public ProjectileController()
        {
            Projectiles = new Projectile[MAX_PROJECTILES];
        }

        public void LoadContent(ContentManager content)
        {
            _texDude = content.Load<Texture2D>("dude");

            for (int i = 0; i < MAX_PROJECTILES; i++)
                Projectiles[i] = new Projectile(_texDude);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Projectile p in Projectiles)
            {
                p.Update(gameTime);

                if (!p.Active) continue;

                // Check collision
                foreach (Dude d in GameSession.Instance.DudeController.Dudes)
                {
                    if (!d.Active || !p.Active) continue;
                    if (d.Team == p.Team) continue;
                    if ((p.Position - d.HitPosition).Length() < 20f)
                    {
                        if (p.Explosive)
                        {
                            continue;
                            //CalculateExplosion(p);
                        }
                        else
                        {
                            d.Hit(p.Damage);
                            GameSession.Instance.ParticleController.AddGSW(p.Position, p.Velocity, d.IsShielded);
                        }
                        p.Active = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();

            foreach (Projectile p in Projectiles)
            {
                p.Draw(sb);
            }

            sb.End();
        }

        public void Add(Vector2 spawnPos, Vector2 velocity, float size, bool affectedbygravity, bool explosive, int damage, int team)
        {
            foreach (Projectile p in Projectiles)
                if (!p.Active)
                {
                    p.Spawn(spawnPos, velocity, size, affectedbygravity, explosive, damage, team);
                    break;
                }
        }

        public void CalculateExplosion(Projectile p)
        {
            GameSession.Instance.ParticleController.AddExplosion(p.Position);
            AudioController.PlaySFX("explode", 1f * (GameSession.Instance.Map.Zoom * 1.5f), ((float)GameSession.Instance.DudeController.Rand.NextDouble() / 1f) - 0.5f, ((2f / GameSession.Instance.Viewport.Width) * p._screenRelativePosition.X) - 1f);
            foreach (Dude d in GameSession.Instance.DudeController.Dudes)
            {
                if (!d.Active) continue;
                if (d.Team==p.Team) continue;

                float dist = (p.Position - d.HitPosition).Length();
                if ((int)dist <= p.Damage)
                {
                    d.Hit(p.Damage - (int)dist);
                    GameSession.Instance.ParticleController.AddGSW(d.HitPosition, (new Vector2(0, -(p.Damage - dist)*0.1f)), d.IsShielded);
                }
            }
        }


        internal void Reset()
        {
            foreach (Projectile p in Projectiles)
            {
                p.Active = false;
            }
        }
    }
}
