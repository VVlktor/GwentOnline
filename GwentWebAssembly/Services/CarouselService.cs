using GwentWebAssembly.Data;
using GwentWebAssembly.Services.Interfaces;

namespace GwentWebAssembly.Services
{
    public class CarouselService : ICarouselService
    {
        public bool IsCarouselShown { get; set; } = false;

        private List<GwentCard> _items = new();

        public List<CarouselSlot> CarouselSlots { get; set; } = new();

        private int CurrentIndex = 0;

        public void NextCard(int offset)
        {
            if (_items.Count == 0)
                return;

            int newIndex = CurrentIndex + offset;
            if (newIndex < 0 || newIndex >= _items.Count)
                return;

            CurrentIndex = newIndex;
        }

        public List<CarouselSlot> GetSlots()
        {
            var slots = new List<CarouselSlot>(5);

            for (int offset = -2; offset <= 2; offset++)
            {
                int index = CurrentIndex + offset;

                if (index >= 0 && index < _items.Count)
                {
                    slots.Add(new CarouselSlot()
                    {
                        Offset = offset,
                        Index = index,
                        Item = _items[index],
                        IsEmpty = false
                    });
                }
                else
                {
                    slots.Add(new CarouselSlot()
                    {
                        Offset = offset,
                        Index = null,
                        Item = default,
                        IsEmpty = true
                    });
                }
            }

            return slots;
        }

        public void ShowCarousel(List<GwentCard> playerUsedCards)
        {
            var cardsForCarousel = playerUsedCards.Where(x => !x.Abilities.HasFlag(Abilities.Hero) && x.Placement != TroopPlacement.Weather && x.Placement != TroopPlacement.Special && x.CardId!=2 && x.CardId != 6);
            if (cardsForCarousel.Count() == 0) return;
            _items = cardsForCarousel.ToList();
            CarouselSlots = GetSlots();
            IsCarouselShown = true;
        }

        public void OnCardClick(CarouselSlot slot)
        {
            if (slot.Offset != 0)
            {
                NextCard(slot.Offset);
                CarouselSlots = GetSlots();
            }
        }

        public void HideCarousel() => IsCarouselShown = false;
    }
}
