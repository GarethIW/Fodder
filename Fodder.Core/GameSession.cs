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
using Fodder.Core.UX;

namespace Fodder.Core
{
    public class Scenario
    {
        public List<Function> AvailableFunctions;
        public int AIReactionTime, T1Reinforcements, T2Reinforcements;
        public double T1SpawnRate, T2SpawnRate;
        public string MapName, ScenarioName;

        public Scenario() { }

        public Scenario(string name, string mapname, List<Function> funcs, int aireactiontime, int t1reinforcements, int t2reinforcements, double t1spawnrate, double t2spawnrate)
        {
            ScenarioName = name;
            MapName = mapname;
            AvailableFunctions = funcs;
            AIReactionTime = aireactiontime;
            T1Reinforcements = t1reinforcements;
            T2Reinforcements = t2reinforcements;
            T1SpawnRate = t1spawnrate;
            T2SpawnRate = t2spawnrate;
        }
    }

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
        internal DudeController DudeController;
        internal ButtonController ButtonController;
        internal ParticleController ParticleController;
        internal ProjectileController ProjectileController;
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

        internal bool Team1Win;
        internal bool Team2Win;

        internal Viewport Viewport;

        internal List<Function> AvailableFunctions;

        internal bool IsAttractMode;

        internal int ScreenBottom;

        AIController AI1 = new AIController();
        AIController AI2 = new AIController();

        private IHumanPlayerControls PlayerControls;

        public GameSession(IHumanPlayerControls playerControls, GameClientType t1CT, GameClientType t2CT, Scenario scenario, Viewport vp, bool attractmode)
        {
            if (playerControls == null)
                throw new ArgumentException("GameSession cannot be created without PlayerControls");

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

            Viewport = vp;

            IsAttractMode = attractmode;

            ScreenBottom = (IsAttractMode ? 0 : 60);

            this.PlayerControls = playerControls;

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

            Map.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            Map.Update(gameTime);
            DudeController.Update(gameTime);

            if (Team1ClientType == GameClientType.AI) AI1.Update(gameTime, 0);
            if (Team2ClientType == GameClientType.AI) AI2.Update(gameTime, 1);

            if (!IsAttractMode)
            {
                if (this.PlayerControls.Reset) this.Reset();

                var zoomDir = this.PlayerControls.Zoom;
                if (zoomDir == ZoomDirection.In) this.Map.DoZoom(0.05f, 0);
                if (zoomDir == ZoomDirection.Out) this.Map.DoZoom(-0.05f, 0);

                var scroll = 0f;
                if (this.PlayerControls.Scroll == ScrollDirection.Right) scroll = -10f;
                if (this.PlayerControls.Scroll == ScrollDirection.Left) scroll = 10f;
                if (this.PlayerControls.IsPhone) scroll = scroll * 4;
                if (scroll != 0f) this.Map.DoScroll(new Vector2(scroll, 0f));

                if (Team1ClientType == GameClientType.Human) DudeController.HandleInput(this.PlayerControls, 0);
                if (Team2ClientType == GameClientType.Human) DudeController.HandleInput(this.PlayerControls, 1);

                ButtonController.Update(gameTime);
                ButtonController.HandleInput(this.PlayerControls);
            }

            SoulController.Update(gameTime);
            HUD.Update(gameTime);
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
        }

        public void Reset()
        {
            Team1Reinforcements = Team1StartReinforcements;
            Team2Reinforcements = Team2StartReinforcements;

            Team1DeadCount = 0;
            Team2DeadCount = 0;

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

                    if (d.Team == 0 && d.Position.X > Map.Width) Team1Win = true;
                    if (d.Team == 1 && d.Position.X < 0) Team2Win = true;
                }
            }

            if(Team1Reinforcements==0 && Team2Reinforcements==0)
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


        }
       
    }
}
