import { useAuth } from '../auth/useAuth'
import type { ChatRequest } from '../contracts/chat'
import type { TransactionCreate } from '../contracts/transactions'

const BASE = import.meta.env.VITE_API_BASE_URL ?? '/api'

export interface ChatStreamResult {
  message: string
  proposal: TransactionCreate | null
}

export async function streamChat(
  request: ChatRequest,
  onToken: (token: string) => void,
): Promise<ChatStreamResult> {
  const { getAccessToken } = useAuth()
  const token = getAccessToken()

  const res = await fetch(`${BASE}/chat`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: JSON.stringify(request),
  })

  if (!res.ok) {
    throw new Error(`Chat request failed: HTTP ${res.status}`)
  }

  const reader = res.body?.getReader()
  if (!reader) throw new Error('No response stream')

  const decoder = new TextDecoder()
  let fullMessage = ''
  let proposal: TransactionCreate | null = null
  let buffer = ''

  while (true) {
    const { done, value } = await reader.read()
    if (done) break

    buffer += decoder.decode(value, { stream: true })

    const lines = buffer.split('\n')
    buffer = lines.pop() ?? ''

    let eventType = ''
    for (const line of lines) {
      if (line.startsWith('event: ')) {
        eventType = line.slice(7).trim()
      } else if (line.startsWith('data: ')) {
        const data = JSON.parse(line.slice(6))

        if (eventType === 'token') {
          fullMessage += data.content
          onToken(data.content)
        } else if (eventType === 'done') {
          proposal = data.proposal ?? null
        }
      }
    }
  }

  return { message: fullMessage, proposal }
}

export async function checkAvailability(): Promise<boolean> {
  try {
    const res = await fetch(`${BASE}/chat/available`)
    if (!res.ok) return false
    return await res.json()
  } catch {
    return false
  }
}
