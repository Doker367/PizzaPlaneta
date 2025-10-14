// firmaCanvas.js - Canvas de firma responsivo para Blazor MAUI
window.firmaCanvas = {
    init: function (canvasId) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        let drawing = false;
        let lastX = 0, lastY = 0;
        let hasDrawn = false;

        // Ajustar tamaño responsivo
        function resizeCanvas() {
            const ratio = Math.max(window.devicePixelRatio || 1, 1);
            const width = canvas.parentElement.offsetWidth;
            const height = Math.max(180, Math.round(width * 0.5));
            canvas.width = width * ratio;
            canvas.height = height * ratio;
            canvas.style.width = width + 'px';
            canvas.style.height = height + 'px';
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            ctx.scale(ratio, ratio);
            ctx.fillStyle = '#fff';
            ctx.fillRect(0, 0, width, height);
        }
        resizeCanvas();
        window.addEventListener('resize', resizeCanvas);

        function getPos(e) {
            if (e.touches && e.touches.length > 0) {
                const rect = canvas.getBoundingClientRect();
                return {
                    x: (e.touches[0].clientX - rect.left),
                    y: (e.touches[0].clientY - rect.top)
                };
            } else {
                const rect = canvas.getBoundingClientRect();
                return {
                    x: (e.clientX - rect.left),
                    y: (e.clientY - rect.top)
                };
            }
        }

        function startDraw(e) {
            e.preventDefault();
            drawing = true;
            hasDrawn = true;
            const pos = getPos(e);
            lastX = pos.x;
            lastY = pos.y;
        }
        function draw(e) {
            if (!drawing) return;
            e.preventDefault();
            const pos = getPos(e);
            ctx.strokeStyle = '#222';
            ctx.lineWidth = 2;
            ctx.lineCap = 'round';
            ctx.beginPath();
            ctx.moveTo(lastX, lastY);
            ctx.lineTo(pos.x, pos.y);
            ctx.stroke();
            lastX = pos.x;
            lastY = pos.y;
        }
        function endDraw(e) {
            drawing = false;
        }
        // Mouse
        canvas.onmousedown = startDraw;
        canvas.onmousemove = draw;
        canvas.onmouseup = endDraw;
        canvas.onmouseleave = endDraw;
        // Touch
        canvas.ontouchstart = startDraw;
        canvas.ontouchmove = draw;
        canvas.ontouchend = endDraw;
        canvas.ontouchcancel = endDraw;

        // Limpia el canvas
        canvas.clearFirma = function () {
            ctx.fillStyle = '#fff';
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            hasDrawn = false;
        };
        // Devuelve la imagen base64
        canvas.getFirmaBase64 = function () {
            return hasDrawn ? canvas.toDataURL('image/png') : null;
        };
    },
    clear: function (canvasId) {
        const canvas = document.getElementById(canvasId);
        if (canvas && canvas.clearFirma) canvas.clearFirma();
    },
    getBase64: function (canvasId) {
        const canvas = document.getElementById(canvasId);
        if (canvas && canvas.getFirmaBase64) return canvas.getFirmaBase64();
        return null;
    }
};