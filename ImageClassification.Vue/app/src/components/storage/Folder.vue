<template>
  <v-hover v-slot="{ hover }">
    <div>
      <div @click="openFolder" @contextmenu="show" v-longpress="show">
        <v-icon :color="hover ? 'primary' : 'accent'" large>
          mdi-folder
        </v-icon>
        <v-text-field
          ref="folderChangeNameField"
          class="mt-0 pt-0 d-inline-block d-sm-block shrink"
          v-if="changeMode"
          label="Type folder name..."
          single-line
          :value="folder.name"
          append-icon="mdi-check"
          @click:append="changeName"
          @keypress="changeNameKeyPress"
          @blur="changeMode = false"
        />
        <span v-else class="ml-2 ml-sm-0 d-sm-block font-weight-light">
          {{ folder.name }}
        </span>
      </div>
      <v-menu
        v-model="showMenu"
        :position-x="x"
        :position-y="y"
        absolute
        offset-y
      >
        <v-list>
          <v-list-item v-for="(item, index) in items" :key="index" link>
            <v-list-item-title
              class="d-flex align-center"
              @click="action(folder, item.type)"
            >
              <v-icon :color="item.color" left>{{ item.icon }}</v-icon>
              <span class="overline">{{ item.title }}</span>
            </v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>
    </div>
  </v-hover>
</template>

<script>
import longpress from '@/plugins/vue-long-press'
import { mapActions } from 'vuex'

export default {
  data () {
    return {
      changeMode: false,
      showMenu: false,
      x: 0,
      y: 0,
      items: [
        {
          title: 'Delete',
          icon: 'mdi-delete-forever',
          color: 'error',
          type: 'delete'
        },
        {
          title: 'Rename',
          icon: 'mdi-pencil',
          color: 'success',
          type: 'rename'
        },
        {
          title: 'Cancel',
          icon: 'mdi-cancel',
          color: 'secondary'
        }
      ]
    }
  },
  props: {
    folder: {
      type: Object,
      required: true
    }
  },
  methods: {
    ...mapActions(['deleteStorageFolder', 'openModal', 'changeFolderName']),
    openFolder () {
      if (this.changeMode) {
        return
      }
      this.$router.push({
        name: 'classificationList',
        params: { name: this.folder.name }
      })
      this.$emit('opened', this.folder)
    },
    show (e) {
      if (this.showMenu) {
        return
      }

      e.preventDefault()
      this.showMenu = false
      this.x = e.clientX || e.touches[0].clientX
      this.y = e.clientY || e.touches[0].clientY
      this.$nextTick(() => {
        this.showMenu = true
      })
    },
    action (folder, type) {
      if (!folder || !type) {
        return
      }

      const vm = this

      if (type === 'delete') {
        this.openModal({
          type: 'actionModal',
          opts: {
            text: `Do you really want to delete <br/>'<b>${folder.name}</b>'?`,
            confirmAction: async () => {
              const success = await vm.deleteStorageFolder(folder.name)
              if (success) {
                this.openModal({
                  type: 'alertModal',
                  opts: {
                    text: `Successfully deleted '<b>${folder.name}</b>'!`,
                    icon: 'checkbox-marked-circle',
                    iconColor: 'success'
                  }
                })
              }
            },
            confirmText: 'Delete'
          }
        })
      }

      if (type === 'rename') {
        this.changeMode = true
        this.$nextTick(() => {
          const field = this.$refs.folderChangeNameField.$el.querySelector(
            'input'
          )
          field.focus()
          field.setSelectionRange(0, field.value.length)
        })
      }
    },
    changeName () {
      this.changeMode = false
      const field = this.$refs.folderChangeNameField.$el.querySelector('input')
      this.changeFolderName({ folder: this.folder, newName: field.value })
    },
    changeNameKeyPress (e) {
      if (e.keyCode === 13) {
        this.changeName()
      }
    }
  },
  directives: {
    longpress
  }
}
</script>

<style>
.v-text-field input {
  text-align: center;
}
</style>
