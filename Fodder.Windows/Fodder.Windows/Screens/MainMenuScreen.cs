#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Fodder.GameState;
using Fodder.Core;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Xml.Serialization;
using System;
#endregion

namespace Fodder.Windows.GameState
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    public class MainMenuScreen : MenuScreen
    {
        #region Initialization

        ContentManager content;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Fodder.Content");

            // Create our menu entries.
            MenuEntry campaignGameMenuEntry = new MenuEntry("CAMPAIGN");
            MenuEntry quickGameMenuEntry = new MenuEntry("WAR");
            MenuEntry optionsMenuEntry = new MenuEntry("OPTIONS");
            MenuEntry exitMenuEntry = new MenuEntry("EXIT GAME");

            // Hook up menu event handlers.
            campaignGameMenuEntry.Selected += CampaignGameMenuEntrySelected;
            quickGameMenuEntry.Selected += QuickGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += ExitMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(campaignGameMenuEntry);
            MenuEntries.Add(quickGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            if (!ScreenManager.IsPhone)
            {
                MenuEntries.Add(exitMenuEntry);
            }

            base.LoadContent();
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void CampaignGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CampaignScreen(-1, null), e.PlayerIndex);
        }

        void QuickGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
              //                 new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void ExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
