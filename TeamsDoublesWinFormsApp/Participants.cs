using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace TeamsDoublesWinFormsApp
{
    public static class Participants
    {
        public static List<Participant> participants = new List<Participant>();

        public static List<Participant> GetAll(string currentSport)
        {
            string[] teams = File.ReadAllLines(@"\\server.gkbaltbet.local\\profiles\\alitvinov\\Desktop\\Participants.csv");

            foreach (var line in teams)
            {
                string[] info = line.Split('\t');

                if (info.Length == 8)
                {
                    string ID = info[0];
                    string Name = info[1];
                    string NameEn = info[2];
                    string NameSr = info[3];
                    string NameExtra = info[4];
                    string Branch = info[5];
                    string IsLive = info[6];
                    string Sport = info[7];

                    if (IsLive == "0" &&
                        Sport == currentSport &&
                        Branch != "" && new[] { "Итоги", "Дабл матч", "Дабл шанс", "Тройной шанс", "Тестирование", "Статистика", "Cтатистика", 
                                                "Сравнение результативности", "Матч всех звезд", "Матч дня" }.All(c => !Branch.ToLower().Contains(c.ToLower())) &&

                        Name != "" && new[] { "хозяева", "гости", "участни", "тест ", "команда ", "группа ", "итоговое место" }.All(c => !Name.ToLower().Contains(c.ToLower())))
                    {
                        participants.Add(new Participant(ID, Name, NameEn, NameSr, NameExtra, Branch, IsLive, Sport));
                    }
                }
                else
                {

                }
            }
            participants.RemoveAt(0);
            return participants;
        }
    }
}
