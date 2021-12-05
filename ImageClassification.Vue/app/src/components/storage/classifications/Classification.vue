<template>
  <div>
    <v-hover v-slot="{ hover }">
      <div @contextmenu="show" v-longpress="show">
        <v-icon :color="hover ? 'primary' : 'accent'" large>
          mdi-tag-text-outline
        </v-icon>
        <div class="ml-2 ml-sm-0 d-sm-block font-weight-light">
          {{ value.classification }}
          <br/>
          <span class="caption">{{ value.fileCount }} pcs.</span>
        </div>
      </div>
    </v-hover>
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
            @click="action(value.classification, item.type)"
          >
            <v-icon :color="item.color" left>{{ item.icon }}</v-icon>
            <span class="overline">{{ item.title }}</span>
          </v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>
  </div>
</template>

<script>
import longpress from '@/plugins/vue-long-press'
import { mapActions } from 'vuex'

export default {
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
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
          title: 'Cancel',
          icon: 'mdi-cancel',
          color: 'secondary'
        }
      ]
    }
  },
  methods: {
    ...mapActions(['deleteStorageFolderClassification', 'fetchStorageFolder', 'uploadImage', 'openModal']),
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
    action (classification, type) {
      if (!classification || !type) {
        return
      }

      const vm = this

      if (type === 'delete') {
        this.openModal({
          type: 'actionModal',
          opts: {
            text: `Do you really want to delete <br/>'<b>${classification}</b>'?`,
            confirmAction: async () => {
              const success = await vm.deleteStorageFolderClassification({ folder: vm.value.folder, classification })
              if (success) {
                this.openModal({
                  type: 'alertModal',
                  opts: {
                    text: `Successfully deleted '<b>${classification}</b>'!`,
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
        this.setFocus()
      }
    }
  },
  directives: {
    longpress
  }
}
</script>

<style>
</style>
