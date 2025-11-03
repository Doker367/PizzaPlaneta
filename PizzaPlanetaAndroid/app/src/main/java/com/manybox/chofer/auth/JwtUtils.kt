package com.manybox.chofer.auth

import android.util.Base64
import org.json.JSONObject

object JwtUtils {
    fun hasAdminRole(token: String?): Boolean {
        if (token.isNullOrBlank()) return false
        val parts = token.split('.')
        if (parts.size < 2) return false
        return try {
            val payloadJson = String(Base64.decode(parts[1].replace('-', '+').replace('_', '/'), Base64.DEFAULT))
            val obj = JSONObject(payloadJson)
            // Common claim keys for roles
            val roleKeys = listOf(
                "role",
                "roles",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                "authorities"
            )
            roleKeys.any { key ->
                if (!obj.has(key)) false else {
                    val v = obj.get(key)
                    when (v) {
                        is String -> v.equals("admin", ignoreCase = true)
                        is org.json.JSONArray -> {
                            (0 until v.length()).any { idx ->
                                v.optString(idx).equals("admin", ignoreCase = true)
                            }
                        }
                        else -> false
                    }
                }
            }
        } catch (_: Exception) {
            false
        }
    }

    fun getDisplayName(token: String?): String? {
        if (token.isNullOrBlank()) return null
        val parts = token.split('.')
        if (parts.size < 2) return null
        return try {
            val payloadJson = String(Base64.decode(parts[1].replace('-', '+').replace('_', '/'), Base64.DEFAULT))
            val obj = JSONObject(payloadJson)
            // Common claim keys where a display name might live
            val keys = listOf(
                // Prefer Spanish custom claim first
                "nombre",
                // Then common name claims
                "name",
                "given_name",
                // Then usernames/emails
                "preferred_username",
                "unique_name",
                "email",
                // Fallback subject id
                "sub"
            )
            keys.firstNotNullOfOrNull { key -> obj.optString(key).takeIf { it.isNotBlank() } }
        } catch (_: Exception) {
            null
        }
    }

    fun getNameOrGivenName(token: String?): String? {
        if (token.isNullOrBlank()) return null
        val parts = token.split('.')
        if (parts.size < 2) return null
        return try {
            val payloadJson = String(Base64.decode(parts[1].replace('-', '+').replace('_', '/'), Base64.DEFAULT))
            val obj = JSONObject(payloadJson)
            // Prioritize custom 'nombre' claim, then 'name', then 'given_name'
            val keys = listOf("nombre", "name", "given_name")
            keys.firstNotNullOfOrNull { key -> obj.optString(key).takeIf { it.isNotBlank() } }
        } catch (_: Exception) {
            null
        }
    }
}
