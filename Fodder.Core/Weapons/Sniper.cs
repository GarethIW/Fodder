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
    class Sniper:BaseWeapon
    {

        public Sniper(Dude owner)
            : base(owner)
        {
            Range = 2000f;
            StartingAmmo = 20;
            CurrentAmmo = 20;
            WeaponOffset = new Vector2(5, -20);
            FeetPlanted = true;

            _targetAttackTime = 5000;
            _currentAttackTime = 5000;
            _isPlantingWeapon = true;
        }


        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Owner.texDude, Owner._screenRelativePosition - new Vector2(0, 80 * GameSession.Instance.Map.Zoom), new Rectangle(180, 0, 20, 20),
                     Color.White,
                     0f,
                     new Vector2(10, 0),
                     1f,
                     SpriteEffects.None, 0);

            base.Draw(sb);
        }

        public override void Attack(Dude targetDude)
        {
            Vector2 velocity = ((targetDude.HitPosition - new Vector2(0,10f)) - Owner.WeaponPosition);
            velocity.Normalize();

            GameSession.Instance.ProjectileController.Add(Owner.WeaponPosition,
                                     velocity * 30f,
                                     0.5f, false, false, 100, Owner.Team);
            CurrentAmmo--;

            AudioController.PlaySFX("sniper", 0.8f * (GameSession.Instance.Map.Zoom * 1.5f), ((float)GameSession.Instance.DudeController.Rand.NextDouble() / 1f) - 0.5f, ((2f / GameSession.Instance.Viewport.Width) * Owner._screenRelativePosition.X) - 1f);

            base.Attack(targetDude);
        }

    }
}

