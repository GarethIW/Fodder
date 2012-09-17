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

namespace Fodder.Core
{
    class AIAction
    {
        public double CurrentCooldown;
        public double MaxCooldown;
        public string Function;
        public bool IsEnabled;

        public AIAction(double cd, string func, bool enabled)
        {
            CurrentCooldown = 0;
            MaxCooldown = cd;
            Function = func;
            IsEnabled = enabled;
        }
    }

    class AIController
    {
        public double HasteTime = 0;

        List<AIAction> Actions = new List<AIAction>();

        List<Dude> TeamInOrder = new List<Dude>();

        double _currentReactionTime;
        double _randomReactionTime;

        static Random _rand = new Random();

        double REACTION_TIME = 5000;

        public void Initialize(double rtime)
        {
            REACTION_TIME = rtime;

            foreach(Function f in GameSession.Instance.AvailableFunctions)
                Actions.Add(new AIAction(f.CoolDown, f.Name, f.IsEnabled));

            _randomReactionTime = _rand.NextDouble() * REACTION_TIME;
        }

        public void LoadContent(ContentManager content)
        {
            
        }

        public void Update(GameTime gameTime, int team)
        {
            if (GameSession.Instance.Team1Win || GameSession.Instance.Team2Win) return;
            
            foreach (AIAction a in Actions)
            {
                if (a.CurrentCooldown > 0) a.CurrentCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (HasteTime > 0) a.CurrentCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (HasteTime > 0) HasteTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

            int otherTeamCount = 0;

            TeamInOrder.Clear();

            foreach (Dude d in GameSession.Instance.DudeController.Dudes)
            {
                if (!d.Active) continue;
                if (d.Team != team)
                {
                    otherTeamCount++;
                    continue;
                }
                
                int insertindex = 0;
                foreach (Dude o in TeamInOrder)
                {
                    if (team == 0)
                    {
                        if (d.Position.X >= o.Position.X)
                        {
                            insertindex = TeamInOrder.IndexOf(o);
                            break;
                        }
                        else
                            insertindex = TeamInOrder.IndexOf(o) + 1;
                        
                    }
                    if (team == 1)
                    {
                        if (d.Position.X <= o.Position.X)
                        {
                            insertindex = TeamInOrder.IndexOf(o);
                            break;
                        }
                        else
                            insertindex = TeamInOrder.IndexOf(o) + 1;
                        
                    }
                }
                if (insertindex > TeamInOrder.Count - 1)
                    TeamInOrder.Add(d);
                else
                    TeamInOrder.Insert(insertindex, d);
            }


            _currentReactionTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_currentReactionTime < _randomReactionTime) return;

            _currentReactionTime = 0;
            _randomReactionTime = _rand.NextDouble() * REACTION_TIME;


            int souls = (team == 0 ? GameSession.Instance.Team1SoulCount : GameSession.Instance.Team2SoulCount);

            // are we on the ropes?
            // if so, use a soul power!
            if (otherTeamCount - TeamInOrder.Count > 10)
            {

                if (souls >= Actions[10].MaxCooldown * 3)
                {
                    // use elite squad
                    GameSession.Instance.SoulController.EliteSquad(team);
                    if (team == 0) GameSession.Instance.Team1SoulCount -= 3 * (int)Actions[10].MaxCooldown;
                    if (team == 1) GameSession.Instance.Team2SoulCount -= 3 * (int)Actions[10].MaxCooldown;
                }
                else if (souls >= Actions[9].MaxCooldown * 2)
                {
                    // use airstrike
                    GameSession.Instance.SoulController.AirStrike(team);
                    if (team == 0) GameSession.Instance.Team1SoulCount -= 2 * (int)Actions[9].MaxCooldown;
                    if (team == 1) GameSession.Instance.Team2SoulCount -= 2 * (int)Actions[9].MaxCooldown;
                }
            }
            else
            {
                if (Actions[7].CurrentCooldown + Actions[6].CurrentCooldown + Actions[5].CurrentCooldown > 60000)
                {
                    if (souls >= Actions[8].MaxCooldown)
                    {
                        // use haste
                        HasteTime = 20000;
                        if (team == 0) GameSession.Instance.Team1SoulCount -= (int)Actions[8].MaxCooldown;
                        if (team == 1) GameSession.Instance.Team2SoulCount -= (int)Actions[8].MaxCooldown;
                    }
                }
            }

            // use boost
            if (Actions[0].CurrentCooldown <= 0 && Actions[0].IsEnabled)
            {
                if (TeamInOrder.Count > 0)
                {
                    // dudes with weapons get priority for boosting
                    for (int i = 0; i < TeamInOrder.Count - 1; i++)
                    {
                        if (TeamInOrder[i].BoostTime <= 0)
                            if (TeamInOrder[i].Weapon.GetType() != typeof(Weapons.Sword) && TeamInOrder[i].Weapon.GetType() != typeof(Weapons.Mortar) && TeamInOrder[i].Weapon.GetType() != typeof(Weapons.MachineGun))
                                if (GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[i], 1500, false) != null)
                                {
                                    ActivateFunction(Actions[0], TeamInOrder[i]);
                                    break;
                                }
                    }

                    // didn't find dude with weapon, boost first available
                    for (int i = 0; i < TeamInOrder.Count - 1; i++)
                    {
                        if(TeamInOrder[i].BoostTime<=0)
                            if (GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[i], 1500, false) != null)
                            {
                                ActivateFunction(Actions[0], TeamInOrder[i]);
                                break;
                            }
                    }
                }
            }

