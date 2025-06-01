import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5042/api",
  headers: {
    "Content-Type": "multipart/form-data",
  },
});

// Interceptor para token (si aplica)
apiClient.interceptors.request.use((config) => {
  const token = sessionStorage.getItem("token");
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

export default {
  // === PRODUCTOS (almacenable = true) ===
  getProductos(page = 1, pageSize = 10, filtros = {}) {
    const params = new URLSearchParams({
      page,
      pageSize,
      ...filtros,
    });

    return apiClient.get(
      `/productosservicios/almacenables?${params.toString()}`
    );
  },

  getProducto(id) {
    return apiClient.get(`/productosservicios/${id}`);
  },

  crearProducto(productoData) {
    return apiClient.post("/productosservicios", productoData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

  actualizarProducto(id, productoData) {
    return apiClient.put(`/productosservicios/${id}`, productoData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

  eliminarProducto(id) {
    return apiClient.delete(`/productosservicios/${id}`);
  },

  // === SERVICIOS (almacenable = false) ===
  getServicios(page = 1, pageSize = 10, filtros = {}) {
    const params = new URLSearchParams({
      page,
      pageSize,
      ...filtros,
    });
    return apiClient.get(
      `/productosservicios/noAlmacenables?${params.toString()}`
    );
  },

  getServicio(id) {
    return apiClient.get(`/productosservicios/${id}`);
  },

  crearServicio(servicioData) {
    return apiClient.post("/productosservicios", servicioData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

  actualizarServicio(id, servicioData) {
    return apiClient.put(`/productosservicios/${id}`, servicioData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

  eliminarServicio(id) {
    return apiClient.delete(`/productosservicios/${id}`);
  },
};
