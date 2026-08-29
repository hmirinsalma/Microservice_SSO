import React, { useState } from 'react';
import { Box } from '@mui/material';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Navbar from './Navbar';

export default function AppLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: '#F0F4F8' }}>
      <Sidebar open={sidebarOpen} onToggle={() => setSidebarOpen(!sidebarOpen)} />
      <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0, overflow: 'hidden' }}>
        <Navbar />
        <Box component="main" sx={{ flex: 1, p: { xs: 2, md: 3 }, overflowY: 'auto' }} className="fade-in">
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}
