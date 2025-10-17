package com.manybox.chofer.ui

import androidx.compose.animation.*
import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.*
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import com.manybox.chofer.model.GuiaAsignada
import com.manybox.chofer.model.EstadoGuia
import com.manybox.chofer.model.EmpleadoPerfil
import com.manybox.chofer.model.BitacoraEntrega


@Composable
fun DashboardScreen(
    nombre: String,
    empleadoPerfil: EmpleadoPerfil?,
    pendientes: Int,
    confirmados: Int,
    incidencias: Int,
    guias: List<GuiaAsignada>,
    bitacoraEntregas: List<BitacoraEntrega>,
    entregasPorDia: List<com.manybox.chofer.api.EntregasPorDia>,
    notificaciones: List<com.manybox.chofer.api.NotificacionUsuarioVM>,
    notificacionesNoLeidas: Int,
    notifsConectado: Boolean = true,
    onMarcarNotificacionLeida: (Int) -> Unit,
    onMarcarTodasLeidas: () -> Unit,
    onRecoger: (GuiaAsignada) -> Unit,
    onIniciarEntrega: (GuiaAsignada) -> Unit,
    onEntregar: (GuiaAsignada) -> Unit,
    onScanBarcode: () -> Unit
) {
    var selectedTab by remember { mutableStateOf(0) }
    var expandedGuiaId by remember { mutableStateOf<Int?>(null) }
    var showSignatureDialog by remember { mutableStateOf(false) }
    var currentGuiaForSignature by remember { mutableStateOf<GuiaAsignada?>(null) }

    Scaffold(
    bottomBar = { BottomNavigationBar(selectedTab, notificacionesNoLeidas, notifsConectado) { selectedTab = it } },
        containerColor = Color(0xFF0F1419)
    ) { padding ->
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
        ) {
            when (selectedTab) {
                0 -> HomeTab(nombre, empleadoPerfil, pendientes, confirmados, incidencias, entregasPorDia)
                1 -> GuiasTab(
                    guias,
                    expandedGuiaId,
                    onExpandGuia = { guiaId -> expandedGuiaId = if (expandedGuiaId == guiaId) null else guiaId },
                    onRecoger,
                    onIniciarEntrega,
                    onEntregar = { guia ->
                        currentGuiaForSignature = guia
                        showSignatureDialog = true
                    },
                    onScanBarcode
                )
                2 -> BitacoraTab(bitacoraEntregas)
                3 -> NotificacionesTab(
                    notificaciones = notificaciones,
                    onMarcarNotificacionLeida = onMarcarNotificacionLeida,
                    onMarcarTodasLeidas = onMarcarTodasLeidas
                )
            }
        }

        if (showSignatureDialog && currentGuiaForSignature != null) {
            SignatureDialog(
                guia = currentGuiaForSignature!!,
                onDismiss = { showSignatureDialog = false },
                onConfirm = { guia ->
                    showSignatureDialog = false
                    onEntregar(guia)
                    currentGuiaForSignature = null
                }
            )
        }
    }
}


