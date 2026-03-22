// src/api/transactionService.ts
import { api } from './http';
import type {Transaction, TransactionCreate, TransactionUpdate} from '../contracts/transactions'; 

// GetAll()
export function getTransactions() {
    return api<Transaction[]>('/transactions');
}

// GetById()
export function getTransactionById(id: number) {
    return api<Transaction>(`/transactions/${id}`);
}

// Create()
export function createTransaction(transaction: TransactionCreate) {
    return api<Transaction>('/transactions', {
        method: 'POST',
        body: JSON.stringify(transaction),
    });
}

// Update() -> void, because backend returns no content
export function updateTransaction(id: number, transaction: TransactionUpdate) {
    return api<void>(`/transactions/${id}`, {
        method: "PUT",
        body: JSON.stringify(transaction),
    });
}

// Delete()
export function deleteTransaction(id: number) {
    return api<void>(`/transactions/${id}`, {
        method: "DELETE",
    });
}