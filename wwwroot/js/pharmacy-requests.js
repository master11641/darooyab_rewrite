(() => {
    const form = document.querySelector("[data-pharmacy-request-form]");
    if (!form) {
        return;
    }

    const searchInput = form.querySelector("#brand-search");
    const results = form.querySelector("#brand-search-results");
    const status = form.querySelector("[data-search-status]");
    const selectedBrands = form.querySelector("[data-selected-brands]");
    const hiddenInputs = form.querySelector("[data-selected-irc-inputs]");
    const detailsContainer = form.querySelector("[data-brand-details-container]");
    const submitButton = form.querySelector("[data-submit-label]");
    const selected = new Map();
    const isEditMode = Boolean(form.querySelector('input[name="Form.Id"]')?.value);
    let timer;
    let activeRequest;
    let activeDetailsRequest;

    const normalizePrice = (value) => value
        .replace(/[۰-۹]/g, digit => String(digit.charCodeAt(0) - 1776))
        .replace(/[٠-٩]/g, digit => String(digit.charCodeAt(0) - 1632))
        .replace(/\D/g, "");

    const formatPrice = (value) => value.replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    const syncBrandPrice = (input, preserveCaret = false) => {
        const hiddenInput = detailsContainer?.querySelector("[data-brand-price-value]");
        if (!hiddenInput) {
            return;
        }

        const caret = preserveCaret ? input.selectionStart : null;
        const digitsBeforeCaret = caret === null
            ? 0
            : normalizePrice(input.value.slice(0, caret)).length;
        const rawValue = normalizePrice(input.value);
        const formattedValue = formatPrice(rawValue);
        hiddenInput.value = rawValue;

        if (input.value !== formattedValue) {
            input.value = formattedValue;
            if (caret !== null) {
                let newCaret = 0;
                let digitCount = 0;
                while (newCaret < formattedValue.length && digitCount < digitsBeforeCaret) {
                    if (/\d/.test(formattedValue[newCaret])) {
                        digitCount++;
                    }
                    newCaret++;
                }
                input.setSelectionRange(newCaret, newCaret);
            }
        }
    };

    const syncHiddenInputs = () => {
        hiddenInputs.replaceChildren();
        for (const [irc] of selected) {
            const input = document.createElement("input");
            input.type = "hidden";
            input.name = "Form.SelectedIrcs";
            input.value = irc;
            hiddenInputs.append(input);
        }
    };

    const updateSubmitLabel = () => {
        if (!submitButton) {
            return;
        }

        const baseLabel = submitButton.dataset.submitLabel;
        const isBuyingRank = detailsContainer?.querySelector("[data-buy-ranking]")?.checked === true;
        submitButton.textContent = isBuyingRank ? `${baseLabel} و خرید رتبه` : baseLabel;
    };

    const updateDetailsControls = () => {
        const rankDays = detailsContainer?.querySelector("[data-rank-days]");
        const rankTotal = detailsContainer?.querySelector("[data-rank-total]");
        if (rankDays && rankTotal) {
            const total = Number(rankDays.dataset.dailyPrice) * Number(rankDays.value);
            rankTotal.textContent = `${new Intl.NumberFormat("fa-IR").format(total)} تومان`;
        }

        const updatePrice = detailsContainer?.querySelector("[data-update-brand-price]");
        const priceFields = detailsContainer?.querySelector("[data-brand-price-fields]");
        if (updatePrice && priceFields) {
            priceFields.hidden = !updatePrice.checked;
        }

        const brandPrice = detailsContainer?.querySelector("[data-brand-price]");
        if (brandPrice) {
            syncBrandPrice(brandPrice);
        }

        updateSubmitLabel();
    };

    const refreshBrandDetails = async () => {
        activeDetailsRequest?.abort();
        if (!detailsContainer) {
            return;
        }

        if (selected.size !== 1) {
            detailsContainer.replaceChildren();
            if (selected.size > 1) {
                detailsContainer.textContent = "برای خرید رتبه یا تغییر قیمت، در هر بار فقط یک برند انتخاب کنید.";
            }
            updateSubmitLabel();
            return;
        }

        if (detailsContainer.querySelector("[data-brand-details]")?.dataset.irc === [...selected.keys()][0]) {
            updateDetailsControls();
            return;
        }

        detailsContainer.textContent = "در حال دریافت اطلاعات رتبه و آخرین قیمت برند…";
        activeDetailsRequest = new AbortController();
        try {
            const irc = [...selected.keys()][0];
            const url = new URL(
                `${form.dataset.brandDetailsUrl}/${encodeURIComponent(irc)}`,
                window.location.origin);
            const response = await fetch(url, {
                headers: { Accept: "text/html" },
                credentials: "same-origin",
                signal: activeDetailsRequest.signal
            });
            if (!response.ok) {
                throw new Error("Brand details could not be loaded.");
            }

            const markup = await response.text();
            if (selected.size !== 1 || !selected.has(irc)) {
                return;
            }

            detailsContainer.innerHTML = markup;
            const details = detailsContainer.querySelector("[data-brand-details]");
            if (details) {
                details.dataset.irc = irc;
                detailsContainer.setAttribute("aria-live", "off");
            } else {
                detailsContainer.textContent = "اطلاعات تکمیلی این برند در دسترس نیست.";
            }
            updateDetailsControls();
        } catch (error) {
            if (error.name !== "AbortError") {
                detailsContainer.textContent = "دریافت اطلاعات رتبه و قیمت برند انجام نشد؛ دوباره برند را انتخاب کنید.";
            }
        }
    };

    const addBrand = (brand) => {
        const irc = String(brand.irc);
        if (selected.has(irc)) {
            searchInput.value = "";
            results.hidden = true;
            return;
        }

        selected.set(irc, brand.name);
        const chip = document.createElement("span");
        chip.className = "pharmacy-request-chip";
        chip.dataset.irc = irc;

        const label = document.createElement("span");
        label.textContent = brand.name;
        chip.append(label);

        if (!isEditMode) {
            const remove = document.createElement("button");
            remove.type = "button";
            remove.dataset.removeBrand = "";
            remove.setAttribute("aria-label", `حذف ${brand.name}`);
            remove.textContent = "×";
            chip.append(remove);
        }

        selectedBrands.append(chip);
        syncHiddenInputs();
        refreshBrandDetails();
        searchInput.value = "";
        results.replaceChildren();
        results.hidden = true;
        searchInput.setAttribute("aria-expanded", "false");
        status.textContent = "";
    };

    for (const chip of selectedBrands.querySelectorAll("[data-irc]")) {
        const irc = chip.dataset.irc;
        const name = chip.querySelector("span")?.textContent ?? "";
        if (irc) {
            selected.set(irc, name);
        }
    }
    syncHiddenInputs();
    if (detailsContainer?.querySelector("[data-brand-details]") && selected.size === 1) {
        detailsContainer.querySelector("[data-brand-details]").dataset.irc = [...selected.keys()][0];
    }
    updateDetailsControls();
    if (selected.size === 1 && !detailsContainer?.querySelector("[data-brand-details]")) {
        refreshBrandDetails();
    }

    const closeResults = () => {
        results.hidden = true;
        searchInput.setAttribute("aria-expanded", "false");
    };

    const renderResults = (brands) => {
        results.replaceChildren();
        if (brands.length === 0) {
            status.textContent = "برندی با این عبارت پیدا نشد.";
            closeResults();
            return;
        }

        for (const brand of brands) {
            const item = document.createElement("li");
            item.setAttribute("role", "option");
            item.setAttribute("aria-selected", "false");
            const button = document.createElement("button");
            button.type = "button";
            button.dataset.irc = String(brand.irc);
            button.textContent = brand.name;
            button.addEventListener("click", () => addBrand(brand));
            item.append(button);
            results.append(item);
        }

        results.hidden = false;
        searchInput.setAttribute("aria-expanded", "true");
        status.textContent = `${brands.length} نتیجه پیدا شد.`;
    };

    searchInput.addEventListener("input", () => {
        window.clearTimeout(timer);
        activeRequest?.abort();
        results.replaceChildren();
        const query = searchInput.value.trim();
        if (query.length < 2) {
            closeResults();
            status.textContent = query.length === 0 ? "" : "برای جستجو حداقل دو نویسه وارد کنید.";
            return;
        }

        status.textContent = "در حال جستجو…";
        timer = window.setTimeout(async () => {
            activeRequest = new AbortController();
            try {
                const url = new URL(searchInput.dataset.searchUrl, window.location.origin);
                url.searchParams.set("query", query);
                const response = await fetch(url, {
                    headers: { Accept: "application/json" },
                    credentials: "same-origin",
                    signal: activeRequest.signal
                });
                if (!response.ok) {
                    throw new Error("Brand search failed.");
                }

                renderResults(await response.json());
            } catch (error) {
                if (error.name !== "AbortError") {
                    status.textContent = "جستجوی برند انجام نشد. دوباره تلاش کنید.";
                    closeResults();
                }
            }
        }, 250);
    });

    searchInput.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeResults();
        } else if (event.key === "ArrowDown" && !results.hidden) {
            event.preventDefault();
            results.querySelector("button")?.focus();
        } else if (event.key === "Enter" && !results.hidden) {
            const firstResult = results.querySelector("button");
            if (firstResult) {
                event.preventDefault();
                firstResult.click();
            }
        }
    });

    results.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeResults();
            searchInput.focus();
        } else if (event.key === "ArrowDown" || event.key === "ArrowUp") {
            event.preventDefault();
            const buttons = [...results.querySelectorAll("button")];
            const currentIndex = buttons.indexOf(document.activeElement);
            const nextIndex = event.key === "ArrowDown"
                ? Math.min(currentIndex + 1, buttons.length - 1)
                : Math.max(currentIndex - 1, 0);
            buttons[nextIndex]?.focus();
        }
    });

    selectedBrands.addEventListener("click", (event) => {
        const removeButton = event.target.closest("[data-remove-brand]");
        if (!removeButton || isEditMode) {
            return;
        }

        selected.delete(removeButton.closest("[data-irc]").dataset.irc);
        removeButton.closest("[data-irc]").remove();
        syncHiddenInputs();
        refreshBrandDetails();
    });

    detailsContainer?.addEventListener("change", event => {
        if (event.target.matches("[data-rank-days], [data-buy-ranking], [data-update-brand-price]")) {
            updateDetailsControls();
        }
    });

    detailsContainer?.addEventListener("input", event => {
        if (event.target.matches("[data-brand-price]")) {
            syncBrandPrice(event.target, true);
        }
    });

    document.addEventListener("click", (event) => {
        if (!form.contains(event.target)) {
            closeResults();
        }
    });

    form.addEventListener("submit", () => {
        const brandPrice = detailsContainer?.querySelector("[data-brand-price]");
        if (brandPrice) {
            syncBrandPrice(brandPrice);
        }

        const submitButton = form.querySelector('button[type="submit"]');
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.setAttribute("aria-busy", "true");
        }
    });

    document.querySelectorAll("form[data-confirm]").forEach((confirmForm) => {
        confirmForm.addEventListener("submit", (event) => {
            if (!window.confirm(confirmForm.dataset.confirm)) {
                event.preventDefault();
            }
        });
    });
})();
