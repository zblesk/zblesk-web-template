<template>
    <div class="hello">
        <h1>{{ msg }}</h1>
        <img src="../assets/logo.png" />
        <p>
          Built by @zblesk. Based mostly on template by Alexandre Malavasi and Vue CLI. 
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
