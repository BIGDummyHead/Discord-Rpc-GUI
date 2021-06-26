using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DiscordRPC;

namespace DiscordRpcGUI
{
    public sealed record Profile
    {
        internal Profile() { }
        public Profile(string name, string applicationID) : this(name, applicationID, false)
        {

        }
        public Profile(string name)
        {
            Name = name;
            ApplicationID = "REQUIRED (APP ID)";
            Invalid = true;
        }

        Profile(string name, string applicationID, bool skipChecks)
        {
            if (!skipChecks)
            {
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Name cannot be null or empty!");
                    return;
                }
                else if (string.IsNullOrEmpty(applicationID))
                {
                    MessageBox.Show("Application ID cannot be null or empty!");
                    return;
                }
            }

            Name = name;
            ApplicationID = applicationID;
            Invalid = false;
        }

        internal bool Invalid { get; init; } = true;

        public string Name { get; init; }
        public string ApplicationID { get; init; }

        public Assets Assets { get; set; }
        public Button[] Buttons { get; set; }
        public bool UseTimer { get; set; }
        public string State { get; set; }
        public string Details { get; set; }

        public int PartySize { get; set; }
        public int MaxPartySize { get; set; }
        public bool AllowJoin { get; set; } = true;

        public RichPresence ToPrecense()
        {
            if (Invalid)
            {
                MessageBox.Show("This profile is invalid!", "Invalid Profile", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            RichPresence prec = new()
            {
                Assets = Assets,
                Buttons = Buttons,

                State = State,
                Details = Details,
            };

            if (AllowJoin)
                prec.Party = new Party
                {
                    Max = MaxPartySize,
                    Size = PartySize,
                    Privacy = Party.PrivacySetting.Public,
                    //this info is not filled in because there is NO ID
                    ID = "PARTY ID"
                };

            if (UseTimer)
                prec.Timestamps = Timestamps.Now;

            return prec;
        }

        public void Save()
        {
            if (Invalid)
            {
                MessageBox.Show("This profile is invalid!", "Invalid Profile", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CheckAndCreate();

            if (!ProfileExist(FindName, out _))
                File.Create(CurrentSave).Close();


            FileJson.Write(CurrentSave, this);
        }

        public static IEnumerable<Profile> GetAll()
        {
            IEnumerable<string> files = Directory.GetFiles(dir).Where(x => Path.GetExtension(x).Equals(extension, StringComparison.OrdinalIgnoreCase));
            foreach (string file in files)
            {
                Profile prof = FileJson.Read<Profile>(file);
                yield return prof;
            }
        }

        private string FindName
        {
            get
            {
                string rep = Name.ToLower().Replace(" ", "_");

                string invalid = new StringBuilder().Append(Path.GetInvalidFileNameChars()).ToString();

                string fName = string.Empty;
                foreach (char c in rep)
                {
                    if (!invalid.Contains(c, StringComparison.OrdinalIgnoreCase))
                        fName += c;
                }

                if (string.IsNullOrEmpty(fName))
                {
                    string tmp = Path.GetFileNameWithoutExtension(Path.GetTempFileName());

                    fName = $"unknown_{tmp}";
                }

                return fName;
            }
        }
        const string dir = "Profiles\\";
        public string CurrentSave => $"{dir}{FindName}.prof";
        const string extension = ".prof";
        static void CheckAndCreate()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static bool ProfileExist(string profileName, out Profile profile)
        {
            CheckAndCreate();

            string profFile = Directory.GetFiles(dir).FirstOrDefault(x => Path.GetExtension(x).Equals(extension) && Path.GetFileNameWithoutExtension(x).Equals(profileName, StringComparison.OrdinalIgnoreCase));

            bool exist = profFile != default;

            if (exist)
                profile = FileJson.Read<Profile>(profileName);
            else
                profile = null;

            return exist;
        }

        public override string ToString()
        {
            return Name;
        }

        public static explicit operator RichPresence(Profile prof)
        {
            return prof.ToPrecense();
        }
    }
}
