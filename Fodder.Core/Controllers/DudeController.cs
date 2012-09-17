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
using Fodder.Core.UX;

namespace Fodder.Core
{
    class DudeController
    {
        const int MAX_DUDES = 1000;

        public Dude[] Dudes;
        public Random Rand = new Random();

        Texture2D _texDude;

        public DudeController()
        {
            Dudes = new Dude[MAX_DUDES];
        }

        public void LoadContent(ContentManager content)
        {
            _texDude = content.Load<Texture2D>("dude");

            for (int i = 0; i < MAX_DUDES; i++)
                Dudes[i] = new Dude(_texDude);
        }

        public void Update(GameTime gameTime)
        {
            ResetShield();

            foreach (Dude d in Dudes)
            {
                if (d.ShieldTime > 0) PropogateShield(d);

                d.Update(gameTime);
            }

            // We resolve deaths separately because it will be possible for a combat to be a draw
            foreach (Dude d in Dudes)
            {
                if (d.Active && d.Health <= 0)
                {
                    d.Active = false;
                    GameSession.Instance.SoulController.Add(d.Position, d.Team);
                    GameSession.Instance.ParticleController.AddGibs(d.HitPosition, d.Team);
                    if (d.Team == 0) { GameSession.Instance.Team1DeadCount++; }
                    if (d.Team == 1) { GameSession.Instance.Team2DeadCount++; }
                }
            }
        }

        public void HandleInput(IHumanPlayerControls playerControls, int team)
        {
            if (playerControls == null)
                throw new ArgumentException("Cannot handle little dude input without PlayerControls");

            bool found = false;

            foreach (Dude d in Dudes)
            {
                if (!d.Active) continue;

                if (!found)
                {
                    // This assumes that player is controlling team 0 (left)
                    if (d.UIRect.Contains(playerControls.X, playerControls.Y) && d.Team == team)
                    {
                        d.UIHover = true;
                        found = true;

                        if (playerControls.Select)
                        {
                            GameSession.Instance.ButtonController.DudeClicked(d);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin();

            foreach (Dude d in Dudes)
            {
                d.Draw(sb);
            }

            sb.End();
        }

        public void DrawShield(SpriteBatch sb)
        {
            sb.Begin();

            foreach (Dude d in Dudes)
            {
                d.DrawShield(sb);
            }

            sb.End();
        }

        public void Add(Vector2 spawnPos, int team)
        {
            foreach(Dude d in Dudes)
                if (!d.Active)
                {
                    d.Spawn(spawnPos, team);
                    break;
                }
        }

        public void AddEliteSquad(Vector2 spawnPos, int team)
        {
            int count = 0;
            foreach (Dude d in Dudes)
                if (!d.Active)
                {
                    d.Spawn(spawnPos, team);
                    d.BoostTime = 20000;
                    if (count == 2) d.ShieldTime = 20000;
                    switch (Rand.Next(3))
                    {
                        case 0:
                            d.GiveWeapon("pistol");
                            d.Weapon.CurrentAmmo = 20;
                            break;
                        case 1:
                            d.GiveWeapon("shotgun");
                            d.Weapon.CurrentAmmo = 16;
                            break;
                        case 2:
                            d.GiveWeapon("smg");
                            d.Weapon.CurrentAmmo = 60;
                            break;
                    }
                    count++;
                    spawnPos += new Vector2((team == 0 ? -50 : 50), 0);
                    if (count == 5) break;
                }
        }

        public Dude EnemyInRange(Dude owner, float range, bool checkLOS)
        {
            Dude returnDude = null;

            foreach (Dude d in Dudes)
            {
                if (!d.Active) continue;

                if (owner == d) continue;

                if (owner.Team == d.Team) continue;

                if (owner.Position.X > d.Position.X && owner.PathDirection == 1) continue;
                if (owner.Position.X < d.Position.X && owner.PathDirection == -1) continue;

                float distance = (owner.Position - d.Position).Length();

                if (distance > range) continue;

                if (checkLOS)
                {
                    Vector2 testVect = owner.WeaponPosition;
                    float amount = 0f;

                    while ((testVect - d.HitPosition).Length() > 2f)
                    {
                        if (GameSession.Instance.Map.TryGetPath((int)testVect.X, (int)testVect.Y) - 10 < testVect.Y) break;

                        testVect = Vector2.Lerp(owner.WeaponPosition, d.HitPosition, amount);
                        amount += 0.01f;
                    }
                    if ((testVect - d.HitPosition).Length() <= 2f) returnDude = d;
                }
                else returnDude = d;

                
            }

            return returnDude;
        }

        private void ResetShield()
        {
            foreach (Dude t in Dudes) t.IsShielded = false;
        }

        private void PropogateShield(Dude d)
        {
            if (!d.Active) return;

            foreach (Dude t in Dudes)
            {
                if (!t.Active) continue;
                if (d.Team != t.Team) continue;

                if ((d.Position - t.Position).Length() <= 300) 
                    t.IsShielded = true;
            }
        }

        internal void Reset()
        {
            foreach (Dude d in Dudes)
            {
                d.Active = false;
            }
        }
    }
}
