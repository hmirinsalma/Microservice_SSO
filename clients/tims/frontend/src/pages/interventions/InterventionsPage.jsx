import React, { useEffect, useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box, Button, TextField, InputAdornment, Select, MenuItem, FormControl,
  InputLabel, Grid, Typography, IconButton, Tooltip, Chip, Avatar,
  Menu, Skeleton
} from '@mui/material'
import { DataGrid } from '@mui/x-data-grid'
import AddRoundedIcon from '@mui/icons-material/AddRounded'
import SearchRoundedIcon from '@mui/icons-material/SearchRounded'
import RefreshRoundedIcon from '@mui/icons-material/RefreshRounded'
import MoreVertRoundedIcon from '@mui/icons-material/MoreVertRounded'
import VisibilityRoundedIcon from '@mui/icons-material/VisibilityRounded'
import EditRoundedIcon from '@mui/icons-material/EditRounded'
import DeleteRoundedIcon from '@mui/icons-material/DeleteRounded'
import FilterListRoundedIcon from '@mui/icons-material/FilterListRounded'
import { getInterventions, deleteIntervention } from '../../api/interventions'
import { useAuth } from '../../context/AuthContext'
import { useSnackbar } from '../../context/SnackbarContext'
import { ROLES, STATUS_CONFIG, PRIORITY_CONFIG } from '../../utils/constants'
import ConfirmDialog from '../../components/common/ConfirmDialog'
import Breadcrumb from '../../components/common/Breadcrumb'
import { formatDate } from '../../components/common/dateUtils'

function useDebounce(value, delay) {
  const [d, setD] = useState(value)
  useEffect(() => { const t = setTimeout(() => setD(value), delay); return () => clearTimeout(t) }, [value, delay])
  return d
}

function StatusBadge({ status }) {
  const cfg = STATUS_CONFIG[status] || {}
  return (
    <Chip label={cfg.label || status} size="small"
      sx={{ bgcolor: cfg.bgColor, color: cfg.textColor, fontWeight: 600,
        fontSize: '0.68rem', height: 20, borderRadius: '4px' }} />
  )
}

function PriorityDot({ priority }) {
  const cfg = PRIORITY_CONFIG[priority] || {}
  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75 }}>
      <Box sx={{ width: 7, height: 7, borderRadius: '50%', bgcolor: cfg.color, flexShrink: 0 }} />
      <Typography sx={{ fontSize: '0.8rem', color: '#334155' }}>{cfg.label || priority}</Typography>
    </Box>
  )
}

function ActionMenu({ row, onView, onEdit, onDelete, canEdit, canDelete }) {
  const [anchor, setAnchor] = useState(null)
  return (
    <>
      <IconButton size="small" onClick={e => { e.stopPropagation(); setAnchor(e.currentTarget) }}
        sx={{ color: '#94a3b8', '&:hover': { color: '#1e3a5f', bgcolor: '#f1f5f9' } }}>
        <MoreVertRoundedIcon sx={{ fontSize: 18 }} />
      </IconButton>
      <Menu anchorEl={anchor} open={!!anchor} onClose={() => setAnchor(null)}
        PaperProps={{ sx: { minWidth: 160, borderRadius: '8px', boxShadow: '0 8px 24px rgba(0,0,0,0.12)' } }}>
        <MenuItem onClick={() => { onView(); setAnchor(null) }} sx={{ gap: 1.5, fontSize: '0.8rem' }}>
          <VisibilityRoundedIcon sx={{ fontSize: 16, color: '#64748b' }} /> Voir le détail
        </MenuItem>
        {canEdit && (
          <MenuItem onClick={() => { onEdit(); setAnchor(null) }} sx={{ gap: 1.5, fontSize: '0.8rem' }}>
            <EditRoundedIcon sx={{ fontSize: 16, color: '#64748b' }} /> Modifier
          </MenuItem>
        )}
        {canDelete && (
          <MenuItem onClick={() => { onDelete(); setAnchor(null) }}
            sx={{ gap: 1.5, fontSize: '0.8rem', color: 'error.main' }}>
            <DeleteRoundedIcon sx={{ fontSize: 16 }} /> Supprimer
          </MenuItem>
        )}
      </Menu>
    </>
  )
}

