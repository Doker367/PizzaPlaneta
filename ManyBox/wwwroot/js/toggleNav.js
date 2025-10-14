
window.toggleNav = function() {
    const navMenu = document.getElementById('navMenu');
    const toggle = document.getElementById('navbar-toggler');
    if (navMenu && toggle) {
        navMenu.classList.toggle('mobile-open');
        toggle.classList.toggle('active');
    }
}

// Cerrar menú al hacer clic en un enlace (móvil)
document.addEventListener('DOMContentLoaded', function() {
    const navLinks = document.querySelectorAll('.nav-item');
    const navMenu = document.getElementById('navMenu');
    const toggle = document.getElementById('navbar-toggler');

    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth <= 768) {
                navMenu?.classList.remove('mobile-open');
                toggle?.classList.remove('active');
            }
        });
    });

    // Cerrar menú al redimensionar ventana
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768) {
            navMenu?.classList.remove('mobile-open');
            toggle?.classList.remove('active');
        }
    });
});
