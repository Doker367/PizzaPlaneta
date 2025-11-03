package com.manybox.chofer.api

import android.content.Context
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.runBlocking

// Simple in-memory holder to avoid async reads on every request
object AuthTokenHolder {
    @Volatile
    var token: String? = null
}

private val Context.dataStore by preferencesDataStore(name = "pizza_planeta_prefs")

object TokenStore {
    private val KEY_TOKEN: Preferences.Key<String> = stringPreferencesKey("auth_token")
    private val KEY_DISPLAY_NAME: Preferences.Key<String> = stringPreferencesKey("display_name")

    suspend fun saveToken(context: Context, token: String) {
        context.dataStore.edit { prefs ->
            prefs[KEY_TOKEN] = token
        }
        AuthTokenHolder.token = token
    }

    fun tokenFlow(context: Context): Flow<String?> =
        context.dataStore.data.map { prefs -> prefs[KEY_TOKEN] }

    fun getTokenBlocking(context: Context): String? = runBlocking {
        tokenFlow(context).first()
    }

    suspend fun clearToken(context: Context) {
        context.dataStore.edit { prefs ->
            prefs.remove(KEY_TOKEN)
            prefs.remove(KEY_DISPLAY_NAME) // También limpiar el nombre
        }
        AuthTokenHolder.token = null
    }

    suspend fun saveDisplayName(context: Context, name: String) {
        context.dataStore.edit { prefs ->
            prefs[KEY_DISPLAY_NAME] = name
        }
    }

    fun displayNameFlow(context: Context): Flow<String?> =
        context.dataStore.data.map { prefs -> prefs[KEY_DISPLAY_NAME] }

    fun getDisplayNameBlocking(context: Context): String? = runBlocking {
        displayNameFlow(context).first()
    }
}
