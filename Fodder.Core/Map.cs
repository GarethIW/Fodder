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
using System.IO;

namespace Fodder.Core
{
    public class Map
    {
        public string Name;
        public List<int> Path;
        public int Width;
        public int Height;
        public float Zoom = 0.5f;
        public Vector2 ScrollPos;
        public Vector2 Gravity = new Vector2(0, 0.05f);

        Texture2D _texSky;
        Texture2D[] _texFG;
        Texture2D[] _texBG;

        int _numScreens = 1;
        double _currentT1SpawnTime = 2000;
        double _currentT2SpawnTime = 2000;

        float _lerpZoom;
        Vector2 _lerpScroll;

        ContentManager content;

        public Map(string name)
        {
            Name = name;

            _currentT1SpawnTime = GameSession.Instance.Team1SpawnRate;
            _currentT2SpawnTime = GameSession.Instance.Team2SpawnRate;
        }

        public void LoadContent(ContentManager content, bool isPreview)
        {
            _numScreens = content.Load<int>("maps/" + Name + "/screens");

            _texFG = new Texture2D[_numScreens];
            _texBG = new Texture2D[3];
            Path = new List<int>();

            for (int i = 0; i < _numScreens; i++)
            {
                _texFG[i] = content.Load<Texture2D>("maps/" + Name + "/fg" + (i+1));

                Width += _texFG[i].Width;
                Height = _texFG[i].Height;

                if (!isPreview)
                {
                    Texture2D texPath = content.Load<Texture2D>("maps/" + Name + "/path" + (i + 1));
                    Path.AddRange(CalculatePath(texPath));
                    texPath.Dispose();
                }
            }

            for (int i = 0; i < 3; i++)
            {
                _texBG[i] = content.Load<Texture2D>("maps/" + Name + "/bg" + (i + 1));
            }

            _texSky = content.Load<Texture2D>("sky");

            if (isPreview) return;
            Zoom = (float)GameSession.Instance.Viewport.Width / (float)Width;
            _lerpZoom = Zoom;
        }

        public void Dispose()
        {
            Path = null;
        }

        public void Update(GameTime gameTime)
        {
            _currentT1SpawnTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            _currentT2SpawnTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (!GameSession.Instance.Team1Win && !GameSession.Instance.Team2Win)
            {
                if (_currentT1SpawnTime >= GameSession.Instance.Team1SpawnRate && GameSession.Instance.Team1Reinforcements > 0)
                {
                    _currentT1SpawnTime = 0;
                    GameSession.Instance.DudeController.Add(new Vector2(-40, Path[0]), 0);
                    GameSession.Instance.Team1Reinforcements--;
                }
                if (_currentT2SpawnTime >= GameSession.Instance.Team2SpawnRate && GameSession.Instance.Team2Reinforcements > 0)
                {
                    _currentT2SpawnTime = 0;
                    GameSession.Instance.DudeController.Add(new Vector2(Width + 39, Path[Width - 1]), 1);
                    GameSession.Instance.Team2Reinforcements--;
                }
            }

            //_lerpZoom = MathHelper.Clamp(_lerpZoom, (float)GameSession.Instance.Viewport.Width / (float)Width, 1f);
            //_lerpScroll = Vector2.Clamp(_lerpScroll, Vector2.Zero, new Vector2((Width * Zoom) - GameSession.Instance.Viewport.Width, (Height * Zoom) - (GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom)));

            Zoom = MathHelper.Lerp(Zoom, _lerpZoom, 0.1f);
            Zoom = MathHelper.Clamp(Zoom, (float)GameSession.Instance.Viewport.Width / (float)Width, 1f);
            

            
            //_lerpScroll = Vector2.Clamp(_lerpScroll, Vector2.Zero, new Vector2((Width * _lerpZoom) - GameSession.Instance.Viewport.Width, (Height * _lerpZoom) - (GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom)));
            ScrollPos = Vector2.Lerp(ScrollPos, _lerpScroll, 0.1f);
            ScrollPos = Vector2.Clamp(ScrollPos, Vector2.Zero, new Vector2((Width * Zoom) - GameSession.Instance.Viewport.Width, (Height * Zoom) - (GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom)));
            

        }

        public void DrawFG(SpriteBatch sb)
        {
            sb.Begin();

            float x = -ScrollPos.X;
            for (int i = 0; i < _numScreens; i++)
            {
                sb.Draw(_texFG[i], new Vector2(x, sb.GraphicsDevice.Viewport.Height - GameSession.Instance.ScreenBottom), null, Color.White,
                        0f, new Vector2(0, _texFG[i].Height), Zoom, SpriteEffects.None, 1);
                x += _texFG[i].Width * Zoom;
            }


            sb.End();
        }

        public void DrawBG(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            sb.Draw(_texSky, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,1f, SpriteEffects.None, 1f);

            float y = sb.GraphicsDevice.Viewport.Height - GameSession.Instance.ScreenBottom;
            for (int i = 0; i < 3; i++)
            {
                //float x = (-ScrollPos.X - (GameSession.Instance.Viewport.Width * Zoom)) / (i+2);
                //float x = (-((ScrollPos.X)) + ((Width / 2) * Zoom)) - (((_texBG[i].Width * Zoom) * (((Width/2)/_texBG[i].Width)+1))) + ((ScrollPos.X) * Zoom);
                //float x = (GameSession.Instance.Viewport.Width/2) + (-((ScrollPos.X) / (i+2)));

                float centerX = (-ScrollPos.X + ((Width / 2) * Zoom));
                float x = centerX + ((((centerX - (GameSession.Instance.Viewport.Width/2)))) * 0.2f * -(i+2));
                x -= (_texBG[i].Width * Zoom) * (((Width / 2) / _texBG[i].Width) + 1);
                while (x < Width)
                {
                    sb.Draw(_texBG[i], new Vector2(x, y), null, Color.White,
                            0f, new Vector2(0, _texBG[i].Height), Zoom, SpriteEffects.None, (i+1) * 0.1f);
                    x += _texBG[i].Width * Zoom;
                }
                y -= (_texBG[i].Height / 4f) * Zoom;
                y -= 50f-(10*(_numScreens-1));
            }

            sb.End();
        }

        public Texture2D DrawPreview(SpriteBatch sb, Rectangle vp, float alpha)
        {
            RenderTarget2D tr = new RenderTarget2D(sb.GraphicsDevice, vp.Width, vp.Height);
            sb.GraphicsDevice.SetRenderTarget(tr);

            float zoom = (float)vp.Width / (float)Width;

            sb.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            sb.Draw(_texSky, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, (float)vp.Width/(float)_texSky.Width,  SpriteEffects.None, 1f);

            float y = vp.Height;
            float x;
            for (int i = 0; i < 3; i++)
            {
                float centerX = (-ScrollPos.X + ((Width / 2) * zoom));
                x = centerX + ((((centerX - (vp.Width / 2)))) * 0.2f * -(i + 2));
                x -= (_texBG[i].Width * zoom) * (((Width / 2) / _texBG[i].Width) + 1);
                while (x < Width)
                {
                    sb.Draw(_texBG[i], new Vector2(x, y), null, Color.White,
                            0f, new Vector2(0, _texBG[i].Height), zoom, SpriteEffects.None, (i + 1) * 0.1f);
                    x += _texBG[i].Width * zoom;
                }
                y -= (_texBG[i].Height / 4f) * zoom;
                y -= 50f - (15 * (_numScreens - 1));
            }

            sb.End();

            sb.Begin();

            x = 0;
            for (int i = 0; i < _numScreens; i++)
            {
                sb.Draw(_texFG[i], new Vector2(x, vp.Height), null, Color.White,
                        0f, new Vector2(0, _texFG[i].Height), zoom, SpriteEffects.None, 1);
                x += _texFG[i].Width * zoom;
            }

            sb.End();

            sb.GraphicsDevice.SetRenderTarget(null);

            return tr;
        }

        private List<int> CalculatePath(Texture2D texPath)
        {
            int width = texPath.Width;
            int height = texPath.Height;
            List<int> returnPath = new List<int>();
            Color[] colorData = new Color[width * height];

            texPath.GetData<Color>(colorData);

            for(int x=0;x<width;x++)
                for(int y=0;y<height;y++)
                    if(colorData[(width * y) + x].A>0) 
                    {
                        returnPath.Add(y);
                        break;
                    }

            colorData = null;
            GC.Collect();

            return returnPath;
        }

        public int TryGetPath(int pos, int failValue)
        {
            if (pos < 0 || pos > Path.Count - 1) return (int)failValue;
            else return Path[pos];
        }

        public void DoZoom(float scaleFactor)
        {
            Point position = new Point(GameSession.Instance.Viewport.Width / 2, (GameSession.Instance.Viewport.Height / 2) - GameSession.Instance.ScreenBottom);

            _lerpZoom = Zoom * scaleFactor;
            if (_lerpZoom >= 0.97f)
            {
                _lerpZoom = 1f;
                _lerpScroll = (ScreenPointToMapPosition(position) * _lerpZoom) - (new Vector2(GameSession.Instance.Viewport.Width / 2, (GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom) / 2));
            }

            if(_lerpZoom<1f)
                _lerpScroll = (ScreenPointToMapPosition(position) * _lerpZoom) - (new Vector2(GameSession.Instance.Viewport.Width / 2, (GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom) / 2));
        }

        public void DoScroll(Vector2 delta)
        {
            _lerpScroll += delta;
            _lerpScroll = Vector2.Clamp(_lerpScroll, Vector2.Zero, new Vector2((Width * _lerpZoom) - GameSession.Instance.Viewport.Width, (Height * _lerpZoom) - (GameSession.Instance.Viewport.Height - GameSession.Instance.ScreenBottom)));
        }

        private Vector2 ScreenPointToMapPosition(Point position)
        {
            Vector2 screenPos = new Vector2(position.X, 0);
            Vector2 mapPos = (screenPos + ScrollPos) / Zoom;

            return mapPos;
        }

    }
}
