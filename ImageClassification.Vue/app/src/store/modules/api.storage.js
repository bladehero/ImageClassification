const STORAGE_URL = `${process.env.VUE_APP_API_URL}/storage`

const storage = {
  state: {
    folders: []
  },
  mutations: {
    updateFolders (state, folders) {
      state.folders = folders
    },
    updateFolderClassifications (state, { folder, classifications }) {
      const folders = state.folders.map(x => x)
      const item = folders.find(x => x.name === folder)
      if (classifications) {
        Object.assign(item, { classifications: classifications })
        state.folders = folders
      }
    }
  },
  actions: {
    async fetchStorage ({ commit }) {
      commit('setLoading', true)

      const response = await fetch(STORAGE_URL)
      const folders = await response.json()
      commit('updateFolders', folders)

      commit('setLoading', false)
    },
    async fetchStorageFolder ({ dispatch, commit, getters }, folder) {
      commit('setLoading', true)

      if (!getters.folderByName(folder)) {
        await dispatch('fetchStorage')
      }

      const response = await fetch(`${STORAGE_URL}/${folder}`)
      const classifications = await response.json()
      commit('updateFolderClassifications', { folder, classifications })

      commit('setLoading', false)
    }
  },
  getters: {
    allFolders (state) {
      return state.folders
    },
    folderByName: state => name => {
      return state.folders.find(x => x.name === name)
    },
    classificationsByFolder: state => folder => {
      return state.folders.find(x => x.name === folder)?.classifications
    }
  }
}

export default storage