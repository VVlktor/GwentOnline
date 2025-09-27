namespace GwentApi.Classes.Dtos
{
    public class WeatherClickedDto
    {
        public GwentCard Card { get; set; }
        public PlayerIdentity Identity { get; set; }
        public string Code { get; set; }
    }
}
