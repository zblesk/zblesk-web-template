import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import axios from "axios";
import Toaster from '@meforma/vue-toaster';
import mitt from 'mitt';
import EventHub from './eventHub';
import i18n from './locales';

import './assets/sass/main.scss';

axios.interceptors.request.use(request => {
    if (store.state.context.jwtToken) {
        request.headers['Authorization'] = 'Bearer ' + store.state.context.jwtToken;
    }
    return request;
});

const app = createApp(App)
  .use(i18n)
  .use(router)
  .use(store)
  .use(Toaster)
  .use(EventHub)
  .mixin({
    created() {
      let opts = { position: "bottom-right" };
      this.$notifyError = (text) => this.$toast.error(text.toString(), opts);
      this.$notifySuccess = (text) => this.$toast.success(text.toString(), opts);
      this.$notifyNormal = (text) => this.$toast.show(text.toString(), opts);
      this.$notifyInfo = (text) => this.$toast.info(text.toString(), opts);
      this.$handleApiError = error => { 
        console.log(error);
        var msg = error?.response?.data;
        if (!msg)
        {
          msg = error;
        }
        this.$notifyError(msg);
      }
    }
  });
const emitter = mitt();
app.config.globalProperties.emitter = emitter;

store.subscribeAction({
  after: (action) => {
    // open hub connection on login or context restore
    if ((action.type == 'context/restoreContext'
      || action.type == 'context/login')
      && store.getters['context/isAuthenticated'])
    {
      store.dispatch("users/getUsers");
    }
  }
});

app.mount('#app');
