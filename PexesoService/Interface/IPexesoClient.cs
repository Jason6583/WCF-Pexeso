using System.ServiceModel;
using PexesoDatabase;

namespace PexesoService.Interfaces
{
    public interface IPexesoClient
    {
        [OperationContract]
        bool GetInvitation(string nickName, string[][] pexesoGrid, GameSize gamesize);
        [OperationContract]
        void CardFlipped(int[] indexes);

        [OperationContract]
        void HidedCard(int[] indexes);
        [OperationContract]
        void RemovedCard(int[] indexes);

        [OperationContract]
        void CompetitorGotPoint();

        [OperationContract]
        void MyTurn();

        [OperationContract]
        void GameEnded(Game game);

        [OperationContract]
        void GameEndedByTime();

        [OperationContract]
        void GotMessage(string message);


    }
}
