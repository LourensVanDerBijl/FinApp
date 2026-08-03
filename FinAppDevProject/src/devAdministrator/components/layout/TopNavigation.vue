<template>
  <header class="top-nav">
    <div class="left">
      <span class="menu-icon">☰</span>
      <h1>Dashboard</h1>
    </div>
    <div class="right">
      <span class="env-badge">Development Environment</span>
      <span class="version">v0.0.1</span>
    </div>
  </header>
</template>

<style scoped>
.top-nav {
  background: #fff;
  padding: 12px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #e5e7eb;
}
.left {
  display: flex;
  align-items: center;
  gap: 12px;
}
.right {
  display: flex;
  align-items: center;
  gap: 16px;
}
.env-badge {
  background: #2563eb;
  color: #fff;
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 14px;
}
.version {
  font-size: 14px;
  color: #6b7280;
}
</style>
