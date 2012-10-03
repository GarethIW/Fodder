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
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Fodder.GameState;

namespace Fodder.Core
{
    

    public class Function
    {
        public string Name;
        public double CoolDown;
        public bool IsEnabled;

        public Function() { }

        public Function(string name, double cd, bool enabled)
        {
            Name = name;
            CoolDown = cd;
            IsEnabled = enabled;
        }
    }

    public enum GameClientType
    {
        Human,
        AI,
        Network
    }

    public class GameSession
    {
        public static GameSession Instance;
        public DudeController DudeController;
        internal ButtonController ButtonController;
        internal ParticleController ParticleController;
        public ProjectileController ProjectileController;
        internal SoulController SoulController;
        internal HUD HUD;

        public Map Map;

        internal GameClientType Team1ClientType;
        internal GameClientType Team2ClientType;
        internal int Team1Reinforcements;
        internal int Team2Reinforcements;
        internal int Team1StartReinforcements;
        internal int Team2StartReinforcements;
        internal double Team1SpawnRate;
        internal double Team2SpawnRate;

        internal int Team1ActiveCount;
        internal int Team2ActiveCount;
        internal int Team1PlantedCount;
        internal int Team2PlantedCount;
        internal int Team1DeadCount;
        internal int Team2DeadCount;
        internal int Team1SoulCount;
        internal int Team2SoulCount;

        public bool Team1Win;
        public bool Team2Win;

        internal Viewport Viewport;

        internal List<Function> AvailableFunctions;

        internal bool IsAttractMode;

        internal int ScreenBottom;

        AIController AI1 = new AIController();
        AIController AI2 = new AIController();

        INetworkController Net;

        public double StartCountdown;
        float prepareTransition;
        float fightTransition;

        SpriteFont largeFont;

        public GameSession(GameClientType t1CT, GameClientType t2CT, INetworkController net, Scenario scenario, Viewport vp, bool attractmode)
        {
            Net = net;

            Instance = this;

            Team1ClientType = t1CT;
            Team2ClientType = t2CT;
            Team1Reinforcements = scenario.T1Reinforcements;
            Team2Reinforcements = scenario.T2Reinforcements;
            Team1StartReinforcements = scenario.T1Reinforcements;
            Team2StartReinforcements = scenario.T2Reinforcements;
            Team1SpawnRate = scenario.T1SpawnRate;
            Team2SpawnRate = scenario.T2SpawnRate;

            Team1DeadCount = 0;
            Team2DeadCount = 0;
            Team1SoulCount = 0;
            Team2SoulCount = 0;

            Team1Win = false;
            Team2Win = false;

            AvailableFunctions = scenario.AvailableFunctions;

            DudeController = new DudeController();
            ButtonController = new ButtonController();
            SoulController = new SoulController();
            ProjectileController = new ProjectileController();
            ParticleController = new ParticleController();
            HUD = new HUD();

            AI1.Initialize(scenario.AIReactionTime);
            AI2.Initialize(scenario.AIReactionTime);

            //if (t1CT == GameClientType.Network) Net.Initialize(0);
            //if (t2CT == GameClientType.Network) Net.Initialize(1);

            Viewport = vp;

            IsAttractMode = attractmode;

            ScreenBottom = (IsAttractMode ? 0 : 60);

            StartCountdown = (IsAttractMode ? 0 : 4000);
            prepareTransition = (IsAttractMode ?0f:1f);
            fightTransition = 0f;

            Map = new Map(scenario.MapName);
        }

        public void LoadContent(ContentManager content)
        {
            DudeController.LoadContent(content);
            ButtonController.LoadContent(content);
            SoulController.LoadContent(content);
            ProjectileController.LoadContent(content);
            ParticleController.LoadContent(content);
            HUD.LoadContent(content);
            Map.LoadContent(content, false);

            largeFont = content.Load<SpriteFont>("largefont");
        }

        public void Dispose()
        {
            if (Net != null)
                Net.CloseConn();
        }

