const sidebarToggle = document.getElementById("sidebarToggle");
const sidebarMobileQuery = window.matchMedia("(max-width: 992px)");

const setSidebarOpen = (isOpen) => {
  document.body.classList.toggle("sidebar-open", isOpen);
  sidebarToggle?.setAttribute("aria-expanded", String(isOpen));
};

const closeSidebar = () => {
  setSidebarOpen(false);
};

if (sidebarToggle) {
  sidebarToggle.addEventListener("click", () => {
    setSidebarOpen(!document.body.classList.contains("sidebar-open"));
  });
}

document.querySelectorAll(".sidebar a").forEach((link) => {
  link.addEventListener("click", () => {
    closeSidebar();
  });
});

window.addEventListener(
  "scroll",
  () => {
    if (document.body.classList.contains("sidebar-open")) {
      closeSidebar();
    }
  },
  { capture: true, passive: true }
);

const closeSidebarOnDesktop = (event) => {
  if (!event.matches) {
    closeSidebar();
  }
};

if (sidebarMobileQuery.addEventListener) {
  sidebarMobileQuery.addEventListener("change", closeSidebarOnDesktop);
} else {
  sidebarMobileQuery.addListener(closeSidebarOnDesktop);
}

document.querySelectorAll("[data-password-toggle]").forEach((button) => {
  button.addEventListener("click", () => {
    const input = document.getElementById(button.dataset.passwordToggle);
    const icon = button.querySelector("i");

    if (!input) {
      return;
    }

    const shouldShowPassword = input.type === "password";
    input.type = shouldShowPassword ? "text" : "password";
    button.setAttribute("aria-label", shouldShowPassword ? "Ocultar senha" : "Mostrar senha");
    icon?.classList.toggle("fa-eye", shouldShowPassword);
    icon?.classList.toggle("fa-eye-slash", !shouldShowPassword);
  });
});
