window.runCardAnimation = function (targetId) {
    return new Promise((resolve) => {
        const card = document.getElementById("card-animation");
        const start = document.getElementById("enemy-deck-count");
        const target = document.getElementById(targetId);

        if (!card || !start || !target) {
            resolve();
            return;
        }

        const startRect = start.getBoundingClientRect();
        const targetRect = target.getBoundingClientRect();

        // Reset
        card.style.transition = "none";
        card.style.display = "block";
        card.style.left = (startRect.left + startRect.width / 2 - card.offsetWidth / 2) + "px";
        card.style.top = (startRect.top + startRect.height / 2 - card.offsetHeight / 2) + "px";

        // Force reflow
        void card.offsetWidth;

        // Start animacji
        card.style.transition = "top 1s ease-in-out, left 1s ease-in-out";
        card.style.left = (targetRect.left + targetRect.width / 2 - card.offsetWidth / 2) + "px";
        card.style.top = (targetRect.top + targetRect.height / 2 - card.offsetHeight / 2) + "px";

        // Obsłuż tylko JEDEN raz
        const handler = () => {
            card.style.display = "none";
            card.style.transition = "none";
            card.style.left = "0px";
            card.style.top = "0px";

            card.removeEventListener("transitionend", handler);
            resolve(); // sygnał końca
        };

        card.addEventListener("transitionend", handler, { once: true });
    });
};
