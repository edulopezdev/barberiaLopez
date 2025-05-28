<template>
  <div class="formulario-cliente">
    <h3>{{ cliente?.id ? "Editar Cliente" : "Nuevo Cliente" }}</h3>

    <div class="campo">
      <label for="nombre">Nombre</label>
      <InputText id="nombre" v-model="form.nombre" />
    </div>

    <div class="campo">
      <label for="email">Email</label>
      <InputText id="email" v-model="form.email" />
    </div>

    <div class="campo">
      <label for="telefono">Teléfono</label>
      <InputText id="telefono" v-model="form.telefono" />
    </div>

    <div class="acciones-formulario">
      <div class="contenedor-boton">
        <Button
          class="btn-guardar"
          :label="cliente?.id ? 'Actualizar' : 'Guardar'"
          icon="pi pi-check"
          @click="onGuardar"
        />
      </div>

      <button class="btn-cerrar" @click="$emit('cerrar')" aria-label="Cerrar">
        <i class="pi pi-times"></i>
      </button>
    </div>
  </div>
</template>

<script>
import InputText from "primevue/inputtext";
import Button from "primevue/button";

export default {
  name: "ClienteForm",
  components: {
    InputText,
    Button,
  },
  props: {
    cliente: {
      type: Object,
      default: null,
    },
  },
  emits: ["guardar", "cerrar"],
  data() {
    return {
      form: {
        nombre: "",
        email: "",
        telefono: "",
        rolId: 3, // siempre cliente
        accedeAlSistema: false, // siempre false
        password: null, // no se usa
      },
    };
  },
  watch: {
    cliente: {
      immediate: true,
      handler(newVal) {
        if (newVal) {
          Object.assign(this.form, {
            nombre: newVal.nombre || "",
            email: newVal.email || "",
            telefono: newVal.telefono || "",
            rolId: 3,
            accedeAlSistema: false,
            password: null,
          });
        } else {
          this.resetForm();
        }
      },
    },
  },
  methods: {
    onGuardar() {
      // Emitimos solo los datos relevantes y fijos rolId=3, accedeAlSistema=false
      const payload = {
        id: this.cliente?.id ?? null,
        nombre: this.form.nombre,
        email: this.form.email,
        telefono: this.form.telefono,
        rolId: 3,
        accedeAlSistema: false,
      };
      this.$emit("guardar", payload);
    },
    resetForm() {
      this.form = {
        nombre: "",
        email: "",
        telefono: "",
        rolId: 3,
        accedeAlSistema: false,
        password: null,
      };
    },
  },
};
</script>

<style scoped>
.formulario-cliente {
  max-width: 400px;
  margin: 0 auto;
  padding: 1rem;
}

.campo {
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
}

label {
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.acciones-formulario {
  display: flex;
  justify-content: space-between;
  margin-top: 1.5rem;
}

.p-button.p-button-danger {
  background-color: #e74c3c;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.5rem 1.2rem;
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
}

.p-button.p-button-danger:hover {
  background-color: #c0392b;
  box-shadow: 0 0 8px #c0392b88;
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
</style>
