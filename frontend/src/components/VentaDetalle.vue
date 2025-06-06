<template>
  <div class="detalle-venta">
    <h3 class="titulo"><i class="pi pi-shopping-cart"></i> Detalle de Venta</h3>

    <!-- Información básica -->
    <div class="seccion">
      <div class="campo">
        <label><i class="pi pi-id-card"></i> Cliente</label>
        <p>{{ venta?.ClienteNombre }}</p>
      </div>

      <div class="campo">
        <label><i class="pi pi-calendar"></i> Fecha de Atención</label>
        <p>{{ formatearFecha(venta?.FechaAtencion) }}</p>
      </div>
    </div>

    <!-- Detalles de productos/servicios -->
    <div class="seccion">
      <h4><i class="pi pi-list"></i> Productos y Servicios</h4>
      <table class="tabla-detalles">
        <thead>
          <tr>
            <th>Producto</th>
            <th>Cantidad</th>
            <th>Precio Unitario</th>
            <th>Subtotal</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(detalle, index) in venta?.Detalles" :key="index">
            <td>{{ detalle.NombreProducto }}</td>
            <td>{{ detalle.Cantidad }}</td>
            <td>$ {{ detalle.PrecioUnitario.toFixed(2) }}</td>
            <td>$ {{ detalle.Subtotal.toFixed(2) }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Totales -->
    <div class="seccion">
      <div class="campo">
        <label><i class="pi pi-dollar"></i> Total de Venta</label>
        <p><strong>$ {{ venta?.TotalVenta.toFixed(2) }}</strong></p>
      </div>

      <div class="campo">
        <label><i class="pi pi-credit-card"></i> Pago</label>
        <Tag v-if="venta?.Pago" severity="success">
          {{ venta.Pago.MetodoPago }} - ${{ venta.Pago.Monto }}
        </Tag>
        <Tag v-else severity="warning">Pendiente</Tag>
      </div>
    </div>

    <!-- Acciones -->
    <div class="acciones-formulario">
      <button class="btn-cerrar" @click="$emit('cerrar')" aria-label="Cerrar">
        <i class="pi pi-times"></i>
      </button>
      <router-link
        :to="{ name: 'VentaEditar', params: { id: venta?.AtencionId } }"
        class="btn-editar"
      >
        <i class="pi pi-pencil"></i> Editar
      </router-link>
    </div>
  </div>
</template>

<script>
import Tag from "primevue/tag";

export default {
  name: "VentaDetalle",
  components: { Tag },
  props: {
    venta: {
      type: Object,
      default: null,
    },
  },
  emits: ["cerrar"],
  methods: {
    formatearFecha(fecha) {
      if (!fecha) return "No disponible";
      const d = new Date(fecha);
      return d.toLocaleDateString("es-AR", {
        day: "2-digit",
        month: "long",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      });
    },
  },
};
</script>

<style scoped>
.detalle-venta {
  max-width: 700px;
  margin: 0 auto;
  padding: 1.5rem;
  background-color: #1e1e1e;
  border-radius: 12px;
  color: #f0f0f0;
  font-family: "Segoe UI", sans-serif;
}

.titulo {
  font-size: 1.5rem;
  margin-bottom: 1.2rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #ffffff;
  border-bottom: 1px solid #333;
  padding-bottom: 0.5rem;
}

.seccion {
  margin-bottom: 1.5rem;
}

.campo {
  margin-bottom: 0.8rem;
}

label {
  font-weight: 600;
  font-size: 0.95rem;
  color: #bbbbbb;
  display: flex;
  align-items: center;
  gap: 0.4rem;
  margin-bottom: 0.3rem;
}

p {
  margin: 0;
  font-size: 0.95rem;
  color: #eeeeee;
}

.tabla-detalles {
  width: 100%;
  border-collapse: collapse;
  margin-top: 0.5rem;
}

.tabla-detalles th,
.tabla-detalles td {
  text-align: left;
  padding: 0.5rem;
  border-bottom: 1px solid #333;
}

.tabla-detalles th {
  background-color: #2c2c2c;
  color: #f0f0f0;
}

.acciones-formulario {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 2rem;
}

.btn-cerrar {
  background-color: transparent;
  border: none;
  color: #ccc;
  font-size: 1.2rem;
  cursor: pointer;
}

.btn-editar {
  background-color: #4a90e2;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.5rem 1rem;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  text-decoration: none;
  transition: background-color 0.3s ease;
}

.btn-editar:hover {
  background-color: #357abd;
}
</style>