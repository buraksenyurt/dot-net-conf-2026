<template>
  <div class="text-center py-16">
    <div class="mb-8">
      <h1 class="text-5xl font-bold text-gray-800 mb-4">
        🚗 Araç Envanter Yönetimi
      </h1>
      <p class="text-xl text-gray-600 mb-2">
        AI Destekli Legacy Sistem Modernizasyonu
      </p>
      <p class="text-gray-500">
        .NET Conference 2026 - Demo Uygulaması
      </p>
    </div>

    <div class="grid md:grid-cols-3 gap-6 max-w-4xl mx-auto mb-12">
      <div class="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow">
        <div class="text-4xl mb-3">📝</div>
        <h3 class="text-lg font-semibold mb-2">Spec-Oriented</h3>
        <p class="text-sm text-gray-600">
          Detaylı dokümanlar ile yönlendirilen geliştirme süreci
        </p>
      </div>

      <div class="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow">
        <div class="text-4xl mb-3">🤖</div>
        <h3 class="text-lg font-semibold mb-2">AI Asistanları</h3>
        <p class="text-sm text-gray-600">
          Claude Sonnet ve GitHub Copilot ile kod üretimi
        </p>
      </div>

      <div class="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow">
        <div class="text-4xl mb-3">⚡</div>
        <h3 class="text-lg font-semibold mb-2">%40 Daha Hızlı</h3>
        <p class="text-sm text-gray-600">
          Geleneksel yöntemlere göre geliştirme süresinde azalma
        </p>
      </div>
    </div>

    <div class="space-x-4">
      <NuxtLink 
        to="/vehicles" 
        class="inline-block bg-blue-600 text-white px-8 py-3 rounded-lg hover:bg-blue-700 transition-colors font-semibold"
      >
        Araç Listesi
      </NuxtLink>
      <NuxtLink 
        to="/vehicles/new" 
        class="inline-block bg-green-600 text-white px-8 py-3 rounded-lg hover:bg-green-700 transition-colors font-semibold"
      >
        Yeni Araç Ekle
      </NuxtLink>
    </div>

    <div class="mt-16 bg-gray-100 p-6 rounded-lg max-w-3xl mx-auto">
      <h2 class="text-2xl font-bold mb-4">Teknoloji Stack</h2>
      <div class="grid md:grid-cols-2 gap-4 text-left">
        <div>
          <h3 class="font-semibold text-blue-600 mb-2">Backend</h3>
          <ul class="text-sm text-gray-700 space-y-1">
            <li>✓ .NET 9 + C# 13</li>
            <li>✓ Clean Architecture</li>
            <li>✓ Entity Framework Core</li>
            <li>✓ PostgreSQL</li>
          </ul>
        </div>
        <div>
          <h3 class="font-semibold text-green-600 mb-2">Frontend</h3>
          <ul class="text-sm text-gray-700 space-y-1">
            <li>✓ Vue 3 + Nuxt 3</li>
            <li>✓ TypeScript</li>
            <li>✓ Tailwind CSS</li>
            <li>✓ Composition API</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Home page
useHead({
  title: 'Ana Sayfa - Araç Envanter Yönetimi'
})
</script>
