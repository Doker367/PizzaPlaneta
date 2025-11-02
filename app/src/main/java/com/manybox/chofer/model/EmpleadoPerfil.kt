package com.manybox.chofer.model

data class EmpleadoPerfil(
    val id: Int,
    val nombre: String,
    val apellido: String,
    val nombreCompleto: String,
    val correo: String,
    val telefono: String,
    val sucursalNombre: String,
    val totalEntregas: Int,
    val entregasCompletadas: Int,
    val entregasPendientes: Int,
    val entregasHoy: Int,
    val entregasSemana: Int,
    val entregasMes: Int
)