@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun BottomNavigationBar(selectedTab: Int, notificacionesNoLeidas: Int, notifsConectado: Boolean, onTabSelected: (Int) -> Unit) {
    NavigationBar(containerColor = Color(0xFF1A1F29)) {
        NavigationBarItem(
            selected = selectedTab == 0,
            onClick = { onTabSelected(0) },
            icon = { Icon(Icons.Default.Home, contentDescription = "Inicio") },
            label = { Text("Inicio", fontSize = 11.sp) },
            colors = NavigationBarItemDefaults.colors(
                selectedIconColor = Color(0xFF6C63FF),
                selectedTextColor = Color(0xFF6C63FF),
                unselectedIconColor = Color(0xFF8E8E93),
                unselectedTextColor = Color(0xFF8E8E93),
                indicatorColor = Color(0xFF2D3748)
            )
        )
        NavigationBarItem(
            selected = selectedTab == 1,
            onClick = { onTabSelected(1) },
            icon = { Icon(Icons.Default.List, contentDescription = "Guías") },
            label = { Text("Guías", fontSize = 11.sp) },
            colors = NavigationBarItemDefaults.colors(
                selectedIconColor = Color(0xFF6C63FF),
                selectedTextColor = Color(0xFF6C63FF),
                unselectedIconColor = Color(0xFF8E8E93),
                unselectedTextColor = Color(0xFF8E8E93),
                indicatorColor = Color(0xFF2D3748)
            )
        )
        NavigationBarItem(
            selected = selectedTab == 2,
            onClick = { onTabSelected(2) },
            icon = { Icon(Icons.Default.Book, contentDescription = "Bitácora") },
            label = { Text("Bitácora", fontSize = 11.sp) },
            colors = NavigationBarItemDefaults.colors(
                selectedIconColor = Color(0xFF6C63FF),
                selectedTextColor = Color(0xFF6C63FF),
                unselectedIconColor = Color(0xFF8E8E93),
                unselectedTextColor = Color(0xFF8E8E93),
                indicatorColor = Color(0xFF2D3748)
            )
        )
        NavigationBarItem(
            selected = selectedTab == 3,
            onClick = { onTabSelected(3) },
            icon = {
                if (notificacionesNoLeidas > 0) {
                    BadgedBox(badge = {
                        Badge(containerColor = Color(0xFFFF5252)) {
                            Text(text = notificacionesNoLeidas.toString(), color = Color.White, fontSize = 10.sp)
                        }
                    }) {
                        Box {
                            Icon(Icons.Default.Notifications, contentDescription = "Notificaciones")
                            // Indicador de conexión
                            Box(
                                modifier = Modifier
                                    .size(8.dp)
                                    .align(Alignment.BottomEnd)
                                    .clip(CircleShape)
                                    .background(if (notifsConectado) Color(0xFF00C853) else Color(0xFF8E8E93))
                            )
                        }
                    }
                } else {
                    Box {
                        Icon(Icons.Default.Notifications, contentDescription = "Notificaciones")
                        Box(
                            modifier = Modifier
                                .size(8.dp)
                                .align(Alignment.BottomEnd)
                                .clip(CircleShape)
                                .background(if (notifsConectado) Color(0xFF00C853) else Color(0xFF8E8E93))
                        )
                    }
                }
            },
            label = { Text("Notif.", fontSize = 11.sp) },
            colors = NavigationBarItemDefaults.colors(
                selectedIconColor = Color(0xFF6C63FF),
                selectedTextColor = Color(0xFF6C63FF),
                unselectedIconColor = Color(0xFF8E8E93),
                unselectedTextColor = Color(0xFF8E8E93),
                indicatorColor = Color(0xFF2D3748)
            )
        )
    }
}

// Home Tab
@Composable
fun HomeTab(nombre: String, empleadoPerfil: EmpleadoPerfil?, pendientes: Int, confirmados: Int, incidencias: Int, entregasPorDia: List<com.manybox.chofer.api.EntregasPorDia>) {
    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(Color(0xFF0F1419))
            .padding(16.dp)
    ) {
        item {
            // Header con avatar y datos del chofer
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                shape = RoundedCornerShape(16.dp)
            ) {
                Column(modifier = Modifier.padding(20.dp)) {
                    Row(
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Box(
                            modifier = Modifier
                                .size(64.dp)
                                .clip(CircleShape)
                                .background(Color(0xFF6C63FF)),
                            contentAlignment = Alignment.Center
                        ) {
                            Icon(
                                Icons.Default.Person,
                                contentDescription = "Avatar",
                                tint = Color.White,
                                modifier = Modifier.size(36.dp)
                            )
                        }
                        Spacer(modifier = Modifier.width(16.dp))
                        Column {
                            Text(
                                "Bienvenido, $nombre",
                                fontSize = 20.sp,
                                fontWeight = FontWeight.Bold,
                                color = Color.White
                            )
                            Text(
                                empleadoPerfil?.sucursalNombre ?: "Cargando...",
                                fontSize = 14.sp,
                                color = Color(0xFF6C63FF)
                            )
                        }
                    }
                    
                    if (empleadoPerfil != null) {
                        Spacer(modifier = Modifier.height(16.dp))
                        Divider(color = Color(0xFF2D3748))
                        Spacer(modifier = Modifier.height(12.dp))
                        
                        // Información de contacto
                        InfoRow(Icons.Default.Email, empleadoPerfil.correo)
                        Spacer(modifier = Modifier.height(8.dp))
                        InfoRow(Icons.Default.Phone, empleadoPerfil.telefono)
                    }
                }
            }
        }

        item {
            // Resumen rápido de entregas
            Text(
                "📊 Resumen de Entregas",
                fontSize = 16.sp,
                fontWeight = FontWeight.SemiBold,
                color = Color.White,
                modifier = Modifier.padding(bottom = 12.dp)
            )
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                SummaryCard("Pendientes", pendientes, Color(0xFF6C63FF), Icons.Default.Inventory, modifier = Modifier.weight(1f))
                Spacer(modifier = Modifier.width(8.dp))
                SummaryCard("Completados", confirmados, Color(0xFF00C853), Icons.Default.CheckCircle, modifier = Modifier.weight(1f))
                Spacer(modifier = Modifier.width(8.dp))
                SummaryCard("Incidencias", incidencias, Color(0xFFFF5252), Icons.Default.Warning, modifier = Modifier.weight(1f))
            }
            
            Spacer(modifier = Modifier.height(16.dp))
        }

        // Gráfica mejorada: justo después del resumen rápido de entregas
        item {
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                shape = RoundedCornerShape(12.dp),
                elevation = CardDefaults.cardElevation(6.dp)
            ) {
                Column(modifier = Modifier.padding(12.dp)) {
                    Text(
                        "� Entregas recientes",
                        fontSize = 16.sp,
                        fontWeight = FontWeight.SemiBold,
                        color = Color.White,
                        modifier = Modifier.padding(bottom = 8.dp)
                    )

                    if (entregasPorDia.isEmpty()) {
                        Box(modifier = Modifier
                            .fillMaxWidth()
                            .height(80.dp), contentAlignment = Alignment.Center) {
                            Text("No hay datos para la gráfica", color = Color(0xFF8E8E93))
                        }
                    } else {
                        val max = (entregasPorDia.maxOfOrNull { it.Cantidad } ?: 1).coerceAtLeast(1)
                        val scrollState = rememberScrollState()
                        Row(modifier = Modifier
                            .fillMaxWidth()
                            .horizontalScroll(scrollState)
                            .padding(vertical = 6.dp), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
                            entregasPorDia.forEach { dia ->
                                Column(horizontalAlignment = Alignment.CenterHorizontally, modifier = Modifier.width(56.dp)) {
                                    // número encima de la barra
                                    Text(dia.Cantidad.toString(), color = Color.White, fontSize = 13.sp, fontWeight = FontWeight.Bold)
                                    Spacer(modifier = Modifier.height(6.dp))
                                    val heightPct = (dia.Cantidad.toFloat() / max.toFloat()).coerceIn(0f, 1f)
                                    val targetHeight = (120 * heightPct).dp
                                    Box(
                                        modifier = Modifier
                                            .height(120.dp)
                                            .width(40.dp), contentAlignment = Alignment.BottomCenter) {
                                        Box(modifier = Modifier
                                            .width(28.dp)
                                            .height(targetHeight)
                                            .clip(RoundedCornerShape(8.dp))
                                            .background(Color(0xFF6C63FF)))
                                    }
                                    Spacer(modifier = Modifier.height(6.dp))
                                    Text(dia.Fecha.takeLast(5), color = Color(0xFF8E8E93), fontSize = 11.sp)
                                }
                            }
                        }
                    }
                }
            }
        }

        // Estadísticas del chofer
        if (empleadoPerfil != null) {
            item {
                Text(
                    "� Tus Estadísticas",
                    fontSize = 16.sp,
                    fontWeight = FontWeight.SemiBold,
                    color = Color.White,
                    modifier = Modifier.padding(bottom = 12.dp)
                )
            }
            
            item {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 12.dp),
                    colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        StatRow("Total de entregas", empleadoPerfil.totalEntregas.toString(), Icons.Default.LocalShipping, Color(0xFF6C63FF))
                        Spacer(modifier = Modifier.height(12.dp))
                        Divider(color = Color(0xFF2D3748))
                        Spacer(modifier = Modifier.height(12.dp))
                        StatRow("Entregas completadas", empleadoPerfil.entregasCompletadas.toString(), Icons.Default.CheckCircle, Color(0xFF00C853))
                        Spacer(modifier = Modifier.height(12.dp))
                        Divider(color = Color(0xFF2D3748))
                        Spacer(modifier = Modifier.height(12.dp))
                        StatRow("Entregas pendientes", empleadoPerfil.entregasPendientes.toString(), Icons.Default.Schedule, Color(0xFFFFA726))
                    }
                }
            }
            
            // Se eliminó la sección "Por Período" como solicitó el usuario. La gráfica se muestra arriba del bloque de estadísticas.
            // ...existing code...
        }
    }
}

@Composable
fun InfoRow(icon: ImageVector, text: String) {
    Row(
        verticalAlignment = Alignment.CenterVertically
    ) {
        Icon(
            icon,
            contentDescription = null,
            tint = Color(0xFF8E8E93),
            modifier = Modifier.size(18.dp)
        )
        Spacer(modifier = Modifier.width(8.dp))
        Text(
            text,
            color = Color(0xFFB0B3B8),
            fontSize = 13.sp
        )
    }
}

