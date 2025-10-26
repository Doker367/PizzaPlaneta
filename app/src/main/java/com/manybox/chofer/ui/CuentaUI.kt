package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.CheckCircle
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.R

data class UserInfo(
	val nombreCompleto: String,
	val correo: String,
	val correoVerificado: Boolean,
	val telefono: String,
	val telefonoVerificado: Boolean
)

data class PedidoResumen(
	val id: String,
	val fecha: String,
	val monto: String,
	val estado: String
)

data class MetodoPago(
	val alias: String,
	val detalle: String
)

data class Favoritos(
	val pizzas: List<String>,
	val combos: List<String>,
	val sucursales: List<String>
)

@Composable
fun CuentaScreen(
	headerImageRes: Int = R.drawable.pizzorra,
	onBack: () -> Unit = {},
	userInfo: UserInfo? = null,
	pedidos: List<PedidoResumen>? = null,
	metodosPago: List<MetodoPago>? = null,
	favoritos: Favoritos? = null,
	onSaveAll: (UserInfo, List<MetodoPago>, Favoritos) -> Unit = { _, _, _ -> }
) {
	val Navy = Color(0xFF1D3557)
	val PanelBg = Color(0xFFF2F2F2)

	// Mock data por defecto (MVP visual)
	val ui = userInfo ?: UserInfo(
		nombreCompleto = "Usuario Demo",
		correo = "usuario@correo.com",
		correoVerificado = true,
		telefono = "+52 55 1234 5678",
		telefonoVerificado = false
	)
	val listPedidos = pedidos ?: listOf(
		PedidoResumen("#A1023", "2025-10-20", "$ 249.00", "Entregado"),
		PedidoResumen("#A1022", "2025-10-17", "$ 189.00", "Cancelado"),
		PedidoResumen("#A1021", "2025-10-12", "$ 310.00", "En camino")
	)
	val pagos = metodosPago ?: listOf(
		MetodoPago("Visa••34", "Tarjeta terminación 1234"),
		MetodoPago("Efectivo", "Pagar al recibir")
	)
	val fav = favoritos ?: Favoritos(
		pizzas = listOf("Pepperoni", "Hawaiana"),
		combos = listOf("Combo Familiar", "Combo 2x1 Martes"),
		sucursales = listOf("Centro", "Norte")
	)

	// Estados de edición (MVP)
	var editBasic by remember { mutableStateOf(false) }
	var nombre by remember { mutableStateOf(ui.nombreCompleto) }
	var correo by remember { mutableStateOf(ui.correo) }
	var telefono by remember { mutableStateOf(ui.telefono) }
	var savedMsg by remember { mutableStateOf("") }

	val pagosState = remember { mutableStateListOf<MetodoPago>().apply { addAll(pagos) } }
	var managePagos by remember { mutableStateOf(false) }
	var newAlias by remember { mutableStateOf("") }
	var newDetalle by remember { mutableStateOf("") }

	val pizzasState = remember { mutableStateListOf<String>().apply { addAll(fav.pizzas) } }
	val combosState = remember { mutableStateListOf<String>().apply { addAll(fav.combos) } }
	val sucursalesState = remember { mutableStateListOf<String>().apply { addAll(fav.sucursales) } }
	var manageFavs by remember { mutableStateOf(false) }
	var newPizza by remember { mutableStateOf("") }
	var newCombo by remember { mutableStateOf("") }
	var newSucursal by remember { mutableStateOf("") }

	Box(modifier = Modifier.fillMaxSize().background(Navy)) {
		Column(modifier = Modifier.fillMaxSize()) {
			// Header imagen
			Box(modifier = Modifier.fillMaxWidth().height(180.dp)) {
				Image(
					painter = painterResource(id = headerImageRes),
					contentDescription = "Header",
					modifier = Modifier.fillMaxSize(),
					contentScale = ContentScale.Crop
				)
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

			// Arco
			Box(
				modifier = Modifier
					.fillMaxWidth()
					.height(90.dp)
					.background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
			)

			// Tarjeta
			Column(modifier = Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
				Card(
					modifier = Modifier
						.fillMaxWidth()
						.padding(horizontal = 16.dp)
						.offset(y = (-60).dp),
					colors = CardDefaults.cardColors(containerColor = PanelBg),
					elevation = CardDefaults.cardElevation(defaultElevation = 12.dp),
					shape = RoundedCornerShape(12.dp)
				) {
					Column(
						modifier = Modifier
							.padding(16.dp)
							.verticalScroll(rememberScrollState())
					) {
						Row(Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.SpaceBetween) {
							Text("👤 Información básica", fontWeight = FontWeight.ExtraBold, color = Color.Black, fontSize = 18.sp)
							if (!editBasic) {
								TextButton(onClick = { editBasic = true; savedMsg = "" }) { Text("Editar") }
							}
						}
						Spacer(Modifier.height(8.dp))
						if (editBasic) {
							EditLinea(label = "Nombre completo", value = nombre, onChange = { nombre = it })
							EditLinea(label = "Correo electrónico", value = correo, onChange = { correo = it })
							EditLinea(label = "Teléfono", value = telefono, onChange = { telefono = it })
							Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(8.dp)) {
								Button(onClick = {
									val updated = UserInfo(nombre, correo, ui.correoVerificado, telefono, ui.telefonoVerificado)
									onSaveAll(updated, pagosState.toList(), Favoritos(pizzasState.toList(), combosState.toList(), sucursalesState.toList()))
									savedMsg = "Datos guardados"
									editBasic = false
								}, modifier = Modifier.weight(1f)) { Text("Guardar") }
								OutlinedButton(onClick = {
									// revertir
									nombre = ui.nombreCompleto
									correo = ui.correo
									telefono = ui.telefono
									editBasic = false
								}, modifier = Modifier.weight(1f)) { Text("Cancelar") }
							}
						} else {
							InfoLinea("Nombre completo", nombre)
							InfoLineaVerificable("Correo electrónico", correo, ui.correoVerificado)
							InfoLineaVerificable("Teléfono", telefono, ui.telefonoVerificado)
						}
						if (savedMsg.isNotBlank()) {
							Spacer(Modifier.height(4.dp))
							Text(savedMsg, color = Color(0xFF2E7D32), fontSize = 12.sp)
						}

						Spacer(Modifier.height(16.dp))
						Divider()
						Spacer(Modifier.height(12.dp))

						Text("🧾 Historial de pedidos", fontWeight = FontWeight.ExtraBold, color = Color.Black, fontSize = 18.sp)
						Spacer(Modifier.height(8.dp))
						listPedidos.forEach { p -> PedidoItem(p) }

						Spacer(Modifier.height(16.dp))
						Divider()
						Spacer(Modifier.height(12.dp))

						Row(Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.SpaceBetween) {
							Text("💳 Métodos de pago", fontWeight = FontWeight.ExtraBold, color = Color.Black, fontSize = 18.sp)
							TextButton(onClick = { managePagos = !managePagos }) { Text(if (managePagos) "Listo" else "Administrar") }
						}
						Spacer(Modifier.height(8.dp))
						pagosState.forEachIndexed { idx, m ->
							if (managePagos) {
								Card(
									modifier = Modifier
										.fillMaxWidth()
										.padding(vertical = 4.dp),
									shape = RoundedCornerShape(10.dp),
									colors = CardDefaults.cardColors(containerColor = Color.White),
									elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
								) {
									Row(Modifier.padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
										Column(Modifier.weight(1f)) {
											Text(m.alias, fontWeight = FontWeight.SemiBold)
											Text(m.detalle, color = Color(0xFF64748B), fontSize = 12.sp)
										}
										TextButton(onClick = { pagosState.removeAt(idx) }) { Text("Eliminar") }
									}
								}
							} else {
								MetodoPagoItem(m)
							}
						}
						if (managePagos) {
							Spacer(Modifier.height(8.dp))
							Text("Agregar método", fontWeight = FontWeight.SemiBold)
							OutlinedTextField(value = newAlias, onValueChange = { newAlias = it }, label = { Text("Alias") }, singleLine = true)
							Spacer(Modifier.height(6.dp))
							OutlinedTextField(value = newDetalle, onValueChange = { newDetalle = it }, label = { Text("Detalle") }, singleLine = true)
							Spacer(Modifier.height(8.dp))
							Button(onClick = {
								if (newAlias.isNotBlank() && newDetalle.isNotBlank()) {
									pagosState.add(MetodoPago(newAlias, newDetalle))
									newAlias = ""
									newDetalle = ""
								}
							}) { Text("Agregar") }
						}

						Spacer(Modifier.height(16.dp))
						Divider()
						Spacer(Modifier.height(12.dp))

						Row(Modifier.fillMaxWidth(), verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.SpaceBetween) {
							Text("⭐ Favoritos", fontWeight = FontWeight.ExtraBold, color = Color.Black, fontSize = 18.sp)
							TextButton(onClick = { manageFavs = !manageFavs }) { Text(if (manageFavs) "Listo" else "Administrar") }
						}
						Spacer(Modifier.height(8.dp))
						FavoritosEditableGrupo("Pizzas", pizzasState, manageFavs, newItem = newPizza, onChangeNew = { newPizza = it }) {
							if (newPizza.isNotBlank()) { pizzasState.add(newPizza); newPizza = "" }
						}
						FavoritosEditableGrupo("Combos", combosState, manageFavs, newItem = newCombo, onChangeNew = { newCombo = it }) {
							if (newCombo.isNotBlank()) { combosState.add(newCombo); newCombo = "" }
						}
						FavoritosEditableGrupo("Sucursales", sucursalesState, manageFavs, newItem = newSucursal, onChangeNew = { newSucursal = it }) {
							if (newSucursal.isNotBlank()) { sucursalesState.add(newSucursal); newSucursal = "" }
						}

						Spacer(Modifier.height(8.dp))
					}
				}
			}
		}
	}
}

@Composable
private fun InfoLinea(label: String, value: String) {
	Column(Modifier.fillMaxWidth()) {
		Text(label, color = Color(0xFF475569), fontSize = 12.sp)
		Text(value, color = Color.Black, fontSize = 14.sp, fontWeight = FontWeight.SemiBold)
	}
	Spacer(Modifier.height(8.dp))
}

@Composable
private fun InfoLineaVerificable(label: String, value: String, verified: Boolean) {
	Column(Modifier.fillMaxWidth()) {
		Text(label, color = Color(0xFF475569), fontSize = 12.sp)
		Row(verticalAlignment = Alignment.CenterVertically) {
			Text(value, color = Color.Black, fontSize = 14.sp, fontWeight = FontWeight.SemiBold)
			if (verified) {
				Spacer(Modifier.width(6.dp))
				Icon(Icons.Default.CheckCircle, contentDescription = null, tint = Color(0xFF2E7D32), modifier = Modifier.size(18.dp))
				Text("Verificado", color = Color(0xFF2E7D32), fontSize = 12.sp, modifier = Modifier.padding(start = 2.dp))
			}
		}
	}
	Spacer(Modifier.height(8.dp))
}

@Composable
private fun EditLinea(label: String, value: String, onChange: (String) -> Unit) {
	Text(label, color = Color(0xFF475569), fontSize = 12.sp)
	OutlinedTextField(
		value = value,
		onValueChange = onChange,
		singleLine = true,
		modifier = Modifier.fillMaxWidth()
	)
	Spacer(Modifier.height(8.dp))
}

