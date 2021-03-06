#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Fodder.GameState;
using System.Collections.Generic;
using Fodder.Core;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace Fodder.Phone.GameState
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;

        GameSession gameSession;
        Scenario gameScenario;

        bool resultReached;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(Scenario scenario)
        {
            gameScenario = scenario;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.EnabledGestures = GestureType.Pinch | GestureType.Tap | GestureType.FreeDrag;

            IsStubbourn = true;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Fodder.Content");

            List<Function> funcs = new List<Function>();
            funcs.Add(new Function("boost", 1000, true));
            funcs.Add(new Function("shield", 10000, true));
            funcs.Add(new Function("pistol", 4000, true));
            funcs.Add(new Function("shotgun", 6000, true));
            funcs.Add(new Function("sniper", 30000, true));
            funcs.Add(new Function("machinegun", 30000, true));
            funcs.Add(new Function("mortar", 30000, true));

            gameSession = new GameSession(GameClientType.Human, GameClientType.AI, gameScenario, ScreenManager.GraphicsDevice.Viewport, false);
            gameSession.LoadContent(content);

            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            bool found = false;

            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                if (screen.GetType() == typeof(PauseBackgroundScreen))
                    found = true;
            }
            if (!found)
            {
                gameSession.Update(gameTime);

                if (gameSession.Team1Win || gameSession.Team2Win)
                {
                    if (!resultReached)
                    {
                        resultReached = true;
                        ScenarioResult result = new ScenarioResult(gameSession, gameScenario);
                        ScreenManager.AddScreen(new ResultPopupScreen(result, gameScenario), null);
                    }
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //this.playerControls.Update(input);

            if (input.IsPauseGame(ControllingPlayer))
            {
                PauseBackgroundScreen pauseBG = new PauseBackgroundScreen();
                ScreenManager.AddScreen(pauseBG, ControllingPlayer);
                ScreenManager.AddScreen(new PauseMenuScreen(pauseBG), ControllingPlayer);
            }

            gameSession.HandleInput(input);
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            gameSession.Draw(gameTime, spriteBatch);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition >= 0f)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }


        #endregion
    }
}
