import Vue from 'vue'
import Vuex from 'vuex'
import apiStorage from './modules/api.storage'

Vue.use(Vuex)

const store = new Vuex.Store({
  strict: true,
  state: {
    isLoading: false
  },
  getters: {
    getIsLoading (state) {
      return state.isLoading
    }
  },
  mutations: {
    setLoading (state, status) {
      if (status === undefined) {
        return
      }

      state.isLoading = status
    }
  },
  modules: {
    apiStorage
  }
})

export default store
