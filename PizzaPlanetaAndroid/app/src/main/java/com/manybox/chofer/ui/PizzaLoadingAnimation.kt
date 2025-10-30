package com.manybox.chofer.ui

import androidx.compose.animation.core.*
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.layout.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.Path
import androidx.compose.ui.graphics.StrokeCap
import androidx.compose.ui.graphics.drawscope.Stroke
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import kotlin.math.cos
import kotlin.math.sin

@Composable
fun HotPizzaAnimation(
    modifier: Modifier = Modifier,
    size: Float = 150f,
    lineColor: Color = Color.Black,
    strokeWidth: Float = 3f
) {
    val infiniteTransition = rememberInfiniteTransition(label = "pizza_steam_animation")

    // Animación para el desplazamiento vertical del vapor
    val steamOffsetY by infiniteTransition.animateFloat(
        initialValue = 0f,
        targetValue = -size * 0.1f, // Se mueve un 10% del tamaño hacia arriba
        animationSpec = infiniteRepeatable(
            animation = tween(durationMillis = 1500, easing = LinearEasing),
            repeatMode = RepeatMode.Reverse
        ),
        label = "steam_offset_y"
    )

    // Animación para la opacidad del vapor
    val steamAlpha by infiniteTransition.animateFloat(
        initialValue = 0.3f,
        targetValue = 1f,
        animationSpec = infiniteRepeatable(
            animation = tween(durationMillis = 1500, easing = LinearEasing),
            repeatMode = RepeatMode.Reverse
        ),
        label = "steam_alpha"
    )

    Box(
        modifier = modifier.size(size.dp),
        contentAlignment = Alignment.Center
    ) {
        // Dibujar las líneas de vapor animadas
        Canvas(modifier = Modifier
            .size(size.dp)
        ) {
            val centerOffset = Offset(this.size.width / 2, this.size.height / 2)
            val steamHeight = size * 0.3f
            val steamSpacing = size * 0.08f

            // Línea de vapor izquierda
            val pathLeft = Path().apply {
                moveTo(centerOffset.x - steamSpacing - size * 0.05f, centerOffset.y - steamHeight * 0.8f + steamOffsetY)
                cubicTo(
                    centerOffset.x - steamSpacing - size * 0.12f, centerOffset.y - steamHeight * 1.2f + steamOffsetY,
                    centerOffset.x - steamSpacing, centerOffset.y - steamHeight * 1.5f + steamOffsetY,
                    centerOffset.x - steamSpacing - size * 0.02f, centerOffset.y - steamHeight * 1.8f + steamOffsetY
                )
            }
            drawPath(
                path = pathLeft,
                color = lineColor.copy(alpha = steamAlpha),
                style = Stroke(width = strokeWidth, cap = StrokeCap.Round)
            )

            // Línea de vapor central
            val pathCenter = Path().apply {
                moveTo(centerOffset.x, centerOffset.y - steamHeight * 0.9f + steamOffsetY)
                cubicTo(
                    centerOffset.x - size * 0.05f, centerOffset.y - steamHeight * 1.3f + steamOffsetY,
                    centerOffset.x + size * 0.05f, centerOffset.y - steamHeight * 1.6f + steamOffsetY,
                    centerOffset.x, centerOffset.y - steamHeight * 1.9f + steamOffsetY
                )
            }
            drawPath(
                path = pathCenter,
                color = lineColor.copy(alpha = steamAlpha),
                style = Stroke(width = strokeWidth, cap = StrokeCap.Round)
            )

            // Línea de vapor derecha
            val pathRight = Path().apply {
                moveTo(centerOffset.x + steamSpacing + size * 0.05f, centerOffset.y - steamHeight * 0.8f + steamOffsetY)
                cubicTo(
                    centerOffset.x + steamSpacing + size * 0.12f, centerOffset.y - steamHeight * 1.2f + steamOffsetY,
                    centerOffset.x + steamSpacing, centerOffset.y - steamHeight * 1.5f + steamOffsetY,
                    centerOffset.x + steamSpacing + size * 0.02f, centerOffset.y - steamHeight * 1.8f + steamOffsetY
                )
            }
            drawPath(
                path = pathRight,
                color = lineColor.copy(alpha = steamAlpha),
                style = Stroke(width = strokeWidth, cap = StrokeCap.Round)
            )

            // Dibujar la base de la pizza (sin animación)
            val pizzaRadiusX = size * 0.4f
            val pizzaRadiusY = size * 0.2f
            val pizzaCenterY = centerOffset.y - size * 0.05f // Un poco más abajo que el centro

            // Borde de la pizza (elipse)
            drawOval(
                color = lineColor,
                topLeft = Offset(centerOffset.x - pizzaRadiusX, pizzaCenterY - pizzaRadiusY),
                size = androidx.compose.ui.geometry.Size(pizzaRadiusX * 2, pizzaRadiusY * 2),
                style = Stroke(width = strokeWidth)
            )

            // Simular el borde irregular de la pizza
            val crustAmplitude = size * 0.03f
            val numWaves = 12
            val pathCrust = Path().apply {
                moveTo(centerOffset.x + pizzaRadiusX, pizzaCenterY)
                for (j in 0..numWaves) {
                    val angle = 2 * Math.PI.toFloat() * j / numWaves
                    val x = centerOffset.x + pizzaRadiusX * kotlin.math.cos(angle)
                    val y = pizzaCenterY + (pizzaRadiusY + crustAmplitude * kotlin.math.sin(angle * 3 + 1)) * kotlin.math.sin(angle)
                    if (j == 0) moveTo(x,y) else lineTo(x, y)
                }
                close()
            }
            drawPath(
                path = pathCrust,
                color = lineColor,
                style = Stroke(width = strokeWidth)
            )

            // Dibujar algunos "toppings" simples
            val toppingSize = size * 0.05f
            val toppings = listOf(
                Offset(centerOffset.x - size * 0.15f, pizzaCenterY - size * 0.08f),
                Offset(centerOffset.x + size * 0.12f, pizzaCenterY - size * 0.03f),
                Offset(centerOffset.x - size * 0.05f, pizzaCenterY + size * 0.05f),
                Offset(centerOffset.x + size * 0.03f, pizzaCenterY - size * 0.12f),
                Offset(centerOffset.x - size * 0.2f, pizzaCenterY + size * 0.02f),
                Offset(centerOffset.x + size * 0.2f, pizzaCenterY + size * 0.08f),
            )
            toppings.forEach { offset ->
                drawOval(
                    color = lineColor,
                    topLeft = Offset(offset.x - toppingSize / 2, offset.y - toppingSize / 3),
                    size = androidx.compose.ui.geometry.Size(toppingSize, toppingSize * 0.6f),
                    style = Stroke(width = strokeWidth / 2)
                )
            }
        }
    }
}

@Preview(showBackground = true)
@Composable
fun HotPizzaAnimationPreview() {
    Column(
        modifier = Modifier.fillMaxSize(),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Spacer(Modifier.height(50.dp))
        HotPizzaAnimation(
            size = 200f,
            lineColor = Color(0xFFE57373), // Un color más "caliente"
            strokeWidth = 4f
        )
        Spacer(Modifier.height(20.dp))
        HotPizzaAnimation(
            size = 120f,
            lineColor = Color.Black,
            strokeWidth = 2f
        )
    }
}