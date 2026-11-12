(() => {
    const form = document.querySelector("[data-prescription-sms-form]");
    if (!form) return;

    const button = form.querySelector("[data-prescription-sms-submit]");
    const status = document.querySelector("[data-prescription-sms-status]");
    let isSubmitting = false;

    form.addEventListener("submit", (event) => {
        if (isSubmitting) {
            event.preventDefault();
            return;
        }

        isSubmitting = true;
        button.disabled = true;
        button.setAttribute("aria-busy", "true");
        button.textContent = "در حال ارسال پیامک…";
        status.textContent = "درخواست ارسال پیامک در حال انجام است؛ صفحه را نبندید.";
    });
})();
