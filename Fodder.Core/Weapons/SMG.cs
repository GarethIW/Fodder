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
    class SMG:BaseWeapon
    {

        public SMG(Dude owner) : base(owner)
        {
            Range = 400f;
            StartingAmmo = 30;
            CurrentAmmo = 30;
            WeaponOffset = new Vector2(5, -20);
            FeetPlanted = false;

            _targetAttackTime = 100;
            _isPlantingWeapon = false;

        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Owner.texDude, Owner._screenRelativePosition - (new Vector2(0, 80) * GameSession.Instance.Map.Zoom), new Rectangle(220, 0, 20, 20),
                     Color.White,
                     0f,
                     new Vector2(10, 0),
                     1f,
                     SpriteEffects.None, 0);

            base.Draw(sb);
        }

        public override void Attack(Dude targetDude)
        {
            Vector2 velocity = (targetDude.WeaponPosition - Owner.WeaponPosition);
            velocity.Y += ((float)(ProjectileController.Rand.NextDouble() * 20))-10f;
            velocity.Normalize();

            GameSession.Instance.ProjectileController.Add(Owner.WeaponPosition,
                                     velocity * 15f,
                                     0.5f, false, false, 25, Owner.Team);
            CurrentAmmo--;

            AudioController.PlaySFX("smg", 0.5f * GameSession.Instance.Map.Zoom, ((float)GameSession.Instance.DudeController.Rand.NextDouble() / 2f) + 0.4f, ((2f / GameSession.Instance.Viewport.Width) * Owner._screenRelativePosition.X) - 1f);

            base.Attack(targetDude);
        }

    }
}

