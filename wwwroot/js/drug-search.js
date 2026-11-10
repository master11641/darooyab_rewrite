(() => {
  const siteHeader = document.querySelector(".site-header");
  if (siteHeader) {
    const updateHeaderHeight = () => {
      document.documentElement.style.setProperty(
        "--site-header-height",
        `${Math.ceil(siteHeader.getBoundingClientRect().height)}px`
      );
    };

    updateHeaderHeight();
    if ("ResizeObserver" in window) {
      new ResizeObserver(updateHeaderHeight).observe(siteHeader);
    } else {
      window.addEventListener("resize", updateHeaderHeight);
    }
  }

  const searchToggle = document.querySelector(".mobile-search-toggle");
  const siteSearch = document.querySelector("#site-drug-search");

  if (searchToggle && siteSearch) {
    searchToggle.addEventListener("click", () => {
      const isExpanded = searchToggle.getAttribute("aria-expanded") === "true";
      searchToggle.setAttribute("aria-expanded", String(!isExpanded));
      searchToggle.setAttribute(
        "aria-label",
        isExpanded ? "نمایش جست‌وجوی دارو" : "پنهان‌کردن جست‌وجوی دارو"
      );
      searchToggle.querySelector(".visually-hidden").textContent =
        isExpanded ? "نمایش جست‌وجوی دارو" : "پنهان‌کردن جست‌وجوی دارو";
      siteSearch.classList.toggle("is-mobile-visible", !isExpanded);

      if (!isExpanded) {
        siteSearch.querySelector('input[name="SearchText"]')?.focus();
      } else {
        searchToggle.focus();
      }
    });
  }

  const searchBoxes = document.querySelectorAll("[data-drug-search]");

  searchBoxes.forEach((form) => {
    const input = form.querySelector('input[name="SearchText"]');
    const panel = form.querySelector(".drug-search-suggestions");
    const list = form.querySelector(".drug-search-suggestion-list");
    const status = form.querySelector(".drug-search-status");
    const moreButton = form.querySelector(".drug-search-more");
    const suggestionsUrl = form.dataset.suggestionsUrl;
    let timer;
    let request;

    if (!input || !panel || !list || !status || !moreButton || !suggestionsUrl) {
      return;
    }

    const closeSuggestions = () => {
      panel.hidden = true;
      input.setAttribute("aria-expanded", "false");
    };

    const showSuggestions = () => {
      panel.hidden = false;
      input.setAttribute("aria-expanded", "true");
    };

    const clearSuggestions = () => {
      list.replaceChildren();
      status.textContent = "";
      moreButton.hidden = true;
    };

    const loadSuggestions = async (query) => {
      request?.abort();
      request = new AbortController();
      clearSuggestions();

      if (query.length < 2) {
        closeSuggestions();
        return;
      }

      status.textContent = "در حال جست‌وجو…";
      showSuggestions();

      try {
        const url = new URL(suggestionsUrl, window.location.origin);
        url.searchParams.set("SearchText", query);
        const response = await fetch(url, {
          headers: { Accept: "application/json" },
          signal: request.signal
        });

        if (!response.ok) {
          throw new Error("Drug search suggestions could not be loaded.");
        }

        const result = await response.json();
        status.textContent = result.items.length === 0 ? "نتیجه‌ای پیدا نشد." : "";

        result.items.forEach((item) => {
          const option = document.createElement("li");
          const link = document.createElement("a");
          option.setAttribute("role", "presentation");
          link.setAttribute("role", "option");
          link.href = item.url;
          link.textContent = item.name;
          option.append(link);
          list.append(option);
        });

        moreButton.hidden = !result.hasMore;
      } catch (error) {
        if (error.name === "AbortError") {
          return;
        }

        status.textContent = "دریافت پیشنهادها ممکن نشد؛ برای دیدن نتایج جست‌وجو را انتخاب کنید.";
        moreButton.hidden = false;
      }
    };

    input.addEventListener("input", () => {
      window.clearTimeout(timer);
      timer = window.setTimeout(() => loadSuggestions(input.value.trim()), 250);
    });

    input.addEventListener("keydown", (event) => {
      if (event.key === "ArrowDown" && !panel.hidden) {
        const firstSuggestion = list.querySelector("a");
        if (firstSuggestion) {
          event.preventDefault();
          firstSuggestion.focus();
        }
      } else if (event.key === "Escape") {
        closeSuggestions();
      }
    });

    form.addEventListener("keydown", (event) => {
      if (event.key === "Escape") {
        closeSuggestions();
        input.focus();
      }
    });

    form.addEventListener("focusout", (event) => {
      if (!form.contains(event.relatedTarget)) {
        closeSuggestions();
      }
    });

    document.addEventListener("pointerdown", (event) => {
      if (!form.contains(event.target)) {
        closeSuggestions();
      }
    });
  });
})();
