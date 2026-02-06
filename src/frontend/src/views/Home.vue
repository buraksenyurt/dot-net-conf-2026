<template>
  <div class="text-center py-12">
    <h1 class="text-4xl font-bold text-gray-800 mb-4">Araç Envanter Yönetim Sistemi</h1>
    <p class="text-xl text-gray-600 mb-8">AI Destekli Legacy Modernizasyonu Demo Uygulaması</p>
    <div class="grid md:grid-cols-3 gap-6 max-w-4xl mx-auto mt-12">
      <div class="bg-white p-6 rounded-lg shadow">
        <h3 class="text-lg font-semibold mb-2">Vue 3 + TypeScript</h3>
        <p class="text-gray-600">Modern frontend framework</p>
      </div>
      <div class="bg-white p-6 rounded-lg shadow">
        <h3 class="text-lg font-semibold mb-2">.NET 9 + Clean Architecture</h3>
        <p class="text-gray-600">Modern backend</p>
      </div>
      <div class="bg-white p-6 rounded-lg shadow">
        <h3 class="text-lg font-semibold mb-2">PostgreSQL</h3>
        <p class="text-gray-600">Modern database</p>
      </div>
    </div>
    <div class="mt-12">
      <router-link to="/vehicles" class="bg-blue-600 text-white px-8 py-3 rounded-lg text-lg hover:bg-blue-700">
        Araçları Görüntüle
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
// Home page
</script>
