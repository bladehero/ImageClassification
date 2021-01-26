import Vue from 'vue'
import Vuex from 'vuex'
import apiStorage from './modules/api.storage'

Vue.use(Vuex)

const store = new Vuex.Store({
  strict: true,
  state: {
  },
  mutations: {
  },
  actions: {
  },
  modules: {
    apiStorage
  }
})

export default store
