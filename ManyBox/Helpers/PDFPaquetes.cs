using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ManyBox.Helpers
{
    public class PDFPaquetes
    {
        public byte[] GenerarComprobante(PaquetePdfData data, byte[]? logoBytes = null)
        {
            var labelStyle = TextStyle.Default.SemiBold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
            var valueStyle = TextStyle.Default.FontColor(QuestPDF.Helpers.Colors.Black);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Row(row =>
                    {
                        row.ConstantColumn(64).Height(64).AlignMiddle().AlignLeft().Element(e =>
                        {
                            if (logoBytes is not null)
                            {
                                e.Image(logoBytes);
                            }
                            else
                            {
                                e.Background(QuestPDF.Helpers.Colors.Grey.Lighten3);
                                e.AlignCenter();
                                e.AlignMiddle();
                                e.Text("LOGO").SemiBold();
                            }
                        });

                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("Comprobante de Envío").FontSize(20).SemiBold();
                            col.Item().Text($"Código de seguimiento: {data.CodigoSeguimiento}")
                               .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                        });

                        row.ConstantColumn(140)
                           .Border(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                           .Padding(8)
                           .Column(col =>
                           {
                               col.Item().Text("Detalle").SemiBold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                               col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                               col.Item().Text($"Guía: {data.NumeroGuia}");
                           });
                    });

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        col.Item().Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(8).Text("Datos del Envío").SemiBold();

                        col.Item().PaddingVertical(6).Grid(grid =>
                        {
                            grid.Columns(2);

                            grid.Item().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("ID: ").Style(labelStyle); t.Span(data.Id.ToString()).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Fecha registro: ").Style(labelStyle); t.Span($"{data.FechaRegistro:dd/MM/yyyy}").Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Sucursal origen: ").Style(labelStyle); t.Span(data.SucursalOrigen).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Tipo de envío: ").Style(labelStyle); t.Span(data.TipoEnvio).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Estado actual: ").Style(labelStyle); t.Span(data.EstadoActual).Style(valueStyle); });
                                if (data.FechaUltimaActualizacion.HasValue)
                                    c.Item().Text(t => { t.Span("Última actualización: ").Style(labelStyle); t.Span($"{data.FechaUltimaActualizacion:dd/MM/yyyy}").Style(valueStyle); });
                            });

                            grid.Item().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Ruta: ").Style(labelStyle); t.Span(data.RutaAsignada).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Peso: ").Style(labelStyle); t.Span($"{data.PesoKg:0.###} kg").Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Costo: ").Style(labelStyle); t.Span($"${data.CostoEnvio:0.00}").Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Ciudad destino: ").Style(labelStyle); t.Span(data.CiudadDestino).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Dirección destino: ").Style(labelStyle); t.Span(data.DireccionDestino).Style(valueStyle); });
                            });
                        });

                        col.Item().PaddingTop(10).Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(8).Text("Contacto").SemiBold();

                        col.Item().PaddingVertical(6).Grid(grid =>
                        {
                            grid.Columns(2);

                            grid.Item().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Remitente: ").Style(labelStyle); t.Span(data.RemitenteNombre).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Tel. Remitente: ").Style(labelStyle); t.Span(data.RemitenteTelefono).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Registrado por: ").Style(labelStyle); t.Span(data.EmpleadoRegistro).Style(valueStyle); });
                            });

                            grid.Item().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Destinatario: ").Style(labelStyle); t.Span(data.DestinatarioNombre).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Tel. Destinatario: ").Style(labelStyle); t.Span(data.DestinatarioTelefono).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Empleado actual: ").Style(labelStyle); t.Span(data.EmpleadoActual).Style(valueStyle); });
                                c.Item().Text(t => { t.Span("Repartidor: ").Style(labelStyle); t.Span(data.RepartidorAsignado).Style(valueStyle); });
                            });
                        });

                        col.Item().PaddingTop(12).Row(r =>
                        {
                            r.RelativeItem().AlignLeft().Text(t =>
                            {
                                t.Span("Observaciones: ").Style(labelStyle);
                                t.Span("N/A").Style(valueStyle);
                            });

                            r.ConstantItem(120)
                             .AlignRight()
                             .Border(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                             .Padding(6)
                             .Column(c =>
                             {
                                 c.Item().AlignCenter().Text("Firma recibido").FontSize(10).SemiBold();
                                 c.Item().Height(40);
                                 c.Item().AlignCenter().Text("__________________").FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                             });
                        });
                    });

                    // FIX: no encadenar después de Text(t => { ... }) porque devuelve void
                    page.Footer()
                        .DefaultTextStyle(x => x.FontColor(QuestPDF.Helpers.Colors.Grey.Darken2))
                        .Row(row =>
                        {
                            row.RelativeItem()
                               .AlignLeft()
                               .Text("ManyBox • https://manybox.example");

                            row.RelativeItem()
                               .AlignRight()
                               .Text(t =>
                               {
                                   t.Span("Página ");
                                   t.CurrentPageNumber();
                                   t.Span(" / ");
                                   t.TotalPages();
                               });
                        });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<string> GuardarAsync(byte[] pdfBytes, string fileName)
        {
            var safeName = string.IsNullOrWhiteSpace(fileName) ? $"documento_{DateTime.Now:yyyyMMdd_HHmmss}.pdf" : fileName;
            if (!safeName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                safeName += ".pdf";

            var fullPath = Path.Combine(FileSystem.CacheDirectory, safeName);
            File.WriteAllBytes(fullPath, pdfBytes);
            return fullPath;
        }

        // Abre el sistema de Compartir/Abrir para permitir imprimir desde el visor nativo
        public async Task ImprimirAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("No se encontró el archivo PDF a imprimir.", filePath);

            try
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Imprimir o compartir PDF",
                    File = new ShareFile(filePath)
                });
            }
            catch
            {
                await Launcher.Default.OpenAsync(new OpenFileRequest("Abrir PDF", new ReadOnlyFile(filePath)));
            }
        }

        public class PaquetePdfData
        {
            public int Id { get; set; }
            public string CodigoSeguimiento { get; set; } = "";
            public DateTime FechaRegistro { get; set; }
            public string SucursalOrigen { get; set; } = "";
            public string RemitenteNombre { get; set; } = "";
            public string RemitenteTelefono { get; set; } = "";
            public string DestinatarioNombre { get; set; } = "";
            public string DestinatarioTelefono { get; set; } = "";
            public string DireccionDestino { get; set; } = "";
            public string CiudadDestino { get; set; } = "";
            public string TipoEnvio { get; set; } = "";
            public string EstadoActual { get; set; } = "";
            public DateTime? FechaUltimaActualizacion { get; set; }
            public string EmpleadoRegistro { get; set; } = "";
            public string EmpleadoActual { get; set; } = "";
            public string RepartidorAsignado { get; set; } = "";
            public decimal PesoKg { get; set; }
            public string NumeroGuia { get; set; } = "";
            public string RutaAsignada { get; set; } = "";
            public decimal CostoEnvio { get; set; }
        }
    }
}