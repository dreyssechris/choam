<template>
  <q-page padding>
    <div class="q-mx-auto chat-page">
      <header class="chat-header">
        <div>
          <p class="chat-header__subtitle font-label">CHOAM Finance</p>
          <h1 class="chat-header__title font-headline">Chat</h1>
        </div>
        <div class="chat-header__actions">
          <span
            class="ai-status font-label"
            :class="isAvailable ? 'ai-status--online' : 'ai-status--offline'"
          >
            {{ isAvailable === null ? 'Checking AI...' : isAvailable ? 'AI Online' : 'AI Offline' }}
          </span>
          <button class="btn-ghost" @click="clearMessages" :disabled="messages.length === 0">
            <q-icon name="sym_o_delete" size="16px" />
            Clear
          </button>
        </div>
      </header>

      <!-- Messages -->
      <div class="chat-messages" ref="chatContainer">
        <div v-if="messages.length === 0" class="chat-empty">
          <p class="chat-empty__text font-label">
            Describe a transaction to get started
          </p>
          <p class="chat-empty__hint">
            e.g. "25 Euro bei Rewe für Lebensmittel"
          </p>
        </div>

        <div
          v-for="(msg, i) in messages"
          :key="i"
          :class="['chat-bubble', `chat-bubble--${msg.role}`]"
        >
          <p class="chat-bubble__role font-label">
            {{ msg.role === 'user' ? 'You' : 'CHOAM AI' }}
          </p>
          <p class="chat-bubble__content">{{ msg.content }}</p>
          <span v-if="msg.role === 'assistant' && isStreaming && i === messages.length - 1" class="chat-cursor" />

          <ProposalCard
            v-if="msg.proposal"
            :proposal="msg.proposal"
            :categories="categoryList.data.value ?? []"
            :disabled="isConfirming"
            @confirm="confirmProposal"
            @edit="openEdit(msg)"
            @discard="msg.proposal = null"
          />
        </div>
      </div>

      <!-- Edit Dialog -->
      <q-dialog v-model="editDialogOpen">
        <q-card dark class="dialog-card">
          <q-card-section class="row items-center q-pb-none">
            <div class="text-h6 font-headline">Edit Proposal</div>
            <q-space />
            <q-btn flat round dense icon="close" @click="editDialogOpen = false" />
          </q-card-section>
          <q-separator />
          <q-card-section>
            <TransactionForm
              v-if="editingProposal"
              :default-values="editingProposal"
              :submitting="isConfirming"
              submit-label="Create"
              @submit="handleEditSubmit"
              @cancel="editDialogOpen = false"
            />
          </q-card-section>
        </q-card>
      </q-dialog>

      <!-- Input -->
      <div class="chat-input-bar">
        <input
          v-model="input"
          class="chat-input"
          type="text"
          placeholder="Describe a transaction..."
          :disabled="isStreaming"
          @keydown.enter="handleSend"
        />
        <button
          class="btn-primary chat-send"
          :disabled="!input.trim() || isStreaming"
          @click="handleSend"
        >
          <q-icon name="sym_o_send" size="18px" />
        </button>
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick, watch } from 'vue'
import { useChat } from '../composables/useChat'
import { useTransactions } from '../composables/useTransactions'
import { useCategories } from '../composables/useCategories'
import { toDateInputValue } from '../utils/formatters'
import ProposalCard from '../components/ProposalCard.vue'
import TransactionForm from '../components/TransactionForm.vue'
import type { TransactionCreate } from '../contracts/transactions'
import type { ChatMessage } from '../contracts/chat'
import '../styles/pages/_chat-page.scss'

const { messages, isStreaming, isAvailable, send, checkAi, clearMessages } = useChat()
const { create } = useTransactions()
const { list: categoryList } = useCategories()

const input = ref('')
const chatContainer = ref<HTMLElement>()
const isConfirming = ref(false)
const editDialogOpen = ref(false)
const editingProposal = ref<Partial<TransactionCreate>>()
const editingMessage = ref<ChatMessage>()

async function handleSend() {
  const text = input.value.trim()
  if (!text || isStreaming.value) return
  input.value = ''
  await send(text)
}

async function confirmProposal(proposal: TransactionCreate) {
  isConfirming.value = true
  try {
    await create.mutateAsync(proposal)
    messages.value.push({
      role: 'assistant',
      content: `Transaction "${proposal.title}" created successfully.`,
    })
  } catch {
    messages.value.push({
      role: 'assistant',
      content: 'Failed to create transaction.',
    })
  } finally {
    isConfirming.value = false
  }
}

function openEdit(msg: ChatMessage) {
  if (!msg.proposal) return
  editingMessage.value = msg
  editingProposal.value = {
    ...msg.proposal,
    date: toDateInputValue(msg.proposal.date),
  }
  editDialogOpen.value = true
}

async function handleEditSubmit(values: TransactionCreate) {
  isConfirming.value = true
  try {
    await create.mutateAsync(values)
    if (editingMessage.value) {
      editingMessage.value.proposal = null
    }
    messages.value.push({
      role: 'assistant',
      content: `Transaction "${values.title}" created successfully.`,
    })
    editDialogOpen.value = false
  } catch {
    messages.value.push({
      role: 'assistant',
      content: 'Failed to create transaction.',
    })
  } finally {
    isConfirming.value = false
  }
}

function scrollToBottom() {
  nextTick(() => {
    if (chatContainer.value) {
      chatContainer.value.scrollTop = chatContainer.value.scrollHeight
    }
  })
}

watch(() => messages.value.length, scrollToBottom)
watch(
  () => messages.value[messages.value.length - 1]?.content,
  scrollToBottom,
)

onMounted(checkAi)
</script>