@Composable
private fun PedidoItem(p: PedidoResumen) {
	Card(
		modifier = Modifier
			.fillMaxWidth()
			.padding(vertical = 4.dp),
		shape = RoundedCornerShape(10.dp),
		colors = CardDefaults.cardColors(containerColor = Color.White),
		elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
	) {
		Row(Modifier.padding(12.dp), verticalAlignment = Alignment.CenterVertically) {
			Column(Modifier.weight(1f)) {
				Text(p.id, fontWeight = FontWeight.SemiBold)
				Text(p.fecha, color = Color(0xFF64748B), fontSize = 12.sp)
			}
			Column(horizontalAlignment = Alignment.End) {
				Text(p.monto, fontWeight = FontWeight.SemiBold)
				Text(p.estado, color = Color(0xFF64748B), fontSize = 12.sp)
			}
		}
	}
}

@Composable
private fun MetodoPagoItem(m: MetodoPago) {
	Card(
		modifier = Modifier
			.fillMaxWidth()
			.padding(vertical = 4.dp),
		shape = RoundedCornerShape(10.dp),
		colors = CardDefaults.cardColors(containerColor = Color.White),
		elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
	) {
		Column(Modifier.padding(12.dp)) {
			Text(m.alias, fontWeight = FontWeight.SemiBold)
			Text(m.detalle, color = Color(0xFF64748B), fontSize = 12.sp)
		}
	}
}

@Composable
private fun FavoritosGrupo(titulo: String, items: List<String>) {
	Text(titulo, fontWeight = FontWeight.SemiBold, color = Color.Black)
	Spacer(Modifier.height(4.dp))
	if (items.isEmpty()) {
		Text("(Vacío)", color = Color(0xFF64748B), fontSize = 12.sp)
	} else {
		items.forEach { Text("• $it", color = Color.Black, fontSize = 14.sp) }
	}
	Spacer(Modifier.height(8.dp))
}

@Composable
private fun FavoritosEditableGrupo(
	titulo: String,
	items: MutableList<String>,
	editable: Boolean,
	newItem: String,
	onChangeNew: (String) -> Unit,
	onAdd: () -> Unit
) {
	Text(titulo, fontWeight = FontWeight.SemiBold, color = Color.Black)
	Spacer(Modifier.height(4.dp))
	if (items.isEmpty()) {
		Text("(Vacío)", color = Color(0xFF64748B), fontSize = 12.sp)
	} else {
		items.forEachIndexed { idx, s ->
			if (editable) {
				Row(Modifier.fillMaxWidth().padding(vertical = 2.dp), verticalAlignment = Alignment.CenterVertically) {
					Text("• $s", color = Color.Black, fontSize = 14.sp, modifier = Modifier.weight(1f))
					TextButton(onClick = { items.removeAt(idx) }) { Text("Eliminar") }
				}
			} else {
				Text("• $s", color = Color.Black, fontSize = 14.sp)
			}
		}
	}
	if (editable) {
		Spacer(Modifier.height(6.dp))
		OutlinedTextField(value = newItem, onValueChange = onChangeNew, label = { Text("Agregar a $titulo") }, singleLine = true)
		Spacer(Modifier.height(6.dp))
		Button(onClick = onAdd) { Text("Agregar") }
	}
	Spacer(Modifier.height(8.dp))
}

