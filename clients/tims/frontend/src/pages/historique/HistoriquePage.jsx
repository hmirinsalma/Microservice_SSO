import React, { useEffect, useState } from 'react'
import {
  Box, Typography, Avatar, Chip, Select, MenuItem, FormControl,
  InputLabel, Skeleton, TextField
} from '@mui/material'
import BuildRoundedIcon from '@mui/icons-material/BuildRounded'
import SwapHorizRoundedIcon from '@mui/icons-material/SwapHorizRounded'
import PersonRoundedIcon from '@mui/icons-material/PersonRounded'
import FlagRoundedIcon from '@mui/icons-material/FlagRounded'
import AddCommentRoundedIcon from '@mui/icons-material/AddCommentRounded'
import AttachFileRoundedIcon from '@mui/icons-material/AttachFileRounded'
import CheckCircleRoundedIcon from '@mui/icons-material/CheckCircleRounded'
import { getInterventions } from '../../api/interventions'
import { getHistory } from '../../api/interventions'
import { formatDateTime } from '../../components/common/dateUtils'
import { ACTION_LABELS } from '../../utils/constants'
import Breadcrumb from '../../components/common/Breadcrumb'

const ACTION_ICONS = {
  Creation: <BuildRoundedIcon sx={{ fontSize: 15 }} />,
  ChangementStatut: <SwapHorizRoundedIcon sx={{ fontSize: 15 }} />,
  ChangementTechnicien: <PersonRoundedIcon sx={{ fontSize: 15 }} />,
  ChangementResponsable: <PersonRoundedIcon sx={{ fontSize: 15 }} />,
  ChangementPriorite: <FlagRoundedIcon sx={{ fontSize: 15 }} />,
  AjoutCommentaire: <AddCommentRoundedIcon sx={{ fontSize: 15 }} />,
  AjoutPieceJointe: <AttachFileRoundedIcon sx={{ fontSize: 15 }} />,
  Affectation: <PersonRoundedIcon sx={{ fontSize: 15 }} />,
  AjoutCompteRendu: <CheckCircleRoundedIcon sx={{ fontSize: 15 }} />,
}
const ACTION_COLORS = {
  Creation: '#0ea5e9', ChangementStatut: '#f59e0b', ChangementTechnicien: '#8b5cf6',
  Affectation: '#10b981', ChangementPriorite: '#ef4444', AjoutCommentaire: '#64748b',
}

export default function HistoriquePage() {
  const [interventions, setInterventions] = useState([])
  const [selected, setSelected] = useState('')
  const [history, setHistory]   = useState([])
  const [loadingH, setLoadingH] = useState(false)

  useEffect(() => {
    getInterventions({ page: 1, pageSize: 100 }).then(r => setInterventions(r.data.data.items))
  }, [])

  useEffect(() => {
    if (!selected) return
    setLoadingH(true)
    getHistory(selected).then(r => setHistory(r.data.data)).finally(() => setLoadingH(false))
  }, [selected])

  return (
    <Box>
      <Breadcrumb />
      <Typography variant="h5" mb={1}>Historique des interventions</Typography>
      <Typography variant="body2" color="text.secondary" mb={3}>
        Traçabilité complète de toutes les modifications
      </Typography>

      <FormControl size="small" sx={{ minWidth: 360, mb: 3 }}>
        <InputLabel>Sélectionner une intervention</InputLabel>
        <Select value={selected} label="Sélectionner une intervention"
          onChange={e => setSelected(e.target.value)}>
          {interventions.map(i => (
            <MenuItem key={i.id} value={i.id}>
              <Typography sx={{ fontSize: '0.82rem' }} noWrap>
                {i.numeroIntervention} — {i.objet}
              </Typography>
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      {!selected && (
        <Box sx={{ textAlign: 'center', py: 8, color: '#94a3b8' }}>
          <BuildRoundedIcon sx={{ fontSize: 48, mb: 1, color: '#e2e8f0' }} />
          <Typography>Sélectionnez une intervention pour voir son historique</Typography>
        </Box>
      )}

      {selected && (
        <Box sx={{ maxWidth: 700 }}>
          {loadingH ? Array.from({length:5}).map((_, i) => (
            <Box key={i} sx={{ display:'flex', gap:2, mb:3 }}>
              <Skeleton variant="circular" width={32} height={32} />
              <Box flex={1}><Skeleton width="60%" /><Skeleton width="80%" /></Box>
            </Box>
          )) : (
            <Box sx={{ position: 'relative' }}>
              {/* Vertical line */}
              <Box sx={{ position: 'absolute', left: 15, top: 0, bottom: 0,
                width: 2, bgcolor: '#e2e8f0', zIndex: 0 }} />

              {history.map((h, i) => {
                const color = ACTION_COLORS[h.actionType] || '#64748b'
                const icon  = ACTION_ICONS[h.actionType] || <BuildRoundedIcon sx={{ fontSize:15 }} />
                return (
                  <Box key={h.id} sx={{ display: 'flex', gap: 2.5, mb: 3, position: 'relative', zIndex: 1 }}>
                    {/* Icon bubble */}
                    <Box sx={{ width: 32, height: 32, borderRadius: '50%', flexShrink: 0,
                      bgcolor: `${color}15`, border: `2px solid ${color}30`,
                      display: 'flex', alignItems: 'center', justifyContent: 'center',
                      color, zIndex: 1, bgcolor: '#fff' }}>
                      {React.cloneElement(icon, { sx: { fontSize: 15, color } })}
                    </Box>

                    {/* Content */}
                    <Box sx={{ flex: 1, bgcolor: 'white', borderRadius: '10px',
                      border: '1px solid #e2e8f0', p: 2, mt: 0.25 }}>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 0.5 }}>
                        <Typography sx={{ fontSize: '0.82rem', fontWeight: 600, color: '#0f172a' }}>
                          {ACTION_LABELS[h.actionType] || h.actionType}
                        </Typography>
                        <Typography sx={{ fontSize: '0.68rem', color: '#94a3b8', flexShrink: 0, ml: 1 }}>
                          {formatDateTime(h.createdAt)}
                        </Typography>
                      </Box>

                      {h.author && (
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75, mb: 0.75 }}>
                          <Avatar sx={{ width: 18, height: 18, fontSize: '0.6rem', bgcolor: color }}>
                            {h.author.fullName?.[0]}
                          </Avatar>
                          <Typography sx={{ fontSize: '0.72rem', color: '#64748b' }}>{h.author.fullName}</Typography>
                        </Box>
                      )}

                      {h.description && (
                        <Typography sx={{ fontSize: '0.78rem', color: '#475569' }}>{h.description}</Typography>
                      )}
                      {h.oldValue && h.newValue && (
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mt: 0.5 }}>
                          <Chip label={h.oldValue} size="small" sx={{ fontSize: '0.65rem', bgcolor: '#fee2e2', color: '#b91c1c', height: 18 }} />
                          <Typography sx={{ fontSize: '0.72rem', color: '#94a3b8' }}>→</Typography>
                          <Chip label={h.newValue} size="small" sx={{ fontSize: '0.65rem', bgcolor: '#dcfce7', color: '#15803d', height: 18 }} />
                        </Box>
                      )}
                    </Box>
                  </Box>
                )
              })}

              {history.length === 0 && !loadingH && (
                <Box sx={{ textAlign: 'center', py: 4, color: '#94a3b8' }}>
                  <Typography>Aucun historique disponible</Typography>
                </Box>
              )}
            </Box>
          )}
        </Box>
      )}
    </Box>
  )
}
