using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PexesoDatabase;

namespace PexesoClient.Pages
{
    /// <summary>
    /// Interaction logic for ActivePlayers_Page.xaml
    /// </summary>
    public partial class ActivePlayers_Page : Page
    {
        MainWindow _mainWindow;
        public ActivePlayers_Page(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            ComboBoxGameSize.ItemsSource = Enum.GetValues(typeof(GameSize)).Cast<GameSize>();
            ComboBoxGameSize.SelectedItem = ComboBoxGameSize.Items[0];
        }

        public void LoadPlayers(List<string> players, string acutualPlayer)
        {
            ListView_Hraci.Items.Clear();
            foreach (var player in players)
            {
                if (player != acutualPlayer)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Content = player;
                    ListView_Hraci.Items.Add(listViewItem);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.SendInvitation(((ListViewItem)ListView_Hraci.SelectedItem).Content.ToString(), (GameSize)ComboBoxGameSize.SelectedItem);
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            if (ListView_Hraci.Items.Count > 0)
            {
                _mainWindow.SendInvitation(
                    ((ListViewItem)ListView_Hraci.Items[random.Next(0, ListView_Hraci.Items.Count)]).Content
                    .ToString(),
                    (GameSize)ComboBoxGameSize.SelectedItem);
            }
        }
    }
}
