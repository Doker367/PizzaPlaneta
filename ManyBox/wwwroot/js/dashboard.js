window.initializeAnimations = function () {
    // Animaciones para los contadores
    const counters = document.querySelectorAll('.counter');
    counters.forEach(counter => {
        const target = parseInt(counter.getAttribute('data-target')) || 0;
        const duration = 800;
        const startTime = performance.now();
        const startValue = 0;

        function updateCounter(currentTime) {
            const elapsedTime = currentTime - startTime;
            if (elapsedTime < duration) {
                const progress = elapsedTime / duration;
                const eased = 1 - Math.pow(1 - progress, 3);
                counter.textContent = Math.floor(startValue + eased * (target - startValue));
                requestAnimationFrame(updateCounter);
            } else {
                counter.textContent = target;
            }
        }
        requestAnimationFrame(updateCounter);
    });

    // Animar las barras de progreso en la métrica
    setTimeout(() => {
        const chartBars = document.querySelectorAll('.chart-bar');
        chartBars.forEach(bar => {
            const height = bar.getAttribute('data-height');
            if (height) bar.style.height = height + '%';
        });
    }, 300);

    // Se elimina el efecto parallax/rotación para que las tarjetas no se muevan
};