window.runCardAnimation = function (startId, targetId) {
    const start = document.getElementById(startId);
    const target = document.getElementById(targetId);

    if (!start || !target) return;

    const cardWrapper = document.createElement("div");
    cardWrapper.classList.add("card-animation-wrapper");

    const cardInner = document.createElement("div");
    cardInner.classList.add("card-animation-inner");
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

window.showOverlay = function (message) {
    const overlay = document.createElement("div");
    overlay.className = "main-black-gwent-overlay";
    overlay.textContent = message;

    document.body.appendChild(overlay);

    requestAnimationFrame(() => {
        overlay.style.opacity = "1";
    });

    setTimeout(() => {
        overlay.style.opacity = "0";
    }, 1500);

    setTimeout(() => {
        overlay.remove();
    }, 2000);
};
