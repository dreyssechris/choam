import { useRegisterSW } from 'virtual:pwa-register/vue'
import { watch } from 'vue'
import { useQuasar } from 'quasar'

export function usePwaUpdate() {
  const $q = useQuasar()
  const { needRefresh, updateServiceWorker } = useRegisterSW()

  watch(needRefresh, (val) => {
    if (!val) return
    $q.notify({
      message: 'Eine neue Version ist verfügbar.',
      timeout: 0,
      actions: [
        {
          label: 'Aktualisieren',
          color: 'white',
          handler: () => updateServiceWorker(),
        },
        {
          label: 'Später',
          color: 'grey',
          handler: () => {
            needRefresh.value = false
          },
        },
      ],
    })
  })

  return { needRefresh, updateServiceWorker }
}
