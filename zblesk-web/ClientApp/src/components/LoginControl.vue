<template>
  <div>
    <div v-if="!isAuthenticated" >
      <div v-if="!showLoginForm" >
        <button class="button is-primary" @click="showLoginForm = true">{{ $t('menus.login') }}</button>
      </div>
      <form @submit.prevent="onSubmit" @keyup.enter="onSubmit" @reset.prevent="onCancel" v-if="showLoginForm" class="login-form">
        <div class="field is-horizontal">
          <div class="field-label is-normal">
            <label class="label">{{ $t('menus.email') }}</label>
          </div>
          <div class="field-body">
            <div class="field">
                <input class="input" type="email" v-model="form.email" required :placeholder="$t('menus.email')">
            </div>
          </div>
          <div class="field-label is-normal">
            <label class="label">{{ $t('menus.password') }}</label>
          </div>
          <div class="field-body">
            <div class="field">
                <input class="input" type="password" v-model="form.password" required :placeholder="$t('menus.password')">
            </div>
          </div>
          <button class="button is-primary" @click="onLogin">{{ $t('menus.login-register') }}</button>
          <button class="button is-primary" @click="onRegister">{{ $t('menus.register') }}</button>
          <button class="button is-primary" type="reset">{{ $t('menus.cancel') }}</button>
        </div>
      </form>
    </div>
    <div v-if="isAuthenticated" class="field is-horizontal">
      <div class="field-label">
        <label class="label">
          <router-link :to="{ name: 'User' }">
            <div>{{ profile.name }}</div>
          </router-link>
        </label>
      </div>
      <div class="field-body">
        <div class="field">
          <button class="button is-primary" @click="onLogout">{{ $t('menus.logout') }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapActions, mapGetters, mapState } from "vuex";

export default {
  name: "LoginControl",
  data() {
    return {
      form: {
        email: "",
        password: "",
      },
      showLoginForm: false,
    };
  },
  computed: {
    ...mapState("context", ["profile"]),
    ...mapGetters("context", ["isAuthenticated"])
  },
  methods: {
    ...mapActions("context", ["login", "logout", "register"]),
    onLogin() 
    {
      this.login(this.form)
        .then(() => {
          this.$notifySuccess(this.$t('messages.welcome'));
          this.showLoginForm = false;
        }
      )
      .catch((err) => {
          this.$notifyError(this.$t('messages.loginFailed'));
          console.log(err);
        }
      );
    },
    onRegister()
    {
      this.register(this.form)
        .then(() => {
          this.onLogin();
        })
        .catch((err) => {
            this.$notifyError(this.$t('messages.loginFailed'));
            console.log(err);
          }
        );
    },
    onCancel() 
    {
      this.form = {};
      this.showLoginForm = false;
    },
    onLogout()
    {
      this.logout();
      this.$router.push({ name: "Home" });
      this.showLoginForm = false;
    }
  },
};
</script>