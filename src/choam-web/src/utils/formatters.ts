import type { TransactionType } from '../contracts/transactions'

export const eur = new Intl.NumberFormat('de-DE', {
  style: 'currency',
  currency: 'EUR',
})

export function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('de-DE')
}

export function toDateInputValue(iso: string): string {
  const d = new Date(iso)
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${y}-${m}-${day}`
}

const typeByIndex: Record<number, TransactionType> = {
  1: 'Income',
  2: 'Expense',
  3: 'Investment',
}

export function resolveTransactionType(type: number | string): TransactionType {
  if (typeof type === 'string') return type as TransactionType
  return typeByIndex[type] ?? 'Expense'
}
