<script setup>
import PublicNavbar from '../../components/B_Auth/PublicNavbar.vue'
import PublicFooter from '../../components/B_Auth/PublicFooter.vue'
</script>

<template>
  <div class="web-home">
    <!-- Navbar -->
    <PublicNavbar />

    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-content">
        <h1>Combine Group Expense Clarity</h1>
        <p>Manage shared financial responsibilities while maintaining your own independence.</p>
        <div class="hero-cta">
          <RouterLink to="/product" class="btn get-started">Get Started</RouterLink>
          <RouterLink to="/user/login" class="btn sign-in">Sign In</RouterLink>
        </div>
      </div>
      <div class="hero-visual">
        <!-- Placeholder for dashboard/product visual -->
        <div class="dashboard-preview">[ Dashboard Preview ]</div>
      </div>
    </section>

    <!-- Group Finance Problem -->
    <section class="problem">
      <h2>The Group Finance Problem</h2>
      <p>Managing shared expenses is difficult. Groups need visibility without losing individual independence.</p>
    </section>

    <!-- FinBine Solution -->
    <section class="solution">
      <h2>The FinBine Solution</h2>
      <p>FinBine provides clarity for groups while ensuring each member maintains financial independence.</p>
    </section>

    <!-- Core Capabilities -->
    <section class="capabilities">
      <h2>Core Capabilities</h2>
      <div class="cards">
        <div class="card">Group Management</div>
        <div class="card">Expense Visibility</div>
        <div class="card">Financial Overview</div>
        <div class="card">Shared Planning</div>
        <div class="card">Documents & Records</div>
        <div class="card">Controlled Access</div>
      </div>
    </section>

    <!-- Security -->
    <section class="security">
      <h2>Security</h2>
      <p>Your financial information is sensitive. FinBine takes security seriously.</p>
    </section>

    <!-- Final CTA -->
    <section class="final-cta">
      <h2>Ready to get started?</h2>
      <RouterLink to="/product" class="btn get-started">Create a FinBine Group</RouterLink>
    </section>

    <!-- Footer -->
    <PublicFooter />
  </div>
</template>

<style scoped>
.web-home {
  display: flex;
  flex-direction: column;
}

/* Hero */
.hero {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  background-color: #04111f;
  color: #fff;
  padding: 60px 20px;
}

.hero-content {
  flex: 1;
  min-width: 280px;
}

.hero-content h1 {
  font-size: 2rem;
  margin-bottom: 12px;
}

.hero-content p {
  font-size: 1rem;
  margin-bottom: 20px;
  color: #94a3b8;
}

.hero-cta {
  display: flex;
  gap: 12px;
}

.hero-visual {
  flex: 1;
  min-width: 280px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.dashboard-preview {
  background: #052b7f49;
  color: #fff;
  padding: 40px;
  border-radius: 8px;
  text-align: center;
}

/* Sections */
section {
  padding: 60px 20px;
  text-align: center;
}

section h2 {
  font-size: 1.5rem;
  margin-bottom: 16px;
}

section p {
  font-size: 1rem;
  color: #333;
  max-width: 600px;
  margin: 0 auto;
}

/* Capabilities */
.capabilities .cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: 16px;
  margin-top: 20px;
}

.capabilities .card {
  background: #f8fafc;
  padding: 20px;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 500;
  color: #04111f;
}

/* CTA Buttons */
.btn {
  padding: 10px 18px;
  border-radius: 4px;
  font-size: 0.85rem;
  text-decoration: none;
  transition: background 0.2s;
}

.sign-in {
  background: none;
  border: 1px solid #02b0a1;
  color: #02b0a1;
}

.sign-in:hover {
  background: #02b0a1;
  color: #fff;
}

.get-started {
  background: #02b0a1;
  color: #fff;
}

.get-started:hover {
  background: #028c81;
}

/* Responsive */
@media (max-width: 768px) {
  .hero {
    flex-direction: column;
    text-align: center;
  }
  .hero-cta {
    justify-content: center;
  }
}
</style>