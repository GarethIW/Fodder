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

namespace Fodder.Core.Weapons
{
    class Sword:BaseWeapon
    {

        public Sword(Dude owner) : base(owner)
        {
            Range = 25f;
            StartingAmmo = -1;
            CurrentAmmo = -1;
            WeaponOffset = new Vector2(5, -20);

            _targetAttackTime = 1000;

        }


        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void Attack(Dude targetDude)
        {
            GameSession.Instance.ParticleController.AddGSW(targetDude.HitPosition, (new Vector2(5, 0) * Owner.PathDirection), Owner.IsShielded);
            targetDude.Hit(20);

            AudioController.PlaySFX("sword", (GameSession.Instance.Map.Zoom * 1.5f), ((float)GameSession.Instance.DudeController.Rand.NextDouble() / 3f) + 0.4f, ((2f / GameSession.Instance.Viewport.Width) * Owner._screenRelativePosition.X) - 1f);

            base.Attack(targetDude);
        }

    }
}

