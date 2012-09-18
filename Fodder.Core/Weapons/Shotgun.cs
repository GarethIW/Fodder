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
    class Shotgun:BaseWeapon
    {

        public Shotgun(Dude owner)
            : base(owner)
        {
            Range = 150f;
            StartingAmmo = 8;
            CurrentAmmo = 8;
            WeaponOffset = new Vector2(5, -20);

            _targetAttackTime = 2000;
            _currentAttackTime = 2000;
        }


        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Owner.texDude, Owner._screenRelativePosition - new Vector2(0, 80 * GameSession.Instance.Map.Zoom), new Rectangle(200, 0, 20, 20),
                     Color.White,
                     0f,
                     new Vector2(10, 0),
                     1f,
                     SpriteEffects.None, 0);

            base.Draw(sb);
        }

        public override void Attack(Dude targetDude)
        {
            

            for(int i=0;i<5;i++)
            {
                Vector2 velocity = (targetDude.WeaponPosition - Owner.WeaponPosition);
                velocity += new Vector2(0, ((float)(ProjectileController.Rand.NextDouble() * 40))-20f);
                velocity.Normalize();
                GameSession.Instance.ProjectileController.Add(Owner.WeaponPosition,
                                     velocity  * 10f,
                                     0.5f, false, false, 25, Owner.Team);
            }

            CurrentAmmo--;

            AudioController.PlaySFX("shotgun", 0.6f * (GameSession.Instance.Map.Zoom * 1.5f), ((float)GameSession.Instance.DudeController.Rand.NextDouble() / 2f) + 0.2f, ((2f / GameSession.Instance.Viewport.Width) * Owner._screenRelativePosition.X) - 1f);

            base.Attack(targetDude);
        }

    }
}

