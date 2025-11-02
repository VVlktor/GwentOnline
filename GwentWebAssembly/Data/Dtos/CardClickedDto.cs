using GwentWebAssembly.Pages;

namespace GwentWebAssembly.Data.Dtos
{
    public class CardClickedDto : BaseClickedDto
    {
        public GwentBoardCard ClickedCard { get; set; }
        public GwentCard SelectedCard { get; set; }
    }
}
