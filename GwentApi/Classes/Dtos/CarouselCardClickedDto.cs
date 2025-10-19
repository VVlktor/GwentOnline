namespace GwentApi.Classes.Dtos
{
    public class CarouselCardClickedDto
    {
        public PlayerIdentity Identity { get; set; }
        public string Code { get; set; }
        public GwentCard Card { get; set; }
    }
}