export default function InterventionsPage() {
  const { role } = useAuth()
  const { notify } = useSnackbar()
  const navigate = useNavigate()

  const [rows, setRows]         = useState([])
  const [loading, setLoading]   = useState(false)
  const [rowCount, setRowCount] = useState(0)
  const [paginationModel, setPM] = useState({ page: 0, pageSize: 20 })
  const [search, setSearch]     = useState('')
  const [statusF, setStatusF]   = useState('')
  const [priorityF, setPriorityF] = useState('')
  const [sortModel, setSort]    = useState([{ field: 'createdAt', sort: 'desc' }])
  const [delTarget, setDelTarget] = useState(null)
  const [deleting, setDeleting] = useState(false)

  const dSearch = useDebounce(search, 300)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const r = await getInterventions({
        page: paginationModel.page + 1, pageSize: paginationModel.pageSize,
        objet: dSearch || undefined,
        status: statusF || undefined, priority: priorityF || undefined,
        sortBy: sortModel[0]?.field, sortOrder: sortModel[0]?.sort,
      })
      setRows(r.data.data.items)
      setRowCount(r.data.data.totalCount)
    } catch { notify('Erreur de chargement', 'error') }
    finally { setLoading(false) }
  }, [paginationModel, dSearch, statusF, priorityF, sortModel])

  useEffect(() => { load() }, [load])

  const handleDelete = async () => {
    setDeleting(true)
    try { await deleteIntervention(delTarget); notify('Intervention supprimée'); setDelTarget(null); load() }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setDeleting(false) }
  }

  const canEdit   = [ROLES.ADMIN, ROLES.CHEF].includes(role)
  const canDelete = role === ROLES.ADMIN

  const columns = [
    {
      field: 'numeroIntervention', headerName: 'N° Intervention', width: 165,
      renderCell: p => (
        <Typography sx={{ fontSize: '0.78rem', fontWeight: 700, color: '#0ea5e9',
          cursor: 'pointer', '&:hover': { textDecoration: 'underline' } }}
          onClick={() => navigate(`/interventions/${p.row.id}`)}>
          {p.value}
        </Typography>
      )
    },
    {
      field: 'objet', headerName: 'Objet', flex: 1, minWidth: 200,
      renderCell: p => (
        <Tooltip title={p.value}>
          <Typography sx={{ fontSize: '0.82rem', color: '#1e293b', cursor: 'pointer' }} noWrap
            onClick={() => navigate(`/interventions/${p.row.id}`)}>
            {p.value}
          </Typography>
        </Tooltip>
      )
    },
    { field: 'typeIntervention', headerName: 'Type', width: 115,
      renderCell: p => <Typography sx={{ fontSize: '0.78rem', color: '#64748b' }}>{p.value}</Typography> },
    {
      field: 'status', headerName: 'Statut', width: 120,
      renderCell: p => <StatusBadge status={p.value} />
    },
    {
      field: 'priority', headerName: 'Priorité', width: 110,
      renderCell: p => <PriorityDot priority={p.value} />
    },
    {
      field: 'technicien', headerName: 'Technicien', width: 155,
      valueGetter: (_, row) => row.technicien?.fullName,
      renderCell: p => p.value ? (
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Avatar sx={{ width: 24, height: 24, fontSize: '0.65rem', bgcolor: '#0ea5e9' }}>
            {p.value.split(' ').map(w => w[0]).slice(0,2).join('')}
          </Avatar>
          <Typography sx={{ fontSize: '0.78rem' }} noWrap>{p.value}</Typography>
        </Box>
      ) : <Typography sx={{ fontSize: '0.78rem', color: '#94a3b8', fontStyle: 'italic' }}>Non affecté</Typography>
    },
    {
      field: 'service', headerName: 'Service', width: 160,
      valueGetter: (_, row) => row.service?.name,
      renderCell: p => <Typography sx={{ fontSize: '0.78rem', color: '#64748b' }} noWrap>{p.value || '—'}</Typography>
    },
    {
      field: 'datePrevue', headerName: 'Date prévue', width: 115,
      renderCell: p => <Typography sx={{ fontSize: '0.78rem', color: '#475569' }}>{formatDate(p.value)}</Typography>
    },
    {
      field: '_actions', headerName: '', width: 52, sortable: false,
      renderCell: p => (
        <ActionMenu row={p.row}
          onView={() => navigate(`/interventions/${p.row.id}`)}
          onEdit={() => navigate(`/interventions/${p.row.id}/edit`)}
          onDelete={() => setDelTarget(p.row.id)}
          canEdit={canEdit} canDelete={canDelete} />
      )
    }
  ]

  return (
    <Box>
      <Breadcrumb />
      <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={3}>
        <Box>
          <Typography variant="h5">Interventions</Typography>
          <Typography variant="body2" color="text.secondary" mt={0.5}>
            {rowCount} intervention{rowCount > 1 ? 's' : ''} au total
          </Typography>
        </Box>
        {canEdit && (
          <Button variant="contained" startIcon={<AddRoundedIcon />}
            onClick={() => navigate('/interventions/new')}
            sx={{ bgcolor: '#1e3a5f', '&:hover': { bgcolor: '#152c47' } }}>
            Nouvelle intervention
          </Button>
        )}
      </Box>

      {/* Filters */}
      <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0', p: 2, mb: 2 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={5} md={4}>
            <TextField fullWidth size="small" placeholder="Rechercher par objet, numéro…"
              value={search} onChange={e => setSearch(e.target.value)}
              InputProps={{ startAdornment: (
                <InputAdornment position="start">
                  <SearchRoundedIcon sx={{ fontSize: 16, color: '#94a3b8' }} />
                </InputAdornment>
              )}} />
          </Grid>
          <Grid item xs={6} sm={3} md={2}>
            <FormControl size="small" fullWidth>
              <InputLabel>Statut</InputLabel>
              <Select value={statusF} label="Statut" onChange={e => setStatusF(e.target.value)}>
                <MenuItem value="">Tous les statuts</MenuItem>
                {Object.entries(STATUS_CONFIG).map(([k, v]) => (
                  <MenuItem key={k} value={k}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Box sx={{ width: 8, height: 8, borderRadius: '50%', bgcolor: v.textColor }} />
                      {v.label}
                    </Box>
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={6} sm={3} md={2}>
            <FormControl size="small" fullWidth>
              <InputLabel>Priorité</InputLabel>
              <Select value={priorityF} label="Priorité" onChange={e => setPriorityF(e.target.value)}>
                <MenuItem value="">Toutes</MenuItem>
                {Object.entries(PRIORITY_CONFIG).map(([k, v]) => (
                  <MenuItem key={k} value={k}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Box sx={{ width: 8, height: 8, borderRadius: '50%', bgcolor: v.color }} />
                      {v.label}
                    </Box>
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item>
            <Tooltip title="Rafraîchir">
              <IconButton size="small" onClick={load}
                sx={{ bgcolor: '#f8fafc', border: '1px solid #e2e8f0', borderRadius: '6px' }}>
                <RefreshRoundedIcon sx={{ fontSize: 18 }} />
              </IconButton>
            </Tooltip>
          </Grid>
        </Grid>
      </Box>

      {/* DataGrid */}
      <Box sx={{ bgcolor: 'white', borderRadius: '10px', border: '1px solid #e2e8f0', overflow: 'hidden' }}>
        <DataGrid
          rows={rows} columns={columns} rowCount={rowCount} loading={loading}
          paginationMode="server" paginationModel={paginationModel}
          onPaginationModelChange={setPM}
          sortingMode="server" sortModel={sortModel} onSortModelChange={setSort}
          pageSizeOptions={[10, 20, 50]} disableRowSelectionOnClick
          autoHeight
          sx={{ border: 'none',
            '& .MuiDataGrid-columnHeaderTitle': { fontSize: '0.7rem', fontWeight: 700,
              textTransform: 'uppercase', letterSpacing: '0.06em', color: '#64748b' },
            '& .MuiDataGrid-row': { cursor: 'pointer' },
          }}
        />
      </Box>

      <ConfirmDialog open={!!delTarget} title="Supprimer l'intervention"
        message="Cette intervention sera marquée comme supprimée. Action irréversible."
        onConfirm={handleDelete} onCancel={() => setDelTarget(null)} loading={deleting} />
    </Box>
  )
}
