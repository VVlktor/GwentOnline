namespace GwentApi.Classes.Dtos
{
    public class CardClickedDto
    {
        public PlayerIdentity Identity { get; set; }
        public string Code { get; set; }
        public GwentBoardCard ClickedCard { get; set; }
        public GwentCard SelectedCard { get; set; }
    }
}
