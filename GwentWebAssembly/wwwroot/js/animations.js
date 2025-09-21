window.runCardAnimation = function (startId, targetId) {
    const start = document.getElementById(startId); //"player-cards-in-hand" lub "enemy-deck-count"
    const target = document.getElementById(targetId);

    if (!start || !target) return;

    const cardWrapper = document.createElement("div");
    cardWrapper.style.width = "84px";
    cardWrapper.style.height = "110px";
    cardWrapper.style.position = "fixed";
    cardWrapper.style.zIndex = 1000;
    cardWrapper.style.transition = "top 1s ease-in-out, left 1s ease-in-out";

    const cardInner = document.createElement("div");
    cardInner.style.width = "100%";
    cardInner.style.height = "100%";
    cardInner.style.borderRadius = "8px";
    cardInner.style.background = "linear-gradient(145deg, #222, #111)";
    cardWrapper.appendChild(cardInner);

    document.getElementById("gwent-board").appendChild(cardWrapper);

    const startRect = start.getBoundingClientRect();
    cardWrapper.style.left = (startRect.left + startRect.width / 2 - 42) + "px";
    cardWrapper.style.top = (startRect.top + startRect.height / 2 - 58) + "px";

    void cardWrapper.offsetWidth;

    const targetRect = target.getBoundingClientRect();
    cardWrapper.style.left = (targetRect.left + targetRect.width / 2 - 42) + "px";
    cardWrapper.style.top = (targetRect.top + targetRect.height / 2 - 58) + "px";

    cardWrapper.addEventListener("transitionend", function handler() {
        cardWrapper.removeEventListener("transitionend", handler);
        document.getElementById("gwent-board").removeChild(cardWrapper);
    }, { once: true });
};