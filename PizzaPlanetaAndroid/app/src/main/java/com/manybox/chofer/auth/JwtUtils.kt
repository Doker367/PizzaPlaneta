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
}
