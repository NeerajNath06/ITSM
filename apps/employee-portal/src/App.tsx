import { Button } from '@itsm/design-system'
import { Route, Routes } from 'react-router-dom'

// Wave 0 placeholder shell. Real routes (Service Catalog, Knowledge,
// My Requests) land in Wave 3 / Phase 5, per
// docs/roadmap/implementation-roadmap.md and
// docs/frontend/react-architecture.md's feature-based routing structure.
function Home() {
  return (
    <main>
      <h1>Employee Portal</h1>
      <p>Wave 0 scaffold — no product features yet.</p>
      <Button>Placeholder action</Button>
    </main>
  )
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
    </Routes>
  )
}
