using GwentShared.Classes;

namespace GwentWebAssembly.Data
{
    public class CarouselSlot
    {
        public int Offset { get; set; }

        public int? Index { get; set; }

        public GwentCard? Item { get; set; }

        public bool IsEmpty { get; set; }
    }
}
