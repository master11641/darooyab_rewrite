document.addEventListener("error", (event) => {
    const image = event.target;
    if (!(image instanceof HTMLImageElement) || !image.matches(".news-card-image, .news-archive-image, .article-image")) {
        return;
    }

    const isArticleImage = image.classList.contains("article-image");
    const isArchiveImage = image.classList.contains("news-archive-image");
    const fallback = document.createElement("div");
    fallback.className = isArticleImage
        ? "article-image article-image-placeholder"
        : isArchiveImage
            ? "news-archive-image news-archive-image-placeholder"
            : "news-card-image news-card-placeholder";
    fallback.setAttribute("role", "img");
    fallback.setAttribute("aria-label", image.alt || "تصویر مطلب در دسترس نیست");
    fallback.textContent = isArticleImage ? "تصویر مطلب در دسترس نیست" : "دارویاب";
    image.replaceWith(fallback);
}, true);
