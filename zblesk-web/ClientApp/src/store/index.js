import { createStore } from 'vuex'
import context from './context'
import users from './users'

export default createStore({
    modules: {
        context,
        users
    }
})