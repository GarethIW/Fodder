#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Fodder.GameState;
using Fodder.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace Fodder.Phone.GameState
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    public class ResultPopupScreen : GameScreen
    {
        #region Fields

        Texture2D texBG;
        Texture2D texStar;
        SpriteFont largeFont;

        Scenario gameScenario;
        ScenarioResult gameResult;

        string resultText;
        int numStars;

        #endregion

        #region Events



        #endregion

        #region Initialization


        public ResultPopupScreen(ScenarioResult result, Scenario scenario)
        {
            gameResult = result;
            gameScenario = scenario;

            if (result.Team1Human && !result.Team2Human)
            {
                if (result.Team1Win && !result.Team2Win) resultText = "Victory";
                if (result.Team1Win && result.Team2Win) resultText = "Stalemate";
                if (!result.Team1Win && result.Team2Win) resultText = "Defeat";
                numStars = result.Team1ScoreRewarded;
            }
            if (!result.Team1Human && result.Team2Human)
            {
                if (!result.Team1Win && result.Team2Win) resultText = "Victory";
                if (result.Team1Win && result.Team2Win) resultText = "Stalemate";
                if (result.Team1Win && !result.Team2Win) resultText = "Defeat";
                numStars = result.Team2ScoreRewarded;
            }
            if (result.Team1Human && result.Team2Human)
            {
                if (result.Team1Win && !result.Team2Win) resultText = "Red Wins";
                if (result.Team1Win && result.Team2Win) resultText = "Stalemate";
                if (!result.Team1Win && result.Team2Win) resultText = "Blue Wins";
            }
            if (!result.Team1Human && !result.Team2Human)
            {
                if (result.Team1Win && !result.Team2Win) resultText = "Red Wins";
                if (result.Team1Win && result.Team2Win) resultText = "Stalemate";
                if (!result.Team1Win && result.Team2Win) resultText = "Blue Wins";
            }

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);
            IsPopup = true;

            EnabledGestures = GestureType.Tap;
        }


        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            texBG = content.Load<Texture2D>("resultpopup");
            texStar = content.Load<Texture2D>("star");
            largeFont = content.Load<SpriteFont>("largefont");
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            if (TransitionPosition == 0)
            {
                foreach (GestureSample gesture in input.Gestures)
                {
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        Exit();

                    }
                }

                if (!ScreenManager.IsPhone)
                {
                    Point mouseLocation = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);

                    if (input.CurrentMouseState.LeftButton == ButtonState.Released && input.LastMouseState.LeftButton == ButtonState.Pressed)
                    {
                        Exit();
                    }
                }

                if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
                {
                    Exit();
                }
            }
        }

        private void Exit()
        {
            if (gameScenario.CampaignMissionNum > 0)
            {
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen(), new CampaignScreen(gameScenario.CampaignMissionNum, gameResult));
            }
            else
            {
                // Not a campaign result, do something else!
            }
        }

        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            Vector2 centerPos = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2;
            float popupTop = -30;
            popupTop -= 60;
            if (resultText == "Victory") popupTop -= 60;

            spriteBatch.Begin();

            spriteBatch.Draw(texBG, centerPos + new Vector2(-(texBG.Width / 2), popupTop), new Rectangle(0, 0, texBG.Width, 30), Color.White * 0.5f* TransitionAlpha);
            popupTop += 30;
            spriteBatch.Draw(texBG, centerPos + new Vector2(-(texBG.Width / 2), popupTop), new Rectangle(0, 30, texBG.Width, 120), Color.White * 0.5f * TransitionAlpha);
            spriteBatch.DrawString(largeFont, resultText, centerPos + new Vector2(0, popupTop+60), Color.White * TransitionAlpha, 0f, largeFont.MeasureString(resultText) / 2, 1f, SpriteEffects.None, 1);
            popupTop += 120;
            if (resultText == "Victory")
            {
                spriteBatch.Draw(texBG, centerPos + new Vector2(-(texBG.Width / 2), popupTop), new Rectangle(0, 150, texBG.Width, 120), Color.White * 0.5f * TransitionAlpha);

                Vector2 starPos = centerPos + new Vector2(-(texBG.Width / 2), popupTop) + new Vector2(80, 62);
                for(int i=0;i<numStars;i++)
                    spriteBatch.Draw(texStar, starPos + new Vector2(i * 118, 0), null, Color.White * TransitionAlpha, 0f, new Vector2(texStar.Width,texStar.Height)/2, 1f, SpriteEffects.None, 1);

                popupTop += 120;
            }
            spriteBatch.Draw(texBG, centerPos + new Vector2(-(texBG.Width / 2), popupTop), new Rectangle(0, 270, texBG.Width, 30), Color.White * 0.5f * TransitionAlpha);
            
            

            // Draw the background rectangle.
            //spriteBatch.Draw(texBG, backgroundRectangle, color);

            // Draw the message box text.
            //spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();
        }


        #endregion
    }
}
