document.addEventListener("DOMContentLoaded", function () {
  const menuButton = document.querySelector("[data-menu-toggle]");
  const mobileMenu = document.getElementById("mobile-menu");

  if (menuButton && mobileMenu) {
    menuButton.addEventListener("click", () => {
      const isOpen = mobileMenu.classList.toggle("is-open");
      menuButton.setAttribute("aria-expanded", String(isOpen));
    });
  }

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
      dot.classList.remove("is-active");
    });

    if (dots[currentIndex]) {
      dots[currentIndex].classList.add("is-active");
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
