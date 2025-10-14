using System;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace ManyBox.Components.Pages.Operaciones
{
    public partial class ConsultarCostos
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private int currentStep = 1;
        private string origen = "";
        private string destino = "";
        private decimal distancia;
        private decimal peso;
        private decimal largo;
        private decimal ancho;
        private decimal alto;
        private decimal costo;
        private decimal pesoVolumetrico;
        private string tipoServicio = "Estándar";
        private string tipoEntrega = "Domicilio";
        private bool conSeguro = false;

        private void NextStep()
        {
            if (currentStep < 3)
            {
                currentStep++;
                CalcularPesoVolumetrico();
            }
        }

        private void PreviousStep()
        {
            if (currentStep > 1)
            {
                currentStep--;
            }
        }

        private void CalcularPesoVolumetrico()
        {
            if (largo > 0 && ancho > 0 && alto > 0)
            {
                pesoVolumetrico = (largo * ancho * alto) / 5000;
            }
        }

        private void CalcularCosto()
        {
            CalcularPesoVolumetrico();
            var pesoFinal = Math.Max(peso, pesoVolumetrico);

            var tarifaBase = tipoServicio == "Exprés" ? 20m : 10m;
            var costoBase = tarifaBase * pesoFinal + (distancia * 2m);

            if (conSeguro)
                costoBase += 50m;

            if (tipoEntrega == "Ocurre")
                costoBase -= 10m;

            costo = Math.Round(costoBase, 2);
        }

        private void GuardarCotizacion()
        {
            // Implementar lógica para guardar cotización
            JSRuntime.InvokeVoidAsync("alert", "Cotización guardada correctamente");
        }

        private void IrARegistro()
        {
            // Implementar navegación a página de registro con datos precargados
            JSRuntime.InvokeVoidAsync("alert", "Redirigiendo a registro de envío...");
        }
    }
}
