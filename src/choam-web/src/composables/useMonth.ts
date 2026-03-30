import { ref } from 'vue'
import type { Transaction } from '../contracts/transactions'

export function useMonth(initialYear: number, initialMonth: number) {
  const year = ref(initialYear)
  const month = ref(initialMonth)

  const previousMonth = () => {
    if (month.value === 0) {
      year.value--
      month.value = 11
    } else {
      month.value--
    }
  }

  const nextMonth = () => {
    if (month.value === 11) {
      year.value++
      month.value = 0
    } else {
      month.value++
    }
  }

  return { year, month, previousMonth, nextMonth }
}

export function filterTransactionByMonth(
  transactions: Transaction[],
  year: number,
  month: number,
): Transaction[] {
  const result: Transaction[] = []
  for (const tx of transactions) {
    const date = new Date(tx.date)
    if (date.getFullYear() === year && date.getMonth() === month) {
      result.push(tx)
    }
  }
  return result
}

export function totalsByType(transactions: Transaction[]) {
  let income = 0, expense = 0, investment = 0, netCashFlow = 0
  for (const tx of transactions) {
    switch (tx.type) {
      case 'Income': income += tx.amount; break
      case 'Expense': expense += tx.amount; break
      case 'Investment': investment += tx.amount; break
    }
  }
  netCashFlow = income - expense - investment
  return { income, expense, investment, netCashFlow }
}
