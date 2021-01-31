<template>
  <v-app :style="themeColors">
    <TopBar />
    <Main />
    <BottomBar />
  </v-app>
</template>

<script>
import TopBar from '@/components/layouts/TopBar'
import Main from '@/components/layouts/Main'
import BottomBar from '@/components/layouts/BottomBar'

export default {
  name: 'App',

  components: {
    TopBar,
    Main,
    BottomBar
  },

  computed: {
    isDarkTheme () {
      return this.$vuetify.theme.isDark
    },
    currentTheme () {
      if (this.isDarkTheme) {
        return this.$vuetify.theme.themes.dark
      }
      return this.$vuetify.theme.themes.light
    },
    themeColors () {
      const colors = {}
      for (const color in this.currentTheme) {
        const key = `--${color}-color`
        colors[key] = this.currentTheme[color]
      }
      return colors
    }
  }
}
</script>

<style>
html,
main {
  overflow: hidden;
  height: 100%;
  width: 100%;
}

/* Width */
::-webkit-scrollbar {
  width: 6px;
}

/* Track */
::-webkit-scrollbar-track {
  background: #f0f0f0;
  border-radius: 3px;
}

/* Handle */
::-webkit-scrollbar-thumb {
  border-radius: 3px;
  background: var(--primary-color);
}
</style>
