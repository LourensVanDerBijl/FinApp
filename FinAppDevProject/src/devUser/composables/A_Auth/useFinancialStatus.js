// ─────────────────────────────────────────────────────────────────────────
// devUser/composables/A_Auth/useFinancialStatus.js
//
// Implements the "My Financial Status" (MF1) calculation rules exactly as
// specified: deterministic, no AI involved, five status tiers driven purely
// by percentage of available spending money remaining.
//
//   Rule 1: Available Spending Money = Income − Group Expenses − Personal Expenses
//   Rule 7: Actual spending reduces what's remaining
//   Rule 8: Status is based solely on % remaining — days-to-next-income is
//           separate, contextual information, not a factor in the tier.
// ─────────────────────────────────────────────────────────────────────────
import { computed, unref } from 'vue'

const STATUS_TIERS = [
  {
    key: 'excellent',
    min: 90,
    emoji: '🔵',
    label: 'Excellent',
    message: 'You are comfortably within your spending plan.'
  },
  {
    key: 'good',
    min: 50,
    emoji: '🟢',
    label: 'Good Standing',
    message: 'You are within your planned spending range.'
  },
  {
    key: 'watch',
    min: 15,
    emoji: '🟠',
    label: 'Watch Your Spending',
    message: 'Your available spending money is getting lower.'
  },
  {
    key: 'critical',
    min: 0,
    emoji: '🔴',
    label: 'Critical',
    message: 'Your available spending money is nearly depleted.'
  },
  {
    key: 'danger',
    min: -Infinity,
    emoji: '🔴',
    label: 'Danger',
    message: 'You have exceeded your planned spending money. Avoid non-essential spending.'
  }
]

/**
 * @param {import('vue').Ref<{expectedIncome:number, groupExpenses:number,
 *   personalExpenses:number, actualSpending:number, nextIncomeInDays:number}>} inputs
 */
export function useFinancialStatus(inputs) {
  // Rule 1 — available spending money, before actual spending is applied.
  const availableSpendingMoney = computed(() => {
    const i = unref(inputs)
    return i.expectedIncome - i.groupExpenses - i.personalExpenses
  })

  // Rule 7 — actual spending reduces what's left.
  const remainingSpendingMoney = computed(
    () => availableSpendingMoney.value - unref(inputs).actualSpending
  )

  const percentRemaining = computed(() => {
    if (availableSpendingMoney.value <= 0) return 0
    return (remainingSpendingMoney.value / availableSpendingMoney.value) * 100
  })

  // Rule 8 — tier is decided purely by % remaining, found by walking the
  // thresholds from highest to lowest and taking the first one we clear.
  const tier = computed(
    () =>
      STATUS_TIERS.find((t) => percentRemaining.value > t.min) ||
      STATUS_TIERS[STATUS_TIERS.length - 1]
  )

  const nextIncomeInDays = computed(() => unref(inputs).nextIncomeInDays)

  return {
    availableSpendingMoney,
    remainingSpendingMoney,
    percentRemaining,
    tier,
    nextIncomeInDays
  }
}
