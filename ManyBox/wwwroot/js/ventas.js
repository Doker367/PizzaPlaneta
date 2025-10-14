window.applyVentasStyles = function () {
    // Comprobar si los estilos ya existen para evitar duplicados
    if (!document.getElementById('ventas-custom-styles')) {
        const styleElement = document.createElement('style');
        styleElement.id = 'ventas-custom-styles';
        styleElement.textContent = `
            .ventas-container {
                width: 100%;
                max-width: 100%;
                margin: 0;
                padding: 0;
                color: #fff;
            }
            
            .ventas-dashboard {
                padding: 0;
                color: #fff;
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                max-width: 100%;
                margin: 0 auto;
            }
            
            .ventas-dashboard .dashboard-header {
                display: flex;
                align-items: center;
                justify-content: center;
                margin-bottom: 2rem;
                padding: 1rem 0;
                text-align: center;
            }
            
            .ventas-dashboard .header-icon-container {
                margin-right: 1rem;
            }
            
            .ventas-dashboard .header-icon {
                font-size: 2rem;
                background: rgba(255, 255, 255, 0.1);
                border-radius: 50%;
                width: 60px;
                height: 60px;
                display: flex;
                align-items: center;
                justify-content: center;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
            }
            
            .ventas-dashboard .dashboard-title {
                font-size: 2.5rem;
                font-weight: 600;
                margin: 0;
                background: linear-gradient(45deg, #a5b4fc, #6366f1);
                -webkit-background-clip: text;
                -webkit-text-fill-color: transparent;
            }
            
            .ventas-dashboard .loading-container {
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                padding: 3rem;
                background: rgba(30, 36, 49, 0.8);
                border-radius: 16px;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
                color: #fff;
            }
            
            .ventas-dashboard .loading-spinner {
                border: 4px solid rgba(255, 255, 255, 0.2);
                border-left-color: #6366f1;
                border-radius: 50%;
                width: 40px;
                height: 40px;
                animation: spin 1s linear infinite;
                margin-bottom: 1rem;
            }
            
            @keyframes spin {
                to { transform: rotate(360deg); }
            }
            
            .ventas-dashboard .empty-state {
                text-align: center;
                padding: 3rem;
                background: rgba(30, 36, 49, 0.8);
                border-radius: 16px;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
            }
            
            .ventas-dashboard .empty-state i {
                font-size: 3rem;
                color: #6366f1;
                margin-bottom: 1rem;
            }
            
            .ventas-dashboard .empty-state p {
                font-size: 1.2rem;
                color: #fff;
                opacity: 0.8;
            }
            
            .ventas-dashboard .data-card {
                background: rgba(30, 36, 49, 0.8);
                border-radius: 16px;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
                overflow: hidden;
                border: 1px solid rgba(255, 255, 255, 0.1);
            }
            
            .ventas-dashboard .card-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 1.5rem;
                border-bottom: 1px solid rgba(255, 255, 255, 0.1);
                background: rgba(30, 36, 49, 1);
            }
            
            .ventas-dashboard .card-header h2 {
                font-size: 1.3rem;
                font-weight: 600;
                margin: 0;
                color: #fff;
            }
            
            .ventas-dashboard .last-update {
                font-size: 0.85rem;
                color: #a0aec0;
                display: flex;
                align-items: center;
                gap: 0.5rem;
            }
            
            .ventas-dashboard .table-container {
                overflow-x: auto;
            }
            
            .ventas-dashboard .data-table {
                width: 100%;
                border-collapse: collapse;
            }
            
            .ventas-dashboard .data-table th,
            .ventas-dashboard .data-table td {
                padding: 1rem 1.2rem;
                text-align: left;
                border-bottom: 1px solid rgba(255, 255, 255, 0.1);
            }
            
            .ventas-dashboard .data-table th {
                font-weight: 600;
                color: #a0aec0;
                background-color: rgba(26, 32, 44, 0.5);
                font-size: 0.9rem;
                text-transform: uppercase;
                letter-spacing: 0.05em;
            }
            
            .ventas-dashboard .data-table tbody tr {
                transition: background-color 0.2s;
            }
            
            .ventas-dashboard .data-table tbody tr:nth-child(odd) {
                background-color: rgba(26, 32, 44, 0.3);
            }
            
            .ventas-dashboard .data-table tbody tr:hover {
                background-color: rgba(99, 102, 241, 0.1);
            }
            
            .ventas-dashboard .view-button {
                background-color: #6366f1;
                color: white;
                border: none;
                border-radius: 8px;
                padding: 0.5rem 1rem;
                font-size: 0.9rem;
                font-weight: 600;
                cursor: pointer;
                transition: all 0.2s;
            }
            
            .ventas-dashboard .view-button:hover {
                background-color: #4f46e5;
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(99, 102, 241, 0.3);
            }
            
            .ventas-dashboard .sucursal-cell {
                text-align: center;
                font-weight: 600;
            }
            
            .ventas-dashboard .card-footer {
                display: flex;
                justify-content: space-between;
                padding: 1.5rem;
                background-color: rgba(26, 32, 44, 0.5);
                border-top: 1px solid rgba(255, 255, 255, 0.1);
            }
            
            .ventas-dashboard .summary-item {
                display: flex;
                flex-direction: column;
                gap: 0.3rem;
            }
            
            .ventas-dashboard .summary-label {
                font-size: 0.9rem;
                color: #a0aec0;
                font-weight: 500;
            }
            
            .ventas-dashboard .summary-value {
                font-size: 1.4rem;
                font-weight: 700;
                color: #fff;
            }
            
            /* Modal */
            .ventas-modal-backdrop {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(0, 0, 0, 0.7);
                backdrop-filter: blur(4px);
                z-index: 1000;
                display: flex;
                align-items: center;
                justify-content: center;
                animation: fadeIn 0.2s ease-out;
            }
            
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
            
            .ventas-modal {
                background: #1e2431;
                border-radius: 16px;
                width: 90%;
                max-width: 600px;
                max-height: 80vh;
                box-shadow: 0 25px 50px rgba(0, 0, 0, 0.3);
                overflow: hidden;
                animation: slideUp 0.3s ease-out;
                display: flex;
                flex-direction: column;
            }
            
            @keyframes slideUp {
                from {
                    opacity: 0;
                    transform: translateY(30px);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
            }
            
            .ventas-modal .modal-header {
                background: linear-gradient(90deg, #6366f1, #8b5cf6);
                padding: 1.5rem;
                display: flex;
                align-items: center;
                position: relative;
            }
            
            .ventas-modal .modal-icon {
                background-color: rgba(0, 0, 0, 0.2);
                color: #fff;
                width: 50px;
                height: 50px;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 1.5rem;
                margin-right: 1rem;
            }
            
            .ventas-modal .modal-title {
                flex: 1;
            }
            
            .ventas-modal .modal-title h2 {
                margin: 0;
                font-size: 1.4rem;
                font-weight: 600;
                color: #fff;
            }
            
            .ventas-modal .modal-subtitle {
                color: rgba(255, 255, 255, 0.8);
                font-size: 0.95rem;
                margin-top: 0.3rem;
            }
            
            .ventas-modal .modal-close {
                background: rgba(255, 255, 255, 0.2);
                border: none;
                color: #fff;
                border-radius: 50%;
                width: 36px;
                height: 36px;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 1rem;
                cursor: pointer;
                transition: background-color 0.2s;
                position: absolute;
                right: 1rem;
                top: 1rem;
            }
            
            .ventas-modal .modal-close:hover {
                background: rgba(255, 255, 255, 0.3);
            }
            
            .ventas-modal .modal-body {
                padding: 1.5rem;
                overflow-y: auto;
                flex: 1;
                scrollbar-width: thin;
                scrollbar-color: rgba(99, 102, 241, 0.5) rgba(30, 36, 49, 0.3);
            }
            
            /* Estilos para la barra de desplazamiento en Chrome/Safari */
            .ventas-modal .modal-body::-webkit-scrollbar {
                width: 8px;
            }
            
            .ventas-modal .modal-body::-webkit-scrollbar-track {
                background: rgba(30, 36, 49, 0.3);
                border-radius: 4px;
            }
            
            .ventas-modal .modal-body::-webkit-scrollbar-thumb {
                background: rgba(99, 102, 241, 0.5);
                border-radius: 4px;
            }
            
            .ventas-modal .modal-body::-webkit-scrollbar-thumb:hover {
                background: rgba(99, 102, 241, 0.8);
            }
            
            /* Indicador de desplazamiento */
            .ventas-modal .scroll-indicator {
                position: absolute;
                right: 10px;
                top: 50%;
                transform: translateY(-50%);
                width: 4px;
                height: 60px;
                background: rgba(99, 102, 241, 0.3);
                border-radius: 4px;
                opacity: 0.6;
                pointer-events: none;
                transition: opacity 0.3s ease;
            }
            
            .ventas-modal .scroll-indicator::after {
                content: '';
                position: absolute;
                width: 100%;
                height: 30%;
                background: #6366f1;
                border-radius: 4px;
                top: 0;
                left: 0;
                animation: scrollPulse 2s infinite ease-in-out;
            }
            
            @keyframes scrollPulse {
                0% { top: 0; }
                50% { top: 70%; }
                100% { top: 0; }
            }
            
            .ventas-modal .detail-grid {
                display: grid;
                grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
                gap: 1.5rem;
                margin-bottom: 1.5rem;
            }
            
            .ventas-modal .detail-item {
                display: flex;
                flex-direction: column;
                gap: 0.5rem;
            }
            
            .ventas-modal .detail-label {
                font-size: 0.9rem;
                color: #a0aec0;
                font-weight: 500;
            }
            
            .ventas-modal .detail-value {
                font-size: 1.1rem;
                font-weight: 600;
                color: #fff;
            }
            
            .ventas-modal .detail-value.highlight {
                color: #10b981;
                font-size: 1.25rem;
            }
            
            .ventas-modal .badge {
                padding: 0.3rem 0.8rem;
                border-radius: 2rem;
                font-size: 0.85rem;
                font-weight: 600;
                display: inline-block;
            }
            
            .ventas-modal .badge-success {
                background-color: rgba(16, 185, 129, 0.2);
                color: #10b981;
                border: 1px solid rgba(16, 185, 129, 0.3);
            }
            
            .ventas-modal .badge-danger {
                background-color: rgba(239, 68, 68, 0.2);
                color: #ef4444;
                border: 1px solid rgba(239, 68, 68, 0.3);
            }
            
            .ventas-modal .detail-section {
                background-color: rgba(26, 32, 44, 0.5);
                border-radius: 12px;
                margin-bottom: 1.5rem;
                overflow: hidden;
            }
            
            .ventas-modal .section-header {
                background-color: rgba(26, 32, 44, 0.8);
                padding: 1rem 1.5rem;
                display: flex;
                align-items: center;
                gap: 0.8rem;
            }
            
            .ventas-modal .section-header i {
                color: #6366f1;
            }
            
            .ventas-modal .section-header h3 {
                margin: 0;
                font-size: 1.1rem;
                font-weight: 600;
                color: #fff;
            }
            
            .ventas-modal .section-content {
                padding: 1.2rem 1.5rem;
                color: #e2e8f0;
                line-height: 1.5;
            }
            
            .ventas-modal .content-list {
                margin: 0.5rem 0 0 1.5rem;
                padding: 0;
            }
            
            .ventas-modal .content-list li {
                margin: 0.5rem 0;
                color: #e2e8f0;
            }
            
            .ventas-modal .modal-footer {
                padding: 1.5rem;
                border-top: 1px solid rgba(255, 255, 255, 0.1);
                display: flex;
                justify-content: flex-end;
            }
            
            .ventas-modal .button-primary {
                background-color: #6366f1;
                color: white;
                border: none;
                border-radius: 8px;
                padding: 0.8rem 1.5rem;
                font-size: 1rem;
                font-weight: 600;
                cursor: pointer;
                transition: all 0.2s;
            }
            
            .ventas-modal .button-primary:hover {
                background-color: #4f46e5;
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(99, 102, 241, 0.3);
            }
            
            /* Media queries */
            @media (max-width: 768px) {
                .ventas-dashboard .dashboard-header {
                    flex-direction: column;
                    gap: 1rem;
                }
            
                .ventas-dashboard .card-header {
                    flex-direction: column;
                    align-items: flex-start;
                    gap: 0.5rem;
                }
            
                .ventas-modal .detail-grid {
                    grid-template-columns: 1fr;
                    gap: 1rem;
                }
            
                .ventas-modal {
                    width: 95%;
                    max-height: 90vh;
                    overflow-y: auto;
                }
            
                .ventas-dashboard .card-footer {
                    flex-direction: column;
                    gap: 1rem;
                }
            }
            
            /* Media query específico para pantallas más grandes */
            @media (min-width: 769px) {
                .ventas-dashboard .dashboard-header {
                    padding: 2rem 0;
                }
                
                .ventas-dashboard .header-icon {
                    width: 70px;
                    height: 70px;
                    font-size: 2.5rem;
                }
                
                .ventas-dashboard .dashboard-title {
                    font-size: 3rem;
                }
                
                .ventas-dashboard .data-table th,
                .ventas-dashboard .data-table td {
                    padding: 1.2rem 1.5rem;
                }
                
                .ventas-dashboard .card-footer {
                    padding: 2rem;
                }
                
                .ventas-dashboard .summary-value {
                    font-size: 1.8rem;
                }
            }
            
            @media (max-width: 480px) {
                .ventas-dashboard .dashboard-title {
                    font-size: 2rem;
                }
            
                .ventas-dashboard .header-icon {
                    width: 50px;
                    height: 50px;
                    font-size: 1.5rem;
                }
            
                .ventas-dashboard .data-table th,
                .ventas-dashboard .data-table td {
                    padding: 0.8rem;
                    font-size: 0.9rem;
                }
            
                .ventas-dashboard .view-button {
                    padding: 0.4rem 0.8rem;
                    font-size: 0.85rem;
                }
            
                .ventas-modal .modal-header {
                    padding: 1.2rem;
                }
            
                .ventas-modal .modal-icon {
                    width: 40px;
                    height: 40px;
                    font-size: 1.2rem;
                }
            
                .ventas-modal .modal-body {
                    padding: 1.2rem;
                }
            }
        `;
        document.head.appendChild(styleElement);
    }

    // Función para manejar el desplazamiento del modal
    function setupModalScroll() {
        const modalBody = document.querySelector('.ventas-modal .modal-body');
        if (modalBody) {
            // Añadir un poco de retraso para que el modal esté completamente renderizado
            setTimeout(() => {
                // Verificar si el contenido es más alto que el contenedor
                const hasScroll = modalBody.scrollHeight > modalBody.clientHeight;
                const indicator = modalBody.querySelector('.scroll-indicator');

                if (indicator) {
                    if (hasScroll) {
                        indicator.style.display = 'block';

                        // Hacer que el indicador desaparezca después de 3 segundos
                        setTimeout(() => {
                            indicator.style.opacity = '0';
                        }, 3000);

                        // Mostrar el indicador cuando el usuario haga hover en el modal
                        modalBody.addEventListener('mouseenter', () => {
                            indicator.style.opacity = '0.6';
                        });

                        // Ocultar el indicador cuando el usuario salga del modal
                        modalBody.addEventListener('mouseleave', () => {
                            indicator.style.opacity = '0';
                        });
                    } else {
                        indicator.style.display = 'none';
                    }
                }
            }, 300);
        }
    }

    // Observar cambios en el DOM para detectar cuando se abre un modal
    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.addedNodes && mutation.addedNodes.length > 0) {
                for (let i = 0; i < mutation.addedNodes.length; i++) {
                    const node = mutation.addedNodes[i];
                    if (node.classList && node.classList.contains('ventas-modal-backdrop')) {
                        setupModalScroll();
                        break;
                    }
                }
            }
        });
    });

    // Iniciar la observación del documento
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
};

