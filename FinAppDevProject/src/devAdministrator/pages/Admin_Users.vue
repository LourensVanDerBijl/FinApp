<script setup>
import { ref } from 'vue'
import { adminOrgChart, addOrgChartMember, terminateOrgMember, removeOrgMember } from '../data/mockData.js'
import OrgChartNode from '../components/orgChart/OrgChartNode.vue'
import AddAdminModal from '../components/orgChart/AddAdminModal.vue'

const modalOpen = ref(false)
const modalParent = ref(null)

function openAddModal(parentNode) {
  modalParent.value = parentNode
  modalOpen.value = true
}

function closeModal() {
  modalOpen.value = false
  modalParent.value = null
}

function handleAdd(formData) {
  if (!modalParent.value) return
  addOrgChartMember(modalParent.value.id, formData)
  closeModal()
}

function handleTerminate(node) {
  const name = [node.preferName, node.surname].filter(Boolean).join(' ')
  const verb = node.status === 'terminated' ? 'Reinstate' : 'Terminate'
  if (confirm(`${verb} ${name}?`)) {
    terminateOrgMember(node.id)
  }
}

function handleRemove(node) {
  const name = [node.preferName, node.surname].filter(Boolean).join(' ')
  if (confirm(`Remove ${name} from the org chart? This can't be undone.`)) {
    removeOrgMember(node.id)
  }
}
</script>

<template>
  <div class="admin-users-page">
    <div class="page-heading">
      <span class="page-heading-text">FinBine Org_Chart</span>
    </div>

    <div class="org-chart-scroll">
      <div class="org-chart-inner">
        <OrgChartNode
          :node="adminOrgChart"
          :on-add-click="openAddModal"
          :on-terminate-click="handleTerminate"
          :on-remove-click="handleRemove"
          :depth="0"
        />
      </div>
    </div>

    <AddAdminModal
      :open="modalOpen"
      :parent="modalParent"
      @close="closeModal"
      @add="handleAdd"
    />
  </div>
</template>

<style scoped>
.admin-users-page {
  height: 100vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.page-heading {
  flex-shrink: 0;
  padding: 14px 24px;
  border-bottom: 1px solid #E5E7EB;
  background: #fff;
}

.page-heading-text {
  font-family: ui-monospace, Consolas, monospace;
  font-size: 0.8rem;
  font-weight: 700;
  letter-spacing: 0.3px;
  color: #0F172A;
}

.org-chart-scroll {
  flex: 1;
  min-height: 0;
  overflow: auto;
  padding: 56px 24px;
  background-color: #F8FAFC;
  background-image: radial-gradient(circle, #E2E8F0 1px, transparent 1px);
  background-size: 22px 22px;
}

/* display: table shrink-wraps to the tree's natural width, so
   margin: auto centers it when it fits — but unlike flex's
   justify-content: center, it doesn't clip the left/start overflow
   once the tree is wider than the viewport. That clipping is a
   known flexbox+overflow quirk: the scroll container's scrollable
   area only extends from the flex line's start edge, so content
   pushed left of center by centering becomes unreachable even with
   overflow: auto set. Block-level auto-margin centering doesn't
   have that problem. */
.org-chart-inner {
  display: table;
  margin: 0 auto;
}
</style>
