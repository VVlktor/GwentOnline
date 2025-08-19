namespace GwentApi.Classes
{
    public class Game
    {
        public DateTime LastMove = DateTime.Now;
        (bool playerOne, bool playerTwo) IsReady = (false, false);
    }
}
