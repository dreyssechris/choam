import { ref } from 'vue'
import { streamChat, checkAvailability } from '../api/chatService'
import type { ChatMessage } from '../contracts/chat'

const messages = ref<ChatMessage[]>([])
const isStreaming = ref(false)
const isAvailable = ref<boolean | null>(null)

export function useChat() {

  async function checkAi() {
    isAvailable.value = await checkAvailability()
  }

  async function send(text: string) {
    const userMessage: ChatMessage = { role: 'user', content: text }
    messages.value.push(userMessage)

    const assistantMessage: ChatMessage = { role: 'assistant', content: '' }
    messages.value.push(assistantMessage)

    isStreaming.value = true

    try {
      const history = messages.value.slice(0, -2).map(m => ({
        role: m.role,
        content: m.content,
      }))

      const result = await streamChat(
        { message: text, history },
        (token) => {
          assistantMessage.content += token
        },
      )

      if (result.proposal) {
        assistantMessage.proposal = result.proposal
      }
    } catch (err) {
      assistantMessage.content = 'Error: could not reach AI service.'
    } finally {
      isStreaming.value = false
    }
  }

  function clearMessages() {
    messages.value = []
  }

  return { messages, isStreaming, isAvailable, send, checkAi, clearMessages }
}
