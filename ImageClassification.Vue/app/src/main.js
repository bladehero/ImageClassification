import Vue from 'vue'
import App from './App.vue'
import VuePageTransition from './plugins/vue-page-transition'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'

Vue.config.productionTip = false

new Vue({
  router,
  store,
  vuetify,

  components: {
    VuePageTransition
  },

  render: h => h(App)
}).$mount('#app')
