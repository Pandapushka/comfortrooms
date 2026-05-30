lucide.createIcons();

document.addEventListener("DOMContentLoaded", function () {
  const observer = new IntersectionObserver((entries, sectionObserver) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add("is-visible");
        sectionObserver.unobserve(entry.target);
      }
    });
  }, { root: null, rootMargin: "0px", threshold: 0.1 });

  document.querySelectorAll(".fade-in-section").forEach(section => observer.observe(section));
  initCarousel();
});

function initCarousel() {
  const track = document.getElementById("carouselTrack");
  if (!track) {
    return;
  }

  const slides = Array.from(track.children);
  const nextBtn = document.getElementById("nextBtn");
  const prevBtn = document.getElementById("prevBtn");
  const dots = Array.from(document.querySelectorAll(".dot"));
  let currentIndex = 0;

  if (slides.length === 0 || !nextBtn || !prevBtn) {
    return;
  }

  function updateCarousel() {
    const slideWidth = slides[0].getBoundingClientRect().width;
    const gap = 32;
    track.style.transform = `translateX(-${(slideWidth + gap) * currentIndex}px)`;

    dots.forEach(dot => {
      dot.classList.remove("bg-yellow-700");
      dot.classList.add("bg-gray-300");
    });

    if (dots[currentIndex]) {
      dots[currentIndex].classList.remove("bg-gray-300");
      dots[currentIndex].classList.add("bg-yellow-700");
    }
  }

  nextBtn.addEventListener("click", () => {
    currentIndex = currentIndex === slides.length - 1 ? 0 : currentIndex + 1;
    updateCarousel();
  });

  prevBtn.addEventListener("click", () => {
    currentIndex = currentIndex === 0 ? slides.length - 1 : currentIndex - 1;
    updateCarousel();
  });

  dots.forEach((dot, index) => {
    dot.addEventListener("click", () => {
      currentIndex = index;
      updateCarousel();
    });
  });

  window.addEventListener("resize", updateCarousel);
  updateCarousel();
}
