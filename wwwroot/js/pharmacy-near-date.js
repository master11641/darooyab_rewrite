(() => {
    const form = document.querySelector("[data-near-date-filter]");
    const provinceSelect = document.querySelector("[data-province-select]");
    const townshipSelect = document.querySelector("[data-township-select]");
    const status = document.querySelector("[data-township-status]");
    const retryButton = document.querySelector("[data-township-retry]");

    if (!(form instanceof HTMLFormElement) ||
        !(provinceSelect instanceof HTMLSelectElement) ||
        !(townshipSelect instanceof HTMLSelectElement) ||
        !(status instanceof HTMLElement) ||
        !(retryButton instanceof HTMLButtonElement)) {
        return;
    }

    let activeRequest;

    const loadTownships = async () => {
        activeRequest?.abort();
        const provinceId = Number(provinceSelect.value);
        townshipSelect.replaceChildren(new Option("در حال دریافت شهرها…", ""));
        townshipSelect.disabled = true;
        status.textContent = "";
        retryButton.hidden = true;

        if (!Number.isInteger(provinceId) || provinceId <= 0) {
            townshipSelect.replaceChildren(new Option("همه شهرها", "0"));
            townshipSelect.disabled = true;
            return;
        }

        activeRequest = new AbortController();
        try {
            const response = await fetch(
                `/DrugShortage/Townships?provinceId=${encodeURIComponent(provinceId)}`,
                {
                    headers: { Accept: "application/json" },
                    signal: activeRequest.signal
                });
            if (!response.ok) {
                throw new Error("Townships request failed.");
            }

            const townships = await response.json();
            if (!Array.isArray(townships)) {
                throw new Error("Invalid townships response.");
            }

            const options = [new Option("همه شهرها", "0")];
            for (const township of townships) {
                if (Number.isInteger(township.id) && typeof township.name === "string") {
                    options.push(new Option(township.name, String(township.id)));
                }
            }
            townshipSelect.replaceChildren(...options);
            townshipSelect.disabled = false;
        } catch (error) {
            if (error instanceof DOMException && error.name === "AbortError") {
                return;
            }

            townshipSelect.replaceChildren(new Option("شهرها در دسترس نیستند", ""));
            status.textContent = "دریافت شهرها ممکن نشد؛ فیلتر استان همچنان قابل جستجو است.";
            retryButton.hidden = false;
        }
    };

    provinceSelect.addEventListener("change", loadTownships);
    retryButton.addEventListener("click", loadTownships);
    form.addEventListener("submit", () => {
        const townshipOption = townshipSelect.selectedOptions[0];
        if (townshipSelect.disabled || !townshipOption || townshipOption.value === "") {
            townshipSelect.disabled = true;
        }
    });
})();
