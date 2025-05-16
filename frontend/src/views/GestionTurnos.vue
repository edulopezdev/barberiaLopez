<template>
  <div>
    <h1>Página de Turnos</h1>
    <p v-if="loading">Cargando turnos...</p>
    <p v-if="error" class="error-message">{{ error }}</p>

    <FullCalendar
      v-if="!loading && turnos.length"
      :options="calendarOptions"
      class="calendar"
    />

    <div v-if="selectedTurno" class="modal-overlay" @click.self="closeModal">
      <div class="modal">
        <h2>Detalles del Turno</h2>
        <p><strong>ID:</strong> {{ selectedTurno.id }}</p>
        <p><strong>Fecha y Hora:</strong> {{ selectedTurno.fechaHora }}</p>
        <p><strong>Cliente ID:</strong> {{ selectedTurno.clienteId }}</p>
        <p><strong>Barbero ID:</strong> {{ selectedTurno.barberoId }}</p>
        <p><strong>Estado ID:</strong> {{ selectedTurno.estadoId }}</p>
        <button @click="closeModal">Cerrar</button>
      </div>
    </div>
  </div>
</template>

<script>
import FullCalendar from "@fullcalendar/vue3";
import dayGridPlugin from "@fullcalendar/daygrid";
import interactionPlugin from "@fullcalendar/interaction";

import api from "@/api";

export default {
  name: "GestionTurnos",
  components: { FullCalendar },
  data() {
    return {
      turnos: [],
      loading: true,
      error: null,
      selectedTurno: null,
      calendarOptions: {
        plugins: [dayGridPlugin, interactionPlugin],
        initialView: "dayGridMonth", // ← solo mes
        locale: "es",
        headerToolbar: {
          left: "prev,next today",
          center: "title",
          right: "", // ← sin botones para cambiar a semana o día
        },
        events: [],
        eventClick: this.handleEventClick,
        height: "auto", // se ajusta al contenido
      },
    };
  },
  methods: {
    handleEventClick(info) {
      this.selectedTurno = info.event.extendedProps;
      this.selectedTurno.id = info.event.id;
      this.selectedTurno.fechaHora = info.event.start.toLocaleString();
    },
    closeModal() {
      this.selectedTurno = null;
    },
  },
  async mounted() {
    try {
      const response = await api.get("/turnos");
      const turnosData = response.data.turnos || [];

      this.turnos = turnosData;

      this.calendarOptions.events = turnosData.map((turno) => ({
        id: turno.id,
        title: `Cliente ${turno.clienteId}`,
        start: turno.fechaHora,
        extendedProps: {
          clienteId: turno.clienteId,
          barberoId: turno.barberoId,
          estadoId: turno.estadoId,
        },
      }));
    } catch (err) {
      this.error = "Error al cargar los turnos.";
      console.error(err);
    } finally {
      this.loading = false;
    }
  },
};
</script>

<style>
/* Estilos del calendario */
.calendar {
  max-width: 100%;
  margin: 20px auto;
  padding: 10px;
}

.gestion-turnos {
  color: #222;
}

h1 {
  color: #111;
  font-weight: 600;
}

/* Achicar la altura de las filas del calendario */
.fc .fc-daygrid-day-frame {
  padding: 4px; /* menos espacio interno */
}

/* Ajustar el alto mínimo de cada celda del calendario */
.fc .fc-daygrid-day {
  min-height: 60px; /* por defecto es más grande */
}

/* Reducir tamaño del texto del título y cabecera */
.fc-toolbar-title {
  font-size: 1.2rem;
}

.fc .fc-col-header-cell-cushion {
  font-size: 0.9rem;
  padding: 4px;
}


/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 999;
}

.modal {
  background-color: #fff;
  padding: 25px;
  border-radius: 10px;
  width: 90%;
  max-width: 400px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.2);
  font-family: "Segoe UI", sans-serif;
}

.modal h2 {
  margin-top: 0;
}

.modal button {
  background-color: #0a5e2a;
  color: white;
  border: none;
  padding: 10px 20px;
  margin-top: 15px;
  border-radius: 5px;
  cursor: pointer;
}

.modal button:hover {
  background-color: #087227;
}

/* Modo oscuro */
@media (prefers-color-scheme: dark) {
  .modal {
    background-color: #2b2b2b;
    color: white;
  }

  .modal button {
    background-color: #33a05f;
  }

  .modal button:hover {
    background-color: #45c278;
  }
}
</style>
