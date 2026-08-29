import React, { useState } from 'react'
import { Box, Toolbar } from '@mui/material'
import Sidebar, { DRAWER_WIDTH, MINI_WIDTH } from './Sidebar'
import Navbar from './Navbar'

export default function AppLayout({ children }) {
  const [sidebarOpen, setSidebarOpen] = useState(true)
  const [mobileOpen,  setMobileOpen]  = useState(false)
  const width = sidebarOpen ? DRAWER_WIDTH : MINI_WIDTH

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      <Sidebar
        open={sidebarOpen}
        onToggle={() => setSidebarOpen(s => !s)}
        mobileOpen={mobileOpen}
        onMobileClose={() => setMobileOpen(false)}
      />
      <Box component="main" sx={{
        flexGrow: 1,
        ml: { md: `${width}px` },
        transition: 'margin-left .2s ease',
        minHeight: '100vh',
        display: 'flex', flexDirection: 'column'
      }}>
        <Navbar sidebarOpen={sidebarOpen} onMenuClick={() => setMobileOpen(true)} />
        <Toolbar sx={{ minHeight: '56px !important' }} />
        <Box sx={{ flex: 1, p: { xs: 2, md: 3 } }}>
          {children}
        </Box>
        <Box component="footer" sx={{ px: 3, py: 1.5, borderTop: '1px solid #e2e8f0',
          bgcolor: '#f8fafc', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Box sx={{ fontSize: '0.7rem', color: '#94a3b8' }}>
            ONEE TIMS v1.0 — Technical Intervention Management System
          </Box>
          <Box sx={{ fontSize: '0.7rem', color: '#94a3b8' }}>
            © 2026 Office National de l'Électricité et de l'Eau Potable
          </Box>
        </Box>
      </Box>
    </Box>
  )
}
