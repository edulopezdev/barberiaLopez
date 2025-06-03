import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5042/api",
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor para agregar token
apiClient.interceptors.request.use((config) => {
  const token = sessionStorage.getItem("token");
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

export default {
  // Obtener lista de ventas
  getVentas(page = 1, pageSize = 10) {
    return apiClient.get("/DetalleAtencion", {
      params: { page, pageSize },
    });
  },

  // Obtener una venta por ID (opcional)
  getVentaById(id) {
    return apiClient.get(`/detalleatencion/ventas`, {
      params: { atencionId: id },
    });
  },
};