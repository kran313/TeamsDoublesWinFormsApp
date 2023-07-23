using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System;
using Newtonsoft.Json;
using Microsoft.VisualBasic.Devices;
using System.Xml.Linq;

namespace TeamsDoublesWinFormsApp
{
    public partial class Form1 : Form
    {
        public List<Participant> participants;
        public Participant currentParticipant;
        public int participantIndex;
        public List<Label> labels;
        public List<CheckBox> checkboxes;
        public List<Participant> participantsTemp;
        Dictionary<string, Dictionary<int, Dictionary<string, string>>> participantsJson;
        string currentSport;

        public Form1()
        {
            currentSport = "Футбол";

            InitializeComponent();
            participants = Participants.GetAll(currentSport);
            participantIndex = 0;
            labels = new List<Label>();
            checkboxes = new List<CheckBox>();
            participantsTemp = new List<Participant>();

            FindStartID();
            StartComparasing();

        }

        private void StartComparasing()
        {
            CreateLable(30, 10, 1000, 30, participants[participantIndex].Branch, 15F);
            CreateLable(30, 40, 1000, 50, $"{participants[participantIndex].Name}  /  {participants[participantIndex].NameEN}", 25F);

            CreateLable(1050, 10, 200, 30, participants[participantIndex].ID, 15F);
            CreateLable(1050, 40, 200, 40, participantIndex.ToString() + " / " + participants.Count().ToString(), 15F);

            currentParticipant = participants[participantIndex];


            int locationHeight = 125;

            for (int i = participantIndex + 1; i < participants.Count(); i++)
            {

                if (SameTeams(participants[participantIndex], participants[i]))
                {
                    participantsTemp.Add(participants[i]);

                    CreateCheckBox(25, locationHeight + 10, 150, 30, participants[participantIndex].Name == participants[i].Name);
                    CreateLable(60, locationHeight, 900, 30, $"{participants[i].Name}  /  {participants[i].NameEN}", 17F);
                    CreateLable(1000, locationHeight, 100, 30, $"{participants[i].ID}");

                    locationHeight += 30;

                    CreateLable(60, locationHeight, 1200, 30, $"{participants[i].Branch}", 12F);
                    CreateLable(25, locationHeight + 30, 1200, 20, SeparateLine());

                    locationHeight += 50;
                }
            }

            if (checkboxes.Count == 0)
            {
                participantIndex++;

                labels.ForEach(t => t.Dispose());
                checkboxes.ForEach(t => t.Dispose());

                labels = new List<Label>();
                checkboxes = new List<CheckBox>();
                participantsTemp = new List<Participant>();

                StartComparasing();
            }
            else if (checkboxes.All(t => t.Checked))
            {
                ToDB();
            }

        }

        public string SeparateLine()
        {
            var separateLine = "";
            for (int i = 0; i < 185; i++)
            {
                separateLine += '-';
            }
            return separateLine;
        }

        public void CreateLable(int locationX, int locationY, int sizeWidth, int sizeHeigth, string text, float font = 12F)
        {
            Label label = new Label();

            label.Font = new Font("Segoe UI", font, FontStyle.Regular, GraphicsUnit.Point);
            label.Location = new Point(locationX, locationY);
            label.Size = new Size(sizeWidth, sizeHeigth);
            label.Text = text;
            label.TextAlign = ContentAlignment.MiddleLeft;

            Controls.Add(label);
            labels.Add(label);
        }

        public void CreateCheckBox(int locationX, int locationY, int sizeWidth, int sizeHeigth, bool isChecked)
        {
            CheckBox checkBox = new CheckBox();

            checkBox.AutoSize = true;
            checkBox.Location = new Point(locationX, locationY);
            checkBox.Size = new Size(sizeWidth, sizeHeigth);
            checkBox.Text = "";
            checkBox.Checked = isChecked;

            Controls.Add(checkBox);
            checkboxes.Add(checkBox);
        }

