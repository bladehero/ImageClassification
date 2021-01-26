import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'storage',
    component: Home
  },
  {
    path: '/classification',
    name: 'classification',
    component: () => import('@/views/Classification')
  },
  {
    path: '/classifiers',
    name: 'classifiers',
    component: () => import('@/views/Classifiers')
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
