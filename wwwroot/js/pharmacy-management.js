(() => {
    const root = document.querySelector("[data-pharmacy-management]");
    if (!root) return;

    const tabs = Array.from(root.querySelectorAll("[data-pharmacy-tab]"));
    const panels = Array.from(root.querySelectorAll("[data-pharmacy-panel]"));
    let map;
    let marker;
    let locationSaveQueue = Promise.resolve();

    function setActiveTab(tabNumber, updateUrl) {
        const selectedTab = tabs.find(tab => Number(tab.dataset.pharmacyTab) === tabNumber);
        const selectedPanel = panels.find(panel => Number(panel.dataset.pharmacyPanel) === tabNumber);
        if (!selectedTab || !selectedPanel) return;

        tabs.forEach(tab => {
            const selected = tab === selectedTab;
            tab.classList.toggle("is-active", selected);
            tab.setAttribute("aria-selected", String(selected));
            tab.tabIndex = selected ? 0 : -1;
        });
        panels.forEach(panel => {
            const selected = panel === selectedPanel;
            panel.hidden = !selected;
            panel.classList.toggle("is-active", selected);
        });

        if (updateUrl) {
            const url = new URL(window.location.href);
            url.searchParams.set("active", String(tabNumber));
            window.history.replaceState(null, document.title, `${url.pathname}${url.search}${url.hash}`);
        }

        if (tabNumber === 3) {
            window.setTimeout(() => {
                initializeMap();
                if (map) map.invalidateSize();
            }, 100);
        }
    }

    tabs.forEach((tab, index) => {
        tab.addEventListener("click", () => setActiveTab(Number(tab.dataset.pharmacyTab), true));
        tab.addEventListener("keydown", event => {
            if (!["ArrowRight", "ArrowLeft", "Home", "End"].includes(event.key)) return;
            event.preventDefault();
            const nextIndex = event.key === "Home"
                ? 0
                : event.key === "End"
                    ? tabs.length - 1
                    : (index + (event.key === "ArrowLeft" ? 1 : -1) + tabs.length) % tabs.length;
            tabs[nextIndex].focus();
            setActiveTab(Number(tabs[nextIndex].dataset.pharmacyTab), true);
        });
    });

    const activeTab = Number(root.dataset.activeTab);
    setActiveTab(Number.isInteger(activeTab) ? activeTab : 2, false);

    const districtForm = root.querySelector("[data-district-form]");
    districtForm?.querySelector("[data-clear-district]")?.addEventListener("click", () => {
        const district = districtForm.querySelector("select[name='districtId']");
        if (district) district.value = "";
    });

    const specialtiesForm = root.querySelector("[data-specialties-form]");
    const specialtyInputs = Array.from(specialtiesForm?.querySelectorAll("input[type='checkbox']") ?? []);
    const specialtyCount = specialtiesForm?.querySelector("[data-specialty-count]");
    const updateSpecialtyCount = () => {
        if (specialtyCount) {
            specialtyCount.textContent = `زمینه‌های انتخاب‌شده: ${specialtyInputs.filter(input => input.checked).length} از ۲`;
        }
    };
    specialtyInputs.forEach(input => input.addEventListener("change", updateSpecialtyCount));
    updateSpecialtyCount();

    const imageForm = root.querySelector("[data-pharmacy-images-form]");
    const imageInputs = Array.from(imageForm?.querySelectorAll("[data-image-number]") ?? []);
    imageInputs.forEach(input => {
        input.addEventListener("change", () => {
            const number = Number(input.dataset.imageNumber);
            const name = imageForm.querySelector(`[data-image-file-name="${number}"]`);
            const preview = imageForm.querySelector(`[data-image-preview="${number}"]`);
            const file = input.files?.[0];
            if (name) name.textContent = file?.name ?? "فایلی انتخاب نشده است.";
            if (preview) {
                preview.replaceChildren();
                if (file) {
                    const image = document.createElement("img");
                    image.alt = `پیش‌نمایش تصویر ${number}`;
                    image.src = URL.createObjectURL(file);
                    preview.append(image);
                }
            }

            const nextInput = imageInputs.find(item => Number(item.dataset.imageNumber) === number + 1);
            if (nextInput && file) {
                nextInput.disabled = false;
                const nextName = imageForm.querySelector(`[data-image-file-name="${number + 1}"]`);
                if (nextName) nextName.textContent = "فایلی انتخاب نشده است.";
            }
        });
    });

    imageForm?.querySelectorAll("[data-delete-pharmacy-image]").forEach(button => {
        button.addEventListener("click", async () => {
            const imageNumber = button.dataset.deletePharmacyImage;
            if (!window.confirm("آیا از حذف این تصویر مطمئن هستید؟")) return;

            button.disabled = true;
            try {
                const formData = new URLSearchParams();
                formData.set("imageNumber", imageNumber);
                formData.set(
                    "__RequestVerificationToken",
                    imageForm.querySelector("input[name='__RequestVerificationToken']")?.value ?? ""
                );
                const response = await fetch("/DarooyabPharmacy/DeleteImage", {
                    method: "POST",
                    credentials: "same-origin",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: formData
                });
                const result = await response.json();
                if (!response.ok || !result.success) {
                    throw new Error(result.message || "حذف تصویر انجام نشد.");
                }

                window.location.assign("/DarooyabPharmacy/PharmacySpecialties?active=4");
            } catch (error) {
                window.alert(error instanceof Error ? error.message : "خطا در ارتباط با سرور.");
                button.disabled = false;
            }
        });
    });

    document.querySelectorAll(".pharmacy-management-mobile").forEach(input => {
        input.addEventListener("input", () => {
            const translated = input.value
                .replace(/[۰-۹]/g, digit => "۰۱۲۳۴۵۶۷۸۹".indexOf(digit))
                .replace(/[٠-٩]/g, digit => "٠١٢٣٤٥٦٧٨٩".indexOf(digit));
            input.value = translated.replace(/[^0-9]/g, "").slice(0, 11);
        });
    });

    function showGeoStatus(type, message) {
        const status = document.getElementById("pharmacy-management-geo-status");
        if (!status) return;
        status.className = `pharmacy-management-alert is-${type}`;
        status.textContent = message;
        status.hidden = false;
    }

    function parseCoordinate(value) {
        if (!value) return null;
        const parsed = Number(String(value).replace(",", "."));
        return Number.isFinite(parsed) ? parsed : null;
    }

    function isValidCoordinate(latitude, longitude) {
        return latitude !== null &&
            longitude !== null &&
            latitude >= -90 &&
            latitude <= 90 &&
            longitude >= -180 &&
            longitude <= 180;
    }

    function saveLocation(latitude, longitude) {
        const mapElement = document.getElementById("pharmacy-management-map");
        const token = root.querySelector("[data-location-token] input[name='__RequestVerificationToken']")?.value
            ?? "";
        const body = new URLSearchParams({
            latitude: latitude.toFixed(8),
            longitude: longitude.toFixed(8),
            __RequestVerificationToken: token
        });

        locationSaveQueue = locationSaveQueue
            .then(async () => {
                const response = await fetch(mapElement.dataset.saveUrl, {
                    method: "POST",
                    credentials: "same-origin",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body
                });
                const result = await response.json();
                if (!response.ok || !result.success) {
                    throw new Error(result.message || "ذخیره موقعیت ناموفق بود.");
                }
                showGeoStatus("success", "موقعیت داروخانه با موفقیت ذخیره شد.");
            })
            .catch(error => {
                showGeoStatus("error", error instanceof Error ? error.message : "خطا در ارتباط با سرور.");
            });
        return locationSaveQueue;
    }

    function reverseGeocode(latitude, longitude) {
        const query = new URLSearchParams({
            format: "jsonv2",
            lat: String(latitude),
            lon: String(longitude),
            "accept-language": "fa"
        });
        fetch(`https://nominatim.openstreetmap.org/reverse?${query}`, {
            headers: { Accept: "application/json" }
        })
            .then(response => response.ok ? response.json() : Promise.reject(new Error("reverse geocoding failed")))
            .then(result => {
                const address = result?.display_name || "آدرس متنی یافت نشد.";
                showGeoStatus("info", `موقعیت روی نقشه انتخاب شد. ${address}`);
            })
            .catch(() => showGeoStatus("info", "موقعیت روی نقشه انتخاب شد؛ آدرس متنی در دسترس نیست."));
    }

    function updateLocation(latitude, longitude, moveMap) {
        if (!isValidCoordinate(latitude, longitude)) return;
        if (marker) {
            marker.setLatLng([latitude, longitude]);
        } else {
            marker = window.L.marker([latitude, longitude], { draggable: true }).addTo(map);
            marker.on("dragend", () => {
                const position = marker.getLatLng();
                updateLocation(position.lat, position.lng, true);
            });
        }
        if (moveMap) map.setView([latitude, longitude], 16);
        void saveLocation(latitude, longitude);
        reverseGeocode(latitude, longitude);
    }

    function initializeMap() {
        const mapElement = document.getElementById("pharmacy-management-map");
        if (!mapElement || map || !window.L) return;

        const initialLatitude = parseCoordinate(mapElement.dataset.latitude);
        const initialLongitude = parseCoordinate(mapElement.dataset.longitude);
        const hasSavedLocation = isValidCoordinate(initialLatitude, initialLongitude);
        const latitude = hasSavedLocation ? initialLatitude : 35.6892;
        const longitude = hasSavedLocation ? initialLongitude : 51.3890;
        map = window.L.map(mapElement, { zoomControl: true })
            .setView([latitude, longitude], hasSavedLocation ? 16 : 12);
        window.pharmacyLocationMap = map;
        window.L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 19,
            attribution: "&copy; OpenStreetMap contributors"
        }).addTo(map);
        map.on("click", event => updateLocation(event.latlng.lat, event.latlng.lng, false));
        marker = window.L.marker([latitude, longitude], { draggable: true }).addTo(map);
        marker.on("dragend", () => {
            const position = marker.getLatLng();
            updateLocation(position.lat, position.lng, true);
        });
        if (hasSavedLocation) {
            showGeoStatus("info", "موقعیت ذخیره‌شده‌ی داروخانه روی نقشه نمایش داده شده است.");
            reverseGeocode(latitude, longitude);
        } else {
            showGeoStatus("info", "موقعیت پیش‌فرض تهران نمایش داده شده است؛ برای ثبت، محل را روی نقشه انتخاب کنید.");
        }
    }

    const currentLocationButton = root.querySelector("[data-get-current-location]");
    currentLocationButton?.addEventListener("click", () => {
        if (!navigator.geolocation) {
            showGeoStatus("error", "مرورگر شما از دریافت موقعیت پشتیبانی نمی‌کند.");
            return;
        }

        currentLocationButton.disabled = true;
        currentLocationButton.textContent = "در حال دریافت موقعیت...";
        navigator.geolocation.getCurrentPosition(
            position => {
                updateLocation(position.coords.latitude, position.coords.longitude, true);
                currentLocationButton.disabled = false;
                currentLocationButton.textContent = "دریافت موقعیت فعلی من";
            },
            error => {
                showGeoStatus("error", `خطا در دریافت موقعیت: ${error.message}`);
                currentLocationButton.disabled = false;
                currentLocationButton.textContent = "دریافت موقعیت فعلی من";
            },
            { enableHighAccuracy: true, timeout: 15000, maximumAge: 0 }
        );
    });
})();
