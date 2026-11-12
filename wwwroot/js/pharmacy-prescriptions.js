(() => {
    const root = document.querySelector("[data-prescriptions]");
    if (!root) return;

    const searchUrl = root.dataset.searchUrl;
    const reviewUrl = root.dataset.reviewUrl;
    const trackingInput = document.querySelector("#prescription-tracking-code");
    const reviewSelect = document.querySelector("#prescription-review-status");
    const rows = root.querySelector("[data-prescription-rows]");
    const pagination = root.querySelector("[data-prescription-pagination]");
    const results = root.querySelector("[data-prescription-results]");
    const status = root.querySelector("[data-prescription-status]");
    const resultCount = root.querySelector("[data-result-count]");
    const token = root.querySelector('input[name="__RequestVerificationToken"]')?.value;
    let activePage = 1;
    let requestId = 0;

    const setStatus = (message, kind = "") => {
        status.textContent = message;
        status.dataset.kind = kind;
    };

    const addCell = (row, label, value) => {
        const cell = document.createElement("td");
        cell.dataset.label = label;
        cell.textContent = value ?? "";
        row.append(cell);
        return cell;
    };

    const renderRows = (items) => {
        rows.replaceChildren();
        if (!items.length) {
            const row = document.createElement("tr");
            row.className = "pharmacy-prescriptions-placeholder-row";
            const cell = document.createElement("td");
            cell.colSpan = 13;
            cell.className = "pharmacy-prescriptions-placeholder";
            cell.textContent = "هیچ نسخه‌ای با این فیلترها یافت نشد.";
            row.append(cell);
            rows.append(row);
            return;
        }

        for (const item of items) {
            const row = document.createElement("tr");
            addCell(row, "شناسه", item.id);
            addCell(row, "کد رهگیری", item.trackingCode);
            addCell(row, "نام دارو", item.drugName);
            addCell(row, "نام کاربر", item.registrationUserName);
            addCell(row, "کد ملی", item.nationalCode);
            addCell(row, "شماره همراه", item.mobileNumber);
            addCell(row, "نوع بیمه", item.insuranceTypeTitle);
            addCell(row, "تاریخ ثبت", item.persianRegistrationDate);
            addCell(row, "منبع ارسال", item.sendOriginTitle);

            const reviewCell = document.createElement("td");
            reviewCell.dataset.label = "بررسی شده";
            const reviewLabel = document.createElement("label");
            reviewLabel.className = "visually-hidden";
            reviewLabel.textContent = `تغییر وضعیت بررسی نسخه ${item.id}`;
            const reviewCheckbox = document.createElement("input");
            reviewCheckbox.type = "checkbox";
            reviewCheckbox.className = "pharmacy-prescriptions-review";
            reviewCheckbox.checked = item.isReviewed === true;
            reviewCheckbox.dataset.prescriptionId = item.id;
            reviewCheckbox.setAttribute("aria-label", reviewLabel.textContent);
            reviewCell.append(reviewCheckbox);
            row.append(reviewCell);

            const imageCell = addCell(row, "تصویر نسخه", "");
            if (isAllowedImageUrl(item.uploadImageUrl)) {
                const link = document.createElement("a");
                link.href = item.uploadImageUrl;
                link.target = "_blank";
                link.rel = "noopener noreferrer";
                link.setAttribute("aria-label", `مشاهده تصویر نسخه ${item.id} در پنجره جدید`);
                const image = document.createElement("img");
                image.className = "pharmacy-prescriptions-image";
                image.src = item.uploadImageUrl;
                image.alt = `تصویر نسخه ${item.id}`;
                image.loading = "lazy";
                link.append(image);
                imageCell.append(link);
            }

            addCell(row, "پیامک شده؟", item.isSendSmsBefore ? "بله" : "خیر");
            const smsCell = document.createElement("td");
            smsCell.dataset.label = "پیامک";
            if (item.mobileNumber) {
                const link = document.createElement("a");
                link.className = "pharmacy-prescriptions-action";
                link.href = `/PharmacyRequest/SendSMS?prescriptionId=${encodeURIComponent(item.id)}`;
                link.textContent = "ارسال";
                smsCell.append(link);
            } else {
                const unavailable = document.createElement("span");
                unavailable.className = "pharmacy-prescriptions-action-disabled";
                unavailable.textContent = item.mobileNumber ? "تنظیم نشده" : "شماره ثبت نشده";
                smsCell.append(unavailable);
            }
            row.append(smsCell);
            rows.append(row);
        }
    };

    const isAllowedImageUrl = (value) => {
        if (!value) return false;
        try {
            const url = new URL(value);
            return url.protocol === "https:" &&
                ["www.darooyab.ir", "api.darooyab.ir"].includes(url.hostname.toLowerCase());
        } catch {
            return false;
        }
    };

    const renderPagination = (totalPages, currentPage) => {
        pagination.replaceChildren();
        if (totalPages <= 1) return;

        for (let page = 1; page <= totalPages; page += 1) {
            const item = document.createElement("li");
            const button = document.createElement("button");
            button.type = "button";
            button.className = "pharmacy-prescriptions-page-button";
            button.textContent = page;
            button.dataset.page = page;
            if (page === currentPage) {
                button.setAttribute("aria-current", "page");
            }
            item.append(button);
            pagination.append(item);
        }
    };

    const loadPage = async (page = 1) => {
        const thisRequest = ++requestId;
        const query = new URLSearchParams({ page: String(page) });
        if (trackingInput.value) query.set("trackingCode", trackingInput.value);
        if (reviewSelect.value !== "") query.set("isReviewed", reviewSelect.value);

        results.setAttribute("aria-busy", "true");
        setStatus("در حال بارگذاری نسخه‌ها…");
        try {
            const response = await fetch(`${searchUrl}?${query.toString()}`, {
                credentials: "same-origin",
                headers: { Accept: "application/json" }
            });
            if (!response.ok) throw new Error("فهرست نسخه‌ها بارگذاری نشد.");
            const data = await response.json();
            if (thisRequest !== requestId) return;
            renderRows(data.items ?? []);
            renderPagination(data.totalPages ?? 0, data.currentPage ?? 1);
            resultCount.textContent = `${data.totalRecords ?? 0} نسخه`;
            activePage = data.currentPage ?? 1;
            setStatus("");
        } catch {
            if (thisRequest !== requestId) return;
            rows.replaceChildren();
            pagination.replaceChildren();
            resultCount.textContent = "";
            setStatus("بارگذاری نسخه‌ها انجام نشد. اتصال را بررسی و دوباره جستجو کنید.", "error");
        } finally {
            if (thisRequest === requestId) results.setAttribute("aria-busy", "false");
        }
    };

    root.addEventListener("click", async (event) => {
        const searchButton = event.target.closest("[data-search-prescriptions]");
        if (searchButton) {
            await loadPage(1);
            return;
        }

        const clearButton = event.target.closest("[data-clear-prescription-filters]");
        if (clearButton) {
            trackingInput.value = "";
            reviewSelect.value = "false";
            await loadPage(1);
            return;
        }

        const pageButton = event.target.closest("[data-page]");
        if (pageButton) await loadPage(Number(pageButton.dataset.page));
    });

    root.addEventListener("keydown", (event) => {
        if (event.target === trackingInput && event.key === "Enter") {
            event.preventDefault();
            loadPage(1);
        }
    });

    root.addEventListener("change", async (event) => {
        const checkbox = event.target.closest(".pharmacy-prescriptions-review");
        if (!checkbox) return;
        if (!token) {
            checkbox.checked = !checkbox.checked;
            setStatus("توکن امنیتی فرم در دسترس نیست؛ صفحه را تازه‌سازی کنید.", "error");
            return;
        }

        const reviewed = checkbox.checked;
        checkbox.disabled = true;
        setStatus("در حال ذخیره وضعیت بررسی…");
        try {
            const body = new URLSearchParams({
                id: checkbox.dataset.prescriptionId,
                isReviewed: String(reviewed),
                __RequestVerificationToken: token
            });
            const response = await fetch(reviewUrl, {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8",
                    Accept: "application/json"
                },
                body
            });
            const data = await response.json();
            if (!response.ok || !data.success) throw new Error(data.message || "تغییر ثبت نشد.");
            await loadPage(activePage);
            setStatus("وضعیت بررسی ذخیره شد.");
        } catch {
            checkbox.checked = !reviewed;
            setStatus("ذخیره وضعیت انجام نشد. دوباره تلاش کنید.", "error");
        } finally {
            checkbox.disabled = false;
        }
    });

    loadPage(Number(root.dataset.startPage) || 1);
})();
