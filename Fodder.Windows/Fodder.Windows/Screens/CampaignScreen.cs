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
using Microsoft.Xna.Framework.Input.Touch;
using Fodder.GameState;
using Fodder.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
#endregion

namespace Fodder.Windows.GameState
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class CampaignScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        ContentManager mapContent;

        Scenario gameScenario;
        Map map;
        Texture2D texPreview;
        Texture2D texBG;
        Texture2D texStar;

        SpriteFont font;

        BackgroundWorker bgw = new BackgroundWorker();

        const int MAX_SCENARIOS = 15;

        int furthestScenario = 1;
        List<int> scenarioScores = new List<int>(MAX_SCENARIOS);

        int scenarioNumber = 0;
        float scenarioAlpha = 0f;
        bool loading = false;

        float arrowLeftAlpha = 0.5f;
        float arrowRightAlpha = 0.5f;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CampaignScreen(int scenarioNum, ScenarioResult result)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);

            EnabledGestures = GestureType.Tap;

            for (int i = 0; i < MAX_SCENARIOS; i++) scenarioScores.Add(0);

            LoadProgress();
            if (scenarioNum == -1) scenarioNumber = furthestScenario; else scenarioNumber = scenarioNum;
            if (result != null)
            {
                if (result.Team1Human && result.Team1Win)
                {
                    if (scenarioNum == furthestScenario && scenarioNum<MAX_SCENARIOS) furthestScenario = scenarioNum + 1;
                    scenarioScores[scenarioNum - 1] = result.Team1ScoreRewarded;
                }
                if (result.Team2Human && result.Team2Win)
                {
                    if (scenarioNum == furthestScenario && scenarioNum < MAX_SCENARIOS) furthestScenario = scenarioNum + 1;
                    scenarioScores[scenarioNum - 1] = result.Team2ScoreRewarded;
                }
            }
            SaveProgress();
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Fodder.Content");

            font = content.Load<SpriteFont>("font");
            texBG = content.Load<Texture2D>("campaign");
            texStar = content.Load<Texture2D>("star");

            LoadScenarioAsync();

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
            if (!loading)
            {
                if (scenarioAlpha < 1f) scenarioAlpha += 0.1f;
            }

            if(scenarioNumber>1)
                arrowLeftAlpha += 0.1f;
            else
                arrowLeftAlpha -= 0.1f;

            if (scenarioNumber < furthestScenario)
                arrowRightAlpha += 0.1f;
            else
                arrowRightAlpha -= 0.1f;

            arrowLeftAlpha = MathHelper.Clamp(arrowLeftAlpha, 0.1f, 1f);
            arrowRightAlpha = MathHelper.Clamp(arrowRightAlpha, 0.1f, 1f);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PlayerIndex pi;

            Rectangle leftRect = new Rectangle((ScreenManager.GraphicsDevice.Viewport.Width/2) - 371, (ScreenManager.GraphicsDevice.Viewport.Height/2) - (texBG.Height/2),40,texBG.Height);
            Rectangle rightRect = new Rectangle((ScreenManager.GraphicsDevice.Viewport.Width / 2) + 331, (ScreenManager.GraphicsDevice.Viewport.Height / 2) - (texBG.Height / 2), 40, texBG.Height);
            leftRect.Inflate(20, 20);
            rightRect.Inflate(20, 20);
            Rectangle beginRect = new Rectangle((ScreenManager.SpriteBatch.GraphicsDevice.Viewport.Width / 2) - 300, (ScreenManager.SpriteBatch.GraphicsDevice.Viewport.Height / 2) - 150, 600, 300);

            if (!loading)
            {
                // look for any taps that occurred and select any entries that were tapped
                foreach (GestureSample gesture in input.Gestures)
                {
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                        if (leftRect.Contains(tapLocation) && scenarioNumber>1)
                        {
                            scenarioNumber--;
                            LoadScenarioAsync();
                        }
                        if (rightRect.Contains(tapLocation)&& scenarioNumber<furthestScenario)
                        {
                            scenarioNumber++;
                            LoadScenarioAsync();
                        }
                        if(beginRect.Contains(tapLocation))
                            LaunchScenario();

                    }
                }

                if (!ScreenManager.IsPhone)
                {
                    Point mouseLocation = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);

                    if (input.CurrentMouseState.LeftButton == ButtonState.Released && input.LastMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (leftRect.Contains(mouseLocation) && scenarioNumber > 1)
                        {
                            scenarioNumber--;
                            LoadScenarioAsync();
                        }
                        if (rightRect.Contains(mouseLocation) && scenarioNumber < furthestScenario)
                        {
                            scenarioNumber++;
                            LoadScenarioAsync();
                        }
                        if(beginRect.Contains(mouseLocation))
                            LaunchScenario();
                    }
                }


                if (input.IsMenuCancel(ControllingPlayer, out pi))
                {
                    this.ExitScreen();
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(texBG, (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) -new Vector2(0,28), new Rectangle(50, 0, 642, texBG.Height), Color.White * TransitionAlpha, 0f, new Vector2(642/2,texBG.Height/2),1f, SpriteEffects.None,1);
            spriteBatch.Draw(texBG, (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) - new Vector2(371, 28), new Rectangle(0, 0, 40, texBG.Height), Color.White * TransitionAlpha * arrowLeftAlpha, 0f, new Vector2(0, texBG.Height / 2), 1f, SpriteEffects.None, 1);
            spriteBatch.Draw(texBG, (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) + new Vector2(331, -28), new Rectangle(0, 0, 40, texBG.Height), Color.White * TransitionAlpha * arrowRightAlpha, 0f, new Vector2(0, texBG.Height / 2), 1f, SpriteEffects.FlipHorizontally, 1);
            
            spriteBatch.End();

            if (!loading)
            {
                spriteBatch.Begin();


                spriteBatch.DrawString(font, "Mission " + scenarioNumber + ": " + gameScenario.ScenarioName, new Vector2(spriteBatch.GraphicsDevice.Viewport.Width / 2, (spriteBatch.GraphicsDevice.Viewport.Height / 2) - 200), Color.White * scenarioAlpha * TransitionAlpha, 0f, font.MeasureString("Mission " + scenarioNumber + ": " + gameScenario.ScenarioName) / 2, 1f, SpriteEffects.None, 1);
                spriteBatch.Draw(texPreview, new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2, null, Color.White * scenarioAlpha * TransitionAlpha, 0f, new Vector2(texPreview.Width,texPreview.Height)/2, 1f, SpriteEffects.None,1);

                for (int i = 1; i < 4; i++)
                {
                    spriteBatch.Draw(texStar, (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) + new Vector2(45 + i * 70, 110), null, Color.Black * 0.5f * TransitionAlpha, 0f, new Vector2(texStar.Width, texStar.Height) / 2, 0.8f, SpriteEffects.FlipHorizontally, 1);
                    if((scenarioScores[scenarioNumber-1] >= i)) spriteBatch.Draw(texStar, (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) + new Vector2(45 + i * 70, 110), null, Color.White * TransitionAlpha, 0f, new Vector2(texStar.Width, texStar.Height)/2, 0.7f, SpriteEffects.FlipHorizontally, 1);
                }

                spriteBatch.End();
            }

            

        }

        private void LoadScenarioAsync()
        {
            scenarioAlpha = 0f;
            loading = true;

           
            bgw.RunWorkerAsync();
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loading = false;
            texPreview = map.DrawPreview(ScreenManager.SpriteBatch, new Rectangle((ScreenManager.SpriteBatch.GraphicsDevice.Viewport.Width / 2) - 300, (ScreenManager.SpriteBatch.GraphicsDevice.Viewport.Height / 2) - 150, 600, 300), scenarioAlpha);
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            mapContent = new ContentManager(ScreenManager.Game.Services);
            mapContent.RootDirectory = "Fodder.Content";

            BackgroundWorker worker = sender as BackgroundWorker;

            string scenarioXML = mapContent.Load<string>("scenarios/" + scenarioNumber);
            StringReader input = new StringReader(scenarioXML);
            XmlSerializer xmls = new XmlSerializer(typeof(Scenario));
            gameScenario = (Scenario)xmls.Deserialize(input);

            map = new Map(gameScenario.MapName);
            map.LoadContent(mapContent, true);

            GC.Collect();
        }

        private void LaunchScenario()
        {
            LoadingScreen.Load(ScreenManager, false, null, new GameplayScreen(gameScenario));
        }

        private void SaveProgress()
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User|IsolatedStorageScope.Assembly|IsolatedStorageScope.Domain,null,null))
                {
                    IsolatedStorageFileStream stream = storage.OpenFile("c", System.IO.FileMode.Create);
                    StreamWriter sr = new StreamWriter(stream);
                    sr.WriteLine(furthestScenario.ToString());
                    foreach (int score in scenarioScores) sr.WriteLine(score.ToString());
                    sr.Flush();
                    sr.Close();
                }

            }
            catch (Exception ex) { }
        }
        private void LoadProgress()
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
                {
                    if (storage.FileExists("c"))
                    {
                        IsolatedStorageFileStream stream = storage.OpenFile("c", System.IO.FileMode.Open);
                        StreamReader sr = new StreamReader(stream);
                        furthestScenario = Convert.ToInt32(sr.ReadLine());
                        int num = 0;
                        while (!sr.EndOfStream) { scenarioScores[num] = Convert.ToInt32(sr.ReadLine()); num++; }
                        sr.Close();
                    }
                }

            }
            catch (Exception ex) { }
        }


        #endregion
    }
}