@Composable
fun StatRow(label: String, value: String, icon: ImageVector, color: Color) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.SpaceBetween,
        verticalAlignment = Alignment.CenterVertically
    ) {
        Row(verticalAlignment = Alignment.CenterVertically) {
            Icon(
                icon,
                contentDescription = null,
                tint = color,
                modifier = Modifier.size(20.dp)
            )
            Spacer(modifier = Modifier.width(12.dp))
            Text(
                label,
                color = Color(0xFFB0B3B8),
                fontSize = 14.sp
            )
        }
        Text(
            value,
            color = Color.White,
            fontSize = 18.sp,
            fontWeight = FontWeight.Bold
        )
    }
}

@Composable
fun PeriodCard(label: String, value: Int, icon: ImageVector, modifier: Modifier = Modifier) {
    Card(
        modifier = modifier.height(85.dp),
        colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
        shape = RoundedCornerShape(12.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(12.dp),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center
        ) {
            Icon(icon, contentDescription = label, tint = Color(0xFF6C63FF), modifier = Modifier.size(24.dp))
            Spacer(modifier = Modifier.height(4.dp))
            Text("$value", color = Color.White, fontSize = 20.sp, fontWeight = FontWeight.Bold)
            Text(label, color = Color(0xFF8E8E93), fontSize = 11.sp)
        }
    }
}

@Composable
fun SummaryCard(label: String, value: Int, color: Color, icon: ImageVector, modifier: Modifier = Modifier) {
    Card(
        modifier = modifier
            .height(100.dp),
        colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
        shape = RoundedCornerShape(12.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(12.dp),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center
        ) {
            Icon(icon, contentDescription = label, tint = color, modifier = Modifier.size(28.dp))
            Spacer(modifier = Modifier.height(4.dp))
            Text("$value", color = Color.White, fontSize = 22.sp, fontWeight = FontWeight.Bold)
            Text(label, color = Color(0xFF8E8E93), fontSize = 11.sp)
        }
    }
}

// Guías Tab
@Composable
fun GuiasTab(
    guias: List<GuiaAsignada>,
    expandedGuiaId: Int?,
    onExpandGuia: (Int) -> Unit,
    onRecoger: (GuiaAsignada) -> Unit,
    onIniciarEntrega: (GuiaAsignada) -> Unit,
    onEntregar: (GuiaAsignada) -> Unit,
    onScanBarcode: () -> Unit
) {
    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(Color(0xFF0F1419))
            .padding(16.dp)
    ) {
        item {
            Text(
                "📦 Guías Asignadas",
                fontSize = 20.sp,
                fontWeight = FontWeight.Bold,
                color = Color.White,
                modifier = Modifier.padding(bottom = 16.dp)
            )
        }

        if (guias.isEmpty()) {
            item {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(vertical = 8.dp),
                    colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Text(
                        "No tienes guías asignadas actualmente.",
                        color = Color(0xFF8E8E93),
                        modifier = Modifier.padding(20.dp),
                        textAlign = TextAlign.Center
                    )
                }
            }
        } else {
            items(guias.filter { it.estadoActual != 3 }) { guia ->
                GuiaCardExpanded(
                    guia,
                    isExpanded = expandedGuiaId == guia.envioId,
                    onToggleExpand = { onExpandGuia(guia.envioId) },
                    onRecoger,
                    onIniciarEntrega,
                    onEntregar
                )
            }
        }

        item {
            Spacer(modifier = Modifier.height(16.dp))
            Button(
                onClick = onScanBarcode,
                modifier = Modifier
                    .fillMaxWidth()
                    .height(56.dp),
                colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF6C63FF)),
                shape = RoundedCornerShape(12.dp)
            ) {
                Icon(Icons.Default.QrCode, contentDescription = null, modifier = Modifier.size(24.dp))
                Spacer(modifier = Modifier.width(8.dp))
                Text("Iniciar Escaneo", fontSize = 16.sp, fontWeight = FontWeight.SemiBold)
            }
        }
    }
}

@Composable
fun GuiaCardExpanded(
    guia: GuiaAsignada,
    isExpanded: Boolean,
    onToggleExpand: () -> Unit,
    onRecoger: (GuiaAsignada) -> Unit,
    onIniciarEntrega: (GuiaAsignada) -> Unit,
    onEntregar: (GuiaAsignada) -> Unit
) {
    val rotationAngle by animateFloatAsState(targetValue = if (isExpanded) 180f else 0f, label = "rotation")

    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 6.dp),
        colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
        shape = RoundedCornerShape(12.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            // Header de guía (clickeable para expandir)
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .clickable { onToggleExpand() },
                verticalAlignment = Alignment.CenterVertically
            ) {
                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        guia.guiaRastreo,
                        color = Color(0xFF6C63FF),
                        fontWeight = FontWeight.Bold,
                        fontSize = 16.sp
                    )
                    Text(
                        guia.destinatario,
                        color = Color.White,
                        fontSize = 14.sp
                    )
                }
                Icon(
                    Icons.Default.ExpandMore,
                    contentDescription = "Expandir",
                    tint = Color(0xFF8E8E93),
                    modifier = Modifier.rotate(rotationAngle)
                )
            }

            // Timeline de estados (expandible)
            AnimatedVisibility(visible = isExpanded) {
                Column(modifier = Modifier.padding(top = 16.dp)) {
                    TimelineView(guia.estados, guia.estadoActual)

                    Spacer(modifier = Modifier.height(16.dp))

                    // Botón de acción según estado actual
                    when (guia.estadoActual) {
                        0 -> ActionButton(
                            text = "Recoger paquete",
                            color = Color(0xFF6C63FF),
                            icon = Icons.Default.LocalShipping,
                            onClick = { onRecoger(guia) }
                        )
                        1 -> ActionButton(
                            text = "Iniciar entrega",
                            color = Color(0xFF6C63FF),
                            icon = Icons.Default.DirectionsCar,
                            onClick = { onIniciarEntrega(guia) }
                        )
                        2 -> ActionButton(
                            text = "Entregar",
                            color = Color(0xFF00C853),
                            icon = Icons.Default.CheckCircle,
                            onClick = { onEntregar(guia) }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun TimelineView(estados: List<EstadoGuia>, estadoActual: Int) {
    Column {
        estados.forEachIndexed { index, estado ->
            TimelineStep(
                estado = estado,
                isCompleted = index < estadoActual,
                isCurrent = index == estadoActual,
                isLast = index == estados.size - 1
            )
        }
    }
}

@Composable
fun TimelineStep(estado: EstadoGuia, isCompleted: Boolean, isCurrent: Boolean, isLast: Boolean) {
    Row(modifier = Modifier.fillMaxWidth()) {
        // Indicador de estado (círculo + línea)
        Column(horizontalAlignment = Alignment.CenterHorizontally) {
            Box(
                modifier = Modifier
                    .size(20.dp)
                    .clip(CircleShape)
                    .background(
                        when {
                            isCompleted -> Color(0xFF00C853)
                            isCurrent -> Color(0xFF6C63FF)
                            else -> Color(0xFF3A3F4B)
                        }
                    )
                    .border(
                        width = 2.dp,
                        color = when {
                            isCompleted || isCurrent -> Color.White
                            else -> Color(0xFF3A3F4B)
                        },
                        shape = CircleShape
                    )
            )
            if (!isLast) {
                Box(
                    modifier = Modifier
                        .width(2.dp)
                        .height(40.dp)
                        .background(
                            if (isCompleted) Color(0xFF00C853) else Color(0xFF3A3F4B)
                        )
                )
            }
        }

        Spacer(modifier = Modifier.width(12.dp))

        // Contenido del estado
        Column(modifier = Modifier.padding(bottom = if (isLast) 0.dp else 16.dp)) {
            Text(
                estado.titulo,
                color = if (isCurrent || isCompleted) Color.White else Color(0xFF8E8E93),
                fontWeight = if (isCurrent) FontWeight.Bold else FontWeight.Normal,
                fontSize = 14.sp
            )
            Text(
                estado.descripcion,
                color = Color(0xFF8E8E93),
                fontSize = 12.sp
            )
            if (estado.fechaHora.isNotEmpty() && estado.fechaHora != "0001-01-01T00:00:00") {
                Text(
                    estado.fechaHora.take(16).replace("T", " "),
                    color = Color(0xFF6C63FF),
                    fontSize = 11.sp
                )
            }
        }
    }
}

@Composable
fun ActionButton(text: String, color: Color, icon: ImageVector, onClick: () -> Unit) {
    Button(
        onClick = onClick,
        modifier = Modifier
            .fillMaxWidth()
            .height(48.dp),
        colors = ButtonDefaults.buttonColors(containerColor = color),
        shape = RoundedCornerShape(10.dp)
    ) {
        Icon(icon, contentDescription = null, modifier = Modifier.size(20.dp))
        Spacer(modifier = Modifier.width(8.dp))
        Text(text, fontSize = 15.sp, fontWeight = FontWeight.SemiBold)
    }
}

// Signature Dialog
@Composable
fun SignatureDialog(guia: GuiaAsignada, onDismiss: () -> Unit, onConfirm: (GuiaAsignada) -> Unit) {
    var signaturePath by remember { mutableStateOf<List<Pair<Float, Float>>>(emptyList()) }

    Dialog(onDismissRequest = onDismiss) {
        Card(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
            shape = RoundedCornerShape(16.dp)
        ) {
            Column(
                modifier = Modifier.padding(20.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                Text(
                    "Firma del destinatario",
                    fontSize = 18.sp,
                    fontWeight = FontWeight.Bold,
                    color = Color.White,
                    modifier = Modifier.padding(bottom = 16.dp)
                )

                // Canvas de firma (simplificado - en producción usar Canvas real)
                Box(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(150.dp)
                        .clip(RoundedCornerShape(8.dp))
                        .background(Color.White)
                        .border(2.dp, Color(0xFF6C63FF), RoundedCornerShape(8.dp))
                        .pointerInput(Unit) {
                            detectDragGestures { change, _ ->
                                change.consume()
                                // Aquí iría la lógica de dibujo real
                            }
                        },
                    contentAlignment = Alignment.Center
                ) {
                    Text(
                        "Firme aquí",
                        color = Color(0xFFCCCCCC),
                        fontSize = 14.sp
                    )
                }

                Spacer(modifier = Modifier.height(20.dp))

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(12.dp)
                ) {
                    OutlinedButton(
                        onClick = { signaturePath = emptyList() },
                        modifier = Modifier.weight(1f),
                        colors = ButtonDefaults.outlinedButtonColors(contentColor = Color(0xFF8E8E93))
                    ) {
                        Text("Limpiar")
                    }
                    Button(
                        onClick = onDismiss,
                        modifier = Modifier.weight(1f),
                        colors = ButtonDefaults.buttonColors(containerColor = Color(0xFFFF5252))
                    ) {
                        Text("Cancelar")
                    }
                    Button(
                        onClick = { onConfirm(guia) },
                        modifier = Modifier.weight(1f),
                        colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF00C853))
                    ) {
                        Text("Aceptar")
                    }
                }
            }
        }
    }
}

// Bitácora Tab
@Composable
fun BitacoraTab(entregas: List<BitacoraEntrega>) {
    var entregaSeleccionada by remember { mutableStateOf<BitacoraEntrega?>(null) }
    // Calcular estadísticas
    val puntualidad = if (entregas.isNotEmpty()) {
        val entregasATiempo = entregas.count { it.estado.equals("Entregado", ignoreCase = true) }
        ((entregasATiempo.toFloat() / entregas.size) * 100).toInt()
    } else 0
    
    val eficiencia = puntualidad // Simplificado, igual que en el web

    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(Color(0xFF0F1419))
            .padding(16.dp)
    ) {
        // Header
        item {
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                shape = RoundedCornerShape(16.dp)
            ) {
                Row(
                    modifier = Modifier.padding(20.dp),
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.SpaceBetween
                ) {
                    Column(modifier = Modifier.weight(1f)) {
                        Text(
                            "🧾 Bitácora de Viajes",
                            fontSize = 20.sp,
                            fontWeight = FontWeight.Bold,
                            color = Color.White
                        )
                        Text(
                            "Revisa tu desempeño y entregas recientes",
                            fontSize = 13.sp,
                            color = Color(0xFF8E8E93),
                            modifier = Modifier.padding(top = 4.dp)
                        )
                    }
                    Box(
                        modifier = Modifier
                            .size(50.dp)
                            .clip(CircleShape)
                            .background(Color(0xFF6C63FF)),
                        contentAlignment = Alignment.Center
                    ) {
                        Icon(
                            Icons.Default.Person,
                            contentDescription = "Avatar",
                            tint = Color.White,
                            modifier = Modifier.size(28.dp)
                        )
                    }
                }
            }
        }

        // Estadísticas
        item {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(bottom = 16.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                StatsCard(
                    icon = Icons.Default.AccessTime,
                    label = "Puntualidad",
                    value = "$puntualidad%",
                    description = "Entregas a tiempo",
                    color = Color(0xFF6C63FF),
                    modifier = Modifier.weight(1f)
                )
                StatsCard(
                    icon = Icons.Default.Route,
                    label = "Eficiencia",
                    value = "$eficiencia%",
                    description = "Optimización de rutas",
                    color = Color(0xFF00C853),
                    modifier = Modifier.weight(1f)
                )
            }
        }

        // Título de entregas
        item {
            Row(
                verticalAlignment = Alignment.CenterVertically,
                modifier = Modifier.padding(bottom = 12.dp, top = 8.dp)
            ) {
                Icon(
                    Icons.Default.LocalShipping,
                    contentDescription = null,
                    tint = Color(0xFF6C63FF),
                    modifier = Modifier.size(20.dp)
                )
                Spacer(modifier = Modifier.width(8.dp))
                Text(
                    "Entregas Realizadas",
                    fontSize = 16.sp,
                    fontWeight = FontWeight.SemiBold,
                    color = Color.White
                )
            }
        }

        // Tabla de entregas
        if (entregas.isEmpty()) {
            item {
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Column(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(32.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Icon(
                            Icons.Default.Inventory,
                            contentDescription = null,
                            tint = Color(0xFF8E8E93),
                            modifier = Modifier.size(48.dp)
                        )
                        Spacer(modifier = Modifier.height(12.dp))
                        Text(
                            "No hay entregas registradas",
                            color = Color(0xFF8E8E93),
                            textAlign = TextAlign.Center,
                            fontSize = 14.sp
                        )
                    }
                }
            }
        } else {
            items(entregas) { entrega ->
                EntregaCard(entrega) { entregaSeleccionada = entrega }
            }
        }
    }

    entregaSeleccionada?.let { entrega ->
        EntregaDetalleDialog(entrega = entrega) { entregaSeleccionada = null }
    }
}

