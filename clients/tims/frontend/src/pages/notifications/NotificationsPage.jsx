import React, { useEffect, useState } from 'react'
import {
  Box, Typography, Chip, IconButton, Button, Skeleton, Avatar, Tooltip
} from '@mui/material'
import DoneAllRoundedIcon from '@mui/icons-material/DoneAllRounded'
import CheckRoundedIcon from '@mui/icons-material/CheckRounded'
import NotificationsRoundedIcon from '@mui/icons-material/NotificationsRounded'
import BuildRoundedIcon from '@mui/icons-material/BuildRounded'
import PersonAddRoundedIcon from '@mui/icons-material/PersonAddRounded'
import SwapHorizRoundedIcon from '@mui/icons-material/SwapHorizRounded'
import FlagRoundedIcon from '@mui/icons-material/FlagRounded'
import CheckCircleRoundedIcon from '@mui/icons-material/CheckCircleRounded'
import { getNotifications, markRead, markAllRead, getUnreadCount } from '../../api/dashboard'
import { formatDistanceToNow } from '../../components/common/dateUtils'
import Breadcrumb from '../../components/common/Breadcrumb'

const TYPE_CONFIG = {
  InterventionCreee:    { icon: <BuildRoundedIcon sx={{ fontSize:16 }} />,       color: '#0ea5e9', label: 'Création' },
  TechnicienAffecte:    { icon: <PersonAddRoundedIcon sx={{ fontSize:16 }} />,   color: '#10b981', label: 'Affectation' },
  ChangementTechnicien: { icon: <SwapHorizRoundedIcon sx={{ fontSize:16 }} />,   color: '#8b5cf6', label: 'Modification' },
  ChangementResponsable:{ icon: <SwapHorizRoundedIcon sx={{ fontSize:16 }} />,   color: '#f59e0b', label: 'Modification' },
  ChangementPriorite:   { icon: <FlagRoundedIcon sx={{ fontSize:16 }} />,        color: '#ef4444', label: 'Priorité' },
  ChangementStatut:     { icon: <SwapHorizRoundedIcon sx={{ fontSize:16 }} />,   color: '#f59e0b', label: 'Statut' },
  InterventionTerminee: { icon: <CheckCircleRoundedIcon sx={{ fontSize:16 }} />, color: '#10b981', label: 'Clôture' },
}