        public void Update(GameTime gameTime)
        {
            Map.Update(gameTime);
            HUD.Update(gameTime);

            if (Team1ClientType == GameClientType.Network || Team2ClientType == GameClientType.Network)
            {
                Net.Update(gameTime);

                if (Net.GetState() == RemoteClientState.ReadyToStart)
                {
                    Net.SendReady();
                    return;
                }
            }

            if (StartCountdown > 0)
            {
                Net.SendReady();
                StartCountdown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                CalculateWinConditions(gameTime);
                if ((int)StartCountdown < 1000)
                {
                    fightTransition += 0.1f;
                    prepareTransition -= 0.2f;
                    if (Team1ClientType == GameClientType.Human) Map.PanTo(2f, new Vector2(0, Map.Path[0]));
                    if (Team2ClientType == GameClientType.Human) Map.PanTo(2f, new Vector2(Map.Width, Map.Path[Map.Width - 1]));
                    fightTransition = MathHelper.Clamp(fightTransition, 0f, 1f);
                    prepareTransition = MathHelper.Clamp(prepareTransition, 0f, 1f);
                }
                return;
            }
            else
                fightTransition -= 0.1f;

            DudeController.Update(gameTime);

            if (Team1ClientType == GameClientType.AI) AI1.Update(gameTime, 0);
            if (Team2ClientType == GameClientType.AI) AI2.Update(gameTime, 1);
            

            ButtonController.Update(gameTime);
            SoulController.Update(gameTime);
            ProjectileController.Update(gameTime);
            ParticleController.Update(gameTime);

            if (!IsAttractMode)
            {
                CalculateWinConditions(gameTime);
            }
            else
            {
                Team1Reinforcements = 100;
                Team2Reinforcements = 100;

                if (Map.T1Flag.RaisedHeight == 16) Team1SoulCount++;
                if (Map.T2Flag.RaisedHeight == 16) Team2SoulCount++;
            }
        }

