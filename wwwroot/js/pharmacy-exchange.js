(() => {
    const form = document.querySelector("[data-exchange-form]");
    const listContainer = document.querySelector("#exchange-list-container");
    const searchForm = document.querySelector("[data-exchange-search-form]");

    if (!form || !listContainer || !searchForm) {
        return;
    }

    const brandSearch = form.querySelector("#exchange-brand-search");
    const results = form.querySelector("#exchange-brand-results");
    const searchStatus = form.querySelector("[data-search-status]");
    const selectedBrands = form.querySelector("[data-selected-brands]");
    const selectedInputs = form.querySelector("[data-selected-irc-inputs]");
    const submitButton = form.querySelector('button[type="submit"]');
    const expiryOnly = form.dataset.expiryOnly === "true";
    const listStatus = document.querySelector("#exchange-list-status");
    const listLoading = document.querySelector("[data-list-loading]");
    let activeSearch;
    let searchTimer;
    let listRequest;

    const closeResults = () => {
        results.hidden = true;
        results.replaceChildren();
        brandSearch.setAttribute("aria-expanded", "false");
    };

    const addBrand = (brand) => {
        const irc = String(brand.irc ?? brand.Irc ?? "");
        const name = String(brand.name ?? brand.Name ?? "");
        if (!/^[1-9]\d*$/.test(irc) ||
            !name ||
            Array.from(selectedInputs.querySelectorAll('input[name="Form.SelectedIrcs"]'))
                .some(input => input.value === irc)) {
            return;
        }
        if (selectedInputs.querySelectorAll('input[name="Form.SelectedIrcs"]').length >= 25) {
            searchStatus.textContent = "حداکثر ۲۵ برند را می‌توانید انتخاب کنید.";
            return;
        }

        const chip = document.createElement("span");
        chip.className = "pharmacy-request-chip";
        chip.dataset.irc = irc;
        const label = document.createElement("span");
        label.textContent = name;
        const remove = document.createElement("button");
        remove.type = "button";
        remove.dataset.removeBrand = "";
        remove.setAttribute("aria-label", `حذف ${name}`);
        remove.textContent = "×";
        chip.append(label, remove);
        selectedBrands.append(chip);

        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "Form.SelectedIrcs";
        input.value = irc;
        selectedInputs.append(input);
        searchStatus.textContent = `${name} به فهرست انتخاب‌شده‌ها اضافه شد.`;
        brandSearch.value = "";
        closeResults();
    };

    const renderSearchResults = (brands) => {
        results.replaceChildren();
        for (const brand of brands) {
            const button = document.createElement("button");
            button.type = "button";
            button.className = "pharmacy-exchange-search-option";
            button.setAttribute("role", "option");
            button.textContent = brand.name ?? brand.Name ?? "";
            button.addEventListener("click", () => addBrand(brand), { once: true });
            const item = document.createElement("li");
            item.append(button);
            results.append(item);
        }
        results.hidden = results.childElementCount === 0;
        brandSearch.setAttribute("aria-expanded", String(!results.hidden));
        searchStatus.textContent = results.hidden ? "برندی با این عبارت پیدا نشد." : `${brands.length} نتیجه پیدا شد.`;
    };

    brandSearch?.addEventListener("input", () => {
        clearTimeout(searchTimer);
        activeSearch?.abort();
        const query = brandSearch.value.trim();
        closeResults();
        if (query.length < 2) {
            searchStatus.textContent = query ? "برای جستجو حداقل دو نویسه وارد کنید." : "";
            return;
        }

        searchStatus.textContent = "در حال جستجوی برندها…";
        searchTimer = window.setTimeout(async () => {
            activeSearch = new AbortController();
            try {
                const url = new URL(brandSearch.dataset.searchUrl, window.location.origin);
                url.searchParams.set("query", query);
                const response = await fetch(url, {
                    headers: { "Accept": "application/json" },
                    signal: activeSearch.signal
                });
                if (!response.ok) {
                    throw new Error(`Brand search failed (${response.status}).`);
                }
                if (response.redirected) {
                    window.location.assign(response.url);
                    return;
                }
                renderSearchResults(await response.json());
            } catch (error) {
                if (error.name !== "AbortError") {
                    searchStatus.textContent = "جستجوی برند انجام نشد؛ دوباره تلاش کنید.";
                }
            }
        }, 250);
    });

    brandSearch?.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeResults();
        } else if (event.key === "ArrowDown" && !results.hidden) {
            event.preventDefault();
            results.querySelector("button")?.focus();
        }
    });

    selectedBrands.addEventListener("click", (event) => {
        const removeButton = event.target.closest("[data-remove-brand]");
        if (!removeButton) {
            return;
        }
        const chip = removeButton.closest("[data-irc]");
        const irc = chip.dataset.irc;
        Array.from(selectedInputs.querySelectorAll('input[name="Form.SelectedIrcs"]'))
            .find(input => input.value === irc)?.remove();
        chip.remove();
    });

    document.addEventListener("click", (event) => {
        if (!event.target.closest(".pharmacy-exchange-field, [role='combobox']")) {
            closeResults();
        }
    });

    const colleagueCheckbox = form.querySelector("[data-sell-to-colleague]");
    const colleaguePriceField = form.querySelector("[data-colleague-price-field]");
    const colleaguePrice = colleaguePriceField.querySelector("input");
    const updateColleaguePrice = () => {
        colleaguePriceField.hidden = !colleagueCheckbox.checked;
        if (!colleagueCheckbox.checked) {
            colleaguePrice.value = "";
        }
    };
    if (!expiryOnly) {
        colleagueCheckbox.addEventListener("change", updateColleaguePrice);
        updateColleaguePrice();
    }

    form.addEventListener("submit", (event) => {
        if (!expiryOnly &&
            selectedInputs.querySelectorAll('input[name="Form.SelectedIrcs"]').length === 0) {
            event.preventDefault();
            searchStatus.textContent = "حداقل یک برند را انتخاب کنید.";
            brandSearch?.focus();
            return;
        }
        if (!form.reportValidity()) {
            event.preventDefault();
            return;
        }
        submitButton.disabled = true;
        submitButton.textContent = "در حال ثبت…";
    });

    const loadList = async (url) => {
        listRequest?.abort();
        listRequest = new AbortController();
        listContainer.setAttribute("aria-busy", "true");
        listLoading.hidden = false;
        listStatus.textContent = "در حال بارگذاری فهرست…";
        try {
            const response = await fetch(url, {
                headers: { "X-Requested-With": "XMLHttpRequest" },
                signal: listRequest.signal
            });
            if (!response.ok) {
                throw new Error(`Exchange list failed (${response.status}).`);
            }
            if (response.redirected) {
                window.location.assign(response.url);
                return;
            }
            listContainer.innerHTML = await response.text();
            const resultUrl = new URL(url, window.location.origin);
            const searchInput = searchForm.querySelector('[name="searchString"]');
            searchInput.value = resultUrl.searchParams.get("searchString") ?? "";
            const pageUrl = new URL(window.location.href);
            const query = new URLSearchParams(resultUrl.search);
            if (pageUrl.searchParams.has("id")) {
                query.set("id", pageUrl.searchParams.get("id"));
            }
            const queryString = query.toString();
            window.history.replaceState(
                {},
                "",
                `/PharmacyRequest/TabadolManage${queryString ? `?${queryString}` : ""}`);
            listStatus.textContent = "فهرست به‌روزرسانی شد.";
        } catch (error) {
            if (error.name !== "AbortError") {
                let message = listContainer.querySelector("[data-list-error]");
                if (!message) {
                    message = document.createElement("div");
                    message.className = "pharmacy-request-alert pharmacy-request-alert-error";
                    message.dataset.listError = "";
                    message.setAttribute("role", "alert");
                    listContainer.prepend(message);
                }
                message.textContent = "بارگذاری فهرست انجام نشد؛ اتصال را بررسی و دوباره تلاش کنید.";
                listStatus.textContent = "بارگذاری فهرست انجام نشد.";
            }
        } finally {
            if (!listRequest.signal.aborted) {
                listContainer.removeAttribute("aria-busy");
                listLoading.hidden = true;
            }
        }
    };

    searchForm.addEventListener("submit", (event) => {
        event.preventDefault();
        const url = new URL(listContainer.dataset.listUrl, window.location.origin);
        const query = searchForm.querySelector('[name="searchString"]').value.trim();
        if (query) {
            url.searchParams.set("searchString", query);
        }
        url.searchParams.set("page", "1");
        loadList(url);
    });

    listContainer.addEventListener("click", (event) => {
        const pageLink = event.target.closest("[data-exchange-list-page]");
        if (!pageLink) {
            return;
        }
        event.preventDefault();
        loadList(pageLink.href);
    });

    listContainer.addEventListener("submit", (event) => {
        const actionForm = event.target.closest("form[data-confirm]");
        if (actionForm && !window.confirm(actionForm.dataset.confirm)) {
            event.preventDefault();
        }
    });
})();