        public void FindStartID()
        {
            using (StreamReader file = File.OpenText("participants.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                participantsJson = (Dictionary<string, Dictionary<int, Dictionary<string, string>>>)serializer.Deserialize(file, typeof(Dictionary<string, Dictionary<int, Dictionary<string, string>>>));
            }

            if (!participantsJson.ContainsKey(currentSport))
            {
                participantsJson[currentSport] = new Dictionary<int, Dictionary<string, string>>();
                using (StreamWriter file = File.CreateText("participants.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, participantsJson);
                }
            }
            else
            {
                var startKey = participantsJson[currentSport].Keys.ToList()[^1];
                var startIDIndex = participantsJson[currentSport][startKey].Keys.ToList()[0];

                while (participants[participantIndex].ID != startIDIndex)
                {
                    participantIndex++;
                }
                participantIndex++;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            ToDB();
        }

        public void ToDB()
        {
            if (checkboxes.Where(t => t.Checked).ToList().Count != 0)
            {
                using (StreamReader file = File.OpenText("participants.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    participantsJson = (Dictionary<string, Dictionary<int, Dictionary<string, string>>>)serializer.Deserialize(file, typeof(Dictionary<string, Dictionary<int, Dictionary<string, string>>>));
                }


                Dictionary<string, string> participantsToAdd = new Dictionary<string, string>();
                participantsToAdd[currentParticipant.ID] = currentParticipant.Name;

                for (int i = 0; i < checkboxes.Count; i++)
                {
                    if (checkboxes[i].Checked)
                    {
                        participantsToAdd[participantsTemp[i].ID] = participantsTemp[i].Name;
                    }
                }

                if (!participantsJson.ContainsKey(currentSport))
                {
                    participantsJson[currentSport] = new Dictionary<int, Dictionary<string, string>>();
                }
                participantsJson[currentSport][participantsJson[currentSport].Count + 1] = participantsToAdd;


                using (StreamWriter file = File.CreateText("participants.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, participantsJson);
                }
            }



            participantIndex++;
            foreach (var label in labels)
            {
                label.Dispose();
            }
            foreach (var checkbox in checkboxes)
            {
                checkbox.Dispose();
            }
            labels = new List<Label>();
            checkboxes = new List<CheckBox>();
            participantsTemp = new List<Participant>();
            StartComparasing();
        }





        #region Comparasing

        public Dictionary<char, int> GetCharDict(string team)
        {
            Dictionary<char, int> teamSymbols = new Dictionary<char, int>();

            foreach (var symbol in team)
            {
                if (!teamSymbols.ContainsKey(symbol))
                    teamSymbols[symbol] = 1;
                else
                    teamSymbols[symbol]++;
            }
            return teamSymbols;
        }

        public int GetMissingSymbols(Dictionary<char, int> team1Symbols, Dictionary<char, int> team2Symbols)
        {
            int missingTeam1Symbols = 0;

            foreach (var symbol in team1Symbols.Keys)
            {
                if (team2Symbols.ContainsKey(symbol))
                    missingTeam1Symbols += Math.Max(0, team1Symbols[symbol] - team2Symbols[symbol]);
                else
                    missingTeam1Symbols += team1Symbols[symbol];
            }
            return missingTeam1Symbols;
        }

        public bool CompareByMissingSymbols(string team1, string team2)
        {
            var team1Symbols = GetCharDict(team1);
            var team2Symbols = GetCharDict(team2);


            if (Math.Max(GetMissingSymbols(team1Symbols, team2Symbols),
                         GetMissingSymbols(team2Symbols, team1Symbols)) <=
                Math.Min(Math.Min(team1.Length, team2.Length) * 0.2, 3))
            {
                return true;
            }
            return false;
        }

        public int GetMissingSubstrings(string team1, string team2, int substringLength)
        {
            if (team1.Length <= substringLength || team2.Length <= substringLength)
            {
                return 0;
            }

            int missingSubstrings = 0;

            for (int i = 0; i < team1.Length - substringLength + 1; i++)
            {
                if (!team2.Contains(team1.Substring(i, substringLength)))
                    missingSubstrings++;
            }
            return missingSubstrings;
        }

        public bool CompareByMissingSubstrings(string team1, string team2)
        {
            if ((float)GetMissingSubstrings(team1, team2, 2) / (team1.Length - 1) < 0.5 &&
                (float)GetMissingSubstrings(team2, team1, 2) / (team2.Length - 1) < 0.5 &&
                (float)GetMissingSubstrings(team1, team2, 3) / (team1.Length - 2) < 0.5 &&
                (float)GetMissingSubstrings(team2, team1, 3) / (team2.Length - 2) < 0.5)
            {
                return true;
            }
            return false;
        }



        public bool Compare(string team1, string team2)
        {
            team1 = team1.Replace("-", " ").Replace(".", "").Trim();
            team2 = team2.Replace("-", " ").Replace(".", "").Trim();


            if (!team1.Contains(' ') && !team2.Contains(' '))
            {
                return CompareByMissingSymbols(team1, team2) &
                       CompareByMissingSubstrings(team1, team2);
            }
            else
            {
                string[] team1Array = team1.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
                string[] team2Array = team2.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();

                if (team1Array.Length == team2Array.Length && team1Array.Length == 2)
                {
                    if (team2Array[0].Length > 2 && team2Array[1].Length > 2)
                    {
                        return Compare(team1Array[0], team2Array[0]) && Compare(team1Array[1], team2Array[1]) ||
                               Compare(team1Array[0], team2Array[1]) && Compare(team1Array[1], team2Array[0]);
                    }
                    else
                    {
                        return (Compare(team1Array[0], team2Array[0]) && team1Array[1][0] == team2Array[1][0]) ||
                               (Compare(team1Array[1], team2Array[1]) && team1Array[0][0] == team2Array[0][0]) ||
                               (Compare(team1Array[1], team2Array[0]) && team1Array[0][0] == team2Array[1][0]) ||
                               (Compare(team1Array[0], team2Array[1]) && team1Array[1][0] == team2Array[0][0]);
                    }
                }
                else
                {
                    int sum = 0;
                    for (int i = 0; i < team1Array.Length; i++)
                    {
                        for (int j = 0; j < team2Array.Length; j++)
                        {
                            if (Compare(team1Array[i], team2Array[j]))
                            {
                                sum++;
                                return sum > 1 ? true : false;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public List<string> GetAllNames(Participant team)
        {
            List<string> teamNames = new List<string>() { team.Name };

            if (team.NameEN != "" && !teamNames.Contains(team.NameEN))
            {
                teamNames.Add(team.NameEN);
            }
            if (team.NameSR != "" && !teamNames.Contains(team.NameSR))
            {
                teamNames.Add(team.NameSR);
            }

            foreach (var item in team.NameExtra.Split(',').ToArray())
            {
                if (item != "" && !teamNames.Contains(item))
                {
                    teamNames.Add(item);
                }
            }
            return teamNames;
        }

        public bool SameTeams(Participant team1, Participant team2)
        {
            var tempTeam1Name = team1.Name;
            var tempTeam2Name = team2.Name;

            while (tempTeam1Name.Contains('('))
            {
                string substring = "(" + tempTeam1Name.Split(')')[0].Split('(')[^1] + ")";
                if (!tempTeam2Name.Contains(substring))
                {
                    return false;
                }
                tempTeam1Name = tempTeam1Name.Replace(substring, "");
                tempTeam2Name = tempTeam2Name.Replace(substring, "");
            }
            if (tempTeam2Name.Contains('('))
            {
                return false;
            }


            foreach (var team1Name in GetAllNames(team1))
            {
                foreach (var team2Name in GetAllNames(team2))
                {
                    if (Compare(team1Name, team2Name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}