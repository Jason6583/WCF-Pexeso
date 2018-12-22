using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using PexesoDatabase;

namespace PexesoService.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IPexesoClient))]
    public interface IPexesoService
    {
        [OperationContract]
        List<string> GetListOfPlayers();

        [OperationContract]
        string[][] CreateGame(GameSize gameSize, string[][] pexesoGrid, string NickOfCreator, string NickOfCompetitor);

        [OperationContract]
        PexesoPlayer AddPlayer(string nickName, string password);

        [OperationContract]
        PexesoPlayer LoginPlayer(string nickName, string password);

        [OperationContract]
        void FlipCard(string nickOfCompetitor, int[] indexes);

        [OperationContract]
        void AddPoint(string nickOfCompetitor);

        [OperationContract]
        void HideCard(string nickOfCompetitor, int[] indexes);

        [OperationContract]
        void RemoveCard(string nickOfCompetitor, int[] indexes);

        [OperationContract]
        void ChangeTurn(string nickOfCompetitor);

        [OperationContract]
        void EndGame(string nickOfCompetitor, Game game);

        [OperationContract]
        void EndGameByTime(string nickOfCompetitor);
        [OperationContract]
        void SendMessage(string nickOfCompetitor, string message);

        [OperationContract]
        void Disconnect(string name);
    }

    [DataContract]
    public class PexesoPlayer
    {
        public PexesoPlayer(PexesoDatabase.PexesoPlayer pexesoPlayer)
        {
            NickName = pexesoPlayer.NickName;
            Password = pexesoPlayer.Password;
        }

        [DataMember]
        public string NickName { get; set; }
        [DataMember]
        public string Password { get; set; }
    }

    [DataContract]
    public class Game
    {
        public Game()
        {

        }

        [DataMember]
        public GameSize GameSize { get; set; }
        [DataMember]
        public int CountOfMovesCompetitor { get; set; }
        [DataMember]
        public int CountOfUncoveredCardsCompetitor { get; set; }
        [DataMember]
        public int CountOfUncoveredCardsChallenger { get; set; }
        [DataMember]
        public int CountOfMovesChallenger { get; set; }
        [DataMember]
        public TimeSpan GameTime { get; set; }
        [DataMember]
        public string NickOfChallenger { get; set; }
        [DataMember]
        public string Winner { get; set; }
        [DataMember]
        public string NickOfCompetitor { get; set; }
    }
}
