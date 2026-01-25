import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import Ex3 from './Ex3.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Ex3 />
  </StrictMode>,
)
