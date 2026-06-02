document.addEventListener("DOMContentLoaded", () => {
  initHomeTestimonials();
  initPortfolioCarousels();
});

function initHomeTestimonials() {
  document.querySelectorAll("[data-home-testimonials]").forEach(initHomeTestimonialsCarousel);
}

function initHomeTestimonialsCarousel(root) {
  const track = root.querySelector("[data-home-testimonials-track]");
  const slides = Array.from(track?.children ?? []);
  const prevButton = root.querySelector("[data-home-testimonials-prev]");
  const nextButton = root.querySelector("[data-home-testimonials-next]");
  const dots = Array.from(root.querySelectorAll(".home-carousel-dot"));
  let currentIndex = 0;

  if (!track || slides.length === 0) {
    return;
  }

  function visibleCount() {
    return window.matchMedia("(max-width: 760px)").matches ? 1 : 3;
  }

  function maxIndex() {
    return Math.max(0, slides.length - visibleCount());
  }

  function update() {
    currentIndex = Math.min(currentIndex, maxIndex());
    const slideWidth = slides[0].getBoundingClientRect().width;
    const gap = 18;
    const lastIndex = maxIndex();
    track.style.transform = `translateX(-${(slideWidth + gap) * currentIndex}px)`;

    dots.forEach((dot, index) => {
      dot.hidden = index > lastIndex;
      dot.classList.toggle("is-active", index === currentIndex);
    });
  }

  prevButton?.addEventListener("click", () => {
    currentIndex = currentIndex <= 0 ? maxIndex() : currentIndex - 1;
    update();
  });

  nextButton?.addEventListener("click", () => {
    currentIndex = currentIndex >= maxIndex() ? 0 : currentIndex + 1;
    update();
  });

  dots.forEach((dot, index) => {
    dot.addEventListener("click", () => {
      currentIndex = Math.min(index, maxIndex());
      update();
    });
  });

  window.addEventListener("resize", update);
  update();
}

function initPortfolioCarousels() {
  document.querySelectorAll("[data-portfolio-carousel]").forEach(initPortfolioCarousel);
}

function initPortfolioCarousel(root) {
  const track = root.querySelector("[data-portfolio-track]");
  const slides = Array.from(track?.children ?? []);
  const prevButton = root.querySelector("[data-portfolio-prev]");
  const nextButton = root.querySelector("[data-portfolio-next]");
  const dotsRoot = root.parentElement?.querySelector("[data-portfolio-dots]");
  const dots = Array.from(dotsRoot?.querySelectorAll(".builder-portfolio-dot") ?? []);
  let currentIndex = 0;

  if (!track || slides.length === 0) {
    return;
  }

  function visibleCount() {
    return window.matchMedia("(max-width: 760px)").matches ? 1 : 3;
  }

  function maxIndex() {
    return Math.max(0, slides.length - visibleCount());
  }

  function update() {
    currentIndex = Math.min(currentIndex, maxIndex());
    const slideWidth = slides[0].getBoundingClientRect().width;
    const gap = 24;
    const lastIndex = maxIndex();
    track.style.transform = `translateX(-${(slideWidth + gap) * currentIndex}px)`;

    dots.forEach((dot, index) => {
      dot.hidden = index > lastIndex;
      dot.classList.toggle("is-active", index === currentIndex);
    });
  }

  prevButton?.addEventListener("click", () => {
    currentIndex = currentIndex <= 0 ? maxIndex() : currentIndex - 1;
    update();
  });

  nextButton?.addEventListener("click", () => {
    currentIndex = currentIndex >= maxIndex() ? 0 : currentIndex + 1;
    update();
  });

  dots.forEach((dot, index) => {
    dot.addEventListener("click", () => {
      currentIndex = Math.min(index, maxIndex());
      update();
    });
  });

  window.addEventListener("resize", update);
  update();
}
