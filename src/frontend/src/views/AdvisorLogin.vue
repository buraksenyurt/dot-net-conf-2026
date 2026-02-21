<template>
  <div class="min-vh-100 d-flex">
    <!-- Sol: Marka Paneli -->
    <div
      class="d-none d-lg-flex flex-column justify-content-between text-white p-5"
      style="width: 420px; flex-shrink: 0; background: linear-gradient(150deg, #0f172a 0%, #1e3a5f 60%, #1a4a8a 100%);"
    >
      <div>
        <div class="d-flex align-items-center mb-5">
          <div class="bg-primary rounded-2 p-2 me-3">
            <i class="bi bi-car-front-fill fs-3"></i>
          </div>
          <div>
            <div class="fw-bold fs-4 lh-1">AIO Demo</div>
            <div class="text-white-50 small">Araç Envanter Sistemi</div>
          </div>
        </div>

        <h2 class="display-6 fw-bold mb-3 lh-sm">
          Servis Danışmanı<br>
          <span class="text-primary">Portalı</span>
        </h2>
        <p class="text-white-50 mb-5">
          Opsiyonladığınız araçları tek ekranda takip edin.
          Rezervasyon durumlarını anlık olarak görün.
        </p>

        <div class="d-flex flex-column gap-3">
          <div class="d-flex align-items-center gap-3 bg-white bg-opacity-10 rounded-3 p-3">
            <i class="bi bi-bookmark-star-fill text-primary fs-5"></i>
            <div>
              <div class="small fw-semibold">Opsiyon Takibi</div>
              <div class="text-white-50" style="font-size:0.78rem">Aktif rezervasyonlarınızı görüntüleyin</div>
            </div>
          </div>
          <div class="d-flex align-items-center gap-3 bg-white bg-opacity-10 rounded-3 p-3">
            <i class="bi bi-bar-chart-line-fill text-success fs-5"></i>
            <div>
              <div class="small fw-semibold">Kişisel Dashboard</div>
              <div class="text-white-50" style="font-size:0.78rem">İstatistiklerinizi anlık takip edin</div>
            </div>
          </div>
          <div class="d-flex align-items-center gap-3 bg-white bg-opacity-10 rounded-3 p-3">
            <i class="bi bi-shield-check-fill text-warning fs-5"></i>
            <div>
              <div class="small fw-semibold">Güvenli Erişim</div>
              <div class="text-white-50" style="font-size:0.78rem">Şifreli oturum koruması</div>
            </div>
          </div>
        </div>
      </div>

      <div>
        <div class="bg-white bg-opacity-10 rounded-3 p-3 border border-white border-opacity-25">
          <div class="small text-white-50 mb-1">
            <i class="bi bi-info-circle me-1"></i>Demo Giriş Bilgileri
          </div>
          <div class="small font-monospace">
            <div><span class="text-white-50">Şifre: </span><span class="text-warning fw-bold">Demo1234!</span></div>
            <div class="mt-1 text-white-50">
              w.klorp@aio-demo.com<br>
              r.dunbar@aio-demo.com<br>
              j.sprock@aio-demo.com
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Sağ: Giriş Formu -->
    <div class="flex-grow-1 d-flex align-items-center justify-content-center bg-light p-4">
      <div style="width: 100%; max-width: 420px;">
        <!-- Mobile logo -->
        <div class="d-flex d-lg-none align-items-center mb-5">
          <div class="bg-primary rounded-2 p-2 me-2 text-white">
            <i class="bi bi-car-front-fill fs-4"></i>
          </div>
          <span class="fw-bold fs-5">AIO Demo</span>
        </div>

        <h3 class="fw-bold mb-1">Hoş Geldiniz</h3>
        <p class="text-muted mb-4">Servis danışmanı hesabınızla giriş yapın</p>

        <!-- Error -->
        <div v-if="errorMsg" class="alert alert-danger alert-dismissible d-flex align-items-center py-2 mb-4" role="alert">
          <i class="bi bi-exclamation-triangle-fill flex-shrink-0 me-2"></i>
          <div class="small">{{ errorMsg }}</div>
          <button type="button" class="btn-close btn-close-sm" @click="errorMsg = ''"></button>
        </div>

        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label class="form-label fw-semibold text-dark">E-posta</label>
            <div class="input-group">
              <span class="input-group-text bg-white"><i class="bi bi-envelope text-muted"></i></span>
              <input
                v-model="email"
                type="email"
                class="form-control form-control-lg border-start-0 ps-0"
                placeholder="ornek@aio-demo.com"
                required
                autofocus
                :disabled="loading"
              />
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label fw-semibold text-dark">Şifre</label>
            <div class="input-group">
              <span class="input-group-text bg-white"><i class="bi bi-lock text-muted"></i></span>
              <input
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                class="form-control form-control-lg border-start-0 border-end-0 ps-0"
                placeholder="••••••••"
                required
                :disabled="loading"
              />
              <button type="button" class="btn btn-outline-secondary border-start-0"
                @click="showPassword = !showPassword" tabindex="-1">
                <i :class="showPassword ? 'bi-eye-slash' : 'bi-eye'" class="bi"></i>
              </button>
            </div>
          </div>

          <button
            type="submit"
            class="btn btn-primary btn-lg w-100 fw-semibold"
            :disabled="loading"
          >
            <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
            <i v-else class="bi bi-box-arrow-in-right me-2"></i>
            {{ loading ? 'Giriş yapılıyor...' : 'Giriş Yap' }}
          </button>
        </form>

        <div class="mt-4 text-center">
          <router-link to="/" class="text-muted small text-decoration-none">
            <i class="bi bi-arrow-left me-1"></i>Ana sayfaya dön
          </router-link>
        </div>

        <!-- Mobile demo hint -->
        <div class="d-lg-none mt-4 p-3 bg-warning bg-opacity-10 border border-warning border-opacity-25 rounded-3">
          <div class="small text-warning fw-semibold mb-1">
            <i class="bi bi-info-circle me-1"></i>Demo Bilgileri
          </div>
          <div class="small text-muted">Şifre: <strong>Demo1234!</strong></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { serviceAdvisorApi } from '../api/serviceAdvisor'
import { useAuth } from '../composables/useAuth'

const router   = useRouter()
const { setAdvisor } = useAuth()

const email        = ref('')
const password     = ref('')
const showPassword = ref(false)
const loading      = ref(false)
const errorMsg     = ref('')

const handleLogin = async () => {
  loading.value  = true
  errorMsg.value = ''

  try {
    const advisor = await serviceAdvisorApi.login({
      email:    email.value.trim(),
      password: password.value
    })
    setAdvisor(advisor)
    router.push('/advisor/dashboard')
  } catch (e: any) {
    errorMsg.value =
      e.response?.data?.error ||
      e.message ||
      'Giriş yapılamadı. Lütfen bilgilerinizi kontrol edin.'
  } finally {
    loading.value = false
  }
}
</script>
