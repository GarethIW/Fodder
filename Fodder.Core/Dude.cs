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
    class Dude
    {
        public Vector2 Position;
        public bool Active = false;
        public int Team;
        public BaseWeapon Weapon;
        public int PathDirection = 1;
        public int Health;
        public Vector2 WeaponPosition;
        public Vector2 HitPosition;

        public bool Jumping;
        public float JumpSpeed;

        public Rectangle UIRect;
        public bool UIHover;

        // Effects
        public double BoostTime = 0;
        public double ShieldTime = 0;

        public bool IsShielded = false;

        public Texture2D texDude;

        double _currentMoveTime = 0;
        double _targetMoveTime = 10;
        float _rot = 0f;
        int _movePixels = 1;
        internal Vector2 _screenRelativePosition;
        Rectangle _sourceRect;

        public Dude(Texture2D texture)
        {
            texDude = texture;
        }

        public void Spawn(Vector2 spawnPos, int team)
        {
            Position = spawnPos;
            Team = team;
            Active = true;
            Weapon = new Sword(this);
            Health = 100;
            IsShielded = false;
            Jumping = false;

            if (team == 0) PathDirection = 1; else PathDirection = -1;

            _sourceRect = new Rectangle(team * 40, 0, 40, 40);

            BoostTime = 0;
            ShieldTime = 0;
            UIHover = false;

            _currentMoveTime = 0;
            _targetMoveTime = 10;
            _rot = 0f;
            _movePixels = 1;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active) return;
            
            _currentMoveTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (!GameSession.Instance.Team1Win && !GameSession.Instance.Team2Win)
            {
                if (_currentMoveTime >= _targetMoveTime && !Weapon.FeetPlanted)
                {
                    _currentMoveTime = 0;

                    Position.X += PathDirection * _movePixels;
                    Position.Y = GameSession.Instance.Map.TryGetPath((int)Position.X, (int)Position.Y);
                }

                // Look at height of next piece of the path, and adjust things accordingly
                int slope = (GameSession.Instance.Map.TryGetPath((int)Position.X, (int)Position.Y) - GameSession.Instance.Map.TryGetPath((int)Position.X + (PathDirection * _movePixels), (int)Position.Y));// *PathDirection;
                if (slope == 0)
                {
                    if (_rot > 0f) _rot -= 0.01f;
                    if (_rot < 0f) _rot += 0.01f;
                    _targetMoveTime = 10;
                }
                if (slope > 0) { _rot += (Team==0?-0.02f:0.02f); _targetMoveTime = 25; }
                if (slope < 0) { _rot += (Team == 0 ? 0.02f : -0.02f); _targetMoveTime = 2; }
                _rot = MathHelper.Clamp(_rot, -0.4f, 0.4f);

                Weapon.Update(gameTime);

                // Effects
                if (BoostTime > 0)
                {
                    GameSession.Instance.ParticleController.AddBoost(Position);
                    BoostTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    _targetMoveTime = 0;
                    _movePixels = 2;
                }
                else _movePixels = 1;

                if (ShieldTime > 0)
                    ShieldTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if ((PathDirection == 1 && (int)Position.X >= GameSession.Instance.Map.Width + 40) || (PathDirection == -1 && (int)Position.X <= -40)) Active = false;
            }
            else
            {
                if (!(GameSession.Instance.Team1Win && GameSession.Instance.Team2Win))
                {
                    if ((GameSession.Instance.Team1Win && Team == 0) || (GameSession.Instance.Team2Win && Team == 1))
                    {
                        if (Position.X > 0 && Position.X < GameSession.Instance.Map.Width)
                        {
                            // DO some kind of win anim
                            if (!Jumping)
                            {
                                if (GameSession.Instance.DudeController.Rand.Next(100) == 1)
                                {
                                    Jumping = true;
                                    JumpSpeed = 2f;
                                }
                            }
                            else
                            {
                                Position += new Vector2(0, -1f * JumpSpeed);
                                JumpSpeed -= (GameSession.Instance.Map.Gravity.Y * 2);
                                if (Position.Y >= GameSession.Instance.Map.TryGetPath((int)Position.X, (int)Position.Y)) Jumping = false;
                            }
                        }
                    }
                }
            }

            _screenRelativePosition = -GameSession.Instance.Map.ScrollPos + (new Vector2(0, (GameSession.Instance.Viewport.Height- GameSession.Instance.ScreenBottom) - (GameSession.Instance.Map.Height * GameSession.Instance.Map.Zoom)) + (Position * GameSession.Instance.Map.Zoom));

            WeaponPosition = Position + (Weapon.WeaponOffset * new Vector2(PathDirection, 1));
            HitPosition = Position + new Vector2(0, -(_sourceRect.Width / 2));

            UIRect = new Rectangle((int)(_screenRelativePosition.X - ((_sourceRect.Width / 2) * GameSession.Instance.Map.Zoom)),
                                   (int)(_screenRelativePosition.Y - (_sourceRect.Height * GameSession.Instance.Map.Zoom)), 
                                   (int)(_sourceRect.Width * GameSession.Instance.Map.Zoom), 
                                   (int)(_sourceRect.Height * GameSession.Instance.Map.Zoom));
            UIRect.Inflate((int)((1f - GameSession.Instance.Map.Zoom) * 15f), (int)((1f - GameSession.Instance.Map.Zoom) * 15f));
            
        }

        public void Draw(SpriteBatch sb)
        {
            if (!Active) return;

            // We'll draw the dude with an origin of bottom middle, bcause his position will be fixed to the path
            sb.Draw(texDude, _screenRelativePosition, _sourceRect, 
                    Color.White,
                    _rot,
                    new Vector2(_sourceRect.Width/2, _sourceRect.Height),
                    GameSession.Instance.Map.Zoom, 
                    SpriteEffects.None, 0);

            Weapon.Draw(sb);

            if (UIHover)
            {
                sb.Draw(texDude, _screenRelativePosition - new Vector2(0, 60 * GameSession.Instance.Map.Zoom), new Rectangle(80, 0, 20, 20),
                     Color.White,
                     0f,
                     new Vector2(10, 0),
                     GameSession.Instance.Map.Zoom,
                     SpriteEffects.None, 0);
                UIHover = false;
            }
        }

        public void DrawShield(SpriteBatch sb)
        {
            if (!Active) return;

            if (ShieldTime > 0)
                sb.Draw(texDude, _screenRelativePosition, new Rectangle(0, 40, 600, 600),
                    (Team == 0 ? Color.Red : Color.Blue) * 0.2f,
                    0f,
                    new Vector2(300, 300),
                    GameSession.Instance.Map.Zoom,
                    SpriteEffects.None, 0);
        }

        public void Hit(int amount)
        {
            if (GameSession.Instance.Team1Win && GameSession.Instance.Team2Win) return;

            AudioController.PlaySFX("hit", 0.2f * (GameSession.Instance.Map.Zoom * 1.5f), ((float)GameSession.Instance.DudeController.Rand.NextDouble() * 2f) - 1f, ((2f / GameSession.Instance.Viewport.Width) * _screenRelativePosition.X) - 1f);

            Health -= amount / (IsShielded?10:1);
        }

        public void GiveWeapon(string function)
        {
            Weapon = null;

            switch (function)
            {
                case "sword":
                    Weapon = new Sword(this);
                    break;
                case "pistol":
                    Weapon = new Pistol(this);
                    break;
                case "smg":
                    Weapon = new SMG(this);
                    break;
                case "sniper":
                    Weapon = new Sniper(this);
                    break;
                case "shotgun":
                    Weapon = new Shotgun(this);
                    break;
                case "machinegun":
                    Weapon = new MachineGun(this);
                    break;
                case "mortar":
                    Weapon = new Mortar(this);
                    break;
            }
        }
    }
}
