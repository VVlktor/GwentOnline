window.showOverlay = function (message) {
    const overlay = document.createElement("div");
    overlay.className = "main-black-gwent-overlay";
    overlay.style.fontSize = "48px";
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

async function moveCardByElementIds(cardElemId, destElemId, cardData) {
    const startElem = document.getElementById(cardElemId);
    const destElem = document.getElementById(destElemId);
    const overlay = document.getElementById("card-overlay");
    if (!startElem || !destElem || !overlay) return false;

    const { image } = cardData;

    const card = document.createElement("div");
    card.className = "card noclick";
    card.style.backgroundImage = `url('${image}')`;
    card.style.height = "6.35vw";
    card.style.width = "4.45vw";
    card.style.position = "absolute";
    card.style.transition = "transform 0.8s ease-in-out";
    card.style.zIndex = "99999";
    card.style.pointerEvents = "none";

    overlay.appendChild(card);

    const startRect = startElem.getBoundingClientRect();
    const endRect = destElem.getBoundingClientRect();

    const startCenterX = startRect.left + startRect.width / 2;
    const startCenterY = startRect.top + startRect.height / 2;
    const destCenterX = endRect.left + endRect.width / 2;
    const destCenterY = endRect.top + endRect.height / 2;

    card.style.left = `${startCenterX - card.offsetWidth / 2}px`;
    card.style.top = `${startCenterY - card.offsetHeight / 2}px`;

    const dx = destCenterX - startCenterX;
    const dy = destCenterY - startCenterY;

    requestAnimationFrame(() => {
        card.style.transform = `translate(${dx}px, ${dy}px)`;
    });

    await new Promise(r => setTimeout(r, 800));
    overlay.removeChild(card);
    return true;
}