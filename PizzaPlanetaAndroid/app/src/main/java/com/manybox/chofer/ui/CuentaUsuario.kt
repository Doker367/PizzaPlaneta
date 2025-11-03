@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)
package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp

data class OrderSummary(
	val id: String,
	val date: String,
	val total: String,
	val items: List<String>
)

/**
 * Pantalla de "Mi cuenta" que muestra: nombre completo, teléfono, correo,
 * favoritos (pizzas / sucursal favorita), métodos de pago y historial de pedidos.
 *
 * Esta Composable es independiente y recibe datos (no hace llamadas de red).
 */
@Composable
fun CuentaUsuarioScreen(
	fullname: String,
	phone: String?,
	email: String?,
	favorites: List<String> = emptyList(),
	favoriteBranch: String? = null,
	paymentMethods: List<String> = emptyList(),
	orders: List<OrderSummary> = emptyList(),
	headerImageRes: Int? = null,
	onBack: (() -> Unit)? = null,
	isLoading: Boolean = false,
	errorMessage: String? = null,
	onEditProfile: () -> Unit = {},
	onLogout: () -> Unit = {},
	isLoggedIn: Boolean = true,
	onLogin: () -> Unit = {},
	onReorder: (orderId: String) -> Unit = {}
) {
	val Navy = Color(0xFF1D3557)
	val PanelBg = Color(0xFFF2F2F2)
	val ButtonRed = Color(0xFFE53935)
	val ButtonBlue = Color(0xFF1D3557)

	Box(modifier = Modifier.fillMaxSize().background(Navy)) {
		Column(modifier = Modifier.fillMaxSize()) {
			if (headerImageRes != null) {
				Box(modifier = Modifier
					.fillMaxWidth()
					.height(160.dp)) {
					Image(
						painter = painterResource(id = headerImageRes),
						contentDescription = "Header",
						modifier = Modifier.fillMaxSize(),
						contentScale = ContentScale.Crop
					)
					if (onBack != null) {
						Box(
							modifier = Modifier
								.padding(8.dp)
								.size(36.dp)
								.clip(CircleShape)
								.background(Color.White)
								.clickable { onBack() },
							contentAlignment = Alignment.Center
						) {
							Icon(Icons.Default.ArrowBack, contentDescription = "Volver", tint = Color.Black)
						}
					}
				}
				Box(
					modifier = Modifier
						.fillMaxWidth()
						.height(80.dp)
						.background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
				)
			}

			Box(modifier = Modifier.fillMaxSize()) {
				Card(
					modifier = Modifier
						.fillMaxWidth()
						.padding(horizontal = 16.dp)
						.align(Alignment.TopCenter)
						.offset(y = if (headerImageRes != null) (-60).dp else 0.dp),
					colors = CardDefaults.cardColors(containerColor = PanelBg),
					elevation = CardDefaults.cardElevation(defaultElevation = 12.dp),
					shape = RoundedCornerShape(12.dp)
				) {
					LazyColumn(modifier = Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(12.dp)) {
						item {
							Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.SpaceBetween, modifier = Modifier.fillMaxWidth()) {
								Column {
									Text(fullname, fontSize = 18.sp, fontWeight = FontWeight.Bold, color = Color.Black)
									Spacer(Modifier.height(4.dp))
									if (!phone.isNullOrBlank()) Text("Tel: $phone", color = Color.DarkGray, fontSize = 14.sp)
									if (!email.isNullOrBlank()) Text(email, color = Color.DarkGray, fontSize = 14.sp)
								}
								Column(horizontalAlignment = Alignment.End) {
									if (isLoggedIn) {
										Button(onClick = onEditProfile, colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue), shape = RoundedCornerShape(8.dp)) { Text("Editar", color = Color.White) }
										Spacer(Modifier.height(6.dp))
										OutlinedButton(onClick = onLogout, colors = ButtonDefaults.outlinedButtonColors(), shape = RoundedCornerShape(8.dp)) { Text("Cerrar sesión") }
									} else {
										Button(onClick = onLogin, colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue), shape = RoundedCornerShape(8.dp)) { Text("Iniciar sesión", color = Color.White) }
									}
								}
							}
						}

						item {
							Text("Favoritos", fontWeight = FontWeight.SemiBold, fontSize = 16.sp, color = Color.Black)
							Spacer(Modifier.height(8.dp))
							if (favorites.isEmpty()) {
								Text("No tienes favoritos aún.", color = Color.DarkGray)
							} else {
								FlowRowSpacing(favorites)
							}
							Spacer(Modifier.height(12.dp))
							Text("Sucursal favorita", fontWeight = FontWeight.SemiBold, fontSize = 14.sp, color = Color.Black)
							Spacer(Modifier.height(4.dp))
							Text(favoriteBranch ?: "No seleccionada", color = Color.DarkGray)
						}

						item {
							Text("Métodos de pago", fontWeight = FontWeight.SemiBold, fontSize = 16.sp, color = Color.Black)
							Spacer(Modifier.height(8.dp))
							if (paymentMethods.isEmpty()) {
								Text("No hay métodos agregados.", color = Color.DarkGray)
							} else {
								for (pm in paymentMethods) {
									Row(modifier = Modifier.fillMaxWidth().padding(vertical = 6.dp), verticalAlignment = Alignment.CenterVertically) {
										Text(pm, color = Color.Black, modifier = Modifier.weight(1f), maxLines = 1, overflow = TextOverflow.Ellipsis)
										TextButton(onClick = { /* editar/seleccionar */ }) { Text("Editar") }
									}
								}
							}
						}

						item {
							Text("Historial de pedidos", fontWeight = FontWeight.Bold, fontSize = 18.sp, color = Color.Black)
						}

						if (orders.isEmpty()) {
							item {
								Text("No hay pedidos todavía.", color = Color.DarkGray)
							}
						} else {
							items(orders) { order ->
								Card(shape = RoundedCornerShape(12.dp), colors = CardDefaults.cardColors(containerColor = Color.White), modifier = Modifier.fillMaxWidth()) {
									Column(modifier = Modifier.padding(12.dp)) {
										Row(modifier = Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically) {
											Column(modifier = Modifier.weight(1f)) {
												Text("Pedido #${order.id}", fontWeight = FontWeight.SemiBold, color = Color.Black)
												Spacer(Modifier.height(4.dp))
												Text(order.date, color = Color.DarkGray, fontSize = 12.sp)
												Spacer(Modifier.height(6.dp))
												Text(order.items.joinToString(", ") { it }, color = Color.Black, maxLines = 2, overflow = TextOverflow.Ellipsis)
											}
											Column(horizontalAlignment = Alignment.End) {
												Text(order.total, fontWeight = FontWeight.Bold, color = Color.Black)
												Spacer(Modifier.height(8.dp))
												Button(onClick = { onReorder(order.id) }, colors = ButtonDefaults.buttonColors(containerColor = ButtonRed), shape = RoundedCornerShape(8.dp)) { Text("Reordenar", color = Color.White) }
											}
										}
									}
								}
							}
						}

						if (isLoading) {
							item { Spacer(Modifier.height(8.dp)); CircularProgressIndicator(color = ButtonRed) }
						}
						if (!errorMessage.isNullOrEmpty()) {
							item { Spacer(Modifier.height(8.dp)); Text(errorMessage, color = Color.Red, fontWeight = FontWeight.Bold) }
						}
					}
				}
			}
		}
	}
}

