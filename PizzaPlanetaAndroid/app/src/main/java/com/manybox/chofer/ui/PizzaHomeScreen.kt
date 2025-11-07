package com.manybox.chofer.ui
import coil.compose.AsyncImage

// Removed incorrect import that caused compilation error
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Menu
import androidx.compose.material.icons.filled.ShoppingCart
import androidx.compose.material.icons.filled.CreditCard
import androidx.compose.material.icons.filled.FavoriteBorder
import androidx.compose.material.icons.filled.Favorite
import androidx.compose.material.icons.filled.CheckCircle
import androidx.compose.animation.core.rememberInfiniteTransition
import androidx.compose.animation.core.infiniteRepeatable
import androidx.compose.animation.core.tween
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.FastOutSlowInEasing
import androidx.compose.animation.core.RepeatMode
import androidx.compose.animation.core.animateFloat
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Search
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.RectangleShape
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.graphics.graphicsLayer
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.foundation.Image
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.layout.ContentScale
import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.interaction.collectIsPressedAsState
import com.manybox.chofer.R
import com.manybox.chofer.api.PizzaApiService
import com.manybox.chofer.api.PizzaLoginRequest
import com.manybox.chofer.api.PizzaLoginResponse
import com.manybox.chofer.api.SucursalDto
import com.manybox.chofer.api.RetrofitProvider
import com.manybox.chofer.api.TokenStore
import com.manybox.chofer.api.AuthTokenHolder
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.HostnameVerifier
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager
import android.os.Build
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.zIndex
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import com.manybox.chofer.ui.components.SubtleSnackHost
import androidx.compose.runtime.LaunchedEffect
import com.manybox.chofer.auth.JwtUtils
import com.manybox.chofer.ui.admin.AdminDashboard


