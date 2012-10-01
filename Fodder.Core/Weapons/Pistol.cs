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
    class Pistol:BaseWeapon
    {

        public Pistol(Dude owner) : base(owner)
        {
            Range = 400f;
            StartingAmmo = 10;
            CurrentAmmo = 10;
            WeaponOffset = new Vector2(5, -20);

            _targetAttackTime = 500;
            _currentAttackTime = 500;
        }


        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Owner.texDude, Owner._screenRelativePosition - new Vector2(0, 80 * GameSession.Instance.Map.Zoom), new Rectangle(120, 0, 20, 20),
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
            velocity.Normalize();

            GameSession.Instance.ProjectileController.Add(Owner.WeaponPosition,
                                     velocity * 10f,
                                     0.5f, false, false, 50, Owner.Team);
            CurrentAmmo--;

            AudioController.PlaySFX("pistol", 0.3f * (GameSession.Instance.Map.Zoom * 1.5f), ((float)GameSession.Instance.DudeController.Rand.NextDouble() / 2f) + 0.4f, ((2f / GameSession.Instance.Viewport.Width) * Owner._screenRelativePosition.X) - 1f);

            base.Attack(targetDude);
        }

    }
}

