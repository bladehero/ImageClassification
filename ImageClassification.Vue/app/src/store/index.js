import Vue from 'vue'
import Vuex from 'vuex'
import apiStorage from './modules/api.storage'

Vue.use(Vuex)

const store = new Vuex.Store({
  strict: true,
  state: {
    isLoading: false,
    modals: {
      actionModal: {
        isModalOpen: false,
        options: {}
      },
      alertModal: {
        isModalOpen: false,
        options: {}
      }
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
    setModal (state, { type, opts }) {
      let modal = null
      for (const key in state.modals) {
        if (Object.hasOwnProperty.call(state.modals, key) && key === type) {
          modal = state.modals[key]
        }
      }

      if (modal) {
        modal.isModalOpen = true
        modal.options = opts
      }
    },
    removeModal (state, type) {
      let modal = null
      for (const key in state.modals) {
        if (Object.hasOwnProperty.call(state.modals, key) && key === type) {
          modal = state.modals[key]
        }
      }

      if (modal) {
        modal.isModalOpen = false
      }
    }
  },
  actions: {
    openModal ({ commit }, { type, opts }) {
      commit('setModal', { type, opts })
    },
    closeModal ({ commit }, type) {
      commit('removeModal', type)
    }
  },
  modules: {
    apiStorage
  }
})

export default store
