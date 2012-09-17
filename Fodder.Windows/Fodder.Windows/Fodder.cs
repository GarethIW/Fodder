using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Fodder.Core;
using Fodder.Windows.UX;
using Fodder.GameState;
using Fodder.Windows.GameState;

namespace Fodder.Windows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Fodder : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //GameSession gameSession;
        ScreenManager screenManager; 

        public Fodder()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Fodder.Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            IsMouseVisible = true;

            screenManager = new ScreenManager(this, false);
            Components.Add(screenManager);

            // Activate the first screens.
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
            //List<Function> funcs = new List<Function>();
            //funcs.Add(new Function("boost",1000,true));
            //funcs.Add(new Function("shield", 10000, true));
            //funcs.Add(new Function("pistol", 4000, true));
            //funcs.Add(new Function("shotgun", 6000, true));
            //funcs.Add(new Function("sniper", 30000, true));
            //funcs.Add(new Function("machinegun", 30000, true));
            //funcs.Add(new Function("mortar", 30000, true));


            //var playerControls = new WindowsPlayerControls(new MouseObserver(), new KeyboardObserver());

            //gameSession = new GameSession(playerControls, GameClientType.Human, GameClientType.AI, 2000, 2000, 100, 100, funcs, "1", GraphicsDevice.Viewport);

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

            //gameSession.LoadContent(Content);


            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //gameSession.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //gameSession.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
