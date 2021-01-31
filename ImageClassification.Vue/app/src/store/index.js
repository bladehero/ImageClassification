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
        mainHeight: 24,
        dividerHeight: 20,
        breadCrumbsHeight: 32,
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
    getFullBarHeight (state) {
      let sum = 0
      for (const key in state.internal.barSettings) {
        if (Object.hasOwnProperty.call(state.internal.barSettings, key)) {
          const element = state.internal.barSettings[key]
          sum += element
        }
      }
      return sum
    }
  },
  mutations: {
    setLoading (state, status) {
      if (status === undefined) {
        return
      }
      state.isLoading = status
    },
    setBarSettings (state, barSettings) {
      for (const key in state.internal.barSettings) {
        if (Object.hasOwnProperty.call(state.internal.barSettings, key) &&
         Object.hasOwnProperty.call(barSettings, key)) {
          state.internal.barSettings[key] = barSettings[key]
        }
      }
      state.internal.barSettings = { ...state.internal.barSettings }
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
