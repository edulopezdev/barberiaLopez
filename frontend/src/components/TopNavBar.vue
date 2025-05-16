<template>
  <header class="top-navbar">
    <div class="left"></div>
    <div class="right">
      <span class="username">Hola, Barbero!</span>
    </div>
  </header>
</template>

<style scoped>
.top-navbar {
  height: 80px;
  background-color: rgba(255, 255, 255, 0.8); /* Fondo transparente */
  color: #000000; /* Texto claro */
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 2rem;
}

.username {
  font-weight: bold;
}
</style>