        public void HandleInput(InputState input)
        {
            if (!IsAttractMode && !Team1Win && !Team2Win && StartCountdown<=0)
            {
                var kbscroll = Vector2.Zero;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.D)) kbscroll.X = 10f;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.A)) kbscroll.X = -10f;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.W)) kbscroll.Y = -10f;
                if (input.CurrentKeyboardStates[0].IsKeyDown(Keys.S)) kbscroll.Y = 10f;
                if (kbscroll != Vector2.Zero) this.Map.DoScroll(kbscroll);

                if (input.PinchGesture.HasValue)
                {
                    this.Map.DoZoom(input.GetScaleFactor(input.PinchGesture.Value));
                }
                if (input.DragGesture.HasValue)
                {
                    this.Map.DoScroll(new Vector2(-input.DragGesture.Value.Delta.X, input.DragGesture.Value.Delta.Y) * 3f);
                }
                if (input.MouseDragging)
                {
                    this.Map.DoScroll(new Vector2(-input.MouseDelta.X, input.MouseDelta.Y));
                }
                if (input.CurrentMouseState.ScrollWheelValue > input.LastMouseState.ScrollWheelValue) this.Map.DoZoom(1.03f);
                if (input.CurrentMouseState.ScrollWheelValue < input.LastMouseState.ScrollWheelValue) this.Map.DoZoom(0.97f);

                if (Team1ClientType == GameClientType.Human) DudeController.HandleInput(input, 0);
                if (Team2ClientType == GameClientType.Human) DudeController.HandleInput(input, 1);

                ButtonController.HandleInput(input);


            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Map.DrawBG(spriteBatch);
            DudeController.DrawShield(spriteBatch);
            Map.DrawFG(spriteBatch);
            SoulController.Draw(spriteBatch);
            DudeController.Draw(spriteBatch);
            ProjectileController.Draw(spriteBatch);
            ParticleController.Draw(spriteBatch);

            // UI always comes last!
            if (!IsAttractMode)
            {
                ButtonController.Draw(spriteBatch);
                HUD.Draw(spriteBatch);
            }

            spriteBatch.Begin();
            if (Net != null && Net.GetState() == RemoteClientState.ReadyToStart)
            {
                spriteBatch.DrawString(largeFont, "Waiting for other player", (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) + Vector2.One, Color.Black, 0f, largeFont.MeasureString("Waiting for other player") / 2, 0.5f, SpriteEffects.None, 1);
                spriteBatch.DrawString(largeFont, "Waiting for other player", new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2, Color.White, 0f, largeFont.MeasureString("Waiting for other player") / 2, 0.5f, SpriteEffects.None, 1);
            }
            else
            {
                if (prepareTransition > 0)
                {
                    spriteBatch.DrawString(largeFont, "Prepare for Battle", (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) + Vector2.One, Color.Black * prepareTransition, 0f, largeFont.MeasureString("Prepare for Battle") / 2, 0.5f, SpriteEffects.None, 1);
                    spriteBatch.DrawString(largeFont, "Prepare for Battle", new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2, Color.White * prepareTransition, 0f, largeFont.MeasureString("Prepare for Battle") / 2, 0.5f, SpriteEffects.None, 1);
                }
            }
            if (fightTransition > 0f)
            {
                spriteBatch.DrawString(largeFont, "Fight", (new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2) + Vector2.One, Color.Black * fightTransition, 0f, largeFont.MeasureString("Fight") / 2, 1f + (1f - fightTransition), SpriteEffects.None, 1);
                spriteBatch.DrawString(largeFont, "Fight", new Vector2(spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height) / 2, Color.White * fightTransition, 0f, largeFont.MeasureString("Fight") / 2, 1f + (1f - fightTransition), SpriteEffects.None, 1);
            }

            spriteBatch.End();
        }

        public void Reset()
        {
            Team1Reinforcements = Team1StartReinforcements;
            Team2Reinforcements = Team2StartReinforcements;

            Team1DeadCount = 0;
            Team2DeadCount = 0;

            prepareTransition = 1f;
            fightTransition = 0f;
            StartCountdown = 4000f;

            Team1Win = false;
            Team2Win = false;

            AI1.Reset();
            AI2.Reset();

            DudeController.Reset();
            ProjectileController.Reset();
            ParticleController.Reset();
            ButtonController.Reset();
            SoulController.Reset();
        }

        internal void CalculateWinConditions(GameTime gameTime)
        {
            Team1ActiveCount = 0;
            Team2ActiveCount = 0;
            Team1PlantedCount = 0;
            Team2PlantedCount = 0;

            foreach (Dude d in DudeController.Dudes)
            {
                if (d.Active)
                {
                    if (d.Team == 0) Team1ActiveCount++; else Team2ActiveCount++;

                    if(d.Weapon.GetType() == typeof(Weapons.Sniper) || 
                       d.Weapon.GetType() == typeof(Weapons.MachineGun) ||
                       d.Weapon.GetType() == typeof(Weapons.Mortar))
                        if(!d.Weapon.IsInRange)
                            if (d.Team == 0) Team1PlantedCount++; else Team2PlantedCount++;

                    //if (d.Team == 0 && d.Position.X > Map.Width) Team1Win = true;
                    //if (d.Team == 1 && d.Position.X < 0) Team2Win = true;
                }
            }

            if (Map.T1Flag.RaisedHeight == 16) Team2Win = true;
            if (Map.T2Flag.RaisedHeight == 16) Team1Win = true;

            // Don't do anything until there's no projectiles left!
            foreach (Projectile p in ProjectileController.Projectiles)
            {
                if (p.Active) return;
            }

            if(Team1Reinforcements==0 || Team2Reinforcements==0)
            {
                if (Team1ActiveCount == 0 || Team2ActiveCount == 0)
                {
                    if (Team2ActiveCount > Team1ActiveCount) Team2Win = true;
                    if (Team1ActiveCount > Team2ActiveCount) Team1Win = true;
                    if (Team1ActiveCount == Team2ActiveCount) { Team1Win = true; Team2Win = true; }
                }
                else
                {
                    if ((Team1ActiveCount - Team1PlantedCount) == 0 && (Team2ActiveCount - Team2PlantedCount) == 0)
                    {
                        if (Team2ActiveCount > Team1ActiveCount) Team2Win = true;
                        if (Team1ActiveCount > Team2ActiveCount) Team1Win = true;
                        if (Team1ActiveCount == Team2ActiveCount) { Team1Win = true; Team2Win = true; }
                    }
                }
            }

            

            if (Team1Win || Team2Win) Map.PanTo(0f, Vector2.Zero);
        }
       
    }
}
