import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5042/api",
  timeout: 5000, // Tiempo límite para respuestas
  headers: {
    "Content-Type": "application/json",
  },
});

export default api;