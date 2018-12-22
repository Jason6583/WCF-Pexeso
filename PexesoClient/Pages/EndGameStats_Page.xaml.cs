using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Game = PexesoService.Interfaces.Game;


namespace PexesoClient.Pages
{
    /// <summary>
    /// Interaction logic for EndGameStats.xaml
    /// </summary>
    public partial class EndGameStats_Page : Page, INotifyPropertyChanged
    {
        Game _game;
        public EndGameStats_Page(Game game)
        {
            InitializeComponent();
            DataContext = this;
            Game = game;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public Game Game
        {
            get { return _game; }
            set
            {
                _game = value;
                OnPropertyChanged("Game");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
