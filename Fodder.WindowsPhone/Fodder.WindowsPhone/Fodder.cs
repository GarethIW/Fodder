using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Fodder.Core;
using Microsoft.Phone.Info;
using Fodder.GameState;
using Fodder.Phone.GameState;

namespace Fodder.WindowsPhone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Fodder : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont debugFont;

        ScreenManager screenManager;

        public Fodder()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Fodder.Content";

            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            IsMouseVisible = false;

            graphics.IsFullScreen = true;

            screenManager = new ScreenManager(this, true);
            Components.Add(screenManager);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AudioController.LoadContent(Content);

            debugFont = Content.Load<SpriteFont>("font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            //if (TouchPanel.IsGestureAvailable)
            //{
            //    GestureSample gest = TouchPanel.ReadGesture();

            //    if (gest.GestureType == GestureType.Pinch)
            //    {
            //        if (GetScaleFactor(gest.Position, gest.Position2, gest.Delta, gest.Delta2) > 1f) gameSession.Map.DoZoom(-0.025f, 0f);
            //        if (GetScaleFactor(gest.Position, gest.Position2, gest.Delta, gest.Delta2) < 1f) gameSession.Map.DoZoom(0.025f, 0f);
            //    }

            //    if (gest.GestureType == GestureType.FreeDrag)
            //    {
            //        gameSession.Map.ScrollPos.X -= gest.Delta.X * 2f;
            //    }
            //}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            //var memuse = (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
            //var maxmem = (long)DeviceExtendedProperties.GetValue("DeviceTotalMemory");
            //var curmem = (long)DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage");
            //memuse /= 1024 * 1024;
            //maxmem /= 1024 * 1024;
            //curmem /= 1024 * 1024;
            //spriteBatch.Begin();
            //spriteBatch.DrawString(debugFont, "Mem Usage: " + curmem + "/" + memuse + "/" + maxmem, new Vector2(10, 10), Color.Black);
            //spriteBatch.DrawString(debugFont, "Mem Usage: " + curmem + "/" + memuse + "/" + maxmem, new Vector2(9, 9), Color.White);
            //spriteBatch.End();
        }

        
    }
}
