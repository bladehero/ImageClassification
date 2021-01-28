import Vue from 'vue'
import Vuex from 'vuex'
import apiStorage from './modules/api.storage'

Vue.use(Vuex)

const store = new Vuex.Store({
  strict: true,
  state: {
    isLoading: false,
    internal: {
      barSettings: {
        topBarHeight: 56,
        bottomBarHeight: 56
      }
    }
  },
  getters: {
    getIsLoading (state) {
      return state.isLoading
    },
    getBarSettings (state) {
      return state.internal.barSettings
    },
    getTheme (state) {
      return state.internal.themes
    }
  },
  mutations: {
    setLoading (state, status) {
      if (status === undefined) {
        return
      }
      state.isLoading = status
    },
    setBarSettings (state, { topBarHeight, bottomBarHeight }) {
      if (topBarHeight) {
        state.internal.barSettings.topBarHeight = topBarHeight
      }
      if (bottomBarHeight) {
        state.internal.barSettings.bottomBarHeight = bottomBarHeight
      }
    }
  },
  modules: {
    apiStorage
  }
})

export default store
