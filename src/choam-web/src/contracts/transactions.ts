/** Enum as string union - matches backend JSON */
export type TransactionType = "Income" | "Expense" | "Investment";

/** Transaction returned by the backend */
export interface Transaction {
    id: number;
    title: string;
    amount: number; 
    description?: string;
    date: string;
    type: TransactionType;
    categoryId: number;
}

/** Create DTO */
export interface TransactionCreate {
    title: string;
    amount: number; 
    description?: string;
    date: string;
    type: TransactionType;
    categoryId: number;
}

/** Update DTO */
export interface TransactionUpdate {
    title?: string;
    amount?: number; 
    description?: string;
    date?: string;
    type?: TransactionType;
    categoryId?: number;
}