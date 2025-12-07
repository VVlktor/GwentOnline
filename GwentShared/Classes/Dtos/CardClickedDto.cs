namespace GwentShared.Classes.Dtos;

public class CardClickedDto : BaseClickedDto
{
    public GwentBoardCard ClickedCard { get; set; }
    public GwentCard SelectedCard { get; set; }
}
