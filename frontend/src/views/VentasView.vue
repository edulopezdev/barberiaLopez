<template>
  <div class="ventas-container">
    <Toast />
    <Card>
      <template #title>
        <div class="encabezado-acciones">
          <h4>Ventas</h4>
          <div class="botones-acciones">
            <Button
              label="Filtros"
              icon="pi pi-filter"
              class="boton-filtros"
              @click="mostrarFiltros = !mostrarFiltros"
            />
            <Button
              label="Nueva Venta"
              icon="pi pi-plus"
              class="boton-nueva-venta"
              @click="crearVenta"
            />
          </div>
        </div>
      </template>

      <template #content>
        <DataTable
          v-model:filters="filters"
          :value="ventas"
          :filterDisplay="mostrarFiltros ? 'row' : 'none'"
          :globalFilterFields="['cliente', 'producto', 'fecha']"
          lazy
          paginator
          :rows="pageSize"
          :first="first"
          :totalRecords="totalVentas"
          tableStyle="min-width: 100%"
          :loading="loading"
          @page="onPageChange"
          @sort="onSort"
          @filter="onFilter"
        >
          <Column field="cliente" sortable>
            <template #header>
              <span class="titulo-columna">Cliente</span>
            </template>
            <template #filter="{ filterModel, filterCallback }">
              <InputText
                v-model="filterModel.value"
                @input="filterCallback()"
                placeholder="Buscar por cliente"
              />
            </template>
          </Column>

          <Column field="producto" header="Producto" sortable>
            <template #filter="{ filterModel, filterCallback }">
              <InputText
                v-model="filterModel.value"
                @input="filterCallback()"
                placeholder="Buscar por producto"
              />
            </template>
          </Column>

          <Column field="fecha" header="Fecha" sortable>
            <template #filter="{ filterModel, filterCallback }">
              <InputText
                v-model="filterModel.value"
                @input="filterCallback()"
                placeholder="Buscar por fecha"
              />
            </template>
          </Column>
          <Column field="montoPagado" header="Monto Total" sortable>
            <template #body="slotProps">
              ${{ slotProps.data.montoPagado?.toFixed(2) || "0.00" }}
            </template>
          </Column>

          <Column field="estado" header="Estado">
            <template #body="slotProps">
              <Tag
                :value="slotProps.data.estado ? 'Completada' : 'Pendiente'"
                :severity="slotProps.data.estado ? 'success' : 'warning'"
              />
            </template>
            <template #filter="{ filterModel, filterCallback }">
              <Dropdown
                v-model="filterModel.value"
                @change="filterCallback()"
                :options="[
                  { label: 'Completada', value: true },
                  { label: 'Pendiente', value: false },
                ]"
                optionLabel="label"
                placeholder="Seleccionar estado"
                showClear
              />
            </template>
          </Column>
          <Column field="montoPagado" header="Pagado" sortable>
            <template #body="slotProps">
              ${{ slotProps.data.montoPagado?.toFixed(2) || 0 }}
            </template>
          </Column>

          <Column header="Acciones" style="min-width: 180px">
            <template #body="slotProps">
              <div class="acciones-botones">
                <Button
                  icon="pi pi-eye"
                  severity="info"
                  text
                  rounded
                  v-tooltip.bottom="'Ver detalles'"
                  @click="verDetalles(slotProps.data)"
                />
                <Button
                  icon="pi pi-pencil"
                  severity="warning"
                  text
                  rounded
                  v-tooltip.bottom="'Editar venta'"
                  @click="editarVenta(slotProps.data)"
                />
                <!-- Botón pagar si está pendiente, ícono check si ya está pagada -->
                <Button
                  v-if="!slotProps.data.estado"
                  icon="pi pi-dollar"
                  severity="success"
                  text
                  rounded
                  v-tooltip.bottom="'Pagar venta'"
                  @click="pagarDialog(slotProps.data)"
                />

                <span
                  v-else
                  class="icono-check"
                  v-tooltip.bottom="'Venta pagada'"
                >
                  <i
                    class="pi pi-check"
                    style="color: #27ae60; font-size: 1.2rem"
                  ></i>
                </span>
              </div>
            </template>
          </Column>
        </DataTable>

        <!-- Mensaje de cantidad total -->
        <div class="total-ventas" v-if="totalVentas > 0">
          Total de ventas registradas: {{ totalVentas }}
        </div>
      </template>
    </Card>

    <!-- Modal Crear / Editar Venta -->
    <Dialog
      v-model:visible="mostrarModal"
      :header="ventaSeleccionada?.id ? 'Editar Venta' : 'Nueva Venta'"
      :modal="true"
      :closeOnEscape="false"
      :closeOnBackdropClick="false"
      :closable="false"
      style="width: 700px"
    >
      <VentaForm
        :venta="ventaSeleccionada"
        @guardar="guardarVenta($event)"
        @cancelar="cerrarModal"
      />
    </Dialog>

    <!-- Modal Detalle Venta -->
    <Dialog
      v-model:visible="mostrarDetalleModal"
      header="Detalle de la Venta"
      :modal="true"
      :closable="false"
      style="width: 450px"
    >
      <!-- Mensaje de carga o error -->
      <div v-if="!ventaSeleccionada" class="mensaje-carga">
        <i class="pi pi-spin pi-spinner" style="font-size: 1.5rem"></i>
        <span>Cargando detalle...</span>
      </div>

      <!-- Mostrar componente solo si ventaSeleccionada tiene datos -->
      <VentaDetalle
        v-else
        :venta="ventaSeleccionada"
        @cerrar="mostrarDetalleModal = false"
      />
    </Dialog>
  </div>
</template>
<script>
import VentaService from "../services/VentaService";
import DataTable from "primevue/datatable";
import Column from "primevue/column";
import Card from "primevue/card";
import Tag from "primevue/tag";
import Button from "primevue/button";
import InputText from "primevue/inputtext";
import Dropdown from "primevue/dropdown";
import Dialog from "primevue/dialog";
import { FilterMatchMode } from "primevue/api";
import Swal from "sweetalert2";

import VentaForm from "../components/VentaForm.vue";
import VentaDetalle from "../components/VentaDetalle.vue";

export default {
  components: {
    DataTable,
    Column,
    Card,
    Tag,
    Button,
    InputText,
    Dropdown,
    Dialog,
    VentaForm,
    VentaDetalle,
  },
  data() {
    return {
      ventas: [],
      totalVentas: 0,
      currentPage: 1,
      pageSize: 10,
      first: 0,
      sortField: null,
      sortOrder: null,
      loading: false,
      mostrarFiltros: false,
      mostrarModal: false,
      ventaSeleccionada: null,
      mostrarDetalleModal: false,
      filters: {
        cliente: { value: null, matchMode: FilterMatchMode.CONTAINS },
        producto: { value: null, matchMode: FilterMatchMode.CONTAINS },
        fecha: { value: null, matchMode: FilterMatchMode.CONTAINS },
        estado: { value: null, matchMode: FilterMatchMode.EQUALS },
      },
    };
  },
  mounted() {
    this.obtenerVentas();
  },
  methods: {
    async obtenerVentas(page = 1, pageSize = 10) {
      this.loading = true;
      try {
        const res = await VentaService.getVentas(page, pageSize);

        // Transformamos cada venta para que tenga los campos que espera la tabla
        this.ventas = res.data.ventas.map((venta) => {
          const pagos = venta.pagos || [];
          const montoPagado = pagos.reduce((acc, p) => acc + p.monto, 0);

          return {
            cliente: venta.clienteNombre,
            producto: venta.detalles.map((d) => d.nombreProducto).join(", "),
            fecha: new Date(venta.fechaAtencion).toLocaleDateString(),
            estado: montoPagado >= venta.totalVenta,
            id: venta.atencionId,
            totalVenta: venta.totalVenta,
            montoPagado,
          };
        });

        this.totalVentas = res.data.pagination?.total || this.ventas.length;
        this.currentPage = page;
        this.pageSize = pageSize;
        this.first = (page - 1) * pageSize;
      } catch (err) {
        console.error("Error al obtener ventas:", err);
        Swal.fire("Error", "No se pudieron cargar las ventas.", "error");
      } finally {
        this.loading = false;
      }
    },

    pagarDialog(venta) {
      Swal.fire({
        title: `Registrar pago de $${venta.totalVenta}?`,
        input: "select",
        inputOptions: {
          Efectivo: "Efectivo",
          "Transferencia - Mercado Pago": "Transferencia - Mercado Pago",
          "Transferencia - NaranjaX": "Transferencia - NaranjaX",
        },
        inputPlaceholder: "Seleccione método de pago",
        showCancelButton: true,
        confirmButtonText: "Registrar Pago",
        cancelButtonText: "Cancelar",
        background: "#18181b",
        color: "#fff",
      }).then((result) => {
        if (result.isConfirmed && result.value) {
          const nuevoPago = {
            atencionId: venta.id,
            metodoPago: result.value,
            monto: venta.totalVenta,
            fecha: new Date().toISOString(),
          };

          VentaService.RegistrarPago(nuevoPago)
            .then(() => {
              Swal.fire({
                icon: "success",
                title: "Pago registrado",
                text: `Método: ${result.value}`,
                background: "#18181b",
                color: "#fff",
                timer: 2000,
                showConfirmButton: false,
              });

              // Esperamos 500ms para dar tiempo al backend a actualizar datos
              setTimeout(() => {
                this.obtenerVentas(this.currentPage, this.pageSize);
              }, 500);
            })

            .catch((error) => {
              console.error("Error al registrar el pago:", error);
              Swal.fire({
                icon: "error",
                title: "Error",
                text: "No se pudo registrar el pago.",
                background: "#18181b",
                color: "#fff",
              });
            });
        }
      });
    },
    onPageChange(event) {
      this.obtenerVentas(event.page + 1, event.rows);
    },

    onSort(event) {
      this.sortField = event.sortField;
      this.sortOrder = event.sortOrder;
      this.obtenerVentas(this.currentPage, this.pageSize);
    },

    onFilter() {
      this.obtenerVentas(1, this.pageSize);
    },

    crearVenta() {
      console.log("Click en Nueva Venta");

      this.ventaSeleccionada = null;
      this.mostrarModal = true;
      document.body.classList.add("modal-open");
    },

    editarVenta(venta) {
      this.ventaSeleccionada = { ...venta };
      this.mostrarModal = true;
      document.body.classList.add("modal-open");
    },

    cerrarModal() {
      this.mostrarModal = false;
      document.body.classList.remove("modal-open");
    },

    verDetalles(venta) {
      const atencionId = venta.id; // <- corregido aquí

      if (!atencionId) {
        Swal.fire("Error", "ID de venta no encontrado.", "error");
        return;
      }

      VentaService.getVentaById(atencionId)
        .then((res) => {
          const data = res.data.venta;

          this.ventaSeleccionada = {
            ClienteNombre: data.clienteNombre,
            FechaAtencion: data.fechaAtencion,
            Detalles: data.detalles.map((d) => ({
              NombreProducto: d.nombreProducto,
              Cantidad: d.cantidad,
              PrecioUnitario: d.precioUnitario,
              Subtotal: d.subtotal,
            })),
            TotalVenta: data.totalVenta,
            Pago: data.pago
              ? {
                  MetodoPago: data.pago.metodoPago,
                  Monto: data.pago.monto,
                }
              : null,
            AtencionId: atencionId, // <- necesario si usas router-link para editar
          };

          this.mostrarDetalleModal = true;
        })
        .catch(() => {
          Swal.fire(
            "Error",
            "No se pudo cargar el detalle de la venta.",
            "error"
          );
        });
    },
    async guardarVenta(ventaActualizada) {
      this.cerrarModal();

      const mensaje = ventaActualizada.id
        ? "¿Actualizar esta venta?"
        : "¿Registrar nueva venta?";

      const result = await Swal.fire({
        title: mensaje,
        icon: "question",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#6c757d",
        confirmButtonText: "Sí, confirmar",
        cancelButtonText: "Cancelar",
        background: "#18181b",
        color: "#fff",
      });

      if (result.isConfirmed) {
        try {
          if (ventaActualizada.id) {
            // Aquí podrías usar: await VentaService.actualizarVenta(ventaActualizada.id, ventaActualizada);
          } else {
            // Aquí podrías usar: await VentaService.crearVenta(ventaActualizada);
          }

          Swal.fire({
            title: "Éxito",
            text: `Venta ${
              ventaActualizada.id ? "actualizada" : "creada"
            } correctamente.`,
            icon: "success",
            timer: 2000,
            showConfirmButton: false,
            background: "#18181b",
            color: "#fff",
          });

          this.obtenerVentas(this.currentPage, this.pageSize);
        } catch (error) {
          const mensaje =
            error?.response?.data?.message || "No se pudo guardar la venta.";
          console.error("Error al guardar venta:", error);
          Swal.fire({
            title: "Error",
            text: mensaje,
            icon: "error",
            background: "#18181b",
            color: "#fff",
          });
          this.ventaSeleccionada = ventaActualizada;
          this.mostrarModal = true;
        }
      } else {
        this.ventaSeleccionada = ventaActualizada;
        this.mostrarModal = true;
      }
    },

    anularVenta(venta) {
      Swal.fire({
        title: `¿Anular venta de ${venta.cliente}?`,
        text: "Esta acción no se puede deshacer.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, anular",
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#e74c3c",
        cancelButtonColor: "#6c757d",
        background: "#18181b",
        color: "#fff",
      }).then((result) => {
        if (result.isConfirmed) {
          Swal.fire({
            title: "Anulada",
            text: "La venta ha sido anulada.",
            icon: "success",
            timer: 2000,
            showConfirmButton: false,
            background: "#18181b",
            color: "#fff",
          });
          this.obtenerVentas(this.currentPage, this.pageSize);
        }
      });
    },

    reactivarVenta(venta) {
      Swal.fire({
        title: `¿Reactivar venta de ${venta.cliente}?`,
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Sí, reactivar",
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#28a745",
        cancelButtonColor: "#6c757d",
        background: "#18181b",
        color: "#fff",
      }).then((result) => {
        if (result.isConfirmed) {
          Swal.fire({
            title: "Reactivada",
            text: "La venta ha sido reactivada.",
            icon: "success",
            timer: 2000,
            showConfirmButton: false,
            background: "#18181b",
            color: "#fff",
          });
          this.obtenerVentas(this.currentPage, this.pageSize);
        }
      });
    },
  },
};
</script>

<style scoped>
/* ===========================
   CONTENEDOR GENERAL
=========================== */
.ventas-container {
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
  color: #e0e0e0;
}

/* ===========================
   ENCABEZADO DE TABLA Y BOTONES
=========================== */
.encabezado-acciones {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: -2.5rem;
  margin-top: -1rem;
}

.boton-filtros {
  background-color: transparent !important;
  color: white !important;
  border: none !important;
  box-shadow: none !important;
  padding: 0.3rem 0.6rem !important;
  margin-right: 1rem !important;
}
.boton-filtros:focus,
.boton-filtros:focus-visible,
.boton-filtros:active {
  outline: none !important;
  box-shadow: none !important;
  border-color: transparent !important;
}
:deep(.boton-filtros:focus-visible) {
  outline: none !important;
  box-shadow: 0 0 0 2px rgba(40, 167, 69, 0.4) !important;
  border-color: transparent !important;
}

.boton-nueva-venta {
  background-color: #28a745;
  color: white;
  font-weight: normal;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-size: 0.7rem;
  width: auto !important;
  height: auto !important;
  min-width: 120px !important;
}
.boton-nueva-venta:hover {
  background-color: #218838;
}

/* ===========================
   DATATABLE ESTILOS GENERALES
=========================== */
:deep(.p-datatable) {
  background-color: #121212;
  color: #eee;
  border-radius: 10px;
  border: none;
  box-shadow: 0 3px 8px rgb(0 0 0 / 0.5);
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  font-size: 0.95rem;
}

:deep(.p-datatable-tbody > tr > td),
:deep(.p-datatable-thead > tr > th) {
  padding: 0.4rem 0.75rem !important;
  line-height: 1.2rem;
  border-bottom: 1px solid #333 !important;
  vertical-align: middle;
  text-align: center !important;
}

:deep(.p-datatable-thead > tr > th) {
  background-color: #2a2a2a;
  color: #ffffff;
  font-weight: 600;
  text-align: center !important;
}

:deep(.p-datatable-tbody > tr:hover) {
  background-color: #1e3a1e;
}

:deep(.p-sortable-column .p-column-header-content) {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
}

:deep(.p-datatable-thead > tr > th:nth-child(1) .p-column-header-content) {
  gap: 0.1rem !important;
}

:deep(.p-datatable-thead > tr > th:nth-child(5)) /* Estado */ {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

:deep(.p-datatable-thead > tr > th:nth-child(7)) /* Acciones */ {
  text-align: center !important;
  vertical-align: middle !important;
}

:deep(.p-datatable-thead > tr > th:nth-child(7) .p-column-header-content) {
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  gap: 0.4rem;
}

/* ===========================
   ESTILOS DE FILTROS EN COLUMNAS
=========================== */
:deep(.p-column-filter .p-inputtext) {
  height: 28px !important;
  padding: 0.25rem 0.5rem !important;
  font-size: 0.85rem !important;
  margin: 0 !important;
  border-radius: 4px !important;
}

:deep(.p-column-filter .p-dropdown) {
  height: 28px !important;
  font-size: 0.85rem !important;
}

:deep(.p-column-filter .p-dropdown .p-dropdown-label) {
  line-height: 28px !important;
  padding: 0 0.5rem !important;
}

:deep(.p-column-filter) {
  padding: 0.15rem 0.3rem !important;
  margin: 0 !important;
  min-height: 40px;
}

:deep(.p-column-filter-menu-button) {
  background: transparent !important;
  border: none !important;
  padding: 0 !important;
  width: 14px;
  height: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

:deep(.p-column-filter-clear-button) {
  background: transparent !important;
  border: none !important;
  padding: 0 !important;
  width: auto !important;
  height: auto !important;
  min-width: 0 !important;
  min-height: 0 !important;
  display: inline-flex !important;
  align-items: center !important;
  justify-content: center !important;
  box-shadow: none !important;
  outline: none !important;
}

:deep(.p-column-filter-clear-button > svg) {
  width: 16px !important;
  height: 16px !important;
  pointer-events: auto;
  fill: currentColor;
}

:deep(.p-column-filter-menu-button:focus),
:deep(.p-column-filter-menu-button:active) {
  outline: none !important;
  box-shadow: none !important;
}

/* ===========================
   PAGINADOR
=========================== */
:deep(.p-datatable .p-paginator) {
  background-color: transparent;
  border-top: 1px solid #444;
  padding-top: 0.4rem;
  font-size: 0.85rem;
  color: #ccc;
}

:deep(.p-datatable .p-paginator .p-highlight) {
  background-color: #28a745 !important;
  color: white !important;
  border-radius: 4px;
  font-weight: 600;
}

:deep(.p-paginator .p-paginator-page:focus),
:deep(.p-paginator .p-paginator-page:focus-visible),
:deep(.p-paginator .p-paginator-page.p-highlight) {
  outline: none !important;
  box-shadow: none !important;
  border-color: transparent !important;
}

/* ===========================
   BOTONES DE ACCIÓN EN FILA
=========================== */
.acciones-botones {
  display: flex;
  gap: 0.25rem;
  justify-content: center;
  align-items: center;
}

:deep(.p-button) {
  margin: 0 !important;
  padding: 0.25rem 0.35rem !important;
  font-size: 0.85rem !important;
  width: 30px;
  height: 30px;
  min-width: 30px !important;
  border-radius: 6px !important;
  line-height: 1;
}

:deep(.p-button:last-child) {
  margin-right: 0;
}

/* ===========================
   TAGS DE ESTADO (ACTIVO / INACTIVO)
=========================== */
:deep(.p-tag) {
  border-radius: 12px !important;
  padding: 0.2rem 0.6rem !important;
  font-weight: 600;
}

:deep(.p-tag-success) {
  background-color: #27ae60 !important;
  color: #e0f2f1 !important;
  font-weight: 600;
  border-radius: 12px !important;
  padding: 0.2rem 0.6rem !important;
}
:deep(.p-tag-warning) {
  background-color: #f0ad4e !important;
  color: #000000 !important;
  font-weight: 600;
  border-radius: 12px !important;
  padding: 0.2rem 0.6rem !important;
}

:deep(.p-tag-danger) {
  background-color: #c0392b !important;
  color: #fdecea !important;
  font-weight: 600;
  border-radius: 12px !important;
  padding: 0.2rem 0.6rem !important;
}

/* ===========================
   TÍTULOS Y FILTROS AVANZADOS
=========================== */
.titulo-columna {
  font-weight: 600;
  color: white;
}

.columna-nombre-header {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: flex-start;
  gap: 0.3rem;
}

.icono-filtro {
  position: absolute;
  left: -5rem;
  top: 50%;
  transform: translateY(-50%);
  color: white;
  font-size: 1rem;
  cursor: pointer;
  transition: transform 0.2s ease, color 0.2s ease;
  z-index: 10;
}
.icono-filtro:hover {
  transform: scale(1.2);
  color: #28a745;
}

.total-ventas {
  margin-top: 1.9rem;
  font-size: 1rem;
  font-weight: 500;
  text-align: left;
  color: #aeaeae;
}
.icono-check {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 30px;
}
</style>