@Composable
fun StatsCard(
    icon: ImageVector,
    label: String,
    value: String,
    description: String,
    color: Color,
    modifier: Modifier = Modifier
) {
    Card(
        modifier = modifier.height(120.dp),
        colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
        shape = RoundedCornerShape(12.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp),
            verticalArrangement = Arrangement.SpaceBetween
        ) {
            Row(
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween,
                modifier = Modifier.fillMaxWidth()
            ) {
                Icon(
                    icon,
                    contentDescription = label,
                    tint = color,
                    modifier = Modifier.size(24.dp)
                )
                Text(
                    label,
                    color = Color(0xFF8E8E93),
                    fontSize = 12.sp,
                    fontWeight = FontWeight.Medium
                )
            }
            Column {
                Text(
                    value,
                    color = Color.White,
                    fontSize = 28.sp,
                    fontWeight = FontWeight.Bold
                )
                Text(
                    description,
                    color = Color(0xFF8E8E93),
                    fontSize = 11.sp
                )
            }
        }
    }
}

@Composable
fun EntregaCard(entrega: BitacoraEntrega, onClick: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 6.dp)
            .clickable(onClick = onClick),
        colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
        shape = RoundedCornerShape(12.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        entrega.guia,
                        color = Color(0xFF6C63FF),
                        fontWeight = FontWeight.Bold,
                        fontSize = 14.sp
                    )
                    Spacer(modifier = Modifier.height(4.dp))
                    Text(
                        entrega.destinatario,
                        color = Color.White,
                        fontSize = 13.sp
                    )
                    Spacer(modifier = Modifier.height(2.dp))
                    Text(
                        entrega.direccion,
                        color = Color(0xFF8E8E93),
                        fontSize = 12.sp,
                        maxLines = 1
                    )
                }
                EstadoBadge(entrega.estado)
            }
            
            Spacer(modifier = Modifier.height(8.dp))
            
            Row(
                verticalAlignment = Alignment.CenterVertically
            ) {
                Icon(
                    Icons.Default.CalendarToday,
                    contentDescription = null,
                    tint = Color(0xFF6C63FF),
                    modifier = Modifier.size(14.dp)
                )
                Spacer(modifier = Modifier.width(6.dp))
                Text(
                    formatFecha(entrega.fecha),
                    color = Color(0xFF8E8E93),
                    fontSize = 12.sp
                )
            }
        }
    }
}

