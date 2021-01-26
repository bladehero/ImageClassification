const STORAGE_URL = `${process.env.VUE_APP_API_URL}/storage`

const storage = {
  state: {
    folders: []
  },
  mutations: {
    updateFolders (state, folders) {
      state.folders = folders
    }
  },
  actions: {
    async fetchStorage ({ commit }) {
      const response = await fetch(STORAGE_URL)
      const content = await response.json()
      commit('updateFolders', content)
    }
  },
  getters: {
    allFolders (state) {
      return state.folders
    }
  }
}

export default storage
