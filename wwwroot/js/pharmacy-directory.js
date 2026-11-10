(() => {
  const form = document.querySelector(".pharmacy-filter-panel");
  if (!form) return;

  const province = form.querySelector("#ProvinceId");
  const city = form.querySelector("#CityId");
  const district = form.querySelector("#DistrictId");
  const status = form.querySelector("#pharmacy-filter-status, #availability-filter-status");
  const cityUrl = form.dataset.citiesUrl;
  const districtUrl = form.dataset.districtsUrl;
  const requests = new Map();

  const replaceOptions = (select, items, placeholder) => {
    select.replaceChildren(new Option(placeholder, "0"));
    for (const item of items) {
      select.add(new Option(item.name, String(item.id)));
    }
  };

  const loadOptions = async (select, endpoint, parameter, value, placeholder, requestName) => {
    requests.get(requestName)?.abort();
    const request = new AbortController();
    requests.set(requestName, request);
    select.disabled = true;
    status.textContent = "در حال دریافت فهرست…";

    const url = new URL(endpoint, window.location.origin);
    url.searchParams.set(parameter, value);

    try {
      const response = await fetch(url, {
        headers: { Accept: "application/json" },
        signal: request.signal
      });
      if (!response.ok) {
        throw new Error(`Request failed with status ${response.status}`);
      }

      const result = await response.json();
      replaceOptions(select, result.items, placeholder);
      select.disabled = false;
      status.textContent = "";
    } catch (error) {
      if (error.name === "AbortError") return;
      replaceOptions(select, [], placeholder);
      status.textContent = "دریافت فهرست ممکن نشد. دوباره تلاش کنید.";
      status.classList.add("is-error");
    } finally {
      if (requests.get(requestName) === request) {
        requests.delete(requestName);
        if (status.textContent === "در حال دریافت فهرست…") {
          status.textContent = "";
        }
      }
    }
  };

  province.addEventListener("change", () => {
    requests.get("cities")?.abort();
    requests.get("districts")?.abort();
    replaceOptions(city, [], "ابتدا شهر را انتخاب کنید");
    city.disabled = true;
    replaceOptions(district, [], "ابتدا شهر را انتخاب کنید");
    district.disabled = true;
    status.classList.remove("is-error");

    if (province.value === "0") {
      replaceOptions(city, [], "ابتدا استان را انتخاب کنید");
      status.textContent = "";
      return;
    }

    loadOptions(city, cityUrl, "provinceCatId", province.value, "همه‌ی شهرها", "cities");
  });

  city.addEventListener("change", () => {
    requests.get("districts")?.abort();
    replaceOptions(district, [], "ابتدا شهر را انتخاب کنید");
    district.disabled = true;
    status.classList.remove("is-error");

    if (city.value === "0") {
      status.textContent = "";
      return;
    }

    loadOptions(district, districtUrl, "selectedCityId", city.value, "همه‌ی مناطق", "districts");
  });
})();
