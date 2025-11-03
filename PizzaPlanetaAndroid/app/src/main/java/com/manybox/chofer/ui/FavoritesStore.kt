package com.manybox.chofer.ui

import androidx.compose.runtime.mutableStateListOf

// Favoritos genéricos de producto
data class Favorite(val id: Int, val name: String)

// Favoritos de sucursales / lugares
data class FavoriteBranch(val id: Int, val name: String)

object FavoritesStore {
    // productos
    private val _favorites = mutableStateListOf<Favorite>()
    val favorites: List<Favorite> get() = _favorites

    // sucursales
    private val _branches = mutableStateListOf<FavoriteBranch>()
    val favoriteBranches: List<FavoriteBranch> get() = _branches

    // Productos
    fun isFavorite(id: Int): Boolean = _favorites.any { it.id == id }

    fun toggle(f: Favorite) {
        val idx = _favorites.indexOfFirst { it.id == f.id }
        if (idx >= 0) _favorites.removeAt(idx) else _favorites.add(f)
    }

    fun remove(id: Int) {
        _favorites.removeAll { it.id == id }
    }

    // Sucursales
    fun isBranchFavorite(id: Int): Boolean = _branches.any { it.id == id }

    fun toggleBranch(b: FavoriteBranch) {
        val idx = _branches.indexOfFirst { it.id == b.id }
        if (idx >= 0) _branches.removeAt(idx) else _branches.add(b)
    }

    fun removeBranch(id: Int) {
        _branches.removeAll { it.id == id }
    }
}
