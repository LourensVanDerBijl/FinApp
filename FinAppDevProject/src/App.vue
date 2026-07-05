<template>
  <!-- Router outlet fills the whole page -->
  <router-view />
</template>

<style>
/* Ensure App.vue itself takes full height and width */
html, body, #app {
  height: 100%;
  width: 100%;
  margin: 0;
  padding: 0;
}
</style>
