using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Globalization;
using System.Media;
using DiscordRPC;

namespace DiscordRpcGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            RunChange(false);

            Fill("small.png", smallImage);
            Fill("big.jpg", bigImage);

            allowJoin.ChangeCheck(true);

            Load(DefaultProfile);

            UpdateProfiles();
            ContextMenu = new ContextMenu()
            {
                
            };
             ContextMenu.Items.Add("Exit");
        }

        public void UpdateProfiles(Profile toSelect = null)
        {
            profileBox.Items.Clear();

            toSelect ??= DefaultProfile;

            var x = Profile.GetAll().Where(x => x is not null);
            foreach (Profile profile in x)
            {
                profileBox.Items.Add(profile);
            }

            Load(toSelect);

            profileBox.SelectedItem = toSelect;

        }

        void Fill(string resourceName, Shape toFill)
        {
            ImageBrush imgBrush = new();

            imgBrush.Stretch = Stretch.UniformToFill;
            imgBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/" + resourceName, UriKind.Absolute));

            toFill.Fill = imgBrush;
        }

        public DiscordRPC.Button[] GetButtons()
        {
            List<DiscordRPC.Button> buttons = new();

            DiscordRPC.Button b1 = Create(button_1_text.Text, button_1_url.Text, out bool b1Create);
            DiscordRPC.Button b2 = Create(button_2_text.Text, button_2_url.Text, out bool b2Create);

            if (b1Create)
                buttons.Add(b1);

            if (b2Create)
                buttons.Add(b2);

            return buttons.ToArray();
        }

        public DiscordRPC.Button Create(string txt, string url, out bool created)
        {
            if (string.IsNullOrEmpty(txt) || string.IsNullOrEmpty(url))
            {
                created = false;
                return null;
            }

            created = true;
            return new DiscordRPC.Button
            {
                Label = txt,
                Url = url
            };
        }

        public DiscordRPC.Assets GetAssets()
        {
            return new DiscordRPC.Assets
            {
                LargeImageKey = lg_img_key.Text,
                LargeImageText = lg_img_txt.Text,
                SmallImageKey = sm_img_key.Text,
                SmallImageText = sm_img_txt.Text
            };
        }

        public Profile GetProfile()
        {
            int partSize = 0;
            int max = 0;

            if((bool)allowJoin.IsChecked)
            {
                max = Max_PartySize;
                partSize = PartySize;
            }

            return new Profile(profName.Text, appID.Text)
            {
                Details = details.Text,
                Assets = GetAssets(),
                Buttons = GetButtons(),
                State = state.Text,
                UseTimer = (bool)time_start.IsChecked,
                MaxPartySize = max,
                PartySize = partSize,
                AllowJoin = (bool)allowJoin.IsChecked,
            };
        }

        public Profile DefaultProfile
        {
            get
            {
                return new Profile("Default Profile")
                {
                    Assets = null,
                    Buttons = Array.Empty<DiscordRPC.Button>(),
                    Details = "Competitive",
                    MaxPartySize = 5,
                    PartySize = 1,
                    State = "Playing Solo",
                    UseTimer = true,
                    AllowJoin = true
                };
            }
        }

        public Profile CurrentProfile { get; private set; }
        public void Load(Profile profile)
        {
            if (profile is null)
            {
                return;
            }

            int buttonCount = profile.Buttons.Length;

            if (buttonCount >= 1)
            {
                var b1 = profile.Buttons[0];
                button_1_text.Text = b1.Label;
                button_1_url.Text = b1.Url;

                if (buttonCount > 1)
                {
                    var b2 = profile.Buttons[1];
                    button_2_text.Text = b2.Label;
                    button_2_url.Text = b2.Url;
                }
                else
                {
                    button_2_text.Text = string.Empty;
                    button_2_url.Text = string.Empty;
                }
            }
            else
            {
                button_1_text.Text = string.Empty;
                button_1_url.Text = string.Empty;
            }
            state.Text = profile.State;
            time_start.IsChecked = profile.UseTimer;
            details.Text = profile.Details;

            var asset = profile.Assets;

            if (asset != null)
            {
                lg_img_key.Text = asset.LargeImageKey;
                lg_img_txt.Text = asset.LargeImageText;

                sm_img_key.Text = asset.SmallImageKey;
                sm_img_txt.Text = asset.SmallImageText;
            }

            allowJoin.IsChecked = profile.AllowJoin;

            if (!profile.AllowJoin)
            {
                ChangeVisibility(Visibility.Collapsed, sJB, seeJoinButton);

            }
            else
            {
                ChangeVisibility(Visibility.Visible, sJB, seeJoinButton);
            }

            if (profile.UseTimer)
                vis_timer.Visibility = Visibility.Visible;
            else
                vis_timer.Visibility = Visibility.Collapsed;

            part_size.Text = profile.PartySize.ToString();
            part_max.Text = profile.MaxPartySize.ToString();

            profName.Text = profile.Name;
            appID.Text = profile.ApplicationID;

            CurrentProfile = profile;
        }

        


        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void MinimizeApplication(object sender, RoutedEventArgs e)
        {
            this.windowMain.WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void GotoHyper(object sender, RequestNavigateEventArgs e)
        {
            OpenUrl(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }

        public int PartySize
        {
            get
            {
                int.TryParse(part_size.Text, out int res);

                return res;
            }
        }
        public int Max_PartySize
        {
            get
            {
                int.TryParse(part_max.Text, out int res);
                return res;
            }
        }

        KeyConverter keyConverter = new();
        private void NumberOnlyKey(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;

            if (box.Text.Length >= 21)
                e.Handled = true;

            string key = keyConverter.ConvertToString(e.Key);
            if (!AllDigits(key))
            {
                SystemSounds.Exclamation.Play();
                e.Handled = true;
            }
        }

        public bool AllDigits(string check)
        {
            foreach (char c in check)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        private void NumbersChanged(object sender, TextChangedEventArgs e)
        {
            state_label.Text = PartyConverter.MainConverter.Convert(state.Text, null, null, null) as string;

        }

        public void ChangeJoin(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;

            if ((bool)!box.IsChecked)
            {
                ChangeVisibility(Visibility.Collapsed, maxPartySizeController, partySizeController, seeJoinButton, sJB);
            }
            else
            {
                ChangeVisibility(Visibility.Visible, maxPartySizeController, partySizeController);

                if (PartySize > 0 && Max_PartySize > 0 && Max_PartySize >= PartySize)
                {
                    ChangeVisibility(Visibility.Collapsed, seeJoinButton, sJB);
                }
            }

            state_label.Text = PartyConverter.MainConverter.Convert(state.Text, null, null, null) as string;
        }

        public void ChangeVisibility(Visibility visibility, params UIElement[] controls)
        {
            foreach (UIElement control in controls)
            {
                control.Visibility = visibility;
            }
        }

        private void RunClick(object sender, RoutedEventArgs e)
        {
            RunChange(true);
            Connect();
        }

        public DiscordRpcClient client;
        public void Connect()
        {
            Profile profile = GetProfile();
            DiscordRPC.RichPresence presence = (DiscordRPC.RichPresence)profile;

            client = new DiscordRpcClient(profile.ApplicationID);

            client.Initialize();

            client.SetPresence(presence);


            RunChange(true);
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Profile profile = GetProfile();

            if (profile == DefaultProfile)
                return;

            profile.Save();

            UpdateProfiles(profile);
        }

        private void LoadOtherProfile(object sender, SelectionChangedEventArgs e)
        {
            Profile profile = profileBox.SelectedItem as Profile;
            Load(profile);
        }

        private void UpdateSmall(object sender, TextChangedEventArgs e)
        {
            TextBox box = sender as TextBox;

            if (string.IsNullOrEmpty(box.Text))
                smallImage.Visibility = Visibility.Collapsed;
            else
                smallImage.Visibility = Visibility.Visible;
        }

        private void ActivateButton2(object sender, TextChangedEventArgs e)
        {
            Change(sender as TextBox, visualizer_button2);

        }

        private void ActivateButton1(object sender, TextChangedEventArgs e)
        {
            Change(sender as TextBox, visualizer_button1);
        }

        void Change(TextBox tb, UIElement ele)
        {
            if (string.IsNullOrEmpty(tb.Text))
                ele.Visibility = Visibility.Collapsed;
            else
                ele.Visibility = Visibility.Visible;
        }

        const double OriHeight = 532;
        const double HiddenHeight = 480;

        public void RunChange(bool running)
        {
            if (running)
            {
                //hide items
                windowMain.Height = HiddenHeight;
                ChangeVisibility(Visibility.Visible, hideableButt, hideableOpap, hideableTb);
            }
            else
            {
                windowMain.Height = OriHeight;
                ChangeVisibility(Visibility.Collapsed, hideableButt, hideableOpap, hideableTb);
            }
        }

        private void StopRpc(object sender, RoutedEventArgs e)
        {
            RunChange(false);

            if (client is null)
                return;

            client.Deinitialize();
            client = null;
        }

        private void BigImgEnter(object sender, MouseEventArgs e)
        {
            TextBox block = lg_img_txt;

            if (string.IsNullOrEmpty(block.Text))
                return;

            ChangeVisibility(Visibility.Visible, popup_large);
        }

        private void BigImgLeave(object sender, MouseEventArgs e)
        {
            ChangeVisibility(Visibility.Collapsed, popup_large);
        }

        private void SmallImgEnter(object sender, MouseEventArgs e)
        {
            TextBox block = sm_img_txt;

            if (string.IsNullOrEmpty(block.Text))
                return;

            ChangeVisibility(Visibility.Visible, popup_small);
        }

        private void SmallImgLeave(object sender, MouseEventArgs e)
        {
            ChangeVisibility(Visibility.Collapsed, popup_small);

        }

        private void HideApp(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            ShowInTaskbar = false;

            App.Min();
        }

        private void TimeStopClick(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            bool check = (bool)cb.IsChecked;

            if (check)
                vis_timer.Visibility = Visibility.Visible;
            else
                vis_timer.Visibility = Visibility.Collapsed;
        }
    }

    public class PartyConverter : IValueConverter
    {
        public MainWindow Window => MainWindow.Instance;

        public static PartyConverter MainConverter { get; private set; }

        public PartyConverter()
        {
            MainConverter = this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string txt = value as string;

            if (string.IsNullOrEmpty(txt))
                return string.Empty;

            if (Window.PartySize > 0 && Window.Max_PartySize > 0 && Window.Max_PartySize >= Window.PartySize)
            {

                if ((bool)Window.allowJoin.IsChecked)
                {
                    txt += $" ({Window.part_size.Text} of {Window.part_max.Text})";
                    Window.seeJoinButton.Visibility = Visibility.Visible;
                    Window.sJB.Visibility = Visibility.Visible;
                }
            }

            return txt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public static class Exts
    {
        public static void ChangeCheck(this CheckBox box, bool check)
        {
            box.IsChecked = check;

            MainWindow.Instance.ChangeJoin(box, null);
        }
    }
}
