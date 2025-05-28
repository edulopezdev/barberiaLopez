<template>
  <ClienteForm @submit="crear" />
</template>

<script>
import ClienteForm from "@/components/ClienteForm.vue";
import ClienteService from "@/services/ClienteService";

export default {
  components: { ClienteForm },
  methods: {
    crear(clienteData) {
      ClienteService.crearCliente(clienteData).then(() => {
        this.$router.push({ name: "Clientes" });
      });
    },
  },
};
</script>
