<template>
  <div class="hero-copy">
    <p class="eyebrow">LET'S GET YOU STARTED</p>
    <h1>Create or Join a <span class="highlight">Group</span></h1>
    <p class="subtitle">You're almost there! Choose how you want to use FinBine.</p>
    <span class="hero-divider" aria-hidden="true"></span>
  </div>
</template>

<style scoped>
.hero-copy {
  text-align: center;
  flex-shrink: 0;
  margin-bottom: 10px;
}

.eyebrow {
  font-size: 0.62rem;
  font-weight: 700;
  letter-spacing: 0.12em;
  color: #1855b9;
  margin: 0 0 6px;
}

.hero-copy h1 {
  font-size: 1.4rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 6px;
}

.highlight {
  color: #1855b9;
}

.hero-copy .subtitle {
  font-size: 0.78rem;
  color: #64748b;
  margin: 0 0 10px;
}

.hero-divider {
  display: inline-block;
  width: 70px;
  height: 2px;
  border-radius: 2px;
  background: linear-gradient(90deg, #2dd4bf, #60a5fa);
}

@media (max-width: 960px) {
  .hero-copy h1 {
    font-size: 1.4rem;
  }
}
</style>
