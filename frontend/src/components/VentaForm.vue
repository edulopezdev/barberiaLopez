<template>
  <div class="formulario-venta">
    <h3>{{ ventaId ? "Editar Venta" : "Nueva Venta" }}</h3>

    <!-- Selección de Atención -->
    <div class="campo" :class="{ error: errores.atencion }">
      <label for="atencion">Seleccionar Atención <span class="obligatorio">*</span></label>
      <Dropdown
        id="atencion"
        v-model="formulario.atencionId"
        :options="atenciones"
        optionLabel="resumen"
        optionValue="id"
        placeholder="Seleccione una atención"
        :disabled="ventaId !== null"
      />
      <div v-if="errores.atencion" class="error-msg">
        <i class="pi pi-exclamation-triangle"></i> {{ errores.atencion }}
      </div>
    </div>

    <!-- Búsqueda de producto -->
    <div class="campo">
      <label for="busqueda">Buscar Producto o Servicio</label>
      <InputText id="busqueda" v-model="busquedaProducto" placeholder="Escribe para buscar..." />
    </div>

    <!-- Lista de productos filtrados -->
    <div class="lista-productos">
      <div
        v-for="producto in productosFiltrados"
        :key="producto.id"
        class="producto-item"
        @click="agregarAlCarrito(producto)"
      >
        <strong>{{ producto.nombre }}</strong> - ${{ producto.precio }}
      </div>
    </div>

    <!-- Carrito -->
    <div class="carrito">
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
            <td>$ {{ item.precioUnitario }}</td>
            <td>$ {{ (item.cantidad * item.precioUnitario).toFixed(2) }}</td>
            <td>
              <button @click="eliminarDelCarrito(index)" class="btn-cerrar" aria-label="Eliminar">
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

    <!-- Botones -->
    <div class="acciones-formulario">
      <Button class="p-button" label="Guardar" icon="pi pi-check" @click="guardarVenta" />
      <button class="btn-cerrar" @click="$router.back()" aria-label="Cerrar">
        <i class="pi pi-times"></i>
      </button>
    </div>
  </div>
</template>

<script>
import InputText from "primevue/inputtext";
import InputNumber from "primevue/inputnumber";
import Dropdown from "primevue/dropdown";
import Button from "primevue/button";

export default {
  name: "VentaForm",
  components: { InputText, InputNumber, Dropdown, Button },
  props: {
    id: {
      type: [String, Number],
      default: null,
    },
  },
  data() {
    return {
      ventaId: this.id ? parseInt(this.id) : null,
      formulario: {
        atencionId: null,
      },
      busquedaProducto: "",
      productos: [],
      atenciones: [],
      carrito: [],
      errores: {
        atencion: null,
      },
    };
  },
  computed: {
    productosFiltrados() {
      if (!this.busquedaProducto) return this.productos;
      const filtro = this.busquedaProducto.toLowerCase();
      return this.productos.filter(p =>
        p.nombre.toLowerCase().includes(filtro)
      );
    },
    totalCarrito() {
      return this.carrito.reduce(
        (acc, item) => acc + item.cantidad * item.precioUnitario,
        0
      );
    },
  },
  created() {
    this.cargarProductos();
    this.cargarAtenciones();

    if (this.ventaId) {
      this.cargarVenta();
    }
  },
  methods: {
    async cargarProductos() {
      try {
        const res = await this.$axios.get("/api/productosservicios");
        this.productos = res.data.productosServicios.map((p) => ({
          id: p.id,
          nombre: p.nombre,
          precio: p.precio,
        }));
      } catch (error) {
        console.error("Error al cargar productos:", error);
      }
    },
    async cargarAtenciones() {
      try {
        const res = await this.$axios.get("/api/atencion");
        this.atenciones = res.data.atenciones.map((a) => ({
          id: a.Id,
          resumen: `${a.Cliente.Nombre} - ${a.Mascota.Nombre} (${new Date(a.Fecha).toLocaleDateString()})`,
        }));
      } catch (error) {
        console.error("Error al cargar atenciones:", error);
      }
    },
    async cargarVenta() {
      try {
        const res = await this.$axios.get(`/api/detalleatencion/ventas`, {
          params: { atencionId: this.ventaId },
        });

        const venta = res.data.ventas.find(v => v.AtencionId === this.ventaId);

        if (venta) {
          this.formulario.atencionId = venta.AtencionId;
          this.carrito = venta.Detalles.map(d => ({
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
    },
    eliminarDelCarrito(index) {
      this.carrito.splice(index, 1);
    },
    validarFormulario() {
      this.errores = { atencion: null };

      if (!this.formulario.atencionId) {
        this.errores.atencion = "Debe seleccionar una atención.";
        return false;
      }

      if (this.carrito.length === 0) {
        alert("Debe agregar al menos un producto o servicio.");
        return false;
      }

      return true;
    },
    async guardarVenta() {
      if (!this.validarFormulario()) return;

      const detalles = this.carrito.map((item) => ({
        AtencionId: this.formulario.atencionId,
        ProductoServicioId: item.id,
        Cantidad: item.cantidad,
        PrecioUnitario: item.precioUnitario,
      }));

      try {
        if (this.ventaId) {
          await this.$axios.delete(`/api/detalleatencion/atencion/${this.ventaId}`);
        }

        for (const detalle of detalles) {
          await this.$axios.post("/api/detalleatencion", detalle);
        }

        if (!this.ventaId || !(await this.tienePagoRegistrado())) {
          const pagoData = {
            AtencionId: this.formulario.atencionId,
            Monto: this.totalCarrito,
            MetodoPago: "Efectivo",
          };
          await this.$axios.post("/api/pago", pagoData);
        }

        this.$router.push({ name: "Ventas" });
      } catch (error) {
        console.error("Error al guardar venta:", error);
        alert("Hubo un error al guardar la venta.");
      }
    },
    async tienePagoRegistrado() {
      try {
        const res = await this.$axios.get(`/api/pago/atencion/${this.formulario.atencionId}`);
        return res.data.pago !== null;
      } catch {
        return false;
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

.p-button {
  background-color: #4a90e2;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.5rem 1.2rem;
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
}

.p-button:hover {
  background-color: #357abd;
  box-shadow: 0 0 8px #357abd88;
}

.btn-cerrar {
  background-color: transparent;
  border: none;
  color: #ccc;
  font-size: 1.2rem;
  cursor: pointer;
}
</style>
