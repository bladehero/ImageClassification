import Vue from 'vue'
import Vuex from 'vuex'
import apiStorage from './modules/api.storage'

Vue.use(Vuex)

const store = new Vuex.Store({
  strict: true,
  state: {
    isLoading: false,
    modal: {
      isModalOpen: false,
      options: {}
    },
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
    },
    isModalOpen (state) {
      return state.modal.isModalOpen
    },
    modalOptions (state) {
      return state.modal.options
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
    },
    setModal (state, opts) {
      state.modal.isModalOpen = true
      state.modal.options = opts
    },
    removeModal (state) {
      state.modal.isModalOpen = false
      state.modal.options = {}
    }
  },
  actions: {
    openModal ({ commit }, opts) {
      commit('setModal', opts)
    },
    closeModal ({ commit }) {
      commit('removeModal')
    }
  },
  modules: {
    apiStorage
  }
})

export default store
