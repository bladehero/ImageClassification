const STORAGE_URL = `${process.env.VUE_APP_API_URL}/storage`
const CLASSIFIERS_URL = `${process.env.VUE_APP_API_URL}/classifiers`

const storage = {
  state: {
    folders: [],
    classifiers: []
  },
  mutations: {
    updateFolders (state, folders) {
      state.folders = folders
    },
    updateClassifiers (state, classifiers) {
      state.classifiers = classifiers
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
      state.folders = state.folders.filter(x => x.name !== folder)
    },
    removeClassification (state, { folder, classification }) {
      const folders = state.folders.map(x => x)
      const toUpdate = folders.find(x => x.name === folder)
      if (toUpdate) {
        toUpdate.classifications = toUpdate.classifications.filter(x => x.classification !== classification)
      }
      state.folders = folders
    },
    updateFolderName (state, { folder, newName }) {
      state.folders = state.folders.map(x => {
        if (folder === x) {
          folder.name = newName
        }
        return x
      })
    },
    createNewFolder (state) {
      state.folders = state.folders.map(x => x).concat({ ...{ name: '', fileCount: 0, needSave: true } })
    },
    saveFolder (state, { folder }) {
      state.folders = state.folders.map(x => {
        if (x.needSave) {
          x.name = folder
          x.needSave = false
        }
        return x
      })
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
    async fetchClassifiers ({ commit }) {
      const response = await fetch(CLASSIFIERS_URL)
      const classifiers = await response.json()
      commit('updateClassifiers', classifiers.map(x => ({ name: x })))
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
    async deleteStorageFolderClassification ({ commit }, { folder, classification }) {
      const response = await fetch(`${STORAGE_URL}/${folder}/${classification}`, {
        method: 'DELETE'
      })

      if (response.ok) {
        commit('removeClassification', { folder, classification })
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
    async uploadImage ({ commit }, { folder, classification, files }) {
      const data = new FormData()
      data.append('classification', classification)
      for (const file of files) {
        data.append('files', file)
      }

      const response = await fetch(`${STORAGE_URL}/upload/${folder}`, {
        method: 'POST',
        body: data
      })

      return response.ok
    },
    async prepareClassifier ({ commit }, { classifier, folder }) {
      const response = await fetch(`${CLASSIFIERS_URL}/train/${classifier}?imageFolder=${folder}`, {
        method: 'PATCH'
      })

      return response.ok
    }
  },
  getters: {
    allFolders (state) {
      return state.folders
    },
    allClassifiers (state) {
      return state.classifiers
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
