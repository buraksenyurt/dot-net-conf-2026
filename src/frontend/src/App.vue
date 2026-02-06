<template>
  <div class="d-flex flex-column min-vh-100">
    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
      <div class="container-fluid">
        <router-link to="/" class="navbar-brand">
          <i class="bi bi-car-front-fill me-2"></i>
          Araç Envanter Yönetimi
        </router-link>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <li class="nav-item">
              <router-link to="/" class="nav-link" active-class="active">
                <i class="bi bi-house-fill me-1"></i>
                Ana Sayfa
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/vehicles" class="nav-link" active-class="active">
                <i class="bi bi-list-ul me-1"></i>
                Araçlar
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/vehicles/new" class="nav-link" active-class="active">
                <i class="bi bi-plus-circle-fill me-1"></i>
                Yeni Araç
              </router-link>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="flex-grow-1 bg-light">
      <div class="container-fluid py-4">
        <router-view />
      </div>
    </main>

    <!-- Footer -->
    <footer class="bg-dark text-white text-center py-3 mt-auto">
      <div class="container">
        <p class="mb-0">
          <i class="bi bi-code-slash me-2"></i>
          AI Destekli Legacy Modernizasyonu Demo | .NET Conference 2026
        </p>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
// App root component with Bootstrap layout
</script>

<style scoped>
.navbar-brand {
  font-weight: 600;
  font-size: 1.25rem;
}

.nav-link {
  font-weight: 500;
  transition: all 0.2s;
}

.nav-link:hover {
  transform: translateY(-2px);
}

.router-link-active {
  font-weight: 700;
}
</style>
