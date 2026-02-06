<template>
  <div class="container py-5">
    <div class="text-center mb-5">
      <h1 class="display-4 fw-bold text-primary mb-3">
        <i class="bi bi-car-front-fill me-3"></i>
        Araç Envanter Yönetim Sistemi
      </h1>
      <p class="lead text-muted">AI Destekli Legacy Modernizasyonu Demo Uygulaması</p>
    </div>

    <div class="row g-4 mb-5">
      <div class="col-md-4">
        <div class="card h-100 border-primary shadow-sm hover-card">
          <div class="card-body text-center">
            <i class="bi bi-lightning-charge-fill text-primary fs-1 mb-3"></i>
            <h5 class="card-title">Vue 3 + TypeScript</h5>
            <p class="card-text text-muted">Composition API ile modern frontend framework</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card h-100 border-success shadow-sm hover-card">
          <div class="card-body text-center">
            <i class="bi bi-gear-fill text-success fs-1 mb-3"></i>
            <h5 class="card-title">.NET 9 + Clean Architecture</h5>
            <p class="card-text text-muted">Domain-Driven Design ile modern backend</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card h-100 border-info shadow-sm hover-card">
          <div class="card-body text-center">
            <i class="bi bi-database-fill text-info fs-1 mb-3"></i>
            <h5 class="card-title">PostgreSQL</h5>
            <p class="card-text text-muted">Modern ilişkisel veritabanı</p>
          </div>
        </div>
      </div>
    </div>

    <div class="text-center">
      <router-link to="/vehicles" class="btn btn-primary btn-lg px-5 py-3 shadow">
        <i class="bi bi-list-ul me-2"></i>
        Araçları Görüntüle
      </router-link>
    </div>

    <div class="mt-5 pt-5 border-top">
      <div class="row">
        <div class="col-md-6">
          <h4 class="mb-3"><i class="bi bi-info-circle-fill text-primary me-2"></i>Proje Hakkında</h4>
          <p class="text-muted">
            Bu uygulama, spec-oriented development yaklaşımını ve AI destekli kod üretimini 
            göstermek için hazırlanmış bir demo projedir.
          </p>
        </div>
        <div class="col-md-6">
          <h4 class="mb-3"><i class="bi bi-stack text-success me-2"></i>Teknoloji Stack</h4>
          <ul class="list-unstyled text-muted">
            <li><i class="bi bi-check-circle-fill text-success me-2"></i>Vue 3 + Vite + TypeScript</li>
            <li><i class="bi bi-check-circle-fill text-success me-2"></i>.NET 9 + EF Core</li>
            <li><i class="bi bi-check-circle-fill text-success me-2"></i>Clean Architecture + CQRS</li>
            <li><i class="bi bi-check-circle-fill text-success me-2"></i>PostgreSQL + Docker</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Home page with Bootstrap cards
</script>

<style scoped>
.hover-card {
  transition: all 0.3s ease;
}

.hover-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15) !important;
}

.btn-lg {
  font-size: 1.1rem;
  font-weight: 600;
  border-radius: 0.5rem;
  transition: all 0.3s;
}

.btn-lg:hover {
  transform: translateY(-3px);
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.2);
}
</style>
