(() => {
    const form = document.querySelector("[data-brand-suggestion-form]");
    const message = document.querySelector("[data-brand-suggestion-message]");
    const submitButton = document.querySelector("[data-submit-button]");

    if (!(form instanceof HTMLFormElement) ||
        !(message instanceof HTMLElement) ||
        !(submitButton instanceof HTMLButtonElement)) {
        return;
    }

    const input = form.querySelector("[name='PersianBrandName']");
    if (!(input instanceof HTMLInputElement)) {
        return;
    }

    form.addEventListener("submit", async event => {
        event.preventDefault();
        message.textContent = "";
        delete message.dataset.state;
        submitButton.disabled = true;
        let failureMessage = "ارسال پیشنهاد انجام نشد؛ ارتباط با سامانه را بررسی و دوباره تلاش کنید.";

        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                headers: { Accept: "application/json" }
            });

            if (response.redirected &&
                new URL(response.url, window.location.href).pathname.toLowerCase() === "/ph/login") {
                window.location.assign("/ph/login");
                return;
            }

            let result;
            try {
                result = await response.json();
            } catch {
                throw new Error();
            }

            if (!response.ok || result.success !== true) {
                failureMessage = typeof result.message === "string"
                    ? result.message
                    : failureMessage;
                throw new Error();
            }

            message.textContent = typeof result.message === "string"
                ? result.message
                : "پیشنهاد برند برای بررسی ارجاع شد.";
            message.dataset.state = "success";
            input.value = "";
            input.focus();
        } catch {
            message.textContent = failureMessage;
            message.dataset.state = "error";
        } finally {
            submitButton.disabled = false;
        }
    });
})();