export default function NotificationsPage() {
  const [items, setItems]       = useState([])
  const [loading, setLoading]   = useState(true)
  const [unread, setUnread]     = useState(0)
  const [page, setPage]         = useState(1)
  const [hasMore, setHasMore]   = useState(true)
  const [saving, setSaving]     = useState(false)

  const load = async (p = 1, append = false) => {
    setLoading(true)
    try {
      const r = await getNotifications(p, 20)
      const data = r.data.data
      setItems(prev => append ? [...prev, ...data.items] : data.items)
      setHasMore(p < data.totalPages)
      const u = await getUnreadCount()
      setUnread(u.data.data)
    } finally { setLoading(false) }
  }

  useEffect(() => { load(1) }, [])

  const handleMarkRead = async (id) => {
    await markRead(id)
    setItems(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n))
    setUnread(u => Math.max(0, u - 1))
  }

  const handleMarkAll = async () => {
    setSaving(true)
    await markAllRead()
    setItems(prev => prev.map(n => ({ ...n, isRead: true })))
    setUnread(0)
    setSaving(false)
  }

  const loadMore = () => {
    const next = page + 1
    setPage(next)
    load(next, true)
  }

  return (
    <Box>
      <Breadcrumb />
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box>
          <Typography variant="h5">Notifications</Typography>
          {unread > 0 && (
            <Typography variant="body2" color="text.secondary" mt={0.5}>
              <Chip label={`${unread} non lue${unread > 1 ? 's' : ''}`} size="small"
                sx={{ bgcolor: '#fee2e2', color: '#b91c1c', fontWeight: 600, height: 20, fontSize: '0.7rem' }} />
            </Typography>
          )}
        </Box>
        {unread > 0 && (
          <Button size="small" startIcon={<DoneAllRoundedIcon />} onClick={handleMarkAll} disabled={saving}
            sx={{ color: '#64748b', '&:hover': { color: '#0ea5e9' } }}>
            Tout marquer comme lu
          </Button>
        )}
      </Box>

      <Box sx={{ maxWidth: 680 }}>
        {loading && items.length === 0 ? (
          Array.from({ length: 6 }).map((_, i) => (
            <Box key={i} sx={{ display: 'flex', gap: 2, mb: 2, p: 2, bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0' }}>
              <Skeleton variant="circular" width={40} height={40} />
              <Box flex={1}>
                <Skeleton width="60%" height={20} />
                <Skeleton width="80%" height={16} sx={{ mt: 0.5 }} />
                <Skeleton width="30%" height={14} sx={{ mt: 0.5 }} />
              </Box>
            </Box>
          ))
        ) : items.length === 0 ? (
          <Box sx={{ textAlign: 'center', py: 10 }}>
            <NotificationsRoundedIcon sx={{ fontSize: 56, color: '#e2e8f0', mb: 2 }} />
            <Typography color="text.secondary">Aucune notification</Typography>
          </Box>
        ) : (
          items.map(n => {
            const cfg = TYPE_CONFIG[n.type] || { icon: <NotificationsRoundedIcon sx={{ fontSize:16 }} />, color: '#64748b', label: n.type }
            return (
              <Box key={n.id} sx={{
                display: 'flex', gap: 2, mb: 1.5, p: 2,
                bgcolor: n.isRead ? 'white' : '#f0f9ff',
                borderRadius: '10px',
                border: `1px solid ${n.isRead ? '#e2e8f0' : '#bae6fd'}`,
                transition: 'all .15s',
                '&:hover': { boxShadow: '0 2px 12px rgba(0,0,0,0.06)' }
              }}>
                {/* Icon */}
                <Box sx={{ width: 40, height: 40, borderRadius: '10px', flexShrink: 0,
                  bgcolor: `${cfg.color}15`, display: 'flex', alignItems: 'center',
                  justifyContent: 'center', color: cfg.color }}>
                  {cfg.icon}
                </Box>

                {/* Content */}
                <Box flex={1} minWidth={0}>
                  <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                    <Box flex={1}>
                      <Box display="flex" alignItems="center" gap={1} mb={0.25}>
                        <Chip label={cfg.label} size="small"
                          sx={{ bgcolor: `${cfg.color}15`, color: cfg.color,
                            fontWeight: 600, fontSize: '0.62rem', height: 18, borderRadius: '3px' }} />
                        {!n.isRead && (
                          <Box sx={{ width: 6, height: 6, borderRadius: '50%', bgcolor: '#0ea5e9' }} />
                        )}
                      </Box>
                      <Typography sx={{ fontSize: '0.82rem', fontWeight: n.isRead ? 400 : 600, color: '#0f172a' }}>
                        {n.title}
                      </Typography>
                      <Typography sx={{ fontSize: '0.75rem', color: '#64748b', mt: 0.25 }} noWrap>
                        {n.message}
                      </Typography>
                      <Typography sx={{ fontSize: '0.68rem', color: '#94a3b8', mt: 0.5 }}>
                        {formatDistanceToNow(n.createdAt)}
                      </Typography>
                    </Box>
                    {!n.isRead && (
                      <Tooltip title="Marquer comme lu">
                        <IconButton size="small" onClick={() => handleMarkRead(n.id)}
                          sx={{ ml: 1, color: '#94a3b8', '&:hover': { color: '#0ea5e9' } }}>
                          <CheckRoundedIcon sx={{ fontSize: 16 }} />
                        </IconButton>
                      </Tooltip>
                    )}
                  </Box>
                </Box>
              </Box>
            )
          })
        )}

        {hasMore && !loading && (
          <Box textAlign="center" mt={2}>
            <Button onClick={loadMore} sx={{ color: '#64748b' }}>Charger plus</Button>
          </Box>
        )}
      </Box>
    </Box>
  )
}
