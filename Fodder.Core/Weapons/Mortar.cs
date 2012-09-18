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
    class Mortar:BaseWeapon
    {

        public Mortar(Dude owner) : base(owner)
        {
            Range = 1000f;
            StartingAmmo = 5;
            CurrentAmmo = 5;
            WeaponOffset = new Vector2(5, -30);
            FeetPlanted = true;

            _targetAttackTime = 3000;
            _isPlantingWeapon = true;
            _needsLOS = false;

        }

       

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Owner.texDude, Owner._screenRelativePosition - new Vector2(0, 80 * GameSession.Instance.Map.Zoom), new Rectangle(160, 0, 20, 20),
                     Color.White,
                     0f,
                     new Vector2(10, 0),
                     1f,
                     SpriteEffects.None, 0);

            base.Draw(sb);
        }

        public override void Attack(Dude targetDude)
        {
            //Vector2 velocity = (targetDude.WeaponPosition - Owner.WeaponPosition);
            //velocity.Y += ((float)(ProjectileController.Rand.NextDouble() * 10))-5f;
            Vector2 velocity = new Vector2(Owner.PathDirection, -3f);
            velocity.Normalize();

            GameSession.Instance.ProjectileController.Add(Owner.WeaponPosition,
                                     (velocity * 8f) + new Vector2((float)ProjectileController.Rand.NextDouble()*(float)Owner.PathDirection,(float)ProjectileController.Rand.NextDouble()),
                                     1f, true, true, 300, Owner.Team);
            CurrentAmmo--;

            AudioController.PlaySFX("mortar", 1f * (GameSession.Instance.Map.Zoom * 1.5f), 0f, ((2f / GameSession.Instance.Viewport.Width) * Owner._screenRelativePosition.X) - 1f);

            base.Attack(targetDude);
        }

    }
}