@Composable
fun PizzaHomeScreen() {
    val context = LocalContext.current
    val rootSnackbarHost = remember { SnackbarHostState() }
    val scope = rememberCoroutineScope()
    val drawerState = rememberDrawerState(DrawerValue.Closed)
    var showLogin by remember { mutableStateOf(false) }
    var showRegister by remember { mutableStateOf(false) }
    var showForgot by remember { mutableStateOf(false) }
    var forgotStep by remember { mutableStateOf(0) }
    // Menú lateral
    var showAccount by remember { mutableStateOf(false) }
    var showMetodoPago by remember { mutableStateOf(false) }
    var showOrderSelector by remember { mutableStateOf(false) }
    var showFavoritesDialog by remember { mutableStateOf(false) }
    var showFavoriteBranchesDialog by remember { mutableStateOf(false) }
    var isLoggingIn by remember { mutableStateOf(false) }
    var loginError by remember { mutableStateOf<String?>(null) }
    var showMenu by remember { mutableStateOf(false) }
    var showAdmin by remember { mutableStateOf(false) }
    var isAdmin by remember { mutableStateOf(false) }
    var isLoggedIn by remember { mutableStateOf(false) }
    var loginSuccess by remember { mutableStateOf(false) }
    var sucursales by remember { mutableStateOf<List<SucursalDto>>(emptyList()) }
    var loadingSucursales by remember { mutableStateOf(false) }
    var sucursalesError by remember { mutableStateOf<String?>(null) }
    var selectedSucursalId by remember { mutableStateOf<Int?>(null) }
    var selectedSucursalName by remember { mutableStateOf<String?>(null) }
    var selectedSucursalAddress by remember { mutableStateOf<String?>(null) }
    var selectedSucursalMapsUrl by remember { mutableStateOf<String?>(null) }
    var displayName by remember { mutableStateOf("Usuario") }
    // Acción diferida a ejecutar después de login exitoso
    var postLogin by remember { mutableStateOf<(() -> Unit)?>(null) }
    var showCarrito by remember { mutableStateOf(false) }
    val cartViewModel = remember { CartViewModel() }

    // Palette
    val Navy = Color(0xFF1D3557)
    val Orange = Color(0xFFF77F00)
    val OnNavy = Color(0xFFEAEAEA)

    // Cargar nombre guardado solo si hay un token
    LaunchedEffect(Unit) {
        val token = TokenStore.getTokenBlocking(context)
        isLoggedIn = !token.isNullOrBlank()
        if (isLoggedIn) {
            TokenStore.getDisplayNameBlocking(context)?.let { n ->
                if (n.isNotBlank()) displayName = n
            }
        } else {
            displayName = "Invitado"
        }
    }

    ModalNavigationDrawer(
        drawerState = drawerState,
        gesturesEnabled = true,
        drawerContent = {
            ModalDrawerSheet(
                drawerContainerColor = Color.White,
                modifier = Modifier.width(280.dp)
            ) {
                // Header naranja rectangular (como antes)
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(140.dp)
                        .background(Color(0xFFF77F00)),
                    contentAlignment = Alignment.Center
                ) {
                    Column(horizontalAlignment = Alignment.CenterHorizontally) {
                        Image(
                            painter = painterResource(id = R.drawable.i4m),
                            contentDescription = null,
                            modifier = Modifier.size(56.dp)
                        )
                        Spacer(Modifier.height(6.dp))
                        Text(
                            displayName.uppercase(),
                            color = Color(0xFF1D3557),
                            fontWeight = FontWeight.Bold
                        )
                    }
                }
                Spacer(Modifier.height(16.dp))
                DrawerItemRow(
                    text = "Método de pago",
                    icon = Icons.Default.CreditCard
                ) { 
                    
                    scope.launch { 
                        drawerState.close()
                        showMetodoPago = true
                    } 
                }
                DrawerItemRow(text = "Carrito", icon = Icons.Default.ShoppingCart) {
                    scope.launch {
                        drawerState.close()
                        showCarrito = true
                    }
                }
                DrawerItemRow(text = "Favoritos", icon = Icons.Default.Favorite) {
                    showFavoritesDialog = true
                    scope.launch { drawerState.close() }
                }
                DrawerItemRow(text = "Lugares favoritos", icon = Icons.Default.FavoriteBorder) {
                    showFavoriteBranchesDialog = true
                    scope.launch { drawerState.close() }
                }
                val isGuestUser = !isLoggedIn
                DrawerItemRow(text = "Mi perfil", icon = Icons.Default.Person) {
                    if (isGuestUser) {
                        // Abrir login encima de cualquier pantalla actual y volver a Perfil al terminar
                        postLogin = { showAccount = true }
                        showMenu = false
                        showOrderSelector = false
                        showAccount = false
                        showRegister = false
                        showForgot = false
                        showLogin = true
                        showRegister = false
                        showForgot = false
                    } else {
                        // Navegar a la vista de perfil del usuario
                        showMenu = false
                        showOrderSelector = false
                        showAdmin = false
                        showAccount = true
                    }
                    scope.launch { drawerState.close() }
                }
                DrawerItemRow(text = "Carrito", icon = Icons.Default.ShoppingCart) {
                    if (isGuestUser) {
                        postLogin = { showCarrito = true }
                        showMenu = false
                        showOrderSelector = false
                        showAccount = false
                        showRegister = false
                        showForgot = false
                        showLogin = true
                        showRegister = false
                        showForgot = false
                    } else {
                        showCarrito = true
                    }
                    scope.launch { drawerState.close() }
                }
                DrawerItemRow(text = "Lugares favoritos", icon = Icons.Default.FavoriteBorder) {
                    if (isGuestUser) {
                        postLogin = { showOrderSelector = true }
                        showMenu = false
                        showOrderSelector = false
                        showAccount = false
                        showRegister = false
                        showForgot = false
                        showLogin = true
                        showRegister = false
                        showForgot = false
                    } else {
                        // TODO: Navegar a favoritos
                        scope.launch {
                            rootSnackbarHost.showSnackbar("Favoritos próximamente")
                        }
                    }
                    scope.launch { drawerState.close() }
                }
                // Botón de cerrar sesión sólo si hay sesión activa
                if (isLoggedIn) {
                    Spacer(Modifier.height(8.dp))
                    Divider()
                    // Opción de Administrar sólo para admins
                    if (isAdmin) {
                        DrawerItemRow(text = "Administrar", icon = Icons.Default.Menu) {
                            showMenu = false
                            showOrderSelector = false
                            showAdmin = true
                            scope.launch { drawerState.close() }
                        }
                        Spacer(Modifier.height(8.dp))
                        Divider()
                    }
                    DrawerItemRow(text = "Cerrar sesión", icon = Icons.Default.Person) {
                        scope.launch(Dispatchers.IO) {
                            TokenStore.clearToken(context)
                            launch(Dispatchers.Main) {
                                displayName = "Invitado"
                                isLoggedIn = false
                                isAdmin = false
                                scope.launch { drawerState.close() }
                                scope.launch {
                                    rootSnackbarHost.showSnackbar(
                                        message = "Sesión cerrada",
                                        withDismissAction = false,
                                        duration = SnackbarDuration.Short
                                    )
                                }
                            }
                        }
                    }
                } else {
                    Spacer(Modifier.height(8.dp))
                    Divider()
                    DrawerItemRow(text = "Iniciar sesión", icon = Icons.Default.Person) {
                        // Abre el flujo de login y oculta el selector/menú para que no tape el login
                        postLogin = { showOrderSelector = true }
                        showMenu = false
                        showOrderSelector = false
                        showAccount = false
                        showLogin = true
                        showRegister = false
                        showForgot = false
                        scope.launch { drawerState.close() }
                    }
                }
                Spacer(Modifier.weight(1f))
                Text(
                    "v1.0.0",
                    color = Color.Gray,
                    modifier = Modifier.align(Alignment.CenterHorizontally).padding(12.dp)
                )
            }
        }
    ) {
        Box(
            modifier = Modifier
                .fillMaxSize()
                .background(Navy)
        ) {

            Column(modifier = Modifier.fillMaxSize()) {
                // Header sin botón de menú (se eliminó el menú lateral)
                Spacer(Modifier.height(12.dp))

                // Bloque: Logo + "Bienvenido" más abajo (padding solo afecta a este bloque)
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(top = 48.dp), // Reducido de 72.dp a 48.dp para subir todo el contenido
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Image(
                        painter = painterResource(id = R.drawable.il1_alt),
                        contentDescription = "Logo",
                        modifier = Modifier
                            .fillMaxWidth(0.9f)
                            .height(120.dp)
                    )
                    Spacer(Modifier.height(8.dp))  // Reducido de 12.dp a 8.dp
                    Text(
                        "Bienvenido",
                        color = OnNavy,
                        fontSize = 20.sp,
                        fontWeight = FontWeight.Bold,
                    )
                    Spacer(Modifier.height(2.dp))  // Reducido de 4.dp a 2.dp
                    // Animación de pizza caliente removida temporalmente (no implementada)
                    Spacer(Modifier.height(8.dp))
                }

                // Se elimina la imagen decorativa inferior para que se vea como en el mock

                Spacer(Modifier.weight(1f))
            }

            // Imagen decorativa fija arriba a la derecha (sin offset)
            Image(
                painter = painterResource(id = R.drawable.il1),
                contentDescription = null,
                modifier = Modifier
                    .align(Alignment.TopEnd)
                    .offset(y = (-20).dp)   // empuja hacia arriba para eliminar el gap del PNG
                    .padding(end = 0.dp)
                    .width(180.dp)
                    .height(140.dp)
                    .alpha(0.75f),          // sin zIndex (el drawer sigue encima)
                contentScale = ContentScale.Fit
            )


            // Cargar/Refrescar sucursales cada vez que se abre el selector, sin requerir login
            LaunchedEffect(showOrderSelector) {
                if (showOrderSelector) {
                    loadingSucursales = true
                    sucursalesError = null
                    // Opcional: limpiar para evitar ver datos obsoletos si cambia de usuario
                    sucursales = emptyList()
                    RetrofitProvider.pizzaApi(context).getSucursales()
                        .enqueue(object : Callback<List<SucursalDto>> {
                            override fun onResponse(
                                call: Call<List<SucursalDto>>, response: Response<List<SucursalDto>>
                            ) {
                                loadingSucursales = false
                                if (response.isSuccessful) {
                                    sucursales = response.body().orEmpty()
                                    if (sucursales.isEmpty()) {
                                        sucursalesError = "No hay sucursales disponibles"
                                    }
                                } else {
                                    sucursalesError =
                                        "Error ${response.code()} al cargar sucursales"
                                }
                            }

                            override fun onFailure(call: Call<List<SucursalDto>>, t: Throwable) {
                                loadingSucursales = false
                                sucursalesError = t.message
                            }
                        })
                }
            }

            // Bottom orange action bar overlay (Cuenta / Ordenar)
            Box(
                modifier = Modifier
                    .align(Alignment.BottomCenter)
                    .fillMaxWidth()
                    .height(110.dp)
                    .background(Orange, RectangleShape)
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(horizontal = 24.dp, vertical = 8.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.Top
                ) {
                    val isLoggedInNow = !((AuthTokenHolder.token ?: TokenStore.getTokenBlocking(
                        context
                    )).isNullOrBlank())
                    run {
                        val interaction = remember { MutableInteractionSource() }
                        val pressed by interaction.collectIsPressedAsState()
                        val scale by animateFloatAsState(
                            targetValue = if (pressed) 1.06f else 1f,
                            label = "cuentaScale"
                        )
                        if (!isLoggedInNow) Column(
                            horizontalAlignment = Alignment.CenterHorizontally,
                            modifier = Modifier
                                .weight(1f)
                                .offset(y = (-35).dp)
                                .zIndex(2f)
                                .clickable(interactionSource = interaction, indication = null) {
                                    // Desde Home, el botón "Cuenta" debe ir al Login
                                    postLogin = { showOrderSelector = true }
                                    showLogin = true
                                    showRegister = false
                                    showForgot = false
                                    forgotStep = 0
                                    showAccount = false
                                }
                        ) {
                            Box(
                                modifier = Modifier
                                    .size(72.dp)
                                    .clip(RoundedCornerShape(12.dp))
                                    .background(if (pressed) Color.White.copy(alpha = 0.12f) else Color.Transparent)
                                    .graphicsLayer(scaleX = scale, scaleY = scale),
                                contentAlignment = Alignment.Center
                            ) {
                                Image(
                                    painter = painterResource(id = R.drawable.i2l),
                                    contentDescription = "Cuenta",
                                    modifier = Modifier.size(56.dp),
                                    contentScale = ContentScale.Fit
                                )
                            }
                            Spacer(Modifier.height(2.dp))
                            Text(
                                "Cuenta",
                                color = Color.Black,
                                fontWeight = FontWeight.Medium,
                                fontSize = 16.sp
                            )
                        }
                    }

                    run {
                        val interaction = remember { MutableInteractionSource() }
                        val pressed by interaction.collectIsPressedAsState()
                        val scale by animateFloatAsState(
                            targetValue = if (pressed) 1.06f else 1f,
                            label = "ordenarScale"
                        )
                        Column(
                            horizontalAlignment = Alignment.CenterHorizontally,
                            modifier = Modifier
                                .weight(1f)
                                .offset(y = (-35).dp)
                                .zIndex(2f)
                                .clickable(
                                    interactionSource = interaction,
                                    indication = null
                                ) { showOrderSelector = true }
                        ) {
                            Box(
                                modifier = Modifier
                                    .size(72.dp)
                                    .clip(RoundedCornerShape(12.dp))
                                    .background(if (pressed) Color.White.copy(alpha = 0.12f) else Color.Transparent)
                                    .graphicsLayer(scaleX = scale, scaleY = scale),
                                contentAlignment = Alignment.Center
                            ) {
                                Image(
                                    painter = painterResource(id = R.drawable.i3l),
                                    contentDescription = "Ordenar",
                                    modifier = Modifier.size(56.dp),
                                    contentScale = ContentScale.Fit
                                )
                            }
                            Spacer(Modifier.height(2.dp))
                            Text(
                                "Ordenar",
                                color = Color.Black,
                                fontWeight = FontWeight.Medium,
                                fontSize = 16.sp
                            )
                        }
                    }
                }
            }

            // Login / Register / Forgot
            if (showLogin) {
                if (showRegister) {
                    var registroError by remember { mutableStateOf<String?>(null) }
                    var registroExito by remember { mutableStateOf(false) }
                    var isRegistering by remember { mutableStateOf(false) }
                    Box {
                        RegistroScreen(
                            headerImageRes = R.drawable.pizzorra,
                            onBack = { showRegister = false },
                            isLoading = isRegistering,
                            errorMessage = registroError,
                            onSubmit = { nombre, correo, telefono, pass1, pass2 ->
                                registroError = null
                                registroExito = false
                                if (nombre.isBlank() || correo.isBlank() || telefono.isBlank() || pass1.isBlank() || pass2.isBlank()) {
                                    registroError = "Completa todos los campos."
                                    return@RegistroScreen
                                }
                                if (pass1 != pass2) {
                                    registroError = "Las contraseñas no coinciden."
                                    return@RegistroScreen
                                }
                                isRegistering = true
                                val api = RetrofitProvider.pizzaApi(context)
                                val req = com.manybox.chofer.api.PizzaRegisterRequest(
                                    nombre = nombre,
                                    email = correo,
                                    password = pass1,
                                    telefono = telefono
                                )
                                api.register(req).enqueue(object : retrofit2.Callback<Void> {
                                    override fun onResponse(
                                        call: retrofit2.Call<Void>,
                                        response: retrofit2.Response<Void>
                                    ) {
                                        isRegistering = false
                                        if (response.isSuccessful) {
                                            registroExito = true
                                            // Guarda el nombre para mostrarlo en el menú lateral
                                            CoroutineScope(Dispatchers.IO).launch {
                                                TokenStore.saveDisplayName(context, nombre)
                                            }
                                            // Espera 1.5s y regresa a login
                                            kotlinx.coroutines.CoroutineScope(kotlinx.coroutines.Dispatchers.Main)
                                                .launch {
                                                    kotlinx.coroutines.delay(1500)
                                                    showRegister = false
                                                    showLogin = true
                                                }
                                        } else {
                                            val errorBody = response.errorBody()?.string()
                                            registroError = errorBody
                                                ?: "Error en el registro: ${response.code()}"
                                        }
                                    }

                                    override fun onFailure(
                                        call: retrofit2.Call<Void>,
                                        t: Throwable
                                    ) {
                                        isRegistering = false
                                        registroError = "Error de red: ${t.message}"
                                    }
                                })
                            },
                            onLoginClick = { showRegister = false }
                        )
                        // Mensaje sutil de éxito en registro
                        if (registroExito) {
                            LaunchedEffect(Unit) {
                                rootSnackbarHost.showSnackbar(
                                    message = "Cuenta creada exitosamente",
                                    withDismissAction = false,
                                    duration = SnackbarDuration.Short
                                )
                            }
                        }
                    }
                } else if (showForgot) {
                    if (forgotStep == 0) {
                        RestablecerContrasenaScreen(
                            headerImageRes = R.drawable.pizzorra,
                            onBack = { showForgot = false; forgotStep = 0 },
                            onSubmitEmail = { _ -> forgotStep = 1 }
                        )
                    } else if (forgotStep == 1) {
                        VerificarCodigoScreen(
                            headerImageRes = R.drawable.pizzorra,
                            onBack = { forgotStep = 0 },
                            onVerify = { _ -> forgotStep = 2 }
                        )
                    } else {
                        NuevaContrasenaScreen(
                            headerImageRes = R.drawable.pizzorra,
                            onBack = { forgotStep = 1 },
                            onConfirm = { _, _ -> showForgot = false; forgotStep = 0 }
                        )
                    }
                } else {
                    LoginScreen(
                        onLogin = { email, pass ->
                            isLoggingIn = true
                            loginError = null
                            val api = RetrofitProvider.pizzaApi(context)
                            api.login(PizzaLoginRequest(email, pass))
                                .enqueue(object : Callback<PizzaLoginResponse> {
                                    override fun onResponse(
                                        call: Call<PizzaLoginResponse>,
                                        response: Response<PizzaLoginResponse>
                                    ) {
                                        isLoggingIn = false
                                        if (response.isSuccessful) {
                                            val rawToken =
                                                response.body()?.token?.removePrefix("Bearer ")
                                                    ?.trim() ?: ""
                                            // Calcula el displayName de inmediato con prioridades: name/given_name > display/email > fallback
                                            val immediateName =
                                                JwtUtils.getNameOrGivenName(rawToken)
                                                    ?: JwtUtils.getDisplayName(rawToken)
                                                    ?: "Invitado"
                                            // Actualiza UI inmediatamente
                                            displayName = immediateName
                                            // Persiste token y nombre en segundo plano
                                            CoroutineScope(Dispatchers.IO).launch {
                                                TokenStore.saveToken(context, rawToken)
                                                TokenStore.saveDisplayName(context, immediateName)
                                            }
                                            // Nueva instancia para asegurar que el interceptor use el token actualizado
                                            val apiWithToken = RetrofitProvider.pizzaApi(context)
                                            apiWithToken.getSucursales()
                                                .enqueue(object : Callback<List<SucursalDto>> {
                                                    override fun onResponse(
                                                        call: Call<List<SucursalDto>>,
                                                        response: Response<List<SucursalDto>>
                                                    ) {
                                                        if (response.isSuccessful) {
                                                            // Actualiza la lista de sucursales con la respuesta
                                                            sucursales =
                                                                response.body() ?: emptyList()
                                                            // Snackbar sutil de éxito
                                                            scope.launch {
                                                                rootSnackbarHost.showSnackbar(
                                                                    message = "Inicio de sesión exitoso",
                                                                    withDismissAction = false,
                                                                    duration = SnackbarDuration.Short
                                                                )
                                                            }
                                                            // Detectar admin por JWT y navegar
                                                            val isAdminNow =
                                                                JwtUtils.hasAdminRole(rawToken)
                                                            isAdmin = isAdminNow
                                                            isLoggedIn = true
                                                            showLogin = false
                                                            // Si hay una acción diferida, ejecútala; de lo contrario, flujo por defecto
                                                            val next = postLogin
                                                            postLogin = null
                                                            if (next != null) {
                                                                next.invoke()
                                                            } else if (isAdminNow) {
                                                                showAdmin = true
                                                                showOrderSelector = false
                                                            } else {
                                                                showOrderSelector = true
                                                            }
                                                        } else {
                                                            loginError =
                                                                "No se pudieron cargar sucursales (${response.code()})"
                                                        }
                                                    }

                                                    override fun onFailure(
                                                        call: Call<List<SucursalDto>>,
                                                        t: Throwable
                                                    ) {
                                                        loginError =
                                                            "Error al cargar sucursales: ${t.message}"
                                                    }
                                                })
                                        } else {
                                            loginError = "Usuario o contraseña incorrectos"
                                        }
                                    }

                                    override fun onFailure(
                                        call: Call<PizzaLoginResponse>,
                                        t: Throwable
                                    ) {
                                        isLoggingIn = false
                                        loginError = "Error de red: ${t.message}"
                                    }
                                })
                        },
                        isLoading = isLoggingIn,
                        errorMessage = loginError,
                        onBack = {
                            showLogin = false; showRegister = false; showForgot =
                            false; forgotStep = 0
                        },
                        onRegisterClick = { showRegister = true },
                        headerImageRes = R.drawable.pizzorra
                    )
                }
            }

            // Overlay de login exitoso eliminado: usamos snackbar sutil

            // Order selector
            if (showOrderSelector) {
                Scaffold(
                    containerColor = Color.White,
                    bottomBar = {
                        BottomAppBar(
                            containerColor = Color(0xFFD32F2F),
                            contentColor = Color.White,
                            modifier = Modifier.height(70.dp)
                        ) {
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(horizontal = 16.dp),
                                horizontalArrangement = Arrangement.SpaceBetween,
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                // Icono de menú -> abre menú lateral
                                IconButton(onClick = { scope.launch { drawerState.open() } }) {
                                    Icon(
                                        Icons.Default.Menu,
                                        contentDescription = "Menú",
                                        tint = Color.White
                                    )
                                }

                                // Texto centrado: mostrar "Invitado" si no hay sesión
                                val tokenNow =
                                    AuthTokenHolder.token ?: TokenStore.getTokenBlocking(context)
                                val welcomeName =
                                    if (tokenNow.isNullOrBlank()) "INVITADO" else displayName.uppercase()
                                Text(
                                    "BIENVENIDO, $welcomeName",
                                    color = Color.White,
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 14.sp
                                )
                            }
                        }
                    }
                ) { paddingValues ->
                    LazyColumn(
                        modifier = Modifier
                            .fillMaxSize()
                            .padding(paddingValues)
                            .padding(16.dp)
                    ) {
                        item {
                            // Header con flecha de regreso y título
                            Row(
                                verticalAlignment = Alignment.CenterVertically,
                                modifier = Modifier.fillMaxWidth()
                            ) {
                                IconButton(onClick = { showOrderSelector = false }) {
                                    Icon(
                                        Icons.Default.ArrowBack,
                                        contentDescription = "Regresar",
                                        tint = Color(0xFF1D3557)
                                    )
                                }
                                Text(
                                    "Selecciona tu restaurante",
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 18.sp,
                                    color = Color(0xFF1D3557)
                                )
                            }
                            Spacer(Modifier.height(16.dp))
                        }

                        item {
                            // Mapa con imagen real
                            Box(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .height(150.dp)
                                    .clip(RoundedCornerShape(8.dp))
                            ) {
                                Image(
                                    painter = painterResource(id = R.drawable.map1),
                                    contentDescription = "Mapa",
                                    modifier = Modifier.fillMaxSize(),
                                    contentScale = ContentScale.Crop
                                )
                            }
                            Spacer(Modifier.height(16.dp))
                        }

                        item {
                            // Buscador de CP o Ciudad
                            OutlinedTextField(
                                value = "",
                                onValueChange = {},
                                modifier = Modifier.fillMaxWidth(),
                                placeholder = { Text("Ciudad y estado o CP") },
                                leadingIcon = {
                                    Icon(
                                        Icons.Default.Search,
                                        contentDescription = "Buscar",
                                        tint = Color.Gray
                                    )
                                },
                                colors = OutlinedTextFieldDefaults.colors(
                                    focusedBorderColor = Color(0xFF1D3557),
                                    unfocusedBorderColor = Color.LightGray
                                ),
                                singleLine = true
                            )
                            Spacer(Modifier.height(16.dp))
                        }

                        // Estado de carga y errores
                        if (loadingSucursales) {
                            item {
                                Row(
                                    modifier = Modifier.fillMaxWidth(),
                                    horizontalArrangement = Arrangement.Center
                                ) {
                                    CircularProgressIndicator(color = Orange)
                                }
                                Spacer(Modifier.height(12.dp))
                            }
                        }
                        if (sucursalesError != null && sucursales.isEmpty()) {
                            item {
                                Text(
                                    sucursalesError!!,
                                    color = Color.Red,
                                    modifier = Modifier.padding(vertical = 8.dp)
                                )
                            }
                        }

                        // Lista de sucursales dinámica y scrolleable
                        items(sucursales) { sucursal ->
                            Card(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(vertical = 4.dp),
                                colors = CardDefaults.cardColors(containerColor = Color.White),
                                elevation = CardDefaults.cardElevation(2.dp)
                            ) {
                                Row(
                                    modifier = Modifier
                                        .fillMaxWidth()
                                        .padding(12.dp),
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    // Imagen del restaurante
                                    Image(
                                        painter = painterResource(id = R.drawable.i4m),
                                        contentDescription = null,
                                        modifier = Modifier
                                            .size(60.dp)
                                            .clip(RoundedCornerShape(8.dp))
                                    )
                                    Spacer(Modifier.width(12.dp))
                                    Column(modifier = Modifier.weight(1f)) {
                                        Text(
                                            sucursal.nombre,
                                            fontWeight = FontWeight.Bold
                                        )
                                        Text(
                                            sucursal.direccion,
                                            color = Color.Gray,
                                            fontSize = 14.sp
                                        )
                                    }
                                    Button(
                                        onClick = {
                                            selectedSucursalId = sucursal.id
                                            selectedSucursalName = sucursal.nombre
                                            selectedSucursalAddress = sucursal.direccion
                                            selectedSucursalMapsUrl = sucursal.googleMapsUrl
                                            showOrderSelector = false
                                            showMenu = true
                                        },
                                        colors = ButtonDefaults.buttonColors(
                                            containerColor = Color(0xFF1D3557)
                                        )
                                    ) {
                                        Text("ELEGIR")
                                    }
                                }
                            }
                            Spacer(Modifier.height(8.dp))
                        }
                    }
                }
            }

            // Menu de platillos (nuevo componente)
            if (showMenu) {
                MenuPlatillos(
                    onBackClick = {
                        showMenu = false
                        showOrderSelector = true
                        selectedSucursalId = null
                        selectedSucursalName = null
                        selectedSucursalAddress = null
                    },
                    onMenuClick = { scope.launch { drawerState.open() } },
                    onCarritoClick = { showCarrito = true },
                    onRequireAuth = {
                        postLogin = { showMenu = true }
                        showMenu = false
                        showLogin = true
                    },
                    cartViewModel = cartViewModel,
                    sucursalId = selectedSucursalId ?: 3,
                    sucursalName = selectedSucursalName ?: "",
                    sucursalAddress = selectedSucursalAddress,
                    sucursalMapsUrl = selectedSucursalMapsUrl
                )
            }

            // Pantalla de carrito
            if (showCarrito) {
                CartScreen(
                    viewModel = cartViewModel,
                    onBack = { showCarrito = false },
                    onOrderSuccess = {
                        showCarrito = false
                        // opcional: mostrar selector o menú
                    }
                )
            }

            if (showMetodoPago) {
                MetodoPagoScreen(
                    onBack = { showMetodoPago = false },
                    headerImageRes = R.drawable.pizzorra
                )
            }

            // Cierra el contenedor principal de la pantalla Home
        }
        // Cierra el contenido del ModalNavigationDrawer
    }

}

@Composable
    fun DrawerItemRow(text: String, icon: ImageVector, onClick: () -> Unit) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .clickable { onClick() }
                .padding(horizontal = 16.dp, vertical = 12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Icon(icon, contentDescription = null, tint = Color(0xFF1D3557))
            Spacer(Modifier.width(12.dp))
            Text(text, color = Color(0xFF1D3557), fontWeight = FontWeight.Medium)
        }
    }