// Hacer disponible la función setupModalScroll para llamadas desde Blazor
window.setupModalScroll = function () {
    const modalBody = document.querySelector('.ventas-modal .modal-body');
    if (modalBody) {
        // Verificar si el contenido es más alto que el contenedor
        const hasScroll = modalBody.scrollHeight > modalBody.clientHeight;
        const indicator = modalBody.querySelector('.scroll-indicator');

        if (indicator) {
            if (hasScroll) {
                indicator.style.display = 'block';

                // Hacer que el indicador desaparezca después de 3 segundos
                setTimeout(() => {
                    indicator.style.opacity = '0';
                }, 3000);

                // Mostrar el indicador cuando el usuario haga hover o toque en el modal
                modalBody.addEventListener('mouseenter', () => {
                    indicator.style.opacity = '0.6';
                });

                // Para dispositivos táctiles
                modalBody.addEventListener('touchstart', () => {
                    indicator.style.opacity = '0.6';
                    // Ocultar después de un tiempo
                    setTimeout(() => {
                        indicator.style.opacity = '0';
                    }, 1500);
                });

                // Ocultar el indicador cuando el usuario salga del modal
                modalBody.addEventListener('mouseleave', () => {
                    indicator.style.opacity = '0';
                });
            } else {
                indicator.style.display = 'none';
            }
        }
    }
};