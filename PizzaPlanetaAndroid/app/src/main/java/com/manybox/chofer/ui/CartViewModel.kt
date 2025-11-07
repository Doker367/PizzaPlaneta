package com.manybox.chofer.ui

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel

data class CartLine(
    val productoId: Int,
    val nombre: String,
    val precioUnitario: Double,
    var cantidad: Int,
    val imageUrl: String? = null,
    val imageResId: Int? = null,
    val notes: String? = null
) {
    val subtotal: Double get() = precioUnitario * cantidad
}

class CartViewModel : ViewModel() {
    private val _items = mutableStateListOf<CartLine>()
    val items: List<CartLine> get() = _items

    var sucursalId by mutableStateOf<Int?>(null)

    fun setSucursal(id: Int) { sucursalId = id }

    fun addItem(
        productoId: Int,
        nombre: String,
        precio: Double,
        cantidad: Int = 1,
        imageUrl: String? = null,
        imageResId: Int? = null,
        notes: String? = null
    ) {
        val idx = _items.indexOfFirst { it.productoId == productoId }
        if (idx >= 0) {
            _items[idx] = _items[idx].copy(cantidad = _items[idx].cantidad + cantidad)
        } else {
            _items.add(CartLine(productoId, nombre, precio, cantidad, imageUrl, imageResId, notes))
        }
    }

    fun increment(productoId: Int) {
        val idx = _items.indexOfFirst { it.productoId == productoId }
        if (idx >= 0) _items[idx] = _items[idx].copy(cantidad = _items[idx].cantidad + 1)
    }
    fun decrement(productoId: Int) {
        val idx = _items.indexOfFirst { it.productoId == productoId }
        if (idx >= 0) {
            val current = _items[idx]
            if (current.cantidad > 1) {
                _items[idx] = current.copy(cantidad = current.cantidad - 1)
            } else {
                _items.removeAt(idx)
            }
        }
    }
    fun remove(productoId: Int) {
        val idx = _items.indexOfFirst { it.productoId == productoId }
        if (idx >= 0) _items.removeAt(idx)
    }
    fun clear() { _items.clear() }
    fun total(): Double = _items.sumOf { it.subtotal }
}