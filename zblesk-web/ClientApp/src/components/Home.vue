<template>
    <div class="hello">
        <h1>{{ msg }}</h1>
        <img src="../assets/logo.png" />
        <p>
          Built by <a href="https://zblesk.net/blog/">@zblesk</a>. Based in big part on a template by Alexandre Malavasi and Vue CLI. Source <a href="https://github.com/zblesk/zblesk-web-template">here</a>.
        </p>        
    </div>
</template>

<script>
export default {
  name: 'Home',
  props: {
    msg: String
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
