<script setup>
import PublicNavbar from '../../components/B_Auth/PublicNavbar.vue'
import PublicFooter from '../../components/B_Auth/PublicFooter.vue'
</script>

<template>
  <div class="privacy-page">
    <PublicNavbar />
    <div class="content">
      <h1>Privacy Policy</h1>
      <p>Privacy policy content goes here...</p>
    </div>
    <PublicFooter />
  </div>
</template>

<style scoped>
.privacy-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.content {
  flex: 1;
  padding: 40px 20px;
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  color: #02b0a1;
  margin-bottom: 20px;
}
</style>
