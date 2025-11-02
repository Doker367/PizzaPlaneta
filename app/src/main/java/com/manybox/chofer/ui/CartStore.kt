package com.manybox.chofer.ui

import androidx.compose.runtime.mutableStateListOf

data class CartEntry(
    val id: Int,
    val name: String,
    val unitPrice: Double,
    var qty: Int,
    val notes: String? = null
)

object CartStore {
    val items = mutableStateListOf<CartEntry>()

    fun add(entry: CartEntry) {
        val idx = items.indexOfFirst { it.id == entry.id && it.notes == entry.notes }
        if (idx >= 0) {
            items[idx] = items[idx].copy(qty = items[idx].qty + entry.qty)
        } else {
            items.add(entry)
        }
    }

    fun clear() = items.clear()
}