@Composable
fun EntregaDetalleDialog(entrega: BitacoraEntrega, onDismiss: () -> Unit) {
    Dialog(onDismissRequest = onDismiss) {
        Card(
            colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
            shape = RoundedCornerShape(16.dp)
        ) {
            Column(modifier = Modifier.padding(20.dp)) {
                Text("Detalle de Entrega", color = Color.White, fontSize = 18.sp, fontWeight = FontWeight.Bold)
                Spacer(Modifier.height(12.dp))
                InfoRow(label = "Guía", value = entrega.guia)
                InfoRow(label = "Destinatario", value = entrega.destinatario)
                InfoRow(label = "Dirección", value = entrega.direccion)
                InfoRow(label = "Fecha", value = formatFecha(entrega.fecha))
                InfoRow(label = "Estado", value = entrega.estado)
                Spacer(Modifier.height(16.dp))
                Button(onClick = onDismiss, colors = ButtonDefaults.buttonColors(containerColor = Color(0xFF6C63FF)), modifier = Modifier.fillMaxWidth()) {
                    Text("Cerrar")
                }
            }
        }
    }
}

@Composable
fun InfoRow(label: String, value: String) {
    Column(modifier = Modifier.fillMaxWidth().padding(vertical = 6.dp)) {
        Text(label, color = Color(0xFF8E8E93), fontSize = 12.sp)
        Text(value, color = Color.White, fontSize = 14.sp, fontWeight = FontWeight.Medium)
    }
}

