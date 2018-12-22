using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using PexesoDatabase;
using Game = PexesoService.Interfaces.Game;

namespace PexesoClient.Pages
{
    /// <summary>
    /// Interaction logic for Pexeso_Page.xaml
    /// </summary>
    public partial class Pexeso_Page : Page, INotifyPropertyChanged
    {
        private int challengerPoints;
        private int competitorPoints;
        private string competitor;
        private string challenger;
        private Game game;
        private DateTime startGame;

        public bool CanUserClick { get; set; } = false;

        public int ChallengerPoints
        {
            get { return challengerPoints; }
            set
            {
                challengerPoints = value;
                OnPropertyChanged("ChallengerPoints");
            }
        }


        public int CompetitorPoints
        {
            get { return competitorPoints; }
            set
            {
                competitorPoints = value;
                OnPropertyChanged("CompetitorPoints");
            }
        }

        public string Competitor
        {
            get { return competitor; }
            set
            {
                competitor = value;
                OnPropertyChanged("Competitor");
            }
        }

        public string Challenger
        {
            get { return challenger; }
            set
            {
                challenger = value;
                OnPropertyChanged("Challenger");
            }
        }


        List<Button> _showedCards;

        public string[][] PexesoGrid { get; set; }
        public GameSize GameSize { get; set; }

        private MainWindow _mainWindow;
        public Pexeso_Page(MainWindow MainWindow)
        {
            InitializeComponent();
            DataContext = this;
            _showedCards = new List<Button>();
            _mainWindow = MainWindow;
        }

        public void AddMessage(string message)
        {
            Run competitorRun = new Run($"{competitor}: ")
            {
                FontSize = 12, FontWeight = FontWeights.Bold, Foreground = Brushes.Black
            };

            Run messageRun = new Run($"{message}") {FontSize = 12};

            TextBlock_Chat.Inlines.Add(competitorRun);
            TextBlock_Chat.Inlines.Add(messageRun);

            TextBlock_Chat.Inlines.Add(new LineBreak());
        }

        public void AddMessageMyself(string message)
        {
            Run competitorRun = new Run($"{Challenger}: ")
            {
                FontSize = 12, FontWeight = FontWeights.Bold, Foreground = Brushes.Black
            };

            Run messageRun = new Run($"{message}") {FontSize = 12};

            TextBlock_Chat.Inlines.Add(competitorRun);
            TextBlock_Chat.Inlines.Add(messageRun);

            TextBlock_Chat.Inlines.Add(new LineBreak());
        }
        public void FlipCard(int[] indexes)
        {
            var button = Grid_Pexeso.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == indexes[0] && Grid.GetColumn(e) == indexes[1]);
            ((Button)button).Content = PexesoGrid[indexes[0]][indexes[1]];
            _showedCards.Add(((Button)button));

        }
        private void MakeRows(int count)
        {

            Grid_Pexeso.RowDefinitions.Clear();

            RowDefinition r = new RowDefinition {Height = new GridLength(1, GridUnitType.Star)};
            Grid_Pexeso.RowDefinitions.Add(r);

            for (int j = 0; j < count - 1; j++)
            {
                RowDefinition c1 = new RowDefinition {Height = new GridLength(1, GridUnitType.Star)};
                Grid_Pexeso.RowDefinitions.Add(c1);
            }
        }

        private void MakeColumns(int count)
        {

            Grid_Pexeso.ColumnDefinitions.Clear();

            ColumnDefinition c = new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)};
            Grid_Pexeso.ColumnDefinitions.Add(c);

            for (int j = 0; j < count - 1; j++)
            {
                ColumnDefinition c1 = new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)};
                Grid_Pexeso.ColumnDefinitions.Add(c1);
            }
        }

        private string[] _pexesoValues = {"Slon", "Godzila", "Tarantula", "Opica",
            "Anakonda", "Slimák", "Komár", "Leňochod", "T-Rex","Holub","Vretenica","Tučniak","Zebra","Cikáda",
            "Aligátor","Antilopa","Bažant","Bzdocha","Mäsiarka","Čajka","Kapor","Delfín","Dingo","Dikobraz","Ďateľ",
            "Fretka","Gepard","Gibon","Harpya","Krab","Hyena","Hus","Kačka"

        };

        public void ResetPexeso()
        {
            ChallengerPoints = 0;
            CompetitorPoints = 0;
            CanUserClick = false;
            Dispatcher.Invoke(() =>
            {
                return TextBlock_Chat.Text = "";
            });

        }
        private void MakePexeso()
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                Grid_Pexeso.Children.Clear();
                MakeRows(PexesoGrid.Length);
                MakeColumns(PexesoGrid[0].Length);

                for (int i = 0; i < PexesoGrid.Length; i++)
                {
                    for (int j = 0; j < PexesoGrid[0].Length; j++)
                    {
                        Button button = new Button();
                        button.Click += Button_Click;

                        button.SetValue(Grid.ColumnProperty, j);
                        button.SetValue(Grid.RowProperty, i);

                        Grid_Pexeso.Children.Add(button);
                    }
                }
            });
        }

        private Random random = new Random();

        private class Card
        {
            public Card()
            {
                Left = 2;
            }

            public string Value { get; set; }
            public int Left { get; set; }
        }
        public void MakePexesoGrid(int rows, int colums)
        {
            PexesoGrid = new string[rows][];
            List<Card> cards = (from x in _pexesoValues select new Card { Value = x }).Take(rows * colums / 2).ToList();

            for (int i = 0; i < rows; i++)
            {
                PexesoGrid[i] = new string[colums];
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {

                    Card card = cards[random.Next(0, cards.Count)];
                    PexesoGrid[i][j] = card.Value;

                    card.Left--;
                    if (card.Left == 0)
                        cards.Remove(card);
                }
            }
        }

        public string[][] MakePexesoGrid(GameSize gameSize)
        {
            GameSize = gameSize;
            switch (gameSize)
            {
                case GameSize.size3x2:
                    MakePexesoGrid(3, 2);
                    break;
                case GameSize.size4x3:
                    MakePexesoGrid(4, 3);
                    break;
                case GameSize.size4x4:
                    MakePexesoGrid(4, 4);
                    break;
                case GameSize.size5x4:
                    MakePexesoGrid(5, 4);
                    break;
                case GameSize.size6x5:
                    MakePexesoGrid(6, 5);
                    break;
                case GameSize.size6x6:
                    MakePexesoGrid(6, 6);
                    break;
                case GameSize.size7x6:
                    MakePexesoGrid(7, 6);
                    break;
                case GameSize.size8x7:
                    MakePexesoGrid(8, 8);
                    break;
                case GameSize.size8x8:
                    MakePexesoGrid(8, 8);
                    break;
            }

            return PexesoGrid;
        }

        public void WaitFailed(int count)
        {
            Task.Factory.StartNew(() =>
            {
                CanUserClick = false;

                Thread.Sleep(count);
                _mainWindow.Dispatcher.Invoke(() =>
                {
                    _showedCards[0].Content = "";
                    _showedCards[1].Content = "";

                    int row = (int)(_showedCards[0]).GetValue(Grid.RowProperty);
                    int column = (int)(_showedCards[0]).GetValue(Grid.ColumnProperty);

                    int row2 = (int)(_showedCards[1]).GetValue(Grid.RowProperty);
                    int column2 = (int)(_showedCards[1]).GetValue(Grid.ColumnProperty);

                    _mainWindow._pexesoServiceClient.HideCard(Competitor, new int[] { row, column });
                    _mainWindow._pexesoServiceClient.HideCard(Competitor, new int[] { row2, column2 });
                    _mainWindow._pexesoServiceClient.ChangeTurn(Competitor);
                });

                _showedCards.Clear();
            });
        }

        public void WaitSucess(int count)
        {
            Task.Factory.StartNew(() =>
            {
                CanUserClick = false;

                Thread.Sleep(count);
                _mainWindow.Dispatcher.Invoke(() =>
                {
                    int row = (int)(_showedCards[0]).GetValue(Grid.RowProperty);
                    int column = (int)(_showedCards[0]).GetValue(Grid.ColumnProperty);

                    int row2 = (int)(_showedCards[1]).GetValue(Grid.RowProperty);
                    int column2 = (int)(_showedCards[1]).GetValue(Grid.ColumnProperty);

                    _mainWindow._pexesoServiceClient.RemoveCard(Competitor, new int[] { row, column });
                    _mainWindow._pexesoServiceClient.RemoveCard(Competitor, new int[] { row2, column2 });

                    Grid_Pexeso.Children.Remove(_showedCards[1]);
                    Grid_Pexeso.Children.Remove(_showedCards[0]);


                    if (Grid_Pexeso.Children?.Count == 0)
                    {
                        game.GameTime = DateTime.Now - startGame;
                        game.CountOfUncoveredCardsChallenger = ChallengerPoints;
                        game.CountOfUncoveredCardsCompetitor = CompetitorPoints;

                        if (ChallengerPoints > CompetitorPoints)
                            game.Winner = game.NickOfChallenger;
                        else if (CompetitorPoints < ChallengerPoints)
                            game.Winner = game.NickOfCompetitor;
                        else
                            game.Winner = "Draw";

                        _mainWindow._pexesoServiceClient.EndGame(Competitor, game);
                        _mainWindow.EndGame(game);
                    }
                });

                _showedCards.Clear();

                CanUserClick = true;


            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CanUserClick)
            {
                if (_showedCards.Count == 0 || ((Button)sender) != _showedCards[0])
                {
                    int row = (int)((Button)sender).GetValue(Grid.RowProperty);
                    int column = (int)((Button)sender).GetValue(Grid.ColumnProperty);
                    ((Button)sender).Content = PexesoGrid[row][column];
                    _showedCards.Add((Button)sender);

                    _mainWindow._pexesoServiceClient.FlipCard(Competitor, new int[] { row, column });

                    game.CountOfMovesChallenger++;

                    if (_showedCards.Count > 1)
                    {
                        string cardOne = _showedCards[0].Content.ToString();
                        string cardTwo = _showedCards[1].Content.ToString();
                        if (cardOne == cardTwo)
                        {
                            ChallengerPoints++;
                            _mainWindow._pexesoServiceClient.AddPoint(Competitor);
                            WaitSucess(1000);

                        }
                        else
                        {
                            WaitFailed(2000);
                            CanUserClick = false;
                        }
                    }
                }
            }
        }

        public void HideCard(int[] indexes)
        {
            var button = Grid_Pexeso.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == indexes[0] && Grid.GetColumn(e) == indexes[1]);
            ((Button)button).Content = "";
            _showedCards.Remove(((Button)button));

            game.CountOfMovesCompetitor++;
        }

        public void RemoveCard(int[] indexes)
        {
            var button = Grid_Pexeso.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == indexes[0] && Grid.GetColumn(e) == indexes[1]);
            Grid_Pexeso.Children.Remove(button);
            _showedCards.Remove(((Button)button));
        }

        public void ShowPexeso(GameSize gameSize, string[][] pexesoGrid, string competitor)
        {
            ResetPexeso();
            Challenger = _mainWindow._pexesoPlayer.NickName;
            Competitor = competitor;
            PexesoGrid = pexesoGrid;

            game = new Game {GameSize = gameSize, NickOfChallenger = Challenger, NickOfCompetitor = Competitor};

            startGame = DateTime.Now;

            switch (gameSize)
            {
                case GameSize.size3x2:
                    MakePexeso();
                    break;
                case GameSize.size4x3:
                    MakePexeso();
                    break;
                case GameSize.size4x4:
                    MakePexeso();
                    break;
                case GameSize.size5x4:
                    MakePexeso();
                    break;
                case GameSize.size6x5:
                    MakePexeso();
                    break;
                case GameSize.size6x6:
                    MakePexeso();
                    break;
                case GameSize.size7x6:
                    MakePexeso();
                    break;
                case GameSize.size8x7:
                    MakePexeso();
                    break;
                case GameSize.size8x8:
                    MakePexeso();
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            AddMessageMyself(TextBox_Message.Text);
            _mainWindow._pexesoServiceClient.SendMessage(competitor, TextBox_Message.Text);
        }

        private void TextBox_Message_KeyDown(object sender, KeyEventArgs e)
        {

            if (Key.Enter == e.Key)
            {
                AddMessageMyself(TextBox_Message.Text);
                _mainWindow._pexesoServiceClient.SendMessage(competitor, TextBox_Message.Text);
            }
        }
    }
}
