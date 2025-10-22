package com.manybox.chofer.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.style.TextDecoration
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp

/**
 * Pantalla de Registro (Crear cuenta) – sin imagen superior.
 * Basada en el mock adjunto: tarjeta clara centrada con título, campos y botones.
 */
@Composable
fun RegistroScreen(
	headerImageRes: Int? = null,
	onBack: (() -> Unit)? = null,
	onSubmit: (nombre: String, correo: String, pass1: String, pass2: String) -> Unit = { _, _, _, _ -> },
	onLoginClick: () -> Unit = {}
) {
	val Navy = Color(0xFF1D3557)
	val PanelBg = Color(0xFFF2F2F2)
	val ButtonRed = Color(0xFFE53935)

	var nombre by remember { mutableStateOf("") }
	var correo by remember { mutableStateOf("") }
	var pass1 by remember { mutableStateOf("") }
	var pass2 by remember { mutableStateOf("") }

	val tfColors = OutlinedTextFieldDefaults.colors(
		focusedBorderColor = Color(0xFFBDBDBD),
		unfocusedBorderColor = Color(0xFFBDBDBD),
		disabledBorderColor = Color(0xFFE0E0E0),
		focusedContainerColor = Color.White,
		unfocusedContainerColor = Color.White,
		disabledContainerColor = Color.White,
		focusedTextColor = Color.Black,
		unfocusedTextColor = Color.Black,
		cursorColor = Color.Black
	)

	Box(
		modifier = Modifier
			.fillMaxSize()
			.background(Navy)
	) {
		Column(modifier = Modifier.fillMaxSize()) {
			if (headerImageRes != null) {
				// Imagen superior estilo mock
				Box(modifier = Modifier
					.fillMaxWidth()
					.height(180.dp)) {
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
				// arco azul que cubre la parte baja de la imagen
				Box(
					modifier = Modifier
						.fillMaxWidth()
						.height(90.dp)
						.background(Navy, shape = RoundedCornerShape(topStart = 96.dp, topEnd = 96.dp))
				)
			}

			Box(modifier = Modifier
				.fillMaxSize()) {
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
					Column(modifier = Modifier.padding(16.dp)) {
						if (headerImageRes == null) {
							Row(verticalAlignment = Alignment.CenterVertically) {
								if (onBack != null) {
									IconButton(onClick = { onBack() }) {
										Icon(Icons.Default.ArrowBack, contentDescription = "Volver", tint = Color.Black)
									}
									Spacer(Modifier.width(4.dp))
								}
								Text(
									text = "CREAR CUENTA",
									color = Color.Black,
									fontSize = 20.sp,
									fontWeight = FontWeight.Bold
								)
							}
							Spacer(Modifier.height(8.dp))
						} else {
							Text(
								text = "CREAR CUENTA",
								color = Color.Black,
								fontSize = 20.sp,
								fontWeight = FontWeight.Bold,
								letterSpacing = 1.sp
							)
							Spacer(Modifier.height(8.dp))
						}
				Text(text = "Nombre", color = Color.Black, fontSize = 12.sp)
				OutlinedTextField(
					value = nombre,
					onValueChange = { nombre = it },
					singleLine = true,
					modifier = Modifier.fillMaxWidth(),
					shape = RoundedCornerShape(8.dp),
					colors = tfColors
				)

				Spacer(Modifier.height(10.dp))
				Text(text = "CORREO", color = Color.Black, fontSize = 12.sp)
				OutlinedTextField(
					value = correo,
					onValueChange = { correo = it },
					singleLine = true,
					modifier = Modifier.fillMaxWidth(),
					shape = RoundedCornerShape(8.dp),
					colors = tfColors
				)

				Spacer(Modifier.height(10.dp))
				Text(text = "CONTRASEÑA", color = Color.Black, fontSize = 12.sp)
				OutlinedTextField(
					value = pass1,
					onValueChange = { pass1 = it },
					singleLine = true,
					modifier = Modifier.fillMaxWidth(),
					visualTransformation = PasswordVisualTransformation(),
					shape = RoundedCornerShape(8.dp),
					colors = tfColors
				)

				Spacer(Modifier.height(10.dp))
				Text(text = "CONFIRMAR CONTRASEÑA", color = Color.Black, fontSize = 12.sp)
				OutlinedTextField(
					value = pass2,
					onValueChange = { pass2 = it },
					singleLine = true,
					modifier = Modifier.fillMaxWidth(),
					visualTransformation = PasswordVisualTransformation(),
					shape = RoundedCornerShape(8.dp),
					colors = tfColors
				)

				Spacer(Modifier.height(14.dp))
				Button(
					onClick = { onSubmit(nombre, correo, pass1, pass2) },
					modifier = Modifier
						.fillMaxWidth()
						.height(40.dp),
					colors = ButtonDefaults.buttonColors(containerColor = ButtonRed),
					shape = RoundedCornerShape(8.dp)
				) {
					Text("CREAR CUENTA", color = Color.White, fontWeight = FontWeight.Medium)
				}

				Spacer(Modifier.height(8.dp))
				OutlinedButton(
					onClick = { /* Google Sign-In */ },
					modifier = Modifier
						.fillMaxWidth()
						.height(36.dp),
					shape = RoundedCornerShape(8.dp),
					colors = ButtonDefaults.outlinedButtonColors(containerColor = Color.White)
				) {
					Text("Continuar con Google", color = Color.Black)
				}

				Spacer(Modifier.height(8.dp))
				Row(
					verticalAlignment = Alignment.CenterVertically,
					modifier = Modifier.fillMaxWidth(),
					horizontalArrangement = Arrangement.Center
				) {
					Text(
						text = "¿Ya tienes una cuenta? ",
						color = Color.DarkGray,
						fontSize = 12.sp
					)
					Text(
						text = "Iniciar sesión",
						color = Color(0xFF1A73E8),
						fontSize = 12.sp,
						textDecoration = TextDecoration.Underline,
						modifier = Modifier.clickable { onLoginClick() }
					)
						}
					}
				}
			}
		}
	}
}

