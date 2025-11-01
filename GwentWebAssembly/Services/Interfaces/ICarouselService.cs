using GwentWebAssembly.Data;

namespace GwentWebAssembly.Services.Interfaces
{
    public interface ICarouselService
    {
        void NextCard(int offset);
        List<CarouselSlot> GetSlots();
        void ShowCarousel(List<GwentCard> cards);
        bool IsCarouselShown { get; }
        List<CarouselSlot> CarouselSlots { get; }
        void OnCardClick(CarouselSlot slot);
        void HideCarousel();
    }
}
