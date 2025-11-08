@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)
package com.manybox.chofer.ui

import androidx.compose.animation.Crossfade
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
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.manybox.chofer.R

data class OrderSummary(
    val id: String,
    val date: String,
    val total: String,
    val items: List<String>
)

// Nuevo: Información de una tarjeta (solo front, sin tokenización Stripe todavía)
data class CardInfo(
    val brand: String,
    val maskedNumber: String,
    val holder: String,
    val expiry: String
)

// Tipo de método de pago (Efectivo o Tarjeta específica)
sealed class PaymentSelection {
    object Cash : PaymentSelection()
    data class Card(val card: CardInfo) : PaymentSelection()
}

// Tabs de la cuenta
private enum class AccountTab(val title: String) { Profile("Perfil"), Payments("Pagos"), Orders("Historial") }

@Composable
fun CuentaUsuarioScreen(
    fullname: String,
    phone: String?,
    email: String?,
    favorites: List<String> = emptyList(),
    favoriteBranch: String? = null,
    cards: List<CardInfo> = emptyList(),
    orders: List<OrderSummary> = emptyList(),
    headerImageRes: Int? = null,
    onBack: (() -> Unit)? = null,
    isLoading: Boolean = false,
    errorMessage: String? = null,
    onEditProfile: () -> Unit = {},
    onLogout: () -> Unit = {},
    isLoggedIn: Boolean = true,
    onLogin: () -> Unit = {},
    onReorder: (orderId: String) -> Unit = {},
    onCardAdded: (CardInfo) -> Unit = {}
) {
    val ButtonRed = Color(0xFFE53935)
    val ButtonBlue = Color(0xFF1D3557)

    var cardNumber by remember { mutableStateOf("") }
    var cardHolder by remember { mutableStateOf(fullname) }
    var cardExpiry by remember { mutableStateOf("") } // MM/YY
    var cardCvc by remember { mutableStateOf("") }
    val cardBrand by derivedStateOf { detectBrand(cardNumber) }
    val formattedNumber by derivedStateOf { formatCardNumber(cardNumber) }
    val cardValid by derivedStateOf { cardBrand.isNotBlank() && cardNumber.length >= 12 && isExpiryValid(cardExpiry) && cardCvc.length in 3..4 }
    var selection by remember { mutableStateOf<PaymentSelection>(PaymentSelection.Cash) }
    var tab by remember { mutableStateOf(AccountTab.Profile) }

    Box(modifier = Modifier.fillMaxSize().background(Color.White)) {
        Column(modifier = Modifier.fillMaxSize()) {
            if (headerImageRes != null) {
                Box(modifier = Modifier.fillMaxWidth().height(160.dp)) {
                    Image(painter = painterResource(headerImageRes), contentDescription = "Header", modifier = Modifier.fillMaxSize(), contentScale = ContentScale.Crop)
                    if (onBack != null) {
                        Box(
                            modifier = Modifier.padding(8.dp).size(36.dp).clip(CircleShape).background(Color.White).clickable { onBack() },
                            contentAlignment = Alignment.Center
                        ) { Icon(Icons.Default.ArrowBack, contentDescription = "Volver", tint = Color.Black) }
                    }
                }
            }

            Card(
                modifier = Modifier.fillMaxWidth().padding(horizontal = 16.dp, vertical = 8.dp),
                colors = CardDefaults.cardColors(containerColor = Color.White),
                elevation = CardDefaults.cardElevation(0.dp),
                shape = RoundedCornerShape(12.dp)
            ) {
                Column(Modifier.padding(16.dp)) {
                    Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.SpaceBetween, modifier = Modifier.fillMaxWidth()) {
                        Column {
                            Text(fullname, fontSize = 22.sp, fontWeight = FontWeight.ExtraBold, color = Color.Black)
                            if (!phone.isNullOrBlank()) { Spacer(Modifier.height(2.dp)); Text(phone, color = Color(0xFF444444), fontSize = 16.sp, fontWeight = FontWeight.Medium) }
                            if (!email.isNullOrBlank()) { Spacer(Modifier.height(2.dp)); Text(email, color = Color.DarkGray, fontSize = 13.sp) }
                        }
                        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                            if (isLoggedIn) {
                                OutlinedButton(onClick = onEditProfile, shape = RoundedCornerShape(8.dp)) { Text("Editar") }
                                Button(onClick = onLogout, colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue), shape = RoundedCornerShape(8.dp)) { Text("Salir", color = Color.White) }
                            } else {
                                Button(onClick = onLogin, colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue), shape = RoundedCornerShape(8.dp)) { Text("Iniciar", color = Color.White) }
                            }
                        }
                    }

                    Spacer(Modifier.height(12.dp))
                    val tabs = listOf(AccountTab.Profile, AccountTab.Payments, AccountTab.Orders)
                    TabRow(selectedTabIndex = tabs.indexOf(tab), containerColor = Color.Transparent) {
                        tabs.forEach { t ->
                            Tab(selected = t == tab, onClick = { tab = t }, text = { Text(t.title) })
                        }
                    }

                    Spacer(Modifier.height(12.dp))
                    Crossfade(targetState = tab, label = "account_tab") { current ->
                        when (current) {
                            AccountTab.Profile -> {
                                Column(verticalArrangement = Arrangement.spacedBy(12.dp)) {
                                    if (favorites.isNotEmpty()) {
                                        Text("Favoritos", fontWeight = FontWeight.Bold)
                                        FlowRowSpacing(favorites)
                                    }
                                    if (!favoriteBranch.isNullOrBlank()) {
                                        Divider()
                                        Text("Sucursal favorita", fontWeight = FontWeight.Bold)
                                        Text(favoriteBranch!!, color = Color.DarkGray, fontSize = 13.sp)
                                    }
                                }
                            }

                            AccountTab.Payments -> {
                                LazyColumn(verticalArrangement = Arrangement.spacedBy(12.dp)) {
                                    item {
                                        Text("Método de pago", fontWeight = FontWeight.Bold, fontSize = 18.sp)
                                        Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                                            FilterChip(selected = selection is PaymentSelection.Cash, onClick = { selection = PaymentSelection.Cash }, label = { Text("Efectivo") })
                                            FilterChip(selected = selection is PaymentSelection.Card, onClick = { if (cards.isNotEmpty()) selection = PaymentSelection.Card(cards.first()) }, label = { Text("Tarjeta") })
                                        }
                                    }

                                    if (cards.isEmpty()) {
                                        item { Text("No hay tarjetas guardadas.", color = Color.DarkGray) }
                                    } else {
                                        items(cards) { card ->
                                            Card(
                                                shape = RoundedCornerShape(16.dp),
                                                elevation = CardDefaults.cardElevation(6.dp),
                                                colors = CardDefaults.cardColors(containerColor = Color.White),
                                                modifier = Modifier.fillMaxWidth().clickable { selection = PaymentSelection.Card(card) }
                                            ) {
                                                Row(modifier = Modifier.padding(16.dp), verticalAlignment = Alignment.CenterVertically) {
                                                    val icon = brandIconRes(card.brand)
                                                    if (icon != null) {
                                                        Image(painter = painterResource(icon), contentDescription = card.brand, modifier = Modifier.size(40.dp))
                                                        Spacer(Modifier.width(12.dp))
                                                    }
                                                    Column(modifier = Modifier.weight(1f)) {
                                                        Text(card.brand, fontWeight = FontWeight.Bold, fontSize = 14.sp)
                                                        Spacer(Modifier.height(4.dp))
                                                        Text(card.maskedNumber, fontSize = 16.sp, fontWeight = FontWeight.Medium)
                                                        Spacer(Modifier.height(6.dp))
                                                        Row(horizontalArrangement = Arrangement.SpaceBetween, modifier = Modifier.fillMaxWidth()) {
                                                            Text(card.holder, fontSize = 12.sp)
                                                            Text(card.expiry, fontSize = 12.sp)
                                                        }
                                                        if (selection is PaymentSelection.Card && (selection as PaymentSelection.Card).card == card) {
                                                            Spacer(Modifier.height(6.dp))
                                                            AssistChip(onClick = { /* editar */ }, label = { Text("Seleccionada") })
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    item { Divider() }
                                    item { Text("Agregar tarjeta", fontWeight = FontWeight.SemiBold, fontSize = 16.sp) }
                                    item {
                                        CardPreview(
                                            brand = cardBrand,
                                            number = formattedNumber.ifBlank { "0000 0000 0000 0000" },
                                            holder = cardHolder.ifBlank { "NOMBRE TITULAR" },
                                            expiry = cardExpiry.ifBlank { "MM/YY" }
                                        )
                                    }
                                    item {
                                        Column(verticalArrangement = Arrangement.spacedBy(12.dp)) {
                                            OutlinedTextField(value = cardNumber, onValueChange = { if (it.length <= 19) cardNumber = it.filter { ch -> ch.isDigit() } }, label = { Text("Número de tarjeta") }, singleLine = true, placeholder = { Text("4111 1111 1111 1111") })
                                            Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                                                OutlinedTextField(value = cardExpiry, onValueChange = { if (it.length <= 5) cardExpiry = it.filter { ch -> ch.isDigit() || ch == '/' } }, label = { Text("Expiración (MM/YY)") }, singleLine = true, modifier = Modifier.weight(1f))
                                                OutlinedTextField(value = cardCvc, onValueChange = { if (it.length <= 4) cardCvc = it.filter { ch -> ch.isDigit() } }, label = { Text("CVC") }, singleLine = true, modifier = Modifier.weight(1f))
                                            }
                                            OutlinedTextField(value = cardHolder, onValueChange = { cardHolder = it.uppercase() }, label = { Text("Nombre del titular") }, singleLine = true)
                                            Button(onClick = {
                                                val newCard = CardInfo(brand = cardBrand, maskedNumber = maskCardNumber(cardNumber), holder = cardHolder, expiry = cardExpiry)
                                                onCardAdded(newCard)
                                                cardNumber = ""; cardExpiry = ""; cardCvc = ""; selection = PaymentSelection.Card(newCard)
                                            }, enabled = cardValid, colors = ButtonDefaults.buttonColors(containerColor = ButtonBlue), shape = RoundedCornerShape(10.dp)) { Text("Guardar tarjeta", color = Color.White) }
                                            if (!cardValid) Text("Completa los datos correctamente", color = Color.Red, fontSize = 12.sp)
                                        }
                                    }
                                }
                            }

                            AccountTab.Orders -> {
                                LazyColumn(verticalArrangement = Arrangement.spacedBy(12.dp)) {
                                    if (orders.isEmpty()) {
                                        item { Text("No hay pedidos todavía.", color = Color.DarkGray) }
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
                                    if (isLoading) item { Spacer(Modifier.height(8.dp)); CircularProgressIndicator(color = ButtonRed) }
                                    if (!errorMessage.isNullOrEmpty()) item { Spacer(Modifier.height(8.dp)); Text(errorMessage, color = Color.Red, fontWeight = FontWeight.Bold) }
                                }
                            }
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

@Composable
private fun CardPreview(
    brand: String,
    number: String,
    holder: String,
    expiry: String
) {
    val gradient = Brush.linearGradient(
        colors = listOf(Color(0xFF232526), Color(0xFF414345))
    )
    Card(
        shape = RoundedCornerShape(20.dp),
        elevation = CardDefaults.cardElevation(defaultElevation = 8.dp),
        colors = CardDefaults.cardColors(containerColor = Color.Transparent),
        modifier = Modifier
            .fillMaxWidth()
            .height(190.dp)
            .background(Color.Transparent)
    ) {
        Box(
            modifier = Modifier
                .fillMaxSize()
                .background(gradient)
                .padding(18.dp)
        ) {
            Column(modifier = Modifier.fillMaxSize()) {
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = brand.ifBlank { "TARJETA" },
                        color = Color.White.copy(alpha = 0.9f),
                        fontWeight = FontWeight.Bold
                    )
                    val icon = brandIconRes(brand)
                    if (icon != null) {
                        Image(
                            painter = painterResource(id = icon),
                            contentDescription = brand,
                            modifier = Modifier.size(48.dp)
                        )
                    } else {
                        Box(
                            modifier = Modifier
                                .size(36.dp)
                                .clip(RoundedCornerShape(6.dp))
                                .background(Color(0x66FFFFFF))
                        )
                    }
                }
                Spacer(Modifier.height(18.dp))
                Text(
                    text = spacedNumber(number),
                    color = Color.White,
                    fontSize = 20.sp,
                    fontWeight = FontWeight.Medium
                )
                Spacer(Modifier.height(18.dp))
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column {
                        Text("TITULAR", color = Color.White.copy(alpha = 0.7f), fontSize = 10.sp)
                        Text(holder, color = Color.White, fontSize = 14.sp, fontWeight = FontWeight.SemiBold)
                    }
                    Column(horizontalAlignment = Alignment.End) {
                        Text("VENCE", color = Color.White.copy(alpha = 0.7f), fontSize = 10.sp)
                        Text(expiry, color = Color.White, fontSize = 14.sp, fontWeight = FontWeight.SemiBold)
                    }
                }
            }
        }
    }
}

// Utils de formato (front-only)
private fun formatCardNumber(raw: String): String = raw.filter { it.isDigit() }
private fun spacedNumber(num: String): String = num.chunked(4).joinToString(" ")
private fun maskCardNumber(raw: String): String {
    val digits = raw.filter { it.isDigit() }
    if (digits.length <= 4) return digits
    val visible = digits.takeLast(4)
    val groups = (digits.length - 4 + 3) / 4
    val masked = (1..groups).joinToString(" ") { "••••" }
    return "$masked $visible".trim()
}
private fun detectBrand(raw: String): String {
    val d = raw.filter { it.isDigit() }
    return when {
        d.startsWith("4") -> "VISA"
        d.startsWith("51") || d.startsWith("52") || d.startsWith("53") || d.startsWith("54") || d.startsWith("55") -> "MASTERCARD"
        d.startsWith("34") || d.startsWith("37") -> "AMEX"
        d.startsWith("60") || d.startsWith("62") || d.startsWith("64") || d.startsWith("65") -> "DISCOVER"
        else -> ""
    }
}
private fun isExpiryValid(mmYY: String): Boolean {
    val parts = mmYY.split("/")
    if (parts.size != 2) return false
    val mm = parts[0].toIntOrNull() ?: return false
    val yy = parts[1].toIntOrNull() ?: return false
    return mm in 1..12 && yy in 0..99
}

// Mapea la marca a un ícono dibujable si existe
private fun brandIconRes(brand: String): Int? = when (brand.uppercase()) {
    "VISA" -> R.drawable.ic_visa
    "MASTERCARD", "MASTER CARD" -> R.drawable.ic_mastercard
    "AMEX", "AMERICAN EXPRESS" -> R.drawable.ic_amex
    "DISCOVER" -> R.drawable.ic_discover
    else -> null
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

    val sampleCards = listOf(
        CardInfo(
            brand = "VISA",
            maskedNumber = "•••• •••• •••• 4242",
            holder = "JUAN PEREZ",
            expiry = "12/29"
        )
    )
    CuentaUsuarioScreen(
        fullname = "Juan Pérez",
        phone = "+51 987 654 321",
        email = "juan.perez@example.com",
        favorites = listOf("Margarita", "Hawaiana", "Pepperoni"),
        favoriteBranch = "Sucursal Central - Av. Principal",
        cards = sampleCards,
        orders = sampleOrders,
        onEditProfile = {},
        onLogout = {},
        onReorder = {},
        onCardAdded = {}
    )
}