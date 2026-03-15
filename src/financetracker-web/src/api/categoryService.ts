// src/api/categoryService.ts
import { api } from './http'; 
import type { Category } from '../contracts/categories';

// encapsulates HTTP calls related to categories -> Transport layer
// * builds the URL
// * puts together Header/Body
// * returns promise with typed response

// GetAll()
export function getCategories() {
    return api<Category[]>('/api/categories'); 
}

// GetById()
export function getCategoryById(id: number) {
    return api<Category>(`/api/categories/${id}`); 
}

// Create()
export function createCategory(category: string) {
    return api<Category>('/api/categories', {
        method: 'POST',
        body: JSON.stringify({ name: category }),
    });
}

// Update() -> void, because backend returns no content
export function updateCategory(id: number, name: string) {
  return api<void>(`/api/categories/${id}`, {
    method: "PUT",
    body: JSON.stringify({ name }),
  });
}

// Delete()
export function deleteCategory(id: number) {
  return api<void>(`/api/categories/${id}`, {
    method: "DELETE",
  });
}
