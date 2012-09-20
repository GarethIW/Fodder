using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Fodder.Core;
using System.IO;
using System.Xml.Serialization;

namespace Fodder.Utils
{
    public partial class ScenarioEditor : Form
    {
        public ScenarioEditor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Function> funcs = new List<Function>();
            funcs.Add(new Function("boost", 1000, chkBoost.Checked));
            funcs.Add(new Function("shield", 10000, chkShield.Checked));
            funcs.Add(new Function("pistol", 4000, chkPistol.Checked));
            funcs.Add(new Function("shotgun", 6000, chkShotgun.Checked));
            funcs.Add(new Function("smg", 8000, chkSMG.Checked));
            funcs.Add(new Function("sniper", 30000, chkSniper.Checked));
            funcs.Add(new Function("machinegun", 30000, chkMG.Checked));
            funcs.Add(new Function("mortar", 30000, chkMortar.Checked));
            funcs.Add(new Function("haste", 20, chkSoul.Checked));
            funcs.Add(new Function("meteors", 20, chkSoul.Checked));
            funcs.Add(new Function("elite", 20, chkSoul.Checked));

            Scenario scenario = new Scenario(txtScenarioName.Text, txtMap.Text, funcs, 
                                             Convert.ToInt32(txtAIReaction.Text), 
                                             Convert.ToInt32(txtT1Re.Text),
                                             Convert.ToInt32(txtT2Re.Text),
                                             Convert.ToInt32(txtT1Spawn.Text),
                                             Convert.ToInt32(txtT2Spawn.Text));

            scenario.GoldScore = Convert.ToInt32(txtGold.Text);
            scenario.SilverScore = Convert.ToInt32(txtSilver.Text);
            scenario.BronzeScore = Convert.ToInt32(txtBronze.Text);

            scenario.CampaignMissionNum = Convert.ToInt32(txtMissionNum.Text);

            StringWriter output = new StringWriter(new StringBuilder());
            XmlSerializer xmls = new XmlSerializer(typeof(Scenario));
            xmls.Serialize(output, scenario);
            string outputstring = output.ToString();
            outputstring = outputstring.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\" ?><XnaContent><Asset Type=\"System.String\"><![CDATA[");
            outputstring += "]]></Asset></XnaContent>";
            textBox1.Text = outputstring;
        }
    }
}
