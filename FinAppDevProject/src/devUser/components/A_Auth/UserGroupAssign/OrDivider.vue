<template>
  <div class="or-divider" aria-hidden="true">
    <span class="or-line"></span>
    <span class="or-badge">OR</span>
    <span class="or-line"></span>
  </div>
</template>

<style scoped>
.or-divider {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
}

.or-line {
  flex: 1;
  width: 1px;
  background: linear-gradient(to bottom, rgba(45, 212, 191, 0.5), rgba(37, 99, 235, 0.5));
}

.or-badge {
  flex-shrink: 0;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: 1px solid #dbeafe;
  background: #fff;
  color: #1855b9;
  font-size: 0.62rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 0 14px rgba(37, 99, 235, 0.1);
}

@media (max-width: 960px) {
  .or-divider {
    display: none;
  }
}
</style>
