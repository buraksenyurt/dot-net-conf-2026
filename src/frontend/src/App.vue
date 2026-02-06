<template>
  <div class="d-flex flex-column min-vh-100">
    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark shadow-sm">
      <div class="container">
        <router-link to="/" class="navbar-brand fw-bold d-flex align-items-center">
          <i class="bi bi-car-front-fill me-2 fs-4 text-primary"></i>
          <span>Araç Envanter</span>
        </router-link>
        
        <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto mb-2 mb-lg-0">
            <li class="nav-item me-3">
              <router-link to="/" class="nav-link px-3" active-class="active">
                <i class="bi bi-speedometer2 me-1"></i> Dashboard
              </router-link>
            </li>
            <li class="nav-item me-3">
              <router-link to="/vehicles" class="nav-link px-3" active-class="active">
                <i class="bi bi-list-ul me-1"></i> Envanter
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/vehicles/new" class="btn btn-primary btn-sm px-4 rounded-pill mt-1 mt-lg-0">
                <i class="bi bi-plus-lg me-1"></i> Yeni Araç
              </router-link>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="flex-grow-1 bg-light">
      <div class="container py-5">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </main>

    <!-- Footer -->
    <footer class="bg-white border-top py-4 mt-auto">
      <div class="container text-center">
        <p class="mb-0 text-muted small">
          <i class="bi bi-code-slash me-2 text-primary"></i>
          AI Destekli Legacy Modernizasyonu Demo | .NET Conference 2026
        </p>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
// App root component with corporate layout
</script>

<style scoped>
.navbar {
  backdrop-filter: blur(10px);
}

.nav-link {
  font-weight: 500;
  border-radius: 0.5rem;
  transition: all 0.2s;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: white !important;
}

.nav-link.active {
  background-color: rgba(255, 255, 255, 0.15);
  color: white !important;
  font-weight: 600;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
