package com.manybox.chofer.api

import android.content.Context
import android.os.Build
import okhttp3.Interceptor
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.HostnameVerifier
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

object RetrofitProvider {
    @Volatile
    private var retrofit: Retrofit? = null

    fun pizzaApi(context: Context): PizzaApiService =
        getRetrofit(context).create(PizzaApiService::class.java)

    fun apiService(context: Context): ApiService =
        getRetrofit(context).create(ApiService::class.java)

    fun getRetrofit(context: Context): Retrofit {
        return retrofit ?: synchronized(this) {
            retrofit ?: buildRetrofit(context).also { retrofit = it }
        }
    }

    private fun buildRetrofit(context: Context): Retrofit {
        val client = buildClient(context)
        return Retrofit.Builder()
            .baseUrl(getPizzaBaseUrl())
            .addConverterFactory(GsonConverterFactory.create())
            .client(client)
            .build()
    }

    private fun buildClient(context: Context): OkHttpClient {
        val logging = HttpLoggingInterceptor().apply { level = HttpLoggingInterceptor.Level.BASIC }
        val authInterceptor = Interceptor { chain ->
            val original: Request = chain.request()
            val token = AuthTokenHolder.token ?: TokenStore.getTokenBlocking(context)
            val req = if (!token.isNullOrBlank()) {
                original.newBuilder()
                    .addHeader("Authorization", "Bearer ${token.removePrefix("Bearer ")}")
                    .build()
            } else original
            chain.proceed(req)
        }

        return buildDevHttpsClient()
            .newBuilder()
            .addInterceptor(authInterceptor)
            .addInterceptor(logging)
            .build()
    }

    // Helpers de red para entorno de desarrollo (SSL permisivo solo en dev)
    private fun buildDevHttpsClient(): OkHttpClient {
        return try {
            val trustAll = arrayOf<TrustManager>(object : X509TrustManager {
                override fun checkClientTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
                override fun checkServerTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
                override fun getAcceptedIssuers(): Array<X509Certificate> = arrayOf()
            })
            val ssl = SSLContext.getInstance("SSL").apply { init(null, trustAll, SecureRandom()) }
            OkHttpClient.Builder()
                .sslSocketFactory(ssl.socketFactory, trustAll[0] as X509TrustManager)
                .hostnameVerifier(HostnameVerifier { _, _ -> true })
                .build()
        } catch (e: Exception) {
            OkHttpClient.Builder().build()
        }
    }

    private fun isEmulator(): Boolean {
        val fingerprint = Build.FINGERPRINT
        val model = Build.MODEL
        val manufacturer = Build.MANUFACTURER
        val brand = Build.BRAND
        val device = Build.DEVICE
        val product = Build.PRODUCT

        return (
            fingerprint.startsWith("generic") ||
            fingerprint.lowercase().contains("emulator") ||
            model.contains("Emulator", ignoreCase = true) ||
            model.contains("Android SDK built for x86", ignoreCase = true) ||
            manufacturer.contains("Genymotion", ignoreCase = true) ||
            (brand.startsWith("generic") && device.startsWith("generic")) ||
            product == "google_sdk"
        )
    }

    private fun getPizzaBaseUrl(): String {
        // Siempre usa localhost para pruebas locales (solo funcionará en emulador Android)
        return "http://10.0.2.2:80/"
    }
}
