using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fodder.Core
{
    public class Scenario
    {
        public List<Function> AvailableFunctions;
        public int AIReactionTime, T1Reinforcements, T2Reinforcements;
        public double T1SpawnRate, T2SpawnRate;
        public string MapName, ScenarioName;
        public int BronzeScore, SilverScore, GoldScore;
        public int CampaignMissionNum;

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

    public class ScenarioResult
    {
        public int Team1TotalReinforcements;
        public int Team2TotalReinforcements;
        public int Team1RemainingReinforcements;
        public int Team2RemainingReinforcements;
        public int Team1ActiveCount;
        public int Team2ActiveCount;
        public int Team1DeadCount;
        public int Team2DeadCount;
        public bool Team1Win;
        public bool Team2Win;
        public bool Team1Human;
        public bool Team2Human;
        public int Team1ScoreRewarded;
        public int Team2ScoreRewarded;

        public ScenarioResult() {}

        public ScenarioResult(GameSession session, Scenario scenario)
        {
            Team1TotalReinforcements = session.Team1StartReinforcements;
            Team2TotalReinforcements = session.Team2StartReinforcements;
            Team1RemainingReinforcements = session.Team1Reinforcements;
            Team2RemainingReinforcements = session.Team2Reinforcements;
            Team1ActiveCount = session.Team1ActiveCount;
            Team2ActiveCount = session.Team2ActiveCount;
            Team1DeadCount = session.Team1DeadCount;
            Team2DeadCount = session.Team2DeadCount;
            Team1Win = session.Team1Win;
            Team2Win = session.Team2Win;

            Team1Human = (session.Team1ClientType == GameClientType.Human ? true : false);
            Team2Human = (session.Team2ClientType == GameClientType.Human ? true : false);

            int Team1Percent = (int)((100M / (decimal)Team1TotalReinforcements) * ((decimal)Team1RemainingReinforcements + (decimal)Team1ActiveCount));
            int Team2Percent = (int)((100M / (decimal)Team2TotalReinforcements) * ((decimal)Team2RemainingReinforcements + (decimal)Team2ActiveCount));

            if (Team1Percent >= scenario.BronzeScore) Team1ScoreRewarded = 1;
            if (Team1Percent >= scenario.SilverScore) Team1ScoreRewarded = 2;
            if (Team1Percent >= scenario.GoldScore) Team1ScoreRewarded = 3;

            if (Team2Percent >= scenario.BronzeScore) Team2ScoreRewarded = 1;
            if (Team2Percent >= scenario.SilverScore) Team2ScoreRewarded = 2;
            if (Team2Percent >= scenario.GoldScore) Team2ScoreRewarded = 3;
        }
    }
}
