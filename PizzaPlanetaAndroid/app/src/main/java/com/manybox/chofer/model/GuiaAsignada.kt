package com.manybox.chofer.model

data class GuiaAsignada(
    val envioId: Int,
    val guiaRastreo: String,
    val destinatario: String,
    val ventaId: Int,
    val estados: List<EstadoGuia>,
    val estadoActual: Int
)

data class EstadoGuia(
    val titulo: String,
    val descripcion: String,
    val fechaHora: String
)
