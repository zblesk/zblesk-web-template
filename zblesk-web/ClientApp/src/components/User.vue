<template>
<section>
  <div v-if="isAuthenticated">
    <h1 class="title">{{ profile.name }}</h1>

    <div>Mail:
      <a v-if="profile.email" :href="'mailto:' + profile.email">{{ profile.email }}</a>
      <p v-else>{{$t('user.noneAvailable')}}</p>
    </div>

    <div class="languages" v-if="false">
      <h3 class="languages-title">{{$t('user.language')}}</h3>
      <language-selector v-if="isMe(profile.userId)"></language-selector>
    </div>
  </div>
  <div v-else>
    Log in, please.
  </div>
</section>
</template>

<script>
import { mapActions, mapGetters, mapState } from "vuex";
import LanguageSelector from './LanguageSelector.vue';
export default {
  name: 'User',
  components: { LanguageSelector },
  data() {
      return {
      }
  },
  computed: {
    ...mapGetters("context", ["myUserId", "isMod", "isMe", "isAuthenticated"]),
    ...mapState("context", ["profile"])
  },
  methods: {
    ...mapActions("users", ["getUser", "updateUser", "updateAvatar"]),
  },
  mounted() {
  }
}
</script>

<style scoped>

  .button-cancel {
    margin-top: var(--spacer);
  }

  @media screen and (min-width: 576px) {
    .button-cancel {
      margin-top: 0;
      margin-left: var(--spacer);
    }
  }

  .bio {
    margin-bottom: calc(var(--spacer) * 2);
  }

  .contacts {
    margin-bottom: calc(var(--spacer) * 2);
  }

  .mo {
    display: flex;
    align-items: center;
    padding: 0;
    margin-bottom: calc(var(--spacer) / 2);
  }

  .mo-pic {
    width: 26px;
    flex-shrink: 0;
  }

  .mo-pic img {
    width: 100%;
    vertical-align: bottom;
  }

  .mo-text {
    margin: 0 0 0 calc(var(--spacer) / 2);
  }

  .mo-text a {
    text-decoration: none;
    font-weight: bold;
  }

  @media screen and (min-width: 768px) {
    .contacts {
      display: flex;
      justify-content: space-around;
    }

    .mo {
      margin-bottom: 0;
    }

    .mo:not(:last-child) {
      margin-right: calc(var(--spacer) * 2);
    }
  }

  .buttons-not-edit {
    margin-bottom: calc(var(--spacer) * 2);
  }

  .button-password {
    margin-top: var(--spacer);
  }

  @media screen and (min-width: 576px) {
    .button-password {
      margin-top: 0;
      margin-left: var(--spacer);
    }
  }

  .reading {
    margin-bottom: calc(var(--spacer) * 2);
  }

  .reading-title {
    margin-bottom: var(--spacer);
  }

  .reading a {
    text-decoration: none;
  }

  .reading-list {
    margin-bottom: 0;
  }

  .reading-list li {
    line-height: 1.5;
    margin-bottom: var(--spacer);
  }

  .reading-list li:last-child {
    margin-bottom: 0;
  }

  .button-small {
    padding: 0.3em 0.6em;
  }

  .languages {
    margin-bottom: calc(var(--spacer) * 2);
  }

  .languages-title {
    margin-bottom: var(--spacer);
  }

  .nl-table {
    margin-bottom: var(--spacer);
  }

  .nl-row {
    margin-top: 0;
    margin-bottom: 0;
    border-top: 1px solid var(--c-accent);
  }

  .nl-row:last-child {
    border-bottom: 1px solid var(--c-accent);
  }

  .nl-current {
    text-align: center;
    padding: calc(var(--spacer) / 2);
    margin-bottom: 0;
  }

  .nl-button {
    text-align: center;
    padding: 0 calc(var(--spacer) / 2) calc(var(--spacer) / 2) calc(var(--spacer) / 2);
  }

  @media screen and (min-width: 576px) {
    .nl-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      max-width: 400px;
    }

    .nl-button {
      padding-top: calc(var(--spacer) / 2);
    }
  }

  .page-subtitle {
    font-size: 1.5rem;
    text-align: center;
    margin-top: calc(var(--spacer) * 2);
    margin-bottom: var(--spacer) * 2;
    padding: 0 var(--spacer);
  }

  .recs-message {
    max-width: 800px;
    background-color: var(--c-bckgr-primary);
    margin: var(--spacer);
    padding: calc(2* var(--spacer));
  }

  @media screen and (min-width: 840px) {
    .recs-message {
      margin: var(--spacer) auto;
    }
  }

  .page-subtitle--flex {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    align-items: center;
  }

  .recs-link {
    font-weight: normal;
    font-size: 0.8em;
    margin-left: calc(var(--spacer) / 2);
  }

  .user-pic {
    width: 100px;
    height: 100px;
    margin: 0 auto var(--spacer) auto;
    border: 0;
    border-radius: 50%;
    position: relative;
  }

  .user-pic img {
    width: 100%;
    border-radius: 50%;
  }

  .user-pic-placeholder {
    width: 100px;
    height: 100px;
    margin: 0 auto var(--spacer) auto;
    padding-top: 20px;
    border: 0;
    border-radius: 50%;
    background-color: var(--c-accent);
    position: relative;
  }

  .user-pic-placeholder img {
    max-width: 60%;
    display: block;
    margin: 0 auto;
  }

  @media screen and (min-width: 576px) {
    .user-pic-placeholder,
    .user-pic {
      float: right;
      margin: 0 0 var(--spacer) var(--spacer);
    }
  }

  .mod-text {
    color: var(--c-accent);
    font-variant: small-caps;
    font-size: 1.25rem;
    font-weight: bold;
    transform: rotate(40deg);
    position: absolute;
    top: -10px;
    right: -10px;
  }

  .mod-tooltip {
    visibility: hidden;
    position: absolute;
    top: 0px;
    right: -20px;
    z-index: 1;
    transform: rotate(-40deg);
    width: 12rem;

    color: white;
    background-color: var(--c-accent);
    font-variant: normal;
    font-size: 0.875rem;
    font-weight: normal;
    padding: 5px;
    border-radius: 10px;
  }

  .mod-text:hover .mod-tooltip {
    visibility: visible;
  }

  .toggler {
    width: 0.9em;
    margin-left: calc(var(--spacer) / 2);
    transition-property: transform;
    transition-duration: 0.2s;
  }

  .toggler--hidden {
    transform: rotate(-90deg);
  }

  .reviews-wrapper:empty {
    margin-bottom: var(--spacer);
  }

</style>
