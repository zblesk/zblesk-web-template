<template>
<nav class="navbar" role="navigation" aria-label="main navigation">
  <div class="navbar-brand">
    <a class="navbar-item">
      <img src="../assets/logo.png" />
    </a>
    <a role="button" class="navbar-burger" :class="{ 'is-active': isActive }" aria-label="menu" aria-expanded="false" data-target="navbar" 
      @click="burgerClick">
      <span aria-hidden="true"></span>
      <span aria-hidden="true"></span>
      <span aria-hidden="true"></span>
    </a>
  </div>
      
  <div id="navbar" class="navbar-menu" :class="{ 'is-active': isActive }">
    <div class="navbar-start">
      <a class="navbar-item">
        <router-link :to="{ name: 'Home' }">Home</router-link>
      </a>
      <a class="navbar-item">
        <router-link :to="{ name: 'Counter' }">Counter</router-link>
      </a>
      <a class="navbar-item">
        <router-link :to="{ name: 'FetchData' }">Fetch Data</router-link>
      </a>
      <a class="navbar-item">
        <router-link :to="{ name: 'User' }">Like, profile stuff</router-link>
      </a>
    </div>

    <div class="navbar-end">
      <div class="navbar-item">
          <login-control></login-control>
      </div>
    </div>
  </div>
</nav>
</template>

<script>
import LoginControl from "./LoginControl.vue";
export default {
  name: "NavMenu",
  components: { LoginControl },
  data() {
    return {
      isActive: false
    }
  },
  methods: {
    burgerClick() {
      this.isActive = !this.isActive;
    }
  }
};
</script>
