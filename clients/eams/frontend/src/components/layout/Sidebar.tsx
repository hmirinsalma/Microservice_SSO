import React from 'react';
import { Box, Tooltip, Typography } from '@mui/material';
import {
  IconLayoutDashboard, IconTool, IconEngine, IconCategory,
  IconUsers, IconBell, IconUser, IconChevronLeft, IconChevronRight,
  IconBuildingFactory2
} from '@tabler/icons-react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { getSidebarItems } from '../../utils/roleGuard';

const iconMap: Record<string, React.ReactNode> = {
  Dashboard: <IconLayoutDashboard size={20} />,
  Build: <IconTool size={20} />,
  Engineering: <IconEngine size={20} />,
  Category: <IconCategory size={20} />,
  Group: <IconUsers size={20} />,
  Notifications: <IconBell size={20} />,
  Person: <IconUser size={20} />,
};

const OPEN_WIDTH = 256;
const MINI_WIDTH = 72;

interface Props { open: boolean; onToggle: () => void }

export default function Sidebar({ open, onToggle }: Props) {
  const { user } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const items = user ? getSidebarItems(user.role) : [];

  return (
    <Box sx={{
      width: open ? OPEN_WIDTH : MINI_WIDTH,
      flexShrink: 0,
      height: '100vh',
      position: 'sticky',
      top: 0,
      background: 'linear-gradient(180deg, #0A1628 0%, #0F2040 100%)',
      display: 'flex', flexDirection: 'column',
      transition: 'width 0.3s cubic-bezier(0.4,0,0.2,1)',
      overflow: 'hidden',
      boxShadow: '2px 0 24px rgba(0,0,0,0.15)',
      zIndex: 100,
    }}>
      {/* Logo */}
      <Box sx={{
        height: 64, display: 'flex', alignItems: 'center',
        px: open ? 2.5 : 1.5, gap: 1.5,
        borderBottom: '1px solid rgba(255,255,255,0.06)',
        cursor: 'pointer', flexShrink: 0,
      }} onClick={() => navigate('/')}>
        <Box sx={{
          width: 36, height: 36, borderRadius: '10px', flexShrink: 0,
          background: 'linear-gradient(135deg, #0066CC, #00A86B)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
        }}>
          <IconBuildingFactory2 size={20} color="white" />
        </Box>
        {open && (
          <Box className="slide-in">
            <Typography sx={{ color: '#fff', fontWeight: 800, fontSize: '0.95rem', lineHeight: 1.2 }}>ONEE EAMS</Typography>
            <Typography sx={{ color: 'rgba(255,255,255,0.4)', fontSize: '0.68rem', fontWeight: 500 }}>Patrimoine & Maintenance</Typography>
          </Box>
        )}
      </Box>

      {/* Nav Items */}
      <Box sx={{ flex: 1, py: 1.5, overflowY: 'auto', overflowX: 'hidden' }}>
        {items.map((item) => {
          const active = location.pathname === item.path || (item.path !== '/' && location.pathname.startsWith(item.path));
          return (
            <Tooltip key={item.path} title={!open ? item.label : ''} placement="right" arrow>
              <Box
                onClick={() => navigate(item.path)}
                sx={{
                  mx: 1.5, mb: 0.5, px: open ? 1.5 : 1.2, py: 1.1,
                  borderRadius: '10px', cursor: 'pointer',
                  display: 'flex', alignItems: 'center', gap: 1.5,
                  justifyContent: open ? 'flex-start' : 'center',
                  background: active ? 'rgba(0,102,204,0.3)' : 'transparent',
                  borderLeft: active ? '3px solid #0066CC' : '3px solid transparent',
                  transition: 'all 0.2s ease',
                  '&:hover': { background: 'rgba(255,255,255,0.07)', borderLeft: `3px solid ${active ? '#0066CC' : 'rgba(255,255,255,0.2)'}` },
                }}
              >
                <Box sx={{ color: active ? '#60A5FA' : 'rgba(255,255,255,0.5)', flexShrink: 0, display: 'flex' }}>
                  {iconMap[item.icon] || <IconTool size={20} />}
                </Box>
                {open && (
                  <Typography sx={{
                    color: active ? '#fff' : 'rgba(255,255,255,0.65)',
                    fontSize: '0.875rem', fontWeight: active ? 600 : 400,
                    transition: 'color 0.2s', whiteSpace: 'nowrap',
                  }}>
                    {item.label}
                  </Typography>
                )}
              </Box>
            </Tooltip>
          );
        })}
      </Box>

      {/* Toggle */}
      <Box sx={{ p: 1.5, borderTop: '1px solid rgba(255,255,255,0.06)' }}>
        <Box onClick={onToggle} sx={{
          display: 'flex', alignItems: 'center', justifyContent: open ? 'flex-end' : 'center',
          px: 1, py: 0.8, borderRadius: '8px', cursor: 'pointer',
          color: 'rgba(255,255,255,0.4)',
          '&:hover': { color: '#fff', background: 'rgba(255,255,255,0.06)' },
          transition: 'all 0.2s',
        }}>
          {open ? <IconChevronLeft size={18} /> : <IconChevronRight size={18} />}
        </Box>
      </Box>
    </Box>
  );
}
