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
    },
    removeFolder (state, { folder }) {
      const folders = state.folders.filter(x => x.name !== folder)
      state.folders = folders
    },
    updateFolderName (state, { folder, newName }) {
      const folders = state.folders.map(x => {
        if (folder === x) {
          folder.name = newName
        }
        return x
      })
      state.folders = folders
    },
    createNewFolder (state) {
      const folders = state.folders.map(x => x).concat({ ...{ name: '', fileCount: 0, needSave: true } })
      state.folders = folders
    },
    saveFolder (state, { folder }) {
      const folders = state.folders.map(x => {
        if (x.needSave) {
          x.name = folder
          x.needSave = false
        }
        return x
      })
      state.folders = folders
    },
    sortFolders (state) {
      state.folders = state.folders.map(x => x).sort((a, b) => a.name.localeCompare(b.name))
    }
  },
  actions: {
    async fetchStorage ({ commit }) {
      const response = await fetch(STORAGE_URL)
      const folders = await response.json()
      commit('updateFolders', folders)
    },
    async fetchStorageFolder ({ dispatch, commit, getters }, folder) {
      if (!getters.folderByName(folder)) {
        await dispatch('fetchStorage')
      }

      const response = await fetch(`${STORAGE_URL}/${folder}`)
      const classifications = await response.json()
      commit('updateFolderClassifications', { folder, classifications })
    },
    async deleteStorageFolder ({ commit }, folder) {
      const response = await fetch(`${STORAGE_URL}/${folder}`, {
        method: 'DELETE'
      })

      if (response.ok) {
        commit('removeFolder', { folder })
        return true
      }
      return false
    },
    async changeFolderName ({ commit }, { folder, newName }) {
      const response = await fetch(`${STORAGE_URL}/${folder.name}/${newName}`, {
        method: 'PUT'
      })

      if (response.ok) {
        commit('updateFolderName', { folder, newName })
        commit('sortFolders')
        return true
      }
      return false
    },
    async createFolder ({ commit }, { folder }) {
      const response = await fetch(`${STORAGE_URL}/${folder}`, {
        method: 'POST'
      })

      if (response.ok) {
        commit('saveFolder', { folder })
        commit('sortFolders')
        return true
      }
      return false
    },
    async uploadImage ({ commit }, { folder, classification, file }) {
      const data = new FormData()
      data.append('file', file)
      data.append('classification', classification)

      const response = await fetch(`${STORAGE_URL}/upload/${folder}`, {
        method: 'POST',
        body: data
      })

      return response.ok
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
    },
    hasNewFolder (state) {
      return state.folders.some(x => !!x.needSave)
    }
  }
}

export default storage
