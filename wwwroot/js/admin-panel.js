(() => {
    const root = document.querySelector(".admin-panel");
    if (!root) {
        return;
    }

    const themeToggle = root.querySelector("[data-theme-toggle]");
    const savedTheme = localStorage.getItem("admin-theme");
    const initialTheme = savedTheme === "dark" ? "dark" : "light";
    root.dataset.theme = initialTheme;

    const setTheme = theme => {
        root.dataset.theme = theme;
        const nextThemeLabel = theme === "dark" ? "تغییر به تم روشن" : "تغییر به تم تیره";
        themeToggle?.setAttribute("aria-label", nextThemeLabel);
        themeToggle?.setAttribute("title", nextThemeLabel);
        themeToggle?.setAttribute("aria-pressed", String(theme === "dark"));
    };

    setTheme(initialTheme);
    themeToggle?.addEventListener("click", () => {
        const nextTheme = root.dataset.theme === "dark" ? "light" : "dark";
        setTheme(nextTheme);
        localStorage.setItem("admin-theme", nextTheme);
    });

    const sidebar = root?.querySelector(".admin-sidebar");
    const toggle = root?.querySelector("[data-sidebar-toggle]");
    const closeButton = root?.querySelector("[data-sidebar-close]");
    const collapseButton = root?.querySelector("[data-sidebar-collapse]");
    if (!root || !sidebar || !toggle || !closeButton || !collapseButton) {
        return;
    }

    const setMobileMenuOpen = open => {
        sidebar.classList.toggle("is-open", open);
        closeButton.classList.toggle("is-visible", open);
        toggle.setAttribute("aria-expanded", String(open));
        toggle.setAttribute("aria-label", open ? "بستن منو" : "باز کردن منو");
        if (open) {
            sidebar.querySelector("a")?.focus();
        } else {
            toggle.focus();
        }
    };

    toggle.addEventListener("click", () => {
        setMobileMenuOpen(!sidebar.classList.contains("is-open"));
    });
    closeButton.addEventListener("click", () => setMobileMenuOpen(false));
    collapseButton.addEventListener("click", () => {
        const collapsed = root.classList.toggle("is-sidebar-collapsed");
        collapseButton.setAttribute("aria-pressed", String(collapsed));
        collapseButton.setAttribute(
            "aria-label",
            collapsed ? "باز کردن منو" : "جمع کردن منو");
    });

    document.addEventListener("keydown", event => {
        if (event.key === "Escape" && sidebar.classList.contains("is-open")) {
            setMobileMenuOpen(false);
        }
    });

    window.addEventListener("resize", () => {
        if (window.matchMedia("(min-width: 761px)").matches) {
            if (sidebar.classList.contains("is-open")) {
                sidebar.classList.remove("is-open");
                closeButton.classList.remove("is-visible");
                toggle.setAttribute("aria-expanded", "false");
            }
        } else {
            root.classList.remove("is-sidebar-collapsed");
            collapseButton.setAttribute("aria-pressed", "false");
        }
    });
})();
