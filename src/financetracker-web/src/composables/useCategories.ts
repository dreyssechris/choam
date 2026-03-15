import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { getCategories, createCategory, updateCategory, deleteCategory } from '../api/categoryService'
import type { Category } from '../contracts/categories'

export function useCategories() {
  const queryClient = useQueryClient()

  const list = useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
  })

  const selectById = (id: number): Category | undefined => {
    const data = queryClient.getQueryData<Category[]>(['categories'])
    return data?.find((c) => c.id === id)
  }

  const create = useMutation({
    mutationFn: (name: string) => createCategory(name),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
    },
  })

  const update = useMutation({
    mutationFn: ({ id, name }: { id: number; name: string }) => updateCategory(id, name),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
    },
  })

  const remove = useMutation({
    mutationFn: (id: number) => deleteCategory(id),
    onMutate: async (id: number) => {
      await queryClient.cancelQueries({ queryKey: ['categories'] })
      const previous = queryClient.getQueryData<Category[]>(['categories'])
      queryClient.setQueryData<Category[]>(['categories'], (old) =>
        old ? old.filter((c) => c.id !== id) : old,
      )
      return { previous }
    },
    onError: (_err, _id, ctx) => {
      if (ctx?.previous) {
        queryClient.setQueryData(['categories'], ctx.previous)
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'], refetchType: 'active' })
    },
  })

  return { list, selectById, create, update, remove }
}
