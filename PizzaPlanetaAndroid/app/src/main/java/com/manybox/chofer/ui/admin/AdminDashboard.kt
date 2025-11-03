@file:OptIn(ExperimentalMaterial3Api::class)
package com.manybox.chofer.ui.admin

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Add
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.api.*
import com.manybox.chofer.ui.components.SubtleSnackHost
import androidx.compose.runtime.remember
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.platform.LocalContext
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import kotlinx.coroutines.launch

@Composable
fun AdminDashboard(onBack: () -> Unit, onLogout: () -> Unit) {
    val context = LocalContext.current
    val snackbar = remember { SnackbarHostState() }
    var tabIndex by remember { mutableStateOf(0) }
    val tabs = listOf("Sucursales", "Productos", "Menú por sucursal")

    // Align admin panel with app palette (Navy/Orange, light cards, dark text)
    val Navy = Color(0xFF1D3557)
    val Orange = Color(0xFFF77F00)
    val PanelBg = Color(0xFFF2F2F2)
    val TextPrimary = Color.Black
    val TextSecondary = Color(0xFF3C4A6B)
    Scaffold(
        containerColor = Navy,
        topBar = {
            CenterAlignedTopAppBar(
                title = { Text("Panel de Administración", fontWeight = FontWeight.Bold, color = Navy) },
                navigationIcon = {
                    IconButton(onClick = onBack) { Icon(Icons.Default.ArrowBack, contentDescription = "Atrás", tint = Navy) }
                },
                colors = TopAppBarDefaults.centerAlignedTopAppBarColors(containerColor = Orange)
            )
        },
        snackbarHost = { SubtleSnackHost(hostState = snackbar, bottomPadding = 90.dp) }
    ) { padding ->
        Column(Modifier.fillMaxSize().padding(padding)) {
            TabRow(selectedTabIndex = tabIndex, containerColor = Color.White, contentColor = Navy) {
                tabs.forEachIndexed { index, title ->
                    Tab(
                        selected = tabIndex == index,
                        onClick = { tabIndex = index },
                        text = { Text(title, color = if (tabIndex == index) Navy else TextSecondary) }
                    )
                }
            }
            when (tabIndex) {
                0 -> AdminSucursalesTab(snackbar)
                1 -> AdminProductosTab(snackbar)
                2 -> AdminMenuSucursalTab(snackbar)
            }
        }
    }
}

@Composable
private fun AdminSucursalesTab(snackbar: SnackbarHostState) {
    val context = LocalContext.current
    val api = remember { RetrofitProvider.pizzaApi(context) }
    val scope = rememberCoroutineScope()
    var nombre by remember { mutableStateOf("") }
    var direccion by remember { mutableStateOf("") }
    var ciudad by remember { mutableStateOf("") }
    var estado by remember { mutableStateOf("") }
    var telefono by remember { mutableStateOf("") }
    var maps by remember { mutableStateOf("") }
    var isSubmitting by remember { mutableStateOf(false) }
    var sucursales by remember { mutableStateOf<List<SucursalDto>>(emptyList()) }

    LaunchedEffect(Unit) {
        api.getSucursales().enqueue(object : Callback<List<SucursalDto>> {
            override fun onResponse(call: Call<List<SucursalDto>>, response: Response<List<SucursalDto>>) {
                if (response.isSuccessful) sucursales = response.body().orEmpty()
            }
            override fun onFailure(call: Call<List<SucursalDto>>, t: Throwable) {}
        })
    }

    val Navy = Color(0xFF1D3557)
    val Orange = Color(0xFFF77F00)
    val CardBg = Color(0xFFF2F2F2)
    val OnBg = Color.Black
    val SubText = Color(0xFF3C4A6B)
    val Btn = Orange
    var showCreate by remember { mutableStateOf(false) }

    LazyColumn(Modifier.fillMaxSize().padding(16.dp)) {
        item {
            Row(verticalAlignment = Alignment.CenterVertically, modifier = Modifier.fillMaxWidth()) {
                Text("Sucursales", fontWeight = FontWeight.Bold, fontSize = 20.sp, color = OnBg)
                Spacer(Modifier.weight(1f))
                FilledTonalIconButton(onClick = { showCreate = !showCreate }) { Icon(Icons.Default.Add, contentDescription = null) }
            }
            AnimatedVisibility(visible = showCreate) {
                Surface(color = CardBg, tonalElevation = 2.dp, shape = MaterialTheme.shapes.medium, modifier = Modifier.fillMaxWidth().padding(top = 12.dp)) {
                    Column(Modifier.padding(12.dp)) {
                        OutlinedTextField(value = nombre, onValueChange = { nombre = it }, label = { Text("Nombre") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = direccion, onValueChange = { direccion = it }, label = { Text("Dirección") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = ciudad, onValueChange = { ciudad = it }, label = { Text("Ciudad") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = estado, onValueChange = { estado = it }, label = { Text("Estado") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = telefono, onValueChange = { telefono = it }, label = { Text("Teléfono") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = maps, onValueChange = { maps = it }, label = { Text("Google Maps URL") }, modifier = Modifier.fillMaxWidth())
                        Spacer(Modifier.height(8.dp))
                        Button(
                onClick = {
                    isSubmitting = true
                    val body = CreateSucursalRequest(nombre, direccion, ciudad, estado.ifBlank { null }, telefono.ifBlank { null }, maps)
                    api.createSucursal(body).enqueue(object: Callback<SucursalDto> {
                        override fun onResponse(call: Call<SucursalDto>, response: Response<SucursalDto>) {
                            isSubmitting = false
                            if (response.isSuccessful) {
                                sucursales = sucursales + response.body()!!
                                nombre = ""; direccion = ""; ciudad = ""; estado = ""; telefono = ""; maps = ""
                                showCreate = false
                                scope.launch { snackbar.showSnackbar("Sucursal creada") }
                            } else {
                                scope.launch { snackbar.showSnackbar("Error ${response.code()} al crear sucursal") }
                            }
                        }
                        override fun onFailure(call: Call<SucursalDto>, t: Throwable) {
                            isSubmitting = false
                            scope.launch { snackbar.showSnackbar("Error: ${t.message}") }
                        }
                    })
                },
                enabled = !isSubmitting,
                colors = ButtonDefaults.buttonColors(containerColor = Btn)
            ) { Text(if (isSubmitting) "Creando..." else "Crear sucursal") }
                    }
                }
            }
            Spacer(Modifier.height(12.dp))
            Text("Listado", fontWeight = FontWeight.SemiBold, color = Navy)
        }
        items(sucursales) { s ->
            Card(Modifier.fillMaxWidth().padding(vertical = 4.dp), colors = CardDefaults.cardColors(containerColor = CardBg)) {
                Column(Modifier.padding(12.dp)) {
                    Text(s.nombre, fontWeight = FontWeight.Bold, color = Navy)
                    Text(s.direccion, maxLines = 1, overflow = TextOverflow.Ellipsis, color = SubText)
                    Text(s.ciudad + if (s.estado != null) ", ${s.estado}" else "", color = SubText)
                }
            }
        }
    }
}

@Composable
private fun AdminProductosTab(snackbar: SnackbarHostState) {
    val context = LocalContext.current
    val api = remember { RetrofitProvider.pizzaApi(context) }
    val scope = rememberCoroutineScope()
    var productos by remember { mutableStateOf<List<ProductDto>>(emptyList()) }

    var nombre by remember { mutableStateOf("") }
    var descripcion by remember { mutableStateOf("") }
    var precio by remember { mutableStateOf("") }
    var categoria by remember { mutableStateOf("") }
    var calorias by remember { mutableStateOf("") }
    var isSubmitting by remember { mutableStateOf(false) }

    LaunchedEffect(Unit) {
        api.getProductos().enqueue(object: Callback<List<ProductDto>> {
            override fun onResponse(call: Call<List<ProductDto>>, response: Response<List<ProductDto>>) {
                if (response.isSuccessful) productos = response.body().orEmpty()
            }
            override fun onFailure(call: Call<List<ProductDto>>, t: Throwable) {}
        })
    }
    val Navy = Color(0xFF1D3557)
    val Orange = Color(0xFFF77F00)
    val CardBg = Color(0xFFF2F2F2)
    val OnBg = Color.Black
    val SubText = Color(0xFF3C4A6B)
    val Btn = Orange

    var showCreate by remember { mutableStateOf(false) }

    var showDialog by remember { mutableStateOf(false) }
    var selected by remember { mutableStateOf<ProductDto?>(null) }
    var editMode by remember { mutableStateOf(false) }
    var confirmDelete by remember { mutableStateOf(false) }

    var edNombre by remember { mutableStateOf("") }
    var edDesc by remember { mutableStateOf("") }
    var edPrecio by remember { mutableStateOf("") }
    var edCategoria by remember { mutableStateOf("") }
    var edCalorias by remember { mutableStateOf("") }

    fun openDialog(p: ProductDto) {
        selected = p
        edNombre = p.nombre
        edDesc = p.descripcion ?: ""
        edPrecio = String.format("%.2f", p.precio)
        edCategoria = p.categoria ?: ""
        edCalorias = p.calorias?.toString() ?: ""
        editMode = false
        confirmDelete = false
        showDialog = true
    }

    LazyColumn(Modifier.fillMaxSize().padding(16.dp)) {
        item {
            Row(verticalAlignment = Alignment.CenterVertically, modifier = Modifier.fillMaxWidth()) {
                Text("Productos", fontWeight = FontWeight.Bold, fontSize = 20.sp, color = Navy)
                Spacer(Modifier.weight(1f))
                FilledTonalIconButton(onClick = { showCreate = !showCreate }) { Icon(Icons.Default.Add, contentDescription = null) }
            }
            AnimatedVisibility(visible = showCreate) {
                Surface(color = CardBg, tonalElevation = 2.dp, shape = MaterialTheme.shapes.medium, modifier = Modifier.fillMaxWidth().padding(top = 12.dp)) {
                    Column(Modifier.padding(12.dp)) {
                        OutlinedTextField(value = nombre, onValueChange = { nombre = it }, label = { Text("Nombre") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = descripcion, onValueChange = { descripcion = it }, label = { Text("Descripción") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = precio, onValueChange = { precio = it }, label = { Text("Precio") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = categoria, onValueChange = { categoria = it }, label = { Text("Categoría") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = calorias, onValueChange = { calorias = it }, label = { Text("Calorías") }, modifier = Modifier.fillMaxWidth())
                        Spacer(Modifier.height(8.dp))
                        Button(
                            onClick = {
                                isSubmitting = true
                                val body = CreateProductRequest(
                                    nombre = nombre,
                                    descripcion = descripcion,
                                    precio = precio.toDoubleOrNull() ?: 0.0,
                                    categoria = categoria.ifBlank { null },
                                    calorias = calorias.toIntOrNull(),
                                    activo = true
                                )
                                api.createProducto(body).enqueue(object: Callback<ProductDto> {
                                    override fun onResponse(call: Call<ProductDto>, response: Response<ProductDto>) {
                                        isSubmitting = false
                                        if (response.isSuccessful) {
                                            productos = productos + response.body()!!
                                            nombre = ""; descripcion = ""; precio = ""; categoria = ""; calorias = ""
                                            showCreate = false
                                            scope.launch { snackbar.showSnackbar("Producto creado") }
                                        } else {
                                            scope.launch { snackbar.showSnackbar("Error ${response.code()} al crear producto") }
                                        }
                                    }
                                    override fun onFailure(call: Call<ProductDto>, t: Throwable) {
                                        isSubmitting = false
                                        scope.launch { snackbar.showSnackbar("Error: ${t.message}") }
                                    }
                                })
                            },
                            enabled = !isSubmitting,
                            colors = ButtonDefaults.buttonColors(containerColor = Btn)
                        ) { Text(if (isSubmitting) "Creando..." else "Crear producto") }
                    }
                }
            }
            Spacer(Modifier.height(12.dp))
            Text("Listado", fontWeight = FontWeight.SemiBold, color = Navy)
        }
        items(productos, key = { it.id }) { p ->
            Card(
                Modifier.fillMaxWidth().padding(vertical = 4.dp).clickable { openDialog(p) },
                colors = CardDefaults.cardColors(containerColor = CardBg)
            ) {
                Column(Modifier.padding(12.dp)) {
                    Text(p.nombre, fontWeight = FontWeight.Bold, color = Navy)
                    val line = "$" + String.format("%.2f", p.precio) + (if (p.calorias != null) "  •  ${p.calorias} kcal" else "")
                    Text(line, color = SubText)
                    if (!p.categoria.isNullOrBlank()) Text(p.categoria!!, color = SubText.copy(alpha = 0.85f))
                }
            }
        }
    }

    if (showDialog && selected != null) {
        val p = selected!!
        AlertDialog(
            onDismissRequest = { showDialog = false },
            title = { Text("Producto: ${p.nombre}") },
            text = {
                Column {
                    if (!editMode) {
                        Text("Nombre: ${p.nombre}")
                        if (!p.descripcion.isNullOrBlank()) Text("Descripción: ${p.descripcion}")
                        Text("Precio: $${String.format("%.2f", p.precio)}")
                        if (!p.categoria.isNullOrBlank()) Text("Categoría: ${p.categoria}")
                        if (p.calorias != null) Text("Calorías: ${p.calorias}")
                    } else {
                        OutlinedTextField(value = edNombre, onValueChange = { edNombre = it }, label = { Text("Nombre") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = edDesc, onValueChange = { edDesc = it }, label = { Text("Descripción") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = edPrecio, onValueChange = { edPrecio = it }, label = { Text("Precio") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = edCategoria, onValueChange = { edCategoria = it }, label = { Text("Categoría") }, modifier = Modifier.fillMaxWidth())
                        OutlinedTextField(value = edCalorias, onValueChange = { edCalorias = it }, label = { Text("Calorías") }, modifier = Modifier.fillMaxWidth())
                    }
                    Spacer(Modifier.height(8.dp))
                    if (!confirmDelete) {
                        OutlinedButton(onClick = { confirmDelete = true }) { Text("Eliminar", color = Color.Red) }
                    } else {
                        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                            Button(colors = ButtonDefaults.buttonColors(containerColor = Color.Red), onClick = {
                                api.deleteProducto(p.id).enqueue(object: Callback<Void> {
                                    override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                        if (response.isSuccessful) {
                                            productos = productos.filterNot { it.id == p.id }
                                            scope.launch { snackbar.showSnackbar("Producto eliminado") }
                                            showDialog = false
                                        } else {
                                            scope.launch { snackbar.showSnackbar("Error ${response.code()} al eliminar") }
                                        }
                                    }
                                    override fun onFailure(call: Call<Void>, t: Throwable) {
                                        scope.launch { snackbar.showSnackbar("Error: ${t.message}") }
                                    }
                                })
                            }) { Text("Confirmar eliminación") }
                            OutlinedButton(onClick = { confirmDelete = false }) { Text("Cancelar") }
                        }
                    }
                }
            },
            confirmButton = {
                if (!editMode) {
                    Button(onClick = { editMode = true }, colors = ButtonDefaults.buttonColors(containerColor = Btn)) { Text("Modificar") }
                } else {
                    Button(onClick = {
                        val body = UpdateProductRequest(
                            nombre = edNombre.ifBlank { null },
                            descripcion = edDesc.ifBlank { null },
                            precio = edPrecio.toDoubleOrNull() ?: p.precio,
                            categoria = edCategoria.ifBlank { null },
                            calorias = edCalorias.toIntOrNull()
                        )
                        api.updateProducto(p.id, body).enqueue(object: Callback<ProductDto> {
                            override fun onResponse(call: Call<ProductDto>, response: Response<ProductDto>) {
                                if (response.isSuccessful) {
                                    val updated = response.body()!!
                                    productos = productos.map { if (it.id == p.id) updated else it }
                                    scope.launch { snackbar.showSnackbar("Producto actualizado") }
                                    showDialog = false
                                } else {
                                    scope.launch { snackbar.showSnackbar("Error ${response.code()} al actualizar") }
                                }
                            }
                            override fun onFailure(call: Call<ProductDto>, t: Throwable) {
                                scope.launch { snackbar.showSnackbar("Error: ${t.message}") }
                            }
                        })
                    }, colors = ButtonDefaults.buttonColors(containerColor = Btn)) { Text("Guardar") }
                }
            },
            dismissButton = {
                TextButton(onClick = { showDialog = false }) { Text("Cerrar") }
            }
        )
    }
}

@Composable
private fun AdminMenuSucursalTab(snackbar: SnackbarHostState) {
    val context = LocalContext.current
    val api = remember { RetrofitProvider.pizzaApi(context) }

    var sucursales by remember { mutableStateOf<List<SucursalDto>>(emptyList()) }
    var productos by remember { mutableStateOf<List<ProductDto>>(emptyList()) }

    var selectedSucursal by remember { mutableStateOf<SucursalDto?>(null) }
    var selectedProducto by remember { mutableStateOf<ProductDto?>(null) }
    var precioEspecial by remember { mutableStateOf("") }
    var isSubmitting by remember { mutableStateOf(false) }
    // Acción a ejecutar sobre el menú
    var action by remember { mutableStateOf("agregar") } // agregar | oferta | quitar

    LaunchedEffect(Unit) {
        api.getSucursales().enqueue(object: Callback<List<SucursalDto>> {
            override fun onResponse(call: Call<List<SucursalDto>>, response: Response<List<SucursalDto>>) {
                if (response.isSuccessful) sucursales = response.body().orEmpty()
            }
            override fun onFailure(call: Call<List<SucursalDto>>, t: Throwable) {}
        })
        api.getProductos().enqueue(object: Callback<List<ProductDto>> {
            override fun onResponse(call: Call<List<ProductDto>>, response: Response<List<ProductDto>>) {
                if (response.isSuccessful) productos = response.body().orEmpty()
            }
            override fun onFailure(call: Call<List<ProductDto>>, t: Throwable) {}
        })
    }

    val scope = rememberCoroutineScope()
    val Navy = Color(0xFF1D3557)
    val Orange = Color(0xFFF77F00)
    val CardBg = Color(0xFFF2F2F2)
    val OnBg = Color.Black
    val SubText = Color(0xFF3C4A6B)
    val Btn = Orange
    var showForm by remember { mutableStateOf(true) }
    var showCreateProduct by remember { mutableStateOf(false) }

    // Campos para crear producto inline
    var cpNombre by remember { mutableStateOf("") }
    var cpDescripcion by remember { mutableStateOf("") }
    var cpPrecio by remember { mutableStateOf("") }
    var cpCategoria by remember { mutableStateOf("") }
    var cpCalorias by remember { mutableStateOf("") }
    var creatingProduct by remember { mutableStateOf(false) }

    // Exposed dropdown states
    var sucursalExpanded by remember { mutableStateOf(false) }
    var productoExpanded by remember { mutableStateOf(false) }

    Column(Modifier.fillMaxSize().padding(16.dp)) {
        Row(verticalAlignment = Alignment.CenterVertically, modifier = Modifier.fillMaxWidth()) {
            Text("Menú por sucursal", fontWeight = FontWeight.Bold, fontSize = 20.sp, color = Navy)
            Spacer(Modifier.weight(1f))
            FilledTonalIconButton(onClick = { showForm = !showForm }) { Icon(Icons.Default.Add, contentDescription = null) }
        }
        AnimatedVisibility(visible = showForm) {
            Surface(color = CardBg, tonalElevation = 2.dp, shape = MaterialTheme.shapes.medium, modifier = Modifier.fillMaxWidth().padding(top = 12.dp)) {
                Column(Modifier.padding(12.dp)) {
                    // Sucursal dropdown
                    ExposedDropdownMenuBox(expanded = sucursalExpanded, onExpandedChange = { sucursalExpanded = !sucursalExpanded }) {
                        TextField(
                            value = selectedSucursal?.nombre ?: "",
                            onValueChange = {},
                            readOnly = true,
                            label = { Text("Selecciona sucursal") },
                            trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = sucursalExpanded) },
                            modifier = Modifier.fillMaxWidth().menuAnchor()
                        )
                        ExposedDropdownMenu(expanded = sucursalExpanded, onDismissRequest = { sucursalExpanded = false }) {
                            sucursales.forEach { s ->
                                DropdownMenuItem(text = { Text(s.nombre) }, onClick = {
                                    selectedSucursal = s
                                    sucursalExpanded = false
                                })
                            }
                        }
                    }

                    Spacer(Modifier.height(10.dp))

                    // Producto dropdown
                    ExposedDropdownMenuBox(expanded = productoExpanded, onExpandedChange = { productoExpanded = !productoExpanded }) {
                        TextField(
                            value = selectedProducto?.nombre ?: "",
                            onValueChange = {},
                            readOnly = true,
                            label = { Text("Selecciona producto") },
                            trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = productoExpanded) },
                            modifier = Modifier.fillMaxWidth().menuAnchor()
                        )
                        ExposedDropdownMenu(expanded = productoExpanded, onDismissRequest = { productoExpanded = false }) {
                            productos.forEach { p ->
                                DropdownMenuItem(text = { Text(p.nombre) }, onClick = {
                                    selectedProducto = p
                                    productoExpanded = false
                                    if (precioEspecial.isBlank()) precioEspecial = String.format("%.2f", p.precio)
                                })
                            }
                        }
                    }

                    // Crear producto inline
                    Spacer(Modifier.height(8.dp))
                    TextButton(onClick = { showCreateProduct = !showCreateProduct }) { Text(if (showCreateProduct) "Cancelar nuevo producto" else "Crear nuevo producto") }
                    AnimatedVisibility(visible = showCreateProduct) {
                        Column {
                            OutlinedTextField(value = cpNombre, onValueChange = { cpNombre = it }, label = { Text("Nombre del producto") }, modifier = Modifier.fillMaxWidth())
                            OutlinedTextField(value = cpDescripcion, onValueChange = { cpDescripcion = it }, label = { Text("Descripción") }, modifier = Modifier.fillMaxWidth())
                            OutlinedTextField(value = cpPrecio, onValueChange = { cpPrecio = it }, label = { Text("Precio base") }, modifier = Modifier.fillMaxWidth())
                            OutlinedTextField(value = cpCategoria, onValueChange = { cpCategoria = it }, label = { Text("Categoría (opcional)") }, modifier = Modifier.fillMaxWidth())
                            OutlinedTextField(value = cpCalorias, onValueChange = { cpCalorias = it }, label = { Text("Calorías (opcional)") }, modifier = Modifier.fillMaxWidth())
                            Spacer(Modifier.height(8.dp))
                            Button(onClick = {
                                val precioBase = cpPrecio.toDoubleOrNull()
                                if (precioBase == null || cpNombre.isBlank()) {
                                    scope.launch { snackbar.showSnackbar("Completa nombre y precio válido") }
                                    return@Button
                                }
                                creatingProduct = true
                                val body = CreateProductRequest(
                                    nombre = cpNombre,
                                    descripcion = cpDescripcion,
                                    precio = precioBase,
                                    categoria = cpCategoria.ifBlank { null },
                                    calorias = cpCalorias.toIntOrNull(),
                                    activo = true
                                )
                                api.createProducto(body).enqueue(object: Callback<ProductDto> {
                                    override fun onResponse(call: Call<ProductDto>, response: Response<ProductDto>) {
                                        creatingProduct = false
                                        if (response.isSuccessful) {
                                            val created = response.body()!!
                                            productos = productos + created
                                            selectedProducto = created
                                            precioEspecial = String.format("%.2f", created.precio)
                                            showCreateProduct = false
                                            cpNombre = ""; cpDescripcion = ""; cpPrecio = ""; cpCategoria = ""; cpCalorias = ""
                                            scope.launch { snackbar.showSnackbar("Producto creado y seleccionado") }
                                        } else {
                                            scope.launch { snackbar.showSnackbar("Error ${response.code()} al crear producto") }
                                        }
                                    }
                                    override fun onFailure(call: Call<ProductDto>, t: Throwable) {
                                        creatingProduct = false
                                        scope.launch { snackbar.showSnackbar("Error: ${t.message}") }
                                    }
                                })
                            }, enabled = !creatingProduct, colors = ButtonDefaults.buttonColors(containerColor = Btn)) {
                                Text(if (creatingProduct) "Creando..." else "Crear y seleccionar")
                            }
                        }
                    }

                    Spacer(Modifier.height(10.dp))

                    // Selección de acción
                    Text("Acción", color = Navy)
                    Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        FilterChip(selected = action == "agregar", onClick = { action = "agregar" }, label = { Text("Agregar") })
                        FilterChip(selected = action == "oferta", onClick = { action = "oferta" }, label = { Text("Oferta/Precio sucursal") })
                        FilterChip(selected = action == "quitar", onClick = { action = "quitar" }, label = { Text("Quitar del menú") })
                    }
                    Spacer(Modifier.height(10.dp))

                    OutlinedTextField(
                        value = precioEspecial,
                        onValueChange = { precioEspecial = it },
                        label = { Text("Precio especial (opcional)") },
                        modifier = Modifier.fillMaxWidth()
                    )
                    Spacer(Modifier.height(12.dp))
                    Button(
                        onClick = {
                            val suc = selectedSucursal ?: return@Button
                            val prod = selectedProducto ?: return@Button
                            if (action == "quitar") {
                                scope.launch { snackbar.showSnackbar("Quitar del menú no disponible: falta endpoint en backend") }
                                return@Button
                            }
                            isSubmitting = true
                            val precio = precioEspecial.toDoubleOrNull() ?: prod.precio
                            val body = AddMenuItemRequest(productoId = prod.id, precioEspecial = precio)
                            // Usamos el mismo endpoint para agregar o actualizar precio (si el backend lo soporta como upsert)
                            api.addProductoToSucursalMenu(suc.id, body).enqueue(object: Callback<MenuItemDto> {
                                override fun onResponse(call: Call<MenuItemDto>, response: Response<MenuItemDto>) {
                                    isSubmitting = false
                                    if (response.isSuccessful) {
                                        precioEspecial = ""
                                        selectedProducto = null
                                        val msg = if (action == "oferta") "Precio/Oferta aplicado en ${suc.nombre}" else "Producto agregado al menú de ${suc.nombre}"
                                        scope.launch { snackbar.showSnackbar(msg) }
                                    } else {
                                        scope.launch { snackbar.showSnackbar("Error ${response.code()} al ${if (action == "oferta") "aplicar oferta" else "agregar"}") }
                                    }
                                }
                                override fun onFailure(call: Call<MenuItemDto>, t: Throwable) {
                                    isSubmitting = false
                                    scope.launch { snackbar.showSnackbar("Error: ${t.message}") }
                                }
                            })
                        },
                        enabled = !isSubmitting && selectedSucursal != null && selectedProducto != null,
                        colors = ButtonDefaults.buttonColors(containerColor = Btn),
                        modifier = Modifier.fillMaxWidth()
                    ) { Text(if (isSubmitting) "Procesando..." else when(action){"agregar"->"Agregar al menú";"oferta"->"Aplicar oferta";else->"Quitar del menú"}) }
                }
            }
        }
    }
}
