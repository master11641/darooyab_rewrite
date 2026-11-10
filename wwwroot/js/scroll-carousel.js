document.querySelectorAll("[data-home-carousel]").forEach((carousel) => {
    const track = carousel.querySelector("[data-carousel-track]");
    if (!track) {
        return;
    }

    const scrollTrack = (direction) => {
        const firstItem = track.firstElementChild;
        const distance = firstItem
            ? firstItem.getBoundingClientRect().width + parseFloat(getComputedStyle(track).gap || "0")
            : track.clientWidth;

        track.scrollBy({
            left: direction * distance,
            behavior: window.matchMedia("(prefers-reduced-motion: reduce)").matches ? "instant" : "smooth"
        });
    };

    const controls = carousel.dataset.homeCarousel
        ? document.querySelector(`[data-carousel-controls="${carousel.dataset.homeCarousel}"]`)
        : carousel;
    const nextButton = controls?.querySelector("[data-carousel-next]");
    const previousButton = controls?.querySelector("[data-carousel-previous]");

    const updateControls = () => {
        if (!controls) {
            return;
        }

        const maxScroll = track.scrollWidth - track.clientWidth;
        const canScroll = maxScroll > 1;
        controls.hidden = !canScroll;

        if (canScroll) {
            const currentScroll = Math.abs(track.scrollLeft);
            nextButton.disabled = currentScroll >= maxScroll - 2;
            previousButton.disabled = currentScroll <= 2;
        }
    };

    nextButton?.addEventListener("click", () => scrollTrack(-1));
    previousButton?.addEventListener("click", () => scrollTrack(1));
    track.addEventListener("scroll", updateControls, { passive: true });
    window.addEventListener("resize", updateControls, { passive: true });
    updateControls();
});
