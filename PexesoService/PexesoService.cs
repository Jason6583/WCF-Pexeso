using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using PexesoDatabase;
using PexesoService.Interfaces;
using PexesoPlayer = PexesoService.Interfaces.PexesoPlayer;
using Game = PexesoService.Interfaces.Game;

namespace PexesoService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class PexesoService : IPexesoService
    {
        private PexesoContext _pexesoContext = new PexesoContext();
        private Dictionary<string, IPexesoClient> _pexesoPlayers = new Dictionary<string, IPexesoClient>();
        private List<string> _playersInGame = new List<string>();

        public List<string> GetListOfPlayers()
        {
            List<string> inActvivePlayers = new List<string>();
            foreach (var player in _pexesoPlayers.Keys)
            {
                if (!_playersInGame.Contains(player))
                    inActvivePlayers.Add(player);
            }
            return inActvivePlayers;
        }

        public string[][] CreateGame(GameSize gameSize, string[][] pexesoGrid, string nickOfCreator, string nickOfCompetitor)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);

                if (competitor == null)
                    return null;

                var result = competitor.GetInvitation(nickOfCreator, pexesoGrid, gameSize);

                if (result)
                {
                    _playersInGame.Add(nickOfCreator);
                    _playersInGame.Add(nickOfCompetitor);
                }
                else
                    pexesoGrid = null;

                return pexesoGrid;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public void FlipCard(string nickOfCompetitor, int[] indexes)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);
                competitor.CardFlipped(indexes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AddPoint(string nickOfCompetitor)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);

                competitor.CompetitorGotPoint();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void HideCard(string nickOfCompetitor, int[] indexes)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);

                competitor.HidedCard(indexes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveCard(string nickOfCompetitor, int[] indexes)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);

                competitor.RemovedCard(indexes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ChangeTurn(string nickOfCompetitor)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);

                competitor.MyTurn();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string FindNick(IPexesoClient pexesoClient)
        {
            foreach (var item in _pexesoPlayers)
            {
                if (item.Value == pexesoClient)
                    return item.Key;
            }

            return "";
        }
        public void EndGame(string nickOfCompetitor, Game game)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);
                competitor.GameEnded(game);

                _playersInGame.Remove(FindNick(OperationContext.Current.GetCallbackChannel<IPexesoClient>()));
                _playersInGame.Remove(nickOfCompetitor);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void EndGameByTime(string nickOfCompetitor)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);
                competitor.GameEndedByTime();

                _playersInGame.Remove(FindNick(OperationContext.Current.GetCallbackChannel<IPexesoClient>()));
                _playersInGame.Remove(nickOfCompetitor);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendMessage(string nickOfCompetitor, string message)
        {
            try
            {
                _pexesoPlayers.TryGetValue(nickOfCompetitor, out var competitor);
                competitor.GotMessage(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Disconnect(string name)
        {
            _pexesoPlayers.Remove(name);
        }

        public PexesoPlayer AddPlayer(string nickName, string password)
        {
            try
            {
                PexesoDatabase.PexesoPlayer player = new PexesoDatabase.PexesoPlayer(nickName, password);

                _pexesoContext.PexesoPlayers.Add(player);
                _pexesoContext.SaveChanges();

                return new PexesoPlayer(player);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public PexesoPlayer LoginPlayer(string nickName, string password)
        {
            PexesoDatabase.PexesoPlayer pexesoPlayer = _pexesoContext.PexesoPlayers.SingleOrDefault(x => x.NickName == nickName);
            var conn = OperationContext.Current.GetCallbackChannel<IPexesoClient>();

            if (pexesoPlayer != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, pexesoPlayer.Password))
                {
                    var wcfPexesoPlayer = new PexesoPlayer(pexesoPlayer);
                    _pexesoPlayers[pexesoPlayer.NickName] = conn;

                    return wcfPexesoPlayer;
                }
                else
                    return null;
            }
            else
                return null;
        }
    }
}