            // use shield
            if (Actions[1].CurrentCooldown <= 0 && Actions[1].IsEnabled)
            {
                if (TeamInOrder.Count > 2)
                {
                    if (GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[2], 600, false) != null)
                    {
                        ActivateFunction(Actions[1], TeamInOrder[2]);
                        return;
                    }
                }
                else
                {
                    if (TeamInOrder.Count > 0 && GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[0], 600, false) != null)
                    {
                        ActivateFunction(Actions[1], TeamInOrder[0]);
                        return;
                    }
                }

            }

            // use mortar
            if (Actions[7].CurrentCooldown <= 0 && Actions[7].IsEnabled)
            {
                if (TeamInOrder.Count > 6)
                {
                    for (int i = 6; i < TeamInOrder.Count; i++)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword) && TeamInOrder[i].Position.X > 10 && TeamInOrder[i].Position.X < GameSession.Instance.Map.Width - 10)
                        {
                            if (GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[i], 1000, false) != null)
                            {
                                ActivateFunction(Actions[7], TeamInOrder[i]);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = TeamInOrder.Count - 1; i >= 0; i--)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword) && TeamInOrder[i].Position.X > 10 && TeamInOrder[i].Position.X < GameSession.Instance.Map.Width - 10)
                        {
                            if (GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[i], 1000, false) != null)
                            {
                                ActivateFunction(Actions[7], TeamInOrder[i]);
                                return;
                            }
                        }
                    }
                }
            }

            // use sniper
            if (Actions[5].CurrentCooldown <= 0 && Actions[5].IsEnabled)
            {
                if (TeamInOrder.Count > 3)
                {
                    for (int i = 3; i < TeamInOrder.Count; i++)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword) && TeamInOrder[i].Position.X > 10 && TeamInOrder[i].Position.X < GameSession.Instance.Map.Width - 10)
                        {
                            if (GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[i], 2000, true) != null)
                            {
                                ActivateFunction(Actions[5], TeamInOrder[i]);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = TeamInOrder.Count - 1; i >= 0; i--)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword) && TeamInOrder[i].Position.X > 10 && TeamInOrder[i].Position.X < GameSession.Instance.Map.Width - 10)
                        {
                            if (GameSession.Instance.DudeController.EnemyInRange(TeamInOrder[i], 2000,  true) != null)
                            {
                                ActivateFunction(Actions[5], TeamInOrder[i]);
                                return;
                            }
                        }
                    }
                }
            }

            // use pistol
            if (Actions[2].CurrentCooldown <= 0 && Actions[2].IsEnabled)
            {
                if (TeamInOrder.Count > 2)
                {
                    for (int i = 2; i < TeamInOrder.Count; i++)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword))
                        {
                            ActivateFunction(Actions[2], TeamInOrder[i]);
                            return;
                        }
                    }

                }
                else
                {
                    for (int i = TeamInOrder.Count - 1; i >= 0; i--)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword))
                        {
                            ActivateFunction(Actions[2], TeamInOrder[i]);
                            return;
                        }
                    }
                }
            }

            // use shotty
            if (Actions[3].CurrentCooldown <= 0 && Actions[3].IsEnabled)
            {
                if (TeamInOrder.Count > 2)
                {
                    for (int i = 2; i < TeamInOrder.Count; i++)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword))
                        {
                            ActivateFunction(Actions[3], TeamInOrder[i]);
                            return;
                        }
                    }

                }
                else
                {
                    for (int i = TeamInOrder.Count - 1; i >= 0; i--)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword))
                        {
                            ActivateFunction(Actions[3], TeamInOrder[i]);
                            return;
                        }
                    }
                }
            }


            // use SMG
            if (Actions[4].CurrentCooldown <= 0 && Actions[4].IsEnabled)
            {
                if (TeamInOrder.Count > 2)
                {
                    for (int i = 2; i < TeamInOrder.Count; i++)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword))
                        {
                            ActivateFunction(Actions[4], TeamInOrder[i]);
                            return;
                        }
                    }

                }
                else
                {
                    for (int i = TeamInOrder.Count - 1; i >= 0; i--)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword))
                        {
                            ActivateFunction(Actions[4], TeamInOrder[i]);
                            return;
                        }
                    }
                }
            }
            

            // use machine gun
            if (Actions[6].CurrentCooldown <= 0 && Actions[6].IsEnabled)
            {
                if (TeamInOrder.Count > 5)
                {
                    for (int i = 5; i < TeamInOrder.Count; i++)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword) && TeamInOrder[i].Position.X > 10 && TeamInOrder[i].Position.X < GameSession.Instance.Map.Width - 10)
                        {
                            ActivateFunction(Actions[6], TeamInOrder[i]);
                            return;
                        }
                    }
                }
                else
                {
                    for (int i = TeamInOrder.Count - 1; i >= 0; i--)
                    {
                        if (TeamInOrder[i].Weapon.GetType() == typeof(Weapons.Sword) && TeamInOrder[i].Position.X > 10 && TeamInOrder[i].Position.X < GameSession.Instance.Map.Width - 10)
                        {
                            ActivateFunction(Actions[6], TeamInOrder[i]);
                            return;
                        }
                    }
                }
            }

            
        }

        public void HandleInput(MouseState ms)
        {
           
        }

        public void Draw(SpriteBatch sb)
        {
           
        }

        private void ActivateFunction(AIAction a, Dude d)
        {
            if (a.CurrentCooldown <= 0)
            {
                a.CurrentCooldown = a.MaxCooldown;

                switch (a.Function)
                {
                    case "boost":
                        d.BoostTime = 5000;
                        break;
                    case "shield":
                        d.ShieldTime = 10000;
                        break;
                    default:
                        d.GiveWeapon(a.Function);
                        break;
                }
            }
        }

        public void Reset()
        {
            foreach (AIAction a in Actions)
            {
                a.CurrentCooldown = 0;
            }
        }
    }
}