@Composable
private fun FlowRowSpacing(items: List<String>) {
	// Simple replacement for chips/flow layout: wrap items in rows of chips
	Column {
		var row = mutableListOf<String>()
		var currentLen = 0
		for (it in items) {
			// naive wrapping based on length
			if (currentLen + it.length > 24 && row.isNotEmpty()) {
				Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(8.dp)) {
					for (r in row) ChipText(r)
				}
				Spacer(Modifier.height(6.dp))
				row = mutableListOf(it)
				currentLen = it.length
			} else {
				row.add(it)
				currentLen += it.length
			}
		}
		if (row.isNotEmpty()) {
			Row(modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(8.dp)) {
				for (r in row) ChipText(r)
			}
		}
	}
}

@Composable
private fun ChipText(text: String) {
	Surface(shape = RoundedCornerShape(16.dp), color = Color.White, shadowElevation = 0.dp) {
		Text(text = text, modifier = Modifier.padding(horizontal = 10.dp, vertical = 6.dp), color = Color.Black, fontSize = 13.sp)
	}
}

@Preview(showBackground = true)
@Composable
private fun CuentaUsuarioPreview() {
	val sampleOrders = remember {
		listOf(
			OrderSummary("1001", "2025-10-10 13:20", "S/ 45.00", listOf("Margherita", "Papitas")),
			OrderSummary("1002", "2025-10-12 19:05", "S/ 63.50", listOf("Pepperoni", "Coca-Cola"))
		)
	}

	CuentaUsuarioScreen(
		fullname = "Juan Pérez",
		phone = "+51 987 654 321",
		email = "juan.perez@example.com",
		favorites = listOf("Margarita", "Hawaiana", "Pepperoni"),
		favoriteBranch = "Sucursal Central - Av. Principal",
		paymentMethods = listOf("Tarjeta •••• 4242", "Efectivo"),
		orders = sampleOrders,
		onEditProfile = {},
		onLogout = {},
		onReorder = {}
	)
}
