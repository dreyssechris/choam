import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import {
  getTransactions,
  getTransactionById,
  createTransaction,
  updateTransaction,
  deleteTransaction,
} from '../api/transactionService'
import type { Transaction, TransactionCreate, TransactionUpdate } from '../contracts/transactions'

export function useTransactions() {
  const queryClient = useQueryClient()

  const list = useQuery({
    queryKey: ['transactions'],
    queryFn: getTransactions,
  })

  const byId = (id: number) =>
    useQuery({
      queryKey: ['transactions', id],
      queryFn: () => getTransactionById(id),
      enabled: id > 0,
    })

  const create = useMutation({
    mutationFn: (payload: TransactionCreate) => createTransaction(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
    },
  })

  const update = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: TransactionUpdate }) =>
      updateTransaction(id, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] })
    },
  })

  const remove = useMutation({
    mutationFn: (id: number) => deleteTransaction(id),
    onMutate: async (id: number) => {
      await queryClient.cancelQueries({ queryKey: ['transactions'] })
      const previous = queryClient.getQueryData<Transaction[]>(['transactions'])
      queryClient.setQueryData<Transaction[]>(['transactions'], (old) =>
        old ? old.filter((t) => t.id !== id) : old,
      )
      return { previous }
    },
    onError: (_err, _id, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['transactions'], ctx.previous)
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['transactions'], refetchType: 'active' })
    },
  })

  return { list, byId, create, update, remove }
}
