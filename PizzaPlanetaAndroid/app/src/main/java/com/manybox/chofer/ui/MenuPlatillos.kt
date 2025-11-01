@file:OptIn(ExperimentalMaterial3Api::class)

package com.manybox.chofer.ui

import androidx.compose.foundation.*
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.FavoriteBorder
import androidx.compose.material.icons.filled.Menu
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.SolidColor
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.style.TextAlign
import com.manybox.chofer.R
import android.content.Context
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.platform.LocalContext
import com.manybox.chofer.api.MenuItemDto
import com.manybox.chofer.api.RetrofitProvider
import com.manybox.chofer.api.CreateOrderRequest
import com.manybox.chofer.api.OrderItemRequest
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import kotlinx.coroutines.launch

// Definición de colores
val OrangeBrand = Color(0xFFFF9800)
val RedBrand = Color(0xFFD32F2F)
val GrayBackground = Color(0xFFF5F5F5)
val BrownDark = Color(0xFF3E2723)
val DarkBackground = Color(0xFF212121) // Color oscuro para la imagen de fondo

// Modelo UI derivado del DTO (sin imágenes por ahora)
data class MenuUiItem(
    val id: Int,
    val name: String,
    val description: String,
    val price: Double,
    val calories: Int?,
    val category: String?
)

@Composable
fun MenuPlatillos(
    onBackClick: () -> Unit,
    onMenuClick: () -> Unit,
    sucursalId: Int = 3
) {
    val context = LocalContext.current
    var isLoading by remember { mutableStateOf(true) }
    var error by remember { mutableStateOf<String?>(null) }
    var items by remember { mutableStateOf<List<MenuUiItem>>(emptyList()) }
    var selected by remember { mutableStateOf<MenuUiItem?>(null) }
    var qty by remember { mutableStateOf(1) }
    var search by remember { mutableStateOf("") }
    var selectedCategory by remember { mutableStateOf("Todos") }
    var cartCount by remember { mutableStateOf(0) }
    // Carrito local para mostrar "Ver pedido"
    data class CartItem(val id: Int, val name: String, val unitPrice: Double, var qty: Int)
    val cartItems = remember { mutableStateListOf<CartItem>() }
    var showCartSheet by remember { mutableStateOf(false) }

    // Categorías derivadas de los ítems (memoizadas por cambios en items)
    val categories = remember(items) { items.mapNotNull { it.category?.ifBlank { null } }.distinct() }

    LaunchedEffect(sucursalId) {
        isLoading = true
        error = null
        RetrofitProvider.pizzaApi(context).getMenuBySucursalId(sucursalId)
            .enqueue(object : Callback<List<MenuItemDto>> {
                override fun onResponse(
                    call: Call<List<MenuItemDto>>, response: Response<List<MenuItemDto>>
                ) {
                    if (response.isSuccessful) {
                        val body = response.body().orEmpty()
                        items = body.map {
                            MenuUiItem(
                                id = it.productoId,
                                name = it.nombre,
                                description = it.descripcion,
                                price = it.precio.toDouble(),
                                calories = it.calorias,
                                category = it.categoria
                            )
                        }
                        isLoading = false
                    } else {
                        error = "Error ${response.code()} al cargar menú"
                        isLoading = false
                    }
                }

                override fun onFailure(call: Call<List<MenuItemDto>>, t: Throwable) {
                    error = t.message
                    isLoading = false
                }
            })
    }

    val snackbarHostState = remember { SnackbarHostState() }
    val scope = rememberCoroutineScope()
    var isSubmitting by remember { mutableStateOf(false) }

    Scaffold(
        containerColor = GrayBackground,
        snackbarHost = { com.manybox.chofer.ui.components.SubtleSnackHost(snackbarHostState, bottomPadding = 90.dp) }
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            // =======================================================
            // 1. Encabezado con Imagen, Botones de Navegación y Banner
            // =======================================================
            item {
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(280.dp) 
                ) {
                    // Imagen de fondo
                    Image(
                        painter = painterResource(id = R.drawable.pm1),
                        contentDescription = null,
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(200.dp),
                        contentScale = ContentScale.Crop
                    )

                    // Botones de Regreso y Favoritos
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(16.dp),
                        horizontalArrangement = Arrangement.SpaceBetween
                    ) {
                        // Botón de Regreso
                        FloatingActionButton(
                            onClick = onBackClick,
                            containerColor = Color.Black.copy(alpha = 0.5f), 
                            shape = CircleShape,
                            elevation = FloatingActionButtonDefaults.elevation(defaultElevation = 0.dp),
                            modifier = Modifier.size(40.dp)
                        ) {
                            Icon(Icons.Default.ArrowBack, "Regresar", tint = Color.White)
                        }
                        // Botón de Favorito
                        FloatingActionButton(
                            onClick = { /* TODO */ },
                            containerColor = Color.Black.copy(alpha = 0.5f), 
                            shape = CircleShape,
                            elevation = FloatingActionButtonDefaults.elevation(defaultElevation = 0.dp),
                            modifier = Modifier.size(40.dp)
                        ) {
                            Icon(Icons.Default.FavoriteBorder, "Favorito", tint = Color.White)
                        }
                    }

                    // Banner de Información del Restaurante
                    Card(
                        modifier = Modifier
                            .align(Alignment.BottomCenter) 
                            .padding(horizontal = 16.dp)
                            .fillMaxWidth(),
                        shape = RoundedCornerShape(8.dp),
                        colors = CardDefaults.cardColors(containerColor = Color.White),
                        elevation = CardDefaults.cardElevation(defaultElevation = 4.dp)
                    ) {
                        Row(
                            modifier = Modifier.padding(16.dp),
                            verticalAlignment = Alignment.CenterVertically
                        ) {
                            // Icono del Restaurante
                            Box(
                                modifier = Modifier
                                    .size(40.dp)
                                    .clip(RoundedCornerShape(8.dp))
                                    .background(RedBrand.copy(alpha = 0.1f)),
                                contentAlignment = Alignment.Center
                            ) {
                                Image(
                                    painter = painterResource(id = R.drawable.i4m),
                                    contentDescription = "Logo restaurante",
                                    modifier = Modifier.size(32.dp),
                                    contentScale = ContentScale.Fit
                                )
                            }

                            Spacer(Modifier.width(12.dp))

                            Column {
                                Text(
                                    "Pizza Planet Centro",
                                    fontWeight = FontWeight.Bold,
                                    fontSize = 18.sp,
                                    color = DarkBackground 
                                )
                                Text(
                                    "Av. 1ra poniente #2396",
                                    fontSize = 14.sp,
                                    color = Color.Gray
                                )
                            }
                        }
                    }
                }
            }

            // Estado de carga o error
            if (isLoading) {
                item { 
                    Spacer(Modifier.height(24.dp))
                    CircularProgressIndicator(color = RedBrand)
                    Spacer(Modifier.height(24.dp))
                }
            }
            if (error != null) {
                item {
                    Spacer(Modifier.height(12.dp))
                    Text(error!!, color = Color.Red)
                }
            }

            // Buscador y filtros de categoría
            if (!isLoading && error == null) {
                item { Spacer(Modifier.height(8.dp)) }
                item {
                    OutlinedTextField(
                        value = search,
                        onValueChange = { search = it },
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(horizontal = 16.dp),
                        singleLine = true,
                        label = { Text("Buscar en el menú") }
                    )
                }
                item { Spacer(Modifier.height(8.dp)) }
                item {
                    val all = listOf("Todos") + categories
                    Row(
                        modifier = Modifier
                            .fillMaxWidth()
                            .horizontalScroll(rememberScrollState())
                            .padding(horizontal = 16.dp),
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        all.forEach { cat ->
                            val selectedChip = selectedCategory == cat
                            FilterChip(
                                selected = selectedChip,
                                onClick = { selectedCategory = cat },
                                label = { Text(cat) },
                                modifier = Modifier.padding(end = 8.dp)
                            )
                        }
                    }
                }
            }

            // Lista de productos del menú agrupados por categoría
            val filtered = items.filter { i ->
                val q = search.trim().lowercase()
                val byQuery = if (q.isBlank()) true else i.name.lowercase().contains(q) || i.description.lowercase().contains(q)
                val byCat = selectedCategory == "Todos" || (i.category ?: "Otros") == selectedCategory
                byQuery && byCat
            }
            if (!isLoading && error == null && filtered.isEmpty()) {
                item {
                    Spacer(Modifier.height(32.dp))
                    Column(
                        modifier = Modifier.fillMaxWidth().padding(horizontal = 16.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Text("No hay productos en esta sucursal", fontWeight = FontWeight.SemiBold, color = BrownDark)
                        Spacer(Modifier.height(6.dp))
                        Text("Intenta seleccionar otra sucursal o refrescar.", color = Color.Gray, fontSize = 12.sp)
                        Spacer(Modifier.height(12.dp))
                        OutlinedButton(onClick = {
                            isLoading = true
                            error = null
                            RetrofitProvider.pizzaApi(context).getMenuBySucursalId(sucursalId)
                                .enqueue(object : Callback<List<MenuItemDto>> {
                                    override fun onResponse(call: Call<List<MenuItemDto>>, response: Response<List<MenuItemDto>>) {
                                        if (response.isSuccessful) {
                                            val body = response.body().orEmpty()
                                            items = body.map {
                                                MenuUiItem(
                                                    id = it.productoId,
                                                    name = it.nombre,
                                                    description = it.descripcion,
                                                    price = it.precio.toDouble(),
                                                    calories = it.calorias,
                                                    category = it.categoria
                                                )
                                            }
                                            isLoading = false
                                        } else {
                                            error = "Error ${response.code()} al cargar menú"
                                            isLoading = false
                                        }
                                    }
                                    override fun onFailure(call: Call<List<MenuItemDto>>, t: Throwable) {
                                        error = t.message
                                        isLoading = false
                                    }
                                })
                        }) { Text("Refrescar") }
                    }
                }
            }
            val grouped = filtered.groupBy { it.category?.ifBlank { "Otros" } ?: "Otros" }
            grouped.forEach { (cat, list) ->
                item {
                    Spacer(Modifier.height(16.dp))
                    CategoryHeader(cat)
                    Spacer(Modifier.height(8.dp))
                }
                items(list) { item ->
                    MenuItemRow(
                        item = item,
                        onView = { selected = item; qty = 1 },
                        onQuickAdd = {
                            // Agregar 1 unidad rápidamente
                            isSubmitting = true
                            val body = CreateOrderRequest(
                                sucursalId = sucursalId,
                                items = listOf(OrderItemRequest(productoId = item.id, cantidad = 1))
                            )
                            RetrofitProvider.pizzaApi(context).createOrder(body)
                                .enqueue(object : Callback<Void> {
                                    override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                        isSubmitting = false
                                        if (response.isSuccessful) {
                                            // Actualiza carrito local
                                            val existing = cartItems.indexOfFirst { it.id == item.id }
                                            if (existing >= 0) cartItems[existing] = cartItems[existing].copy(qty = cartItems[existing].qty + 1)
                                            else cartItems.add(CartItem(item.id, item.name, item.price, 1))
                                            cartCount = cartItems.sumOf { it.qty }
                                            scope.launch { snackbarHostState.showSnackbar("Agregado al pedido") }
                                        } else {
                                            scope.launch { snackbarHostState.showSnackbar("Error ${response.code()} al agregar") }
                                        }
                                    }
                                    override fun onFailure(call: Call<Void>, t: Throwable) {
                                        isSubmitting = false
                                        scope.launch { snackbarHostState.showSnackbar("Error: ${t.message}") }
                                    }
                                })
                        }
                    )
                    Spacer(Modifier.height(12.dp))
                }
            }

            // Espaciador para el área del BottomBar flotante
            item {
                Spacer(Modifier.height(80.dp))
            }
        }
        
        // =======================================================
        // 5. Bottom Bar Flotante
        // =======================================================
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues),
            contentAlignment = Alignment.BottomCenter 
        ) {
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .height(70.dp),
                shape = RoundedCornerShape(topStart = 8.dp, topEnd = 8.dp), 
                colors = CardDefaults.cardColors(containerColor = RedBrand),
                elevation = CardDefaults.cardElevation(defaultElevation = 8.dp) 
            ) {
                Row(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    // Hamburguesa para abrir menú lateral (izquierda fija)
                    IconButton(onClick = onMenuClick) {
                        Icon(
                            Icons.Default.Menu,
                            contentDescription = "Menú",
                            tint = Color.White
                        )
                    }
                    Spacer(Modifier.width(6.dp))
                    // Texto + contador apilados
                    Column {
                        Text("Tu pedido", color = Color.White.copy(alpha = 0.95f), fontWeight = FontWeight.SemiBold)
                        Spacer(Modifier.height(2.dp))
                        Surface(
                            color = Color.White.copy(alpha = 0.15f),
                            shape = RoundedCornerShape(12.dp)
                        ) {
                            Text(
                                "$cartCount",
                                color = Color.White,
                                modifier = Modifier.padding(horizontal = 10.dp, vertical = 4.dp),
                                fontWeight = FontWeight.Bold
                            )
                        }
                    }
                    Spacer(Modifier.weight(1f))
                    Button(
                        onClick = { showCartSheet = true },
                        colors = ButtonDefaults.buttonColors(containerColor = Color.White),
                        shape = RoundedCornerShape(10.dp)
                    ) { Text("Ver pedido", color = RedBrand, fontWeight = FontWeight.Bold) }
                }
            }
        }
    }

    // Detalle del producto con contador y sugeridos
    if (selected != null) {
        val current = selected!!
        ModalBottomSheet(
            onDismissRequest = { selected = null },
            containerColor = Color.White
        ) {
            Column(modifier = Modifier.padding(16.dp)) {
                Text(current.name, fontWeight = FontWeight.Bold, fontSize = 20.sp)
                if (current.description.isNotEmpty()) {
                    Spacer(Modifier.height(6.dp))
                    Text(current.description, color = Color.Gray)
                }
                Spacer(Modifier.height(10.dp))
                Text("$" + "%.0f".format(current.price), fontWeight = FontWeight.ExtraBold, fontSize = 18.sp, color = BrownDark)
                if (current.calories != null) {
                    Spacer(Modifier.height(4.dp))
                    Text("${current.calories} kcal", color = Color.Gray, fontSize = 12.sp)
                }

                Spacer(Modifier.height(16.dp))
                Text("Cantidad", fontWeight = FontWeight.SemiBold)
                Row(verticalAlignment = Alignment.CenterVertically) {
                    OutlinedButton(onClick = { if (qty > 1) qty-- }) { Text("-") }
                    Text(qty.toString(), modifier = Modifier.padding(horizontal = 16.dp))
                    OutlinedButton(onClick = { qty++ }) { Text("+") }
                }

                Spacer(Modifier.height(16.dp))
                Button(
                    enabled = !isSubmitting,
                    onClick = {
                        isSubmitting = true
                        val body = CreateOrderRequest(
                            sucursalId = sucursalId,
                            items = listOf(OrderItemRequest(productoId = current.id, cantidad = qty))
                        )
                        RetrofitProvider.pizzaApi(context).createOrder(body)
                            .enqueue(object : Callback<Void> {
                                override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                    isSubmitting = false
                                    if (response.isSuccessful) {
                                        selected = null
                                        // Actualiza carrito local
                                        val existing = cartItems.indexOfFirst { it.id == current.id }
                                        if (existing >= 0) cartItems[existing] = cartItems[existing].copy(qty = cartItems[existing].qty + qty)
                                        else cartItems.add(CartItem(current.id, current.name, current.price, qty))
                                        cartCount = cartItems.sumOf { it.qty }
                                        qty = 1
                                        scope.launch {
                                            snackbarHostState.showSnackbar(
                                                message = "Producto agregado al pedido",
                                                withDismissAction = false,
                                                duration = SnackbarDuration.Short
                                            )
                                        }
                                    } else {
                                        scope.launch {
                                            snackbarHostState.showSnackbar(
                                                message = "Error ${response.code()} al agregar",
                                                withDismissAction = false,
                                                duration = SnackbarDuration.Short
                                            )
                                        }
                                    }
                                }

                                override fun onFailure(call: Call<Void>, t: Throwable) {
                                    isSubmitting = false
                                    scope.launch {
                                        snackbarHostState.showSnackbar(
                                            message = "Error: ${t.message}",
                                            withDismissAction = false,
                                            duration = SnackbarDuration.Short
                                        )
                                    }
                                }
                            })
                    },
                    modifier = Modifier.fillMaxWidth(),
                    colors = ButtonDefaults.buttonColors(containerColor = RedBrand)
                ) {
                    if (isSubmitting) {
                        CircularProgressIndicator(color = Color.White, modifier = Modifier.size(18.dp), strokeWidth = 2.dp)
                        Spacer(Modifier.width(8.dp))
                    }
                    Text("Agregar ($" + "%.0f".format(current.price * qty) + ")", color = Color.White)
                }

                Spacer(Modifier.height(20.dp))
                Text("Sugeridos", fontWeight = FontWeight.Bold)
                Spacer(Modifier.height(8.dp))
                val suggestions = items.filter { it.id != current.id }.take(3)
                suggestions.forEach { s ->
                    SuggestionRow(item = s, onClick = { selected = s; qty = 1 })
                    Spacer(Modifier.height(8.dp))
                }
                Spacer(Modifier.height(16.dp))
            }
        }
    }

    // Hoja con el carrito actual
    if (showCartSheet) {
        ModalBottomSheet(onDismissRequest = { showCartSheet = false }, containerColor = Color.White) {
            Column(Modifier.padding(16.dp)) {
                Text("Tu pedido", fontWeight = FontWeight.Bold, fontSize = 20.sp)
                Spacer(Modifier.height(8.dp))
                if (cartItems.isEmpty()) {
                    Text("Aún no has agregado artículos.", color = Color.Gray)
                } else {
                    var total = 0.0
                    cartItems.forEach { c ->
                        val subtotal = c.unitPrice * c.qty
                        total += subtotal
                        Row(Modifier.fillMaxWidth().padding(vertical = 6.dp), verticalAlignment = Alignment.CenterVertically) {
                            Column(Modifier.weight(1f)) {
                                Text(c.name, fontWeight = FontWeight.SemiBold)
                                Text("${c.qty} x $" + "%.0f".format(c.unitPrice), color = Color.Gray, fontSize = 12.sp)
                            }
                            Text("$" + "%.0f".format(subtotal), fontWeight = FontWeight.Bold)
                        }
                    }
                    Divider(Modifier.padding(vertical = 8.dp))
                    Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                        Text("Total", fontWeight = FontWeight.Bold)
                        Text("$" + "%.0f".format(total), fontWeight = FontWeight.Bold)
                    }
                }
                Spacer(Modifier.height(12.dp))
                Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    OutlinedButton(
                        onClick = { showCartSheet = false },
                        modifier = Modifier.weight(1f),
                        shape = RoundedCornerShape(10.dp)
                    ) { Text("Cerrar") }
                    Button(
                        onClick = { showCartSheet = false; onMenuClick() },
                        modifier = Modifier.weight(1f),
                        colors = ButtonDefaults.buttonColors(containerColor = RedBrand),
                        shape = RoundedCornerShape(10.dp)
                    ) { Text("Continuar", color = Color.White) }
                }
            }
        }
    }
}

// Composable para el botón de categoría (Pizzas, Snacks, Bebidas)
@Composable
fun CategoryButton(text: String) {
    Button(
        onClick = { /* TODO */ },
        colors = ButtonDefaults.buttonColors(containerColor = OrangeBrand),
        shape = RoundedCornerShape(8.dp),
        modifier = Modifier.widthIn(min = 150.dp, max = 200.dp) 
    ) {
        Text(text, color = Color.White, fontWeight = FontWeight.SemiBold)
    }
}

// Composable para cada elemento del menú (Pizza, Snack, Bebida)
@Composable
fun MenuItemRow(item: MenuUiItem, onView: () -> Unit, onQuickAdd: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = Color.White),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Placeholder de imagen hasta que tengamos URLs
            Box(
                modifier = Modifier
                    .size(90.dp)
                    .clip(RoundedCornerShape(8.dp))
                    .background(Color(0xFFF1F1F1)),
                contentAlignment = Alignment.Center
            ) { Text("🍕", fontSize = 28.sp) }

            Spacer(Modifier.width(16.dp))

            Column(
                modifier = Modifier.weight(1f)
            ) {
                Text(
                    item.name,
                    fontWeight = FontWeight.Bold,
                    fontSize = 16.sp
                )
                // Mostrar descripción si existe
                if (item.description.isNotEmpty()) {
                    Text(
                        item.description,
                        color = Color.Gray,
                        fontSize = 12.sp, 
                        modifier = Modifier.padding(vertical = 4.dp)
                    )
                }
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Text(
                        "$" + "%.0f".format(item.price),
                        fontWeight = FontWeight.ExtraBold,
                        color = BrownDark, 
                        fontSize = 16.sp
                    )
                    if (item.calories != null) {
                        Spacer(Modifier.width(8.dp))
                        Text(
                            "• ${item.calories} kcal",
                            color = Color.Gray,
                            fontSize = 12.sp
                        )
                    }
                }
            }

            Spacer(Modifier.width(8.dp))

            Column(horizontalAlignment = Alignment.End) {
                // Ver detalles
                OutlinedButton(
                    onClick = onView,
                    colors = ButtonDefaults.outlinedButtonColors(
                        contentColor = BrownDark,
                        containerColor = Color.White
                    ),
                    border = BorderStroke(1.dp, Color.LightGray),
                    shape = RoundedCornerShape(10.dp),
                    contentPadding = PaddingValues(horizontal = 12.dp, vertical = 6.dp),
                    modifier = Modifier.height(32.dp)
                ) { Text("Detalles", fontSize = 12.sp) }
                Spacer(Modifier.height(6.dp))
                // Agregar rápido
                Button(
                    onClick = onQuickAdd,
                    colors = ButtonDefaults.buttonColors(containerColor = RedBrand),
                    shape = RoundedCornerShape(10.dp),
                    contentPadding = PaddingValues(horizontal = 12.dp, vertical = 6.dp),
                    modifier = Modifier.height(32.dp)
                ) { Text("Agregar", color = Color.White, fontSize = 12.sp) }
            }
        }
    }
}

@Composable
private fun CategoryHeader(title: String) {
    Row(
        modifier = Modifier
            .fillMaxWidth()
            .padding(horizontal = 16.dp),
        verticalAlignment = Alignment.CenterVertically
    ) {
        Box(
            modifier = Modifier
                .height(1.dp)
                .weight(1f)
                .background(Color(0xFFE0E0E0))
        )
        Spacer(Modifier.width(8.dp))
        Text(title, fontWeight = FontWeight.Bold, color = BrownDark)
        Spacer(Modifier.width(8.dp))
        Box(
            modifier = Modifier
                .height(1.dp)
                .weight(1f)
                .background(Color(0xFFE0E0E0))
        )
    }
}

@Composable
private fun SuggestionRow(item: MenuUiItem, onClick: () -> Unit) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = RoundedCornerShape(8.dp),
        colors = CardDefaults.cardColors(containerColor = Color(0xFFFDFDFD)),
        elevation = CardDefaults.cardElevation(defaultElevation = 1.dp)
    ) {
        Row(modifier = Modifier
            .fillMaxWidth()
            .padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
            Box(
                modifier = Modifier.size(48.dp).clip(RoundedCornerShape(8.dp)).background(Color(0xFFF1F1F1)),
                contentAlignment = Alignment.Center
            ) { Text("🍕") }
            Spacer(Modifier.width(12.dp))
            Column(modifier = Modifier.weight(1f)) {
                Text(item.name, fontWeight = FontWeight.SemiBold)
                if (item.description.isNotEmpty()) Text(item.description, color = Color.Gray, fontSize = 12.sp, maxLines = 1)
            }
            Text("$" + "%.0f".format(item.price), fontWeight = FontWeight.Bold)
            Spacer(Modifier.width(8.dp))
            OutlinedButton(onClick = onClick, shape = RoundedCornerShape(10.dp)) { Text("Ver") }
        }
    }
}