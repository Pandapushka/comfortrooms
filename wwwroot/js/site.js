document.addEventListener("DOMContentLoaded", () => {
  initHomeTestimonials();
});

function initHomeTestimonials() {
  const root = document.querySelector("[data-home-testimonials]");
  if (!root) {
    return;
  }

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
    track.style.transform = `translateX(-${(slideWidth + gap) * currentIndex}px)`;

    dots.forEach((dot, index) => {
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
