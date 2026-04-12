import type { TransactionCreate } from './transactions'

export interface ChatMessage {
  role: 'user' | 'assistant'
  content: string
  proposal?: TransactionCreate | null
}

export interface ChatRequest {
  message: string
  history?: { role: string; content: string }[]
}
