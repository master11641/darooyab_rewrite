(() => {
    const form = document.querySelector("[data-shortage-form]");
    if (!form) return;

    const searchInput = document.querySelector("#shortage-drug-search");
    const searchResults = document.querySelector("#shortage-search-results");
    const searchStatus = document.querySelector("[data-search-status]");
    const selectedDrugId = document.querySelector("[data-selected-drug-id]");
    const selectedDrug = document.querySelector("[data-selected-drug]");
    const searchLabel = document.querySelector("[data-search-label]");
    let searchTimeout;
    let searchAbortController;

    const currentSearchType = () => form.querySelector('input[name="Form.SearchType"]:checked')?.value || "2";

    const clearSearchResults = () => {
        searchResults.replaceChildren();
        searchResults.hidden = true;
        searchInput.setAttribute("aria-expanded", "false");
    };

    const clearSelectedDrug = () => {
        selectedDrugId.value = "";
        selectedDrug.replaceChildren();
    };

    const selectDrug = (option) => {
        selectedDrugId.value = option.id;
        selectedDrug.replaceChildren();

        const chip = document.createElement("span");
        chip.className = "shortage-drug-chip";
        const name = document.createElement("span");
        name.textContent = option.name;
        const clearButton = document.createElement("button");
        clearButton.type = "button";
        clearButton.dataset.clearDrug = "";
        clearButton.textContent = "حذف";
        clearButton.setAttribute("aria-label", "حذف داروی انتخاب‌شده");
        chip.append(name, clearButton);
        selectedDrug.append(chip);
        searchInput.value = "";
        searchStatus.textContent = "";
        clearSearchResults();
    };

    const renderSearchResults = (options) => {
        searchResults.replaceChildren();
        if (!options.length) {
            searchStatus.textContent = "نتیجه‌ای برای این عبارت پیدا نشد.";
            clearSearchResults();
            return;
        }

        for (const option of options) {
            const item = document.createElement("li");
            item.setAttribute("role", "presentation");
            const button = document.createElement("button");
            button.type = "button";
            button.className = "shortage-search-option";
            button.setAttribute("role", "option");
            button.dataset.searchId = option.id;
            button.dataset.searchName = option.name;
            button.textContent = option.name;
            item.append(button);
            searchResults.append(item);
        }

        searchResults.hidden = false;
        searchInput.setAttribute("aria-expanded", "true");
        searchStatus.textContent = `${options.length} نتیجه پیدا شد.`;
    };

    searchInput.addEventListener("input", () => {
        window.clearTimeout(searchTimeout);
        if (searchAbortController) searchAbortController.abort();

        const query = searchInput.value.trim();
        if (query.length < 2) {
            clearSearchResults();
            searchStatus.textContent = query ? "برای جستجو حداقل دو نویسه وارد کنید." : "";
            return;
        }

        searchStatus.textContent = "در حال جستجو…";
        searchTimeout = window.setTimeout(async () => {
            searchAbortController = new AbortController();
            const url = new URL(searchInput.dataset.searchUrl, window.location.origin);
            url.searchParams.set("searchType", currentSearchType());
            url.searchParams.set("query", query);
            try {
                const response = await fetch(url, {
                    headers: { Accept: "application/json" },
                    signal: searchAbortController.signal
                });
                if (response.redirected) {
                    window.location.assign(response.url);
                    return;
                }
                if (!response.ok) throw new Error("search failed");
                renderSearchResults(await response.json());
            } catch (error) {
                if (error.name === "AbortError") return;
                clearSearchResults();
                searchStatus.textContent = "جستجو ناموفق بود؛ اتصال را بررسی و دوباره تلاش کنید.";
            }
        }, 250);
    });

    searchResults.addEventListener("click", (event) => {
        const button = event.target.closest("[data-search-id]");
        if (!button) return;
        selectDrug({ id: button.dataset.searchId, name: button.dataset.searchName });
    });

    searchInput.addEventListener("keydown", (event) => {
        const options = [...searchResults.querySelectorAll("[data-search-id]")];
        const activeIndex = options.indexOf(document.activeElement);
        if (event.key === "ArrowDown" && options.length) {
            event.preventDefault();
            options[Math.min(activeIndex + 1, options.length - 1)].focus();
        } else if (event.key === "Escape") {
            clearSearchResults();
        } else if (event.key === "Enter" && options.length && searchResults.contains(document.activeElement)) {
            event.preventDefault();
            document.activeElement.click();
        }
    });

    searchResults.addEventListener("keydown", (event) => {
        const options = [...searchResults.querySelectorAll("[data-search-id]")];
        const activeIndex = options.indexOf(event.target);
        if (event.key === "ArrowDown") {
            event.preventDefault();
            options[Math.min(activeIndex + 1, options.length - 1)]?.focus();
        } else if (event.key === "ArrowUp") {
            event.preventDefault();
            if (activeIndex <= 0) searchInput.focus();
            else options[activeIndex - 1].focus();
        } else if (event.key === "Escape") {
            clearSearchResults();
            searchInput.focus();
        }
    });

    document.addEventListener("click", (event) => {
        if (!event.target.closest(".shortage-drug-picker")) clearSearchResults();
        const clearButton = event.target.closest("[data-clear-drug]");
        if (clearButton) {
            clearSelectedDrug();
            searchInput.focus();
        }
    });

    form.querySelectorAll('input[name="Form.SearchType"]').forEach((radio) => {
        radio.addEventListener("change", () => {
            clearSelectedDrug();
            clearSearchResults();
            searchInput.value = "";
            searchStatus.textContent = "";
            searchLabel.textContent = radio.value === "1" ? "جستجوی شکل دارویی" : "جستجوی برند";
            searchInput.focus();
        });
    });

    form.addEventListener("submit", (event) => {
        if (!selectedDrugId.value) {
            event.preventDefault();
            searchStatus.textContent = "ابتدا یک برند یا شکل دارویی را انتخاب کنید.";
            searchInput.focus();
            return;
        }
        const saveButton = form.querySelector("[data-save-button]");
        saveButton.disabled = true;
        saveButton.setAttribute("aria-busy", "true");
    });

    const modal = document.querySelector("[data-results-modal]");
    const modalTitle = document.querySelector("[data-results-title]");
    const modalFeedback = document.querySelector("[data-modal-feedback]");
    const provinceSelect = document.querySelector("[data-province]");
    const townshipSelect = document.querySelector("[data-township]");
    const tabs = [...modal.querySelectorAll("[data-result-tab]")];
    const panels = [...modal.querySelectorAll("[data-result-panel]")];
    const offerUrl = "/DrugShortage/ExchangeOffers";
    let activeTab = "NormalExchange";
    let activeShortageId = null;
    let previousFocus = null;
    let offerAbortController;
    let offerRequestKey = "";
    const offerCache = new Map();

    const setActiveTab = (name, focusTab = false) => {
        activeTab = name;
        for (const tab of tabs) {
            const selected = tab.dataset.resultTab === name;
            tab.setAttribute("aria-selected", String(selected));
            tab.tabIndex = selected ? 0 : -1;
            if (selected && focusTab) tab.focus();
        }
        for (const panel of panels) panel.hidden = panel.dataset.resultPanel !== name;
        modalFeedback.textContent = "";
    };

    const loadOffers = async (displayType, force = false) => {
        if (!activeShortageId) return;
        const panel = panels.find(item => item.dataset.resultPanel === displayType);
        const key = [
            activeShortageId,
            displayType,
            provinceSelect.value,
            townshipSelect.value
        ].join("|");
        if (!force && offerCache.has(key)) {
            panel.innerHTML = offerCache.get(key);
            return;
        }
        if (offerAbortController && offerRequestKey === key) return;
        if (offerAbortController) offerAbortController.abort();

        const controller = new AbortController();
        offerAbortController = controller;
        offerRequestKey = key;
        const token = form.querySelector('input[name="__RequestVerificationToken"]').value;
        const body = new URLSearchParams({
            __RequestVerificationToken: token,
            PharmacyShortageId: activeShortageId,
            DisplayType: displayType,
            ProvinceId: provinceSelect.value,
            CityId: townshipSelect.value
        });
        panel.innerHTML = '<div class="shortage-empty" role="status">در حال دریافت پیشنهادها…</div>';
        try {
            const response = await fetch(offerUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8",
                    "X-Requested-With": "XMLHttpRequest",
                    Accept: "text/html"
                },
                body,
                signal: controller.signal
            });
            if (response.redirected) {
                window.location.assign(response.url);
                return;
            }
            if (!response.ok) throw new Error("offers failed");
            const html = await response.text();
            if (controller.signal.aborted) return;
            panel.innerHTML = html;
            offerCache.set(key, html);
        } catch (error) {
            if (error.name === "AbortError") return;
            panel.innerHTML = "";
            modalFeedback.textContent = "دریافت پیشنهادها ناموفق بود؛ لطفاً اتصال را بررسی و دوباره تلاش کنید.";
            const retry = document.createElement("button");
            retry.type = "button";
            retry.className = "shortage-secondary-button";
            retry.textContent = "تلاش دوباره";
            retry.addEventListener("click", () => loadOffers(displayType, true), { once: true });
            panel.append(retry);
        } finally {
            if (offerAbortController === controller) {
                offerAbortController = null;
                offerRequestKey = "";
            }
        }
    };

    const openModal = (button) => {
        previousFocus = button;
        activeShortageId = button.dataset.id;
        modalTitle.textContent = `نتیجه برای ${button.dataset.drugName}`;
        for (const panel of panels) panel.replaceChildren();
        offerCache.clear();
        if (offerAbortController) offerAbortController.abort();
        modalFeedback.textContent = "";
        setActiveTab("NormalExchange");
        modal.hidden = false;
        modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("shortage-modal-open");
        modal.querySelector("[data-close-results]").focus();
    };

    const closeModal = () => {
        if (offerAbortController) offerAbortController.abort();
        offerAbortController = null;
        offerRequestKey = "";
        offerCache.clear();
        for (const panel of panels) panel.replaceChildren();
        modal.hidden = true;
        modal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("shortage-modal-open");
        activeShortageId = null;
        if (previousFocus?.isConnected) previousFocus.focus();
    };

    document.addEventListener("click", (event) => {
        const openButton = event.target.closest("[data-open-results]");
        if (openButton) openModal(openButton);
        if (event.target.closest("[data-close-results]")) closeModal();
        if (event.target === modal) closeModal();
    });

    modal.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            event.preventDefault();
            closeModal();
            return;
        }
        if (event.key === "Tab") {
            const focusable = [...modal.querySelectorAll(
                'button:not([disabled]), select:not([disabled]), a[href], [tabindex]:not([tabindex="-1"])'
            )].filter(element => !element.closest("[hidden]"));
            if (!focusable.length) return;
            const first = focusable[0];
            const last = focusable[focusable.length - 1];
            if (event.shiftKey && document.activeElement === first) {
                event.preventDefault();
                last.focus();
            } else if (!event.shiftKey && document.activeElement === last) {
                event.preventDefault();
                first.focus();
            }
        }
    });

    for (const tab of tabs) {
        tab.addEventListener("click", () => {
            setActiveTab(tab.dataset.resultTab);
            loadOffers(activeTab);
        });
        tab.addEventListener("keydown", (event) => {
            if (event.key !== "ArrowLeft" && event.key !== "ArrowRight") return;
            event.preventDefault();
            const index = tabs.indexOf(tab);
            const direction = event.key === "ArrowLeft" ? 1 : -1;
            const next = tabs[(index + direction + tabs.length) % tabs.length];
            setActiveTab(next.dataset.resultTab, true);
            loadOffers(activeTab);
        });
    }

    const updateTownships = async () => {
        const provinceId = provinceSelect.value;
        townshipSelect.replaceChildren(new Option("همه شهرها", "0"));
        townshipSelect.disabled = provinceId === "0";
        if (provinceId !== "0") {
            try {
                const url = new URL("/DrugShortage/Townships", window.location.origin);
                url.searchParams.set("provinceId", provinceId);
                const response = await fetch(url, { headers: { Accept: "application/json" } });
                if (response.redirected) {
                    window.location.assign(response.url);
                    return;
                }
                if (!response.ok) throw new Error("townships failed");
                const townships = await response.json();
                for (const township of townships) {
                    townshipSelect.add(new Option(township.name, township.id));
                }
                townshipSelect.disabled = false;
            } catch {
                modalFeedback.textContent = "بارگذاری شهرها ناموفق بود؛ فیلتر استان تغییر کرد.";
            }
        }
        offerCache.clear();
        loadOffers(activeTab, true);
    };

    provinceSelect.addEventListener("change", updateTownships);
    townshipSelect.addEventListener("change", () => {
        offerCache.clear();
        loadOffers(activeTab, true);
    });

    document.querySelectorAll("[data-delete-form]").forEach((deleteForm) => {
        deleteForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            if (!window.confirm("آیا از حذف این کسری مطمئن هستید؟")) return;
            const button = deleteForm.querySelector("button");
            const feedback = deleteForm.closest("[data-shortage-item]").querySelector("[data-delete-feedback]");
            button.disabled = true;
            button.setAttribute("aria-busy", "true");
            feedback.hidden = true;

            try {
                const response = await fetch(deleteForm.action, {
                    method: "POST",
                    body: new FormData(deleteForm),
                    headers: { "X-Requested-With": "XMLHttpRequest", Accept: "application/json" }
                });
                if (response.redirected) {
                    window.location.assign(response.url);
                    return;
                }
                const result = await response.json();
                if (!response.ok || !result.success) throw new Error(result.message || "حذف انجام نشد.");

                const list = document.querySelector("[data-shortage-list]");
                const items = [...list.querySelectorAll("[data-shortage-item]")];
                if (items.length === 1 && Number(list.dataset.page) > 1) {
                    window.location.assign(`/DrugShortage/AddOrEdit?page=${Number(list.dataset.page) - 1}`);
                    return;
                }
                window.location.reload();
            } catch (error) {
                feedback.textContent = error.message || "حذف ناموفق بود؛ دوباره تلاش کنید.";
                feedback.hidden = false;
                button.disabled = false;
                button.removeAttribute("aria-busy");
            }
        });
    });
})();
