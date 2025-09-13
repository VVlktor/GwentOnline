namespace GwentWebAssembly.Data.Dtos
{
    public class LaneClickedDto
    {
        public PlayerIdentity Identity {  get; set; }
        public string Code { get; set; }
        public GwentLane Lane { get; set; }
        public GwentCard Card {  get; set; }
    }
}
