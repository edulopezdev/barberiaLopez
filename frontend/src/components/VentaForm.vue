<template>
  <div class="formulario-venta" v-if="formularioVisible">
    <!-- Título y botón de cierre -->
    <h3>{{ ventaId ? "Editar Atención" : "Nueva Atención" }}</h3>

    <!-- Selección de Cliente -->
    <div class="campo" :class="{ error: errores.cliente }">
      <label for="cliente">
        Seleccionar Cliente <span class="obligatorio">*</span>
      </label>
      <AutoComplete
        v-model="formulario.cliente"
        :suggestions="clientesFiltrados"
        field="nombre"
        @complete="buscarClientes"
        placeholder="Selecciona un cliente"
        :force-selection="true"
      />
      <div v-if="errores.cliente" class="error-msg">
        <i class="pi pi-exclamation-triangle"></i> {{ errores.cliente }}
      </div>
    </div>

    <!-- Búsqueda de producto -->
    <div class="campo">
      <label for="busqueda">Buscar Producto o Servicio</label>
      <InputText
        v-model="busquedaProducto"
        @input="filtrarProductoServicios({ query: busquedaProducto })"
        placeholder="Escribe para buscar..."
      />
    </div>

    <!-- Lista de resultados -->
    <div class="lista-productos" v-if="productos.length > 0">
      <div
        v-for="producto in productos"
        :key="producto.id"
        class="producto-item"
        @click="agregarAlCarrito(producto)"
      >
        {{ producto.nombre }} - ${{ producto.precio }}
        <small>({{ producto.esAlmacenable ? "Producto" : "Servicio" }})</small>
      </div>
    </div>

    <!-- Carrito -->
    <div class="carrito" v-if="carrito.length > 0">
      <h4>Productos Seleccionados</h4>
      <table class="tabla-carrito">
        <thead>
          <tr>
            <th>Producto</th>
            <th>Cantidad</th>
            <th>Precio Unitario</th>
            <th>Total</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in carrito" :key="index">
            <td>{{ item.nombre }}</td>
            <td>
              <InputNumber v-model="item.cantidad" :min="1" />
            </td>
            <td>$ {{ item.precioUnitario.toFixed(2) }}</td>
            <td>$ {{ (item.cantidad * item.precioUnitario).toFixed(2) }}</td>
            <td>
              <button
                @click="eliminarDelCarrito(index)"
                class="btn-eliminar-item"
                aria-label="Eliminar"
              >
                <i class="pi pi-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      <div class="total">
        <strong>Total: ${{ totalCarrito.toFixed(2) }}</strong>
      </div>
    </div>

    <!-- Botones Guardar y Cerrar -->
    <div class="acciones-formulario">
      <Button
        class="btn-guardar"
        :label="
          guardando
            ? ventaId
              ? 'Actualizando...'
              : 'Guardando...'
            : ventaId
            ? 'Actualizar'
            : 'Guardar'
        "
        icon="pi pi-check"
        @click="guardarAtencion"
        :disabled="guardando"
      />
      <button class="btn-cerrar" @click="$emit('cancelar')" aria-label="Cerrar">
        <i class="pi pi-times"></i>
      </button>
    </div>
  </div>
</template>

<script>
import InputText from "primevue/inputtext";
import InputNumber from "primevue/inputnumber";
import AutoComplete from "primevue/autocomplete";
import Button from "primevue/button";
import UsuarioService from "../services/UsuarioService";
import VentaService from "../services/VentaService";
import Swal from "sweetalert2";

export default {
  name: "VentaForm",
  components: { InputText, InputNumber, AutoComplete, Button },
  props: {
    id: {
      type: [String, Number],
      default: null,
    },
  },
  data() {
    return {
      guardando: false,
      formularioVisible: true,
      ventaId: this.id ? parseInt(this.id) : null,
      formulario: {
        cliente: null,
      },
      clientesFiltrados: [],
      productos: [], // Productos o servicios obtenidos desde backend
      carrito: [],
      busquedaProducto: "",
      errores: {
        cliente: null,
      },
    };
  },
  computed: {
    totalCarrito() {
      return this.carrito.reduce(
        (acc, item) => acc + item.cantidad * item.precioUnitario,
        0
      );
    },
  },
  created() {
    if (this.ventaId) {
      this.cargarVenta();
    }
  },
  methods: {
    async buscarClientes(event) {
      const query = event.query || "";
      if (!query || query.length < 1) {
        this.clientesFiltrados = [];
        return;
      }
      try {
        const response = await UsuarioService.getClientes(1, 10, {
          nombre: query,
          activo: true,
        });
        this.clientesFiltrados =
          response.data.clientes.map((c) => ({
            id: c.id,
            nombre: c.nombre,
          })) || [];
      } catch (error) {
        console.error("Error al buscar clientes:", error);
        this.clientesFiltrados = [];
      }
    },

    async filtrarProductoServicios(event) {
      const query = event.query || "";
      if (!query || query.length < 1) {
        this.productos = [];
        return;
      }

      try {
        const res = await VentaService.getProductosServiciosVenta(1, 10, query);

        this.productos = res.data.productos.map((p) => ({
          id: p.id,
          nombre: p.nombre,
          precio: p.precio,
          esAlmacenable: p.esAlmacenable,
        }));
      } catch (error) {
        console.error("Error al buscar productos/servicios:", error);
        this.productos = [];
      }
    },

    agregarAlCarrito(producto) {
      const existe = this.carrito.find((p) => p.id === producto.id);
      if (existe) {
        existe.cantidad += 1;
      } else {
        this.carrito.push({
          id: producto.id,
          nombre: producto.nombre,
          cantidad: 1,
          precioUnitario: producto.precio,
        });
      }
      this.busquedaProducto = "";
      this.productos = [];
    },

    eliminarDelCarrito(index) {
      this.carrito.splice(index, 1);
    },

    validarFormulario() {
      this.errores = { cliente: null };
      if (this.carrito.length === 0) {
        alert("Debe agregar al menos un producto o servicio.");
        return false;
      }

      if (!this.formulario.cliente) {
        this.errores.cliente = "Debe seleccionar un cliente.";
        return false;
      }

      if (this.carrito.length === 0) {
        alert("Debe agregar al menos un producto o servicio.");
        return false;
      }

      return true;
    },

    async guardarAtencion() {
      if (!this.validarFormulario()) return;

      const confirmResult = await Swal.fire({
        title: `¿${this.ventaId ? "Actualizar" : "Crear"} atención para ${
          this.formulario.cliente.nombre
        }?`,
        icon: "question",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#6c757d",
        confirmButtonText: this.ventaId ? "Sí, actualizar" : "Sí, crear",
        cancelButtonText: "Cancelar",
        background: "#18181b",
        color: "#fff",
      });

      if (!confirmResult.isConfirmed) {
        return; // No ocultar nada, el formulario sigue visible
      }

      this.guardando = true;

      try {
        const response = await VentaService.crearAtencionConDetalles({
          ClienteId: this.formulario.cliente.id,
          Total: this.totalCarrito,
          TurnoId: null,
          Detalles: this.carrito.map((item) => ({
            ProductoServicioId: item.id,
            Cantidad: item.cantidad,
            PrecioUnitario: item.precioUnitario,
          })),
        });

        const data = response.data;

        await Swal.fire({
          title: "Atención creada",
          text: data.message || "La atención fue creada correctamente.",
          icon: "success",
          timer: 2000,
          timerProgressBar: true,
          showConfirmButton: false,
          background: "#18181b",
          color: "#fff",
        });

        this.limpiarFormulario();

        // Emitir evento al padre para que cierre el formulario
        this.$emit("guardar", data.atencion);
      } catch (error) {
        console.error("Error al guardar la atención:", error);
        const msg =
          error?.response?.data?.message ||
          "Hubo un error al guardar la atención. Intente de nuevo.";

        await Swal.fire({
          title: "Error",
          text: msg,
          icon: "error",
          background: "#18181b",
          color: "#fff",
        });
      } finally {
        this.guardando = false;
      }
    },

    limpiarFormulario() {
      this.formulario.cliente = null;
      this.carrito = [];
      this.busquedaProducto = "";
      this.productos = [];
      this.errores = { cliente: null };
    },

    async cargarVenta() {
      try {
        const res = await VentaService.getVentas(1, 10, {
          atencionId: this.ventaId,
        });
        const venta = res.data.ventas.find(
          (v) => v.AtencionId === this.ventaId
        );

        if (venta) {
          this.formulario.cliente = venta.Cliente;
          this.carrito = venta.Detalles.map((d) => ({
            id: d.ProductoServicioId,
            nombre: d.NombreProducto,
            cantidad: d.Cantidad,
            precioUnitario: d.PrecioUnitario,
          }));
        }
      } catch (error) {
        console.error("Error al cargar venta:", error);
      }
    },
  },
};
</script>

<style scoped>
.formulario-venta {
  max-width: 700px;
  margin: 0 auto;
  padding: 1rem;
  color: #f0f0f0;
}

.campo {
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
  transition: all 0.3s ease;
}

.campo.error label {
  color: #e74c3c;
  font-weight: 700;
}

label {
  font-weight: 600;
  margin-bottom: 0.5rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.obligatorio {
  color: #e74c3c;
  font-size: 1.2rem;
  line-height: 1;
  font-weight: 900;
  user-select: none;
  animation: pulse 1.5s infinite alternate ease-in-out;
  margin-left: 0.1rem;
}

@keyframes pulse {
  0% {
    opacity: 1;
    transform: scale(1);
  }
  100% {
    opacity: 0.6;
    transform: scale(1.2);
  }
}

.error-msg {
  margin-top: 0.35rem;
  background-color: #f9d6d5;
  border: 1.5px solid #e74c3c;
  color: #a94442;
  padding: 0.35rem 0.6rem;
  border-radius: 6px;
  font-size: 0.875rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.lista-productos {
  max-height: 200px;
  overflow-y: auto;
  margin-bottom: 1rem;
  border: 1px solid #444;
  border-radius: 6px;
  background-color: #1e1e1e;
  padding: 0.5rem;
}

.producto-item {
  padding: 0.5rem;
  cursor: pointer;
  border-bottom: 1px solid #333;
  transition: background-color 0.2s ease;
}

.producto-item:hover {
  background-color: #2a2a2a;
}

.tabla-carrito {
  width: 100%;
  border-collapse: collapse;
  margin-top: 0.5rem;
  background-color: #1f1f1f;
  border-radius: 6px;
  overflow: hidden;
}

.tabla-carrito th,
.tabla-carrito td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #333;
}

.tabla-carrito th {
  background-color: #292929;
  color: #ccc;
  font-weight: 600;
}

.total {
  margin-top: 1rem;
  font-size: 1.1rem;
  text-align: right;
}

.acciones-formulario {
  display: flex;
  justify-content: space-between;
  margin-top: 1.5rem;
}

.btn-cerrar {
  background-color: transparent;
  border: none;
  color: #f0f0f0;
  font-size: 1.2rem;
  cursor: pointer;
  padding: 0.4rem;
  border-radius: 4px;
  transition: background-color 0.2s ease;
}

.btn-cerrar:hover {
  background-color: #2a2a2a;
  color: #fff;
}

.btn-eliminar-item {
  background: transparent;
  border: none;
  cursor: pointer;
  color: #dc3545; /* rojo para eliminar */
  font-size: 1.2rem;
  padding: 0.2rem 0.6rem;
  margin-left: 0.5rem;
}

.btn-eliminar-item:hover {
  color: #ff6b6b;
}
</style>
