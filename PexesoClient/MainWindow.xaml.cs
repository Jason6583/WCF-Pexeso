using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using PexesoClient.Pages;
using PexesoDatabase;
using PexesoService.Interfaces;
using Game = PexesoService.Interfaces.Game;
using PexesoPlayer = PexesoService.Interfaces.PexesoPlayer;

namespace PexesoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        InstanceContext instanceContext = new InstanceContext(new Client());
        public IPexesoService _pexesoServiceClient;

        private static Frame _frame_Main;
        private Login_Window _loginWindow;
        public PexesoPlayer _pexesoPlayer;
        public static TimeSpan timeLeft;
        private string _ActualCompetitor;

        //Pages
        private ActivePlayers_Page _activePlayers_Page;
        private static Pexeso_Page _pexeso_Page;
        public static bool koniecHry;

        [CallbackBehaviorAttribute(UseSynchronizationContext = true)]
        public class Client : IPexesoClient
        {
            public bool GetInvitation(string nickName, string[][] pexesoGrid, GameSize gameSize)
            {
                MessageBoxResult result = MessageBox.Show($"You got invitation for game from {nickName}\n" +
                                                          $"Game size is: {gameSize}\n" +
                                                          $"Do you wanna accept it?",
                    "Game invitation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _pexeso_Page.ShowPexeso(gameSize, pexesoGrid, nickName);
                    _frame_Main.Content = _pexeso_Page;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void CardFlipped(int[] indexes)
            {
                _pexeso_Page.FlipCard(indexes);
                timeLeft = new TimeSpan(0, 0, 60);
            }

            public void HidedCard(int[] indexes)
            {
                _pexeso_Page.HideCard(indexes);
            }

            public void RemovedCard(int[] indexes)
            {
                _pexeso_Page.RemoveCard(indexes);
            }

            public void CompetitorGotPoint()
            {
                _pexeso_Page.CompetitorPoints++;
            }

            public void MyTurn()
            {
                _pexeso_Page.CanUserClick = true;
            }

            public void GameEnded(Game game)
            {
                koniecHry = true;
                _frame_Main.Content = new EndGameStats_Page(game);
            }

            public void GameEndedByTime()
            {
                MessageBox.Show($"You was idle for too long, game has been ended");
            }

            public void GotMessage(string message)
            {
                _pexeso_Page.AddMessage(message);
            }
        }

        public void TickTimeLeft()
        {
            while (true)
            {
                Thread.Sleep(1000);
                timeLeft = new TimeSpan(timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds - 1);
                if (timeLeft.TotalSeconds <= 0)
                {
                    MessageBox.Show($"Opponent was idle for too long, game has been ended");
                    _pexesoServiceClient.EndGameByTime(_ActualCompetitor);
                    break;
                }

                if (koniecHry)
                    break;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _pexeso_Page = new Pexeso_Page(this);
            var client = new DuplexChannelFactory<IPexesoService>(instanceContext, "PexesoClient");
            _pexesoServiceClient = client.CreateChannel();

            InvitationResultCame += MainWindow_InvitationResultCame;
            _activePlayers_Page = new ActivePlayers_Page(this);
            _frame_Main = new Frame();
            _frame_Main.SetValue(Grid.RowProperty, 1);
            Grid_Main.Children.Add(_frame_Main);
        }

        public void EndGame(Game game)
        {
            koniecHry = true;
            _frame_Main.Content = new EndGameStats_Page(game);
        }

        private void MainWindow_InvitationResultCame(object sender, KeyValuePair<string, bool> e)
        {
            if (e.Value)
            {
                Dispatcher.Invoke(() =>
                {
                    _pexeso_Page.ShowPexeso(pickedGameSize, _pexeso_Page.PexesoGrid, e.Key);
                    _frame_Main.Content = _pexeso_Page;
                    _pexeso_Page.CanUserClick = true;

                    koniecHry = false;
                    timeLeft = new TimeSpan(0, 0, 60);

                    Thread thread = new Thread(TickTimeLeft) {IsBackground = true};
                    thread.Start();

                    _ActualCompetitor = e.Key;
                    MessageBox.Show($"{e.Key} has accepted your invitation");
                });

            }
            else
            {
                MessageBox.Show($"{e.Key} has declined your invitation");
            }
        }

        private GameSize pickedGameSize;
        public PexesoPlayer Player
        {
            get { return _pexesoPlayer; }
            set
            {
                _pexesoPlayer = value;
                OnPropertyChanged("Player");
            }
        }

        public bool LogIn(string name, string password)
        {
            try
            {
                PexesoPlayer pexesoPlayer = _pexesoServiceClient.LoginPlayer(name, password);

                if (pexesoPlayer == null)
                {
                    throw new ArgumentException("Incorrect name or password!");
                }

                else
                {
                    Player = pexesoPlayer;
                    MessageBox.Show("You was succesfully loged in!");
                    return true;
                }

            }
            catch (Exception ex)
            {
                instanceContext = new InstanceContext(new Client());
                var client = new DuplexChannelFactory<IPexesoService>(instanceContext, "PexesoClient");
                _pexesoServiceClient = client.CreateChannel();

                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool CreateAccount(string name, string password)
        {
            try
            {
                PexesoPlayer pexesoPlayer = _pexesoServiceClient.AddPlayer(name, password);

                if (pexesoPlayer == null)
                {
                    throw new ArgumentException("Incorrect name or password!");
                }
                else
                {
                    Player = pexesoPlayer;
                    MessageBox.Show("Your account was successfully created");

                    instanceContext = new InstanceContext(new Client());
                    var client = new DuplexChannelFactory<IPexesoService>(instanceContext, "PexesoClient");
                    _pexesoServiceClient = client.CreateChannel();

                    return true;
                }
            }
            catch (Exception ex)
            {

                instanceContext = new InstanceContext(new Client());
                var client = new DuplexChannelFactory<IPexesoService>(instanceContext, "PexesoClient");
                _pexesoServiceClient = client.CreateChannel();

                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void MenuItem_Login_Click(object sender, RoutedEventArgs e)
        {

            _loginWindow = new Login_Window(this, "Log in");
            _loginWindow.ShowDialog();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MenuItem_CreateAccout_Click(object sender, RoutedEventArgs e)
        {

            _loginWindow = new Login_Window(this, "Create");
            _loginWindow.ShowDialog();

        }

        public event EventHandler<KeyValuePair<string, bool>> InvitationResultCame;

        private void SendInvitationThread(string name, GameSize gameSize)
        {
            pickedGameSize = gameSize;
            var grid = _pexeso_Page.MakePexesoGrid(gameSize);

            var game = _pexesoServiceClient.CreateGame(gameSize, grid, Player.NickName,
                 name);

            if (game == null)
                OnInvitationResultCame(name, false);
            else
                OnInvitationResultCame(name, true);
        }
        public void SendInvitation(string name, GameSize gameSize)
        {
            Thread thread = new Thread(() => SendInvitationThread(name, gameSize));
            thread.Start();

        }
        private void InvitePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (_pexesoPlayer != null)
            {
                _activePlayers_Page.LoadPlayers(_pexesoServiceClient.GetListOfPlayers(), _pexesoPlayer.NickName);
                _frame_Main.Content = _activePlayers_Page;
            }
            else
                MessageBox.Show("You have to log in");

        }

        protected virtual void OnInvitationResultCame(string nickName, bool result)
        {
            InvitationResultCame?.Invoke(this, new KeyValuePair<string, bool>(nickName, result));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                _pexesoServiceClient.Disconnect(_pexesoPlayer.NickName);
            }
            catch (Exception)
            {
            }
        }
    }
}
