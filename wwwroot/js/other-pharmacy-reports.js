(() => {
  const section = document.querySelector(".other-pharmacy-reports");
  if (!section) return;

  const provinceSelect = section.querySelector("#other-pharmacy-province");
  const citySelect = section.querySelector("#other-pharmacy-city");
  const status = section.querySelector(".other-pharmacy-filter-status");
  const citiesUrl = section.dataset.citiesUrl;
  if (!provinceSelect || !citySelect || !citiesUrl) return;

  let pendingRequest;

  provinceSelect.addEventListener("change", async () => {
    pendingRequest?.abort();
    const request = new AbortController();
    pendingRequest = request;
    const provinceId = Number(provinceSelect.value);
    citySelect.replaceChildren(new Option("همه‌ی شهرها", "0"));
    citySelect.value = "0";
    citySelect.disabled = provinceId <= 0;
    if (status) status.textContent = "";
    if (provinceId <= 0) return;

    citySelect.disabled = true;
    if (status) status.textContent = "در حال دریافت فهرست شهرها…";
    try {
      const url = new URL(citiesUrl, window.location.origin);
      url.searchParams.set("provinceCatId", String(provinceId));
      const response = await fetch(url, { signal: request.signal });
      if (!response.ok) {
        throw new Error(`City list request failed with status ${response.status}.`);
      }

      const result = await response.json();
      for (const city of result.items ?? []) {
        citySelect.add(new Option(city.name, String(city.id)));
      }
      if (status) status.textContent = "";
    } catch (error) {
      if (error.name === "AbortError") return;
      console.error("Could not load cities for other pharmacy reports.", error);
      if (status) status.textContent = "دریافت فهرست شهرها ممکن نشد؛ دوباره تلاش کنید.";
    } finally {
      if (pendingRequest === request) {
        citySelect.disabled = false;
      }
    }
  });
})();