@Composable
fun EstadoBadge(estado: String) {
    val (backgroundColor, textColor) = when (estado.lowercase()) {
        "entregado" -> Pair(Color(0xFF00C853), Color.White)
        "incidencia" -> Pair(Color(0xFFFF5252), Color.White)
        "en camino" -> Pair(Color(0xFF6C63FF), Color.White)
        else -> Pair(Color(0xFF3A3F4B), Color(0xFF8E8E93))
    }
    
    Box(
        modifier = Modifier
            .clip(RoundedCornerShape(6.dp))
            .background(backgroundColor)
            .padding(horizontal = 12.dp, vertical = 6.dp)
    ) {
        Text(
            estado,
            color = textColor,
            fontSize = 11.sp,
            fontWeight = FontWeight.SemiBold
        )
    }
}

fun formatFecha(fecha: String): String {
    return try {
        // Formato: "2024-10-15T14:30:00" -> "15/10/2024"
        val parts = fecha.split("T")[0].split("-")
        if (parts.size == 3) {
            "${parts[2]}/${parts[1]}/${parts[0]}"
        } else {
            fecha
        }
    } catch (e: Exception) {
        fecha
    }
}

// Notificaciones Tab
@Composable
fun NotificacionesTab(
    notificaciones: List<com.manybox.chofer.api.NotificacionUsuarioVM>,
    onMarcarNotificacionLeida: (Int) -> Unit,
    onMarcarTodasLeidas: () -> Unit
) {
    LazyColumn(
        modifier = Modifier
            .fillMaxSize()
            .background(Color(0xFF0F1419))
            .padding(16.dp)
    ) {
        item {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    "🔔 Notificaciones",
                    fontSize = 20.sp,
                    fontWeight = FontWeight.Bold,
                    color = Color.White,
                    modifier = Modifier.padding(bottom = 16.dp)
                )
                if (notificaciones.any { !it.leido }) {
                    TextButton(onClick = onMarcarTodasLeidas) {
                        Text("Marcar todas", color = Color(0xFF6C63FF))
                    }
                }
            }
        }

        if (notificaciones.isEmpty()) {
            item {
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Text(
                        "No tienes notificaciones.",
                        color = Color(0xFF8E8E93),
                        modifier = Modifier.padding(20.dp),
                        textAlign = TextAlign.Center
                    )
                }
            }
        } else {
            items(notificaciones) { n ->
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(bottom = 10.dp),
                    colors = CardDefaults.cardColors(containerColor = Color(0xFF1A1F29)),
                    shape = RoundedCornerShape(12.dp)
                ) {
                    Row(modifier = Modifier.padding(14.dp), verticalAlignment = Alignment.CenterVertically) {
                        val dotColor = if (n.leido) Color(0xFF3A3F4B) else Color(0xFFFF5252)
                        Box(modifier = Modifier
                            .size(10.dp)
                            .clip(CircleShape)
                            .background(dotColor))
                        Spacer(modifier = Modifier.width(10.dp))
                        Column(modifier = Modifier.weight(1f)) {
                            Text(n.titulo ?: "(Sin título)", color = Color.White, fontWeight = FontWeight.SemiBold, fontSize = 15.sp)
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(n.mensaje ?: "", color = Color(0xFFB0B3B8), fontSize = 13.sp)
                            Spacer(modifier = Modifier.height(6.dp))
                            Text(n.fechaCreacion?.let { formatFecha(it) } ?: "", color = Color(0xFF8E8E93), fontSize = 11.sp)
                        }
                        Spacer(modifier = Modifier.width(8.dp))
                        Column(horizontalAlignment = Alignment.End) {
                            Text(n.prioridad ?: "", color = Color(0xFF6C63FF), fontSize = 12.sp, fontWeight = FontWeight.SemiBold)
                            if (!n.leido) {
                                Spacer(modifier = Modifier.height(6.dp))
                                TextButton(onClick = { onMarcarNotificacionLeida(n.id) }) {
                                    Text("Marcar leída", color = Color(0xFF00C853), fontSize = 12.sp)
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}