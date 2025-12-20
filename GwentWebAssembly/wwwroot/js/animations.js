async function showOverlay(message) {
    return new Promise(resolve => {
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
            resolve();
        }, 2000);
    });
};

async function showScorchAnimation(listOfIds) {
    const elements = listOfIds.map(id => document.querySelector(`#${id} > div:nth-child(4)`));

    elements.forEach(el => {
        el.classList.remove("hide");
        el.style.backgroundSize = "cover";
        el.style.backgroundImage = "url('/img/icons/anim_scorch.png')";
        el.style.opacity = 0;
        el.style.transition = "opacity 0.5s";
    });

    await new Promise(r => setTimeout(r, 10));

    elements.forEach(el => el.style.opacity = 1);
    await new Promise(r => setTimeout(r, 750));

    elements.forEach(el => el.style.opacity = 0);
    await new Promise(r => setTimeout(r, 750));

    return true;
}

async function showEndGameOverlay(message) {
    const overlay = document.createElement("div");
    overlay.className = "main-black-gwent-overlay";
    overlay.style.fontSize = "48px";
    overlay.textContent = message;
    overlay.style.opacity = "1";

    document.body.appendChild(overlay);
}

async function moveCardByElementIdsWithInfo(cardElemId, destElemId, data) {
    const startElem = document.getElementById(cardElemId);
    const destElem = document.getElementById(destElemId);
    const overlay = document.getElementById("card-overlay");

    if (!startElem || !destElem || !overlay) return false;

    const card = CreateCard(data);

    await moveCard(startElem, destElem, overlay, card);

    return true;
}

async function moveCard(startElem, destElem, overlay, card) {
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

async function moveCardByElementIdsNoInfo(cardElemId, destElemId, cardImage) {
    const startElem = document.getElementById(cardElemId);
    const destElem = document.getElementById(destElemId);
    const overlay = document.getElementById("card-overlay");

    if (!startElem || !destElem || !overlay) return false;

    const card = document.createElement("div");
    card.className = "card noclick";
    card.style.backgroundImage = `url('${cardImage}')`;
    card.style.backgroundSize = "cover";
    card.style.backgroundPosition = "center";
    card.style.height = "6.35vw";
    card.style.width = "4.45vw";
    card.style.position = "absolute";
    card.style.transition = "transform 0.8s ease-in-out";
    card.style.zIndex = "99999";
    card.style.pointerEvents = "none";

    await moveCard(startElem, destElem, overlay, card);

    return true;
}

function CreateCard(data) {
    const card = document.createElement("div");
    card.className = "card noclick";
    card.style.backgroundImage = `url('${data.imagePath}')`;
    card.style.backgroundSize = "cover";
    card.style.backgroundPosition = "center";
    card.style.height = "6.35vw";
    card.style.width = "4.45vw";
    card.style.position = "absolute";
    card.style.transition = "transform 0.8s ease-in-out";
    card.style.zIndex = "99999";
    card.style.pointerEvents = "none";

    const div1 = document.createElement("div");
    div1.style.backgroundImage = `url('img/icons/power_normal.png')`
    if (data.isHero) {
        div1.style.backgroundImage = `url('img/icons/power_hero.png')`
    }

    const div2 = document.createElement("div");
    div2.className = "center";
    div2.textContent = `${data.strength}`;

    const div3 = document.createElement("div");
    div3.style.backgroundImage = `url('img/icons/card_row_${data.placementName}.png')`;

    const div4 = document.createElement("div");
    div4.style.backgroundImage = `url('img/icons/card_ability_${data.abilityName}.png')`;

    const div5 = document.createElement("div");
    div5.backgroundSize = "40%";
    div5.className = "hide";

    card.appendChild(div1);
    div1.appendChild(div2);
    card.appendChild(div3);
    card.appendChild(div4);
    card.appendChild(div5);

    return card;
}

function resizeCardContainer(containerId, overlap_count, gap, coef, cardCount) {
    let param = (cardCount < overlap_count) ? "" + gap + "vw" : defineCardRowMargin(cardCount, coef);
    let children = document.getElementById(containerId).children;
    for (let x of children)
        x.style.marginLeft = x.style.marginRight = param;

    function defineCardRowMargin(n, coef = 0) {
        return "calc((100% - (4.45vw * " + n + ")) / (2*" + n + ") - (" + coef + "vw * " + n + "))";
    }
}