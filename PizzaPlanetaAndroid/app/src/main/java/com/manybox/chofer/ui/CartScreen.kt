package com.manybox.chofer.ui

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.api.CreateOrderRequest
import com.manybox.chofer.api.OrderItemRequest
import com.manybox.chofer.api.RetrofitProvider
import com.manybox.chofer.api.TokenStore
import kotlinx.coroutines.launch
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import androidx.compose.ui.platform.LocalContext

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun CartScreen(
    viewModel: CartViewModel,
    onBack: () -> Unit,
    onOrderSuccess: () -> Unit,
    snackbarHostState: SnackbarHostState = remember { SnackbarHostState() }
) {
    val scope = rememberCoroutineScope()
    val context = LocalContext.current
    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Tu carrito", fontWeight = FontWeight.Bold) },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Regresar")
                    }
                }
            )
        },
        snackbarHost = { SnackbarHost(snackbarHostState) }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(16.dp)
        ) {
            if (viewModel.items.isEmpty()) {
                Text("Tu carrito está vacío", fontSize = 16.sp)
            } else {
                LazyColumn(modifier = Modifier.weight(1f)) {
                    items(viewModel.items) { item ->
                        ElevatedCard(modifier = Modifier.fillMaxWidth().padding(vertical = 6.dp)) {
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(12.dp),
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Column(modifier = Modifier.weight(1f)) {
                                    Text(item.nombre, fontWeight = FontWeight.SemiBold)
                                    Text("${item.cantidad} x $" + "%.0f".format(item.precioUnitario), fontSize = 12.sp, color = MaterialTheme.colorScheme.onSurfaceVariant)
                                }
                                Text("$" + "%.0f".format(item.subtotal), fontWeight = FontWeight.Bold)
                                Spacer(Modifier.width(12.dp))
                                Row(verticalAlignment = Alignment.CenterVertically) {
                                    OutlinedButton(onClick = { viewModel.decrement(item.productoId) }, modifier = Modifier.height(32.dp)) { Text("-") }
                                    Spacer(Modifier.width(6.dp))
                                    OutlinedButton(onClick = { viewModel.increment(item.productoId) }, modifier = Modifier.height(32.dp)) { Text("+") }
                                }
                            }
                        }
                    }
                }
                Divider(modifier = Modifier.padding(vertical = 8.dp))
                Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
                    Text("Total", fontWeight = FontWeight.Bold)
                    Text("$" + "%.0f".format(viewModel.total()), fontWeight = FontWeight.Bold)
                }
                Spacer(Modifier.height(16.dp))
                Button(
                    onClick = {
                        val sucursalId = viewModel.sucursalId
                        if (sucursalId == null) {
                            scope.launch { snackbarHostState.showSnackbar("Falta la sucursal") }
                            return@Button
                        }
                        val token = TokenStore.getTokenBlocking(context)
                        if (token.isNullOrBlank()) {
                            scope.launch { snackbarHostState.showSnackbar("Inicia sesión para ordenar") }
                            return@Button
                        }
                        val body = CreateOrderRequest(
                            sucursalId = sucursalId,
                            items = viewModel.items.map { OrderItemRequest(productoId = it.productoId, cantidad = it.cantidad) },
                            metodoPago = "Efectivo"
                        )
                        RetrofitProvider.pizzaApi(context).createOrder(body).enqueue(object: Callback<Void> {
                            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                                if (response.isSuccessful) {
                                    scope.launch { snackbarHostState.showSnackbar("Pedido enviado") }
                                    viewModel.clear()
                                    onOrderSuccess()
                                } else {
                                    scope.launch { snackbarHostState.showSnackbar("Error ${response.code()}") }
                                }
                            }
                            override fun onFailure(call: Call<Void>, t: Throwable) {
                                scope.launch { snackbarHostState.showSnackbar("Error: ${t.message}") }
                            }
                        })
                    },
                    modifier = Modifier.fillMaxWidth(),
                    enabled = viewModel.items.isNotEmpty()
                ) { Text("Finalizar Pedido") }
            }
        }
    }
}