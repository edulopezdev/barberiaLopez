import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5042/api",
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor para incluir siempre el token más reciente
apiClient.interceptors.request.use((config) => {
  const token = sessionStorage.getItem("token");
  console.log("Token enviado:", token);

  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

// Funciones para interactuar con la API de clientes
export default {
  // Función para obtener todos los clientes
  getClientes(page = 1, pageSize = 10, filtros = {}) {
    const params = new URLSearchParams({
      page,
      pageSize,
      ...filtros, // ejemplo: { nombre: "ana", email: "gmail.com" }
    });

    return apiClient.get(`/Usuarios/clientes?${params.toString()}`);
  },
  getCliente(id) {
    return apiClient.get(`/usuarios/${id}`);
  },

  // Función para crear un nuevo cliente
  crearCliente(clienteData) {
    const data = {
      nombre: clienteData.nombre,
      email: clienteData.email,
      telefono: clienteData.telefono,
      rolId: 3,
      accedeAlSistema: false,
    };

    console.log("Datos que se envían al backend:", data);

    return apiClient.post("/usuarios", data);
  },
  // Función para actualizar un cliente existente
  actualizarCliente(id, clienteData) {
    const dataParaEnviar = {
      nombre: clienteData.nombre,
      email: clienteData.email,
      telefono: clienteData.telefono,
      rolId: 3, // Si siempre es cliente, fijo en 3
      accedeAlSistema: false,
    };

    console.log("Datos para actualizar al backend:", dataParaEnviar);

    return apiClient.put(`/usuarios/${id}`, dataParaEnviar);
  },
};
