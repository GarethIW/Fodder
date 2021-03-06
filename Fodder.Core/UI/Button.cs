﻿using System;
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
    class Button
    {
        public Rectangle UIRect;
        public string Function;
        public bool IsSelected = false;
        public Keys ShortcutKey;
        public bool IsEnabled = true;
        public int SoulButton;

        Texture2D _texButtons;
        double _currentCDTime = 0;
        double _actualCDTime = 0;
        Rectangle _sourceRectFull;

        bool _buttonDown = false;

        public Button(Texture2D texture, int buttonNum, double coolDown, string func, Vector2 screenPosition, Keys shortcutKey, int soul, bool enabled)
        {
            _texButtons = texture;
            _sourceRectFull = new Rectangle(buttonNum * 60, 0, 60, 60);
            _actualCDTime = coolDown;

            Function = func;
            UIRect = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, 60, 60);
            ShortcutKey = shortcutKey;
            IsEnabled = enabled;
            SoulButton = soul;
        }

        public void Update(GameTime gameTime)
        {
            if (SoulButton == 0)
            {
                if (_currentCDTime > 0)
                {
                    _currentCDTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (GameSession.Instance.ButtonController.HasteTime > 0) _currentCDTime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            else
            {
                int soulCount = (GameSession.Instance.Team1ClientType == GameClientType.Human ? GameSession.Instance.Team1SoulCount : GameSession.Instance.Team2SoulCount);
                _currentCDTime = (soulCount - (_actualCDTime * (SoulButton-1)));
                if (_currentCDTime < 0) _currentCDTime = 0;
                if (_currentCDTime > _actualCDTime) _currentCDTime = _actualCDTime;
            }
        }

        public void MouseOver()
        {
            
        }
        public void MouseOut()
        {
            _buttonDown = false;
        }
        public void MouseDown()
        {
            _buttonDown = true;

            if (_buttonDown == true && IsEnabled)
            {
                if (SoulButton == 0)
                {
                    GameSession.Instance.ButtonController.Deselect();
                    IsSelected = true;
                }
                else
                {
                    ActivateFunction(null);
                }

                // Button has been clicked
                _buttonDown = false;
            }
        }
        public void MouseUp()
        {
            _buttonDown = false;
        }

        public void ActivateFunction(Dude d)
        {
            if (SoulButton==0 && _currentCDTime <= 0)
            {
                switch (Function)
                {
                    case "boost":
                        d.BoostTime = 5000;
                        _currentCDTime = _actualCDTime;
                        break;
                    case "shield":
                        d.ShieldTime = 10000;
                        _currentCDTime = _actualCDTime;
                        break;
                    
                    default:
                        if (d.Weapon.GetType() == typeof(Weapons.Sword))
                        {
                            d.GiveWeapon(Function);
                            _currentCDTime = _actualCDTime;
                        }
                        break;
                }
            }

            if(SoulButton>0 && _currentCDTime >= _actualCDTime)
            {
                switch (Function)
                {
                    case "haste":
                        GameSession.Instance.ButtonController.HasteTime = 30000;
                        break;
                    case "meteors":
                        GameSession.Instance.SoulController.AirStrike(GameSession.Instance.Team1ClientType == GameClientType.Human ? 0 : 1);
                        break;
                    case "elite":
                        GameSession.Instance.SoulController.EliteSquad(GameSession.Instance.Team1ClientType == GameClientType.Human ? 0 : 1);
                        break;
                }

                if (GameSession.Instance.Team1ClientType == GameClientType.Human) GameSession.Instance.Team1SoulCount -= (int)_actualCDTime * SoulButton;
                if (GameSession.Instance.Team2ClientType == GameClientType.Human) GameSession.Instance.Team2SoulCount -= (int)_actualCDTime * SoulButton;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (SoulButton == 0)
            {
                int coolDownToHeight = 60 - (int)((60 / _actualCDTime) * _currentCDTime);

                if (IsEnabled)
                {
                    sb.Draw(_texButtons, UIRect, _sourceRectFull, new Color((IsSelected ? 0.8f : 0.5f), 0.5f, 0.5f));
                    sb.Draw(_texButtons,
                            new Rectangle(UIRect.Left, UIRect.Top + (60 - coolDownToHeight), 60, coolDownToHeight),
                            new Rectangle(_sourceRectFull.Left, _sourceRectFull.Top + (60 - coolDownToHeight), 60, coolDownToHeight),
                           (coolDownToHeight < 60) ? (IsSelected ? new Color(1f, 0.7f, 0.7f) : new Color(0.7f, 0.7f, 0.7f)) : (IsSelected ? Color.Red : Color.White));
                }
                else
                {
                    sb.Draw(_texButtons, UIRect, _sourceRectFull, new Color(0.2f,0.2f,0.2f));
                }
            }
            else
            {
                int coolDownToWidth = (int)((60 / _actualCDTime) * _currentCDTime);

                if (IsEnabled)
                {
                    sb.Draw(_texButtons, UIRect, _sourceRectFull, new Color((IsSelected ? 0.8f : 0.5f), 0.5f, 0.5f));
                    sb.Draw(_texButtons,
                            new Rectangle(UIRect.Left, UIRect.Top, coolDownToWidth, 60),
                            new Rectangle(_sourceRectFull.Left, _sourceRectFull.Top, coolDownToWidth, 60),
                            (coolDownToWidth < 60) ? (IsSelected ? new Color(1f,0.7f,0.7f) : new Color(0.7f,0.7f,0.7f)) : (IsSelected ? Color.Red : Color.White));
                }
                else
                {
                    sb.Draw(_texButtons, UIRect, _sourceRectFull, new Color(0.2f, 0.2f, 0.2f));
                }
            }
        }



        internal void Reset()
        {
            _currentCDTime = 0;
        }
    }
}
