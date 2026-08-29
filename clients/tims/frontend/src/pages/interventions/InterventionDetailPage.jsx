import React, { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
  Box, Grid, Card, CardContent, Typography, Button, Divider, Chip,
  TextField, Select, MenuItem, FormControl, InputLabel, IconButton,
  List, ListItem, ListItemAvatar, ListItemText, Avatar, Tabs, Tab,
  CircularProgress, Tooltip
} from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import EditIcon from '@mui/icons-material/Edit'
import SendIcon from '@mui/icons-material/Send'
import AttachFileIcon from '@mui/icons-material/AttachFile'
import DownloadIcon from '@mui/icons-material/Download'
import DeleteIcon from '@mui/icons-material/Delete'
import {
  getIntervention, getHistory, changeStatus, changePriority,
  assignTech, addComment, updateCompteRendu,
  addAttachment, deleteAttachment, downloadAttachment
} from '../../api/interventions'
import { getTechsByService } from '../../api/users'
import { useAuth } from '../../context/AuthContext'
import { useSnackbar } from '../../context/SnackbarContext'
import { ROLES, STATUS_CONFIG, PRIORITY_CONFIG, ACTION_LABELS } from '../../utils/constants'
import StatusChip from '../../components/common/StatusChip'
import PriorityChip from '../../components/common/PriorityChip'
import PageLoader from '../../components/common/PageLoader'
import Breadcrumb from '../../components/common/Breadcrumb'
import { formatDate, formatDateTime } from '../../components/common/dateUtils'

function InfoRow({ label, value }) {
  return (
    <Box sx={{ py: 1, borderBottom: '1px solid', borderColor: 'divider' }}>
      <Typography variant="caption" color="text.secondary">{label}</Typography>
      <Typography variant="body2" fontWeight={500}>{value || '—'}</Typography>
    </Box>
  )
}

export default function InterventionDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { role, user } = useAuth()
  const { notify } = useSnackbar()

  const [data, setData]     = useState(null)
  const [history, setHistory] = useState([])
  const [loading, setLoading] = useState(true)
  const [tab, setTab]         = useState(0)
  const [comment, setComment] = useState('')
  const [compteRendu, setCR]  = useState('')
  const [techs, setTechs]     = useState([])
  const [saving, setSaving]   = useState(false)

  const load = async () => {
    try {
      const [iRes, hRes] = await Promise.all([getIntervention(id), getHistory(id)])
      setData(iRes.data.data)
      setHistory(hRes.data.data)
      setCR(iRes.data.data.compteRendu || '')
      if (iRes.data.data.service?.id)
        getTechsByService(iRes.data.data.service.id).then(r => setTechs(r.data.data))
    } catch { notify('Erreur de chargement', 'error') }
    finally { setLoading(false) }
  }

  useEffect(() => { load() }, [id])

  const canEdit = [ROLES.ADMIN, ROLES.CHEF].includes(role) &&
    !['Terminee','Annulee'].includes(data?.status)

  const handleStatusChange = async (e) => {
    setSaving(true)
    try { await changeStatus(id, { newStatus: e.target.value }); await load(); notify('Statut mis à jour') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const handlePriorityChange = async (e) => {
    setSaving(true)
    try { await changePriority(id, { newPriority: e.target.value }); await load(); notify('Priorité mise à jour') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const handleAssign = async (e) => {
    setSaving(true)
    try { await assignTech(id, { technicienId: e.target.value || null }); await load(); notify('Affectation mise à jour') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const handleComment = async () => {
    if (!comment.trim()) return
    setSaving(true)
    try { await addComment(id, { content: comment }); setComment(''); await load(); notify('Commentaire ajouté') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const handleCR = async () => {
    setSaving(true)
    try { await updateCompteRendu(id, { compteRendu }); await load(); notify('Compte rendu enregistré') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const handleFile = async (e) => {
    const file = e.target.files?.[0]; if (!file) return
    setSaving(true)
    try { await addAttachment(id, file); await load(); notify('Fichier ajouté') }
    catch (e) { notify(e.response?.data?.message || 'Erreur', 'error') }
    finally { setSaving(false) }
  }

  const handleDownload = async (attId, name) => {
    try {
      const r = await downloadAttachment(attId)
      const url = URL.createObjectURL(r.data)
      const a = document.createElement('a'); a.href = url; a.download = name; a.click()
      URL.revokeObjectURL(url)
    } catch { notify('Erreur de téléchargement', 'error') }
  }

  if (loading) return <PageLoader />
  if (!data) return null

  const statusOptions = {
    Nouvelle: ['EnCours','Annulee'], EnCours: ['Suspendue','Terminee','Annulee'],
    Suspendue: ['EnCours','Annulee'], Terminee: [], Annulee: []
  }[data.status] || []

  return (
    <Box>
      <Breadcrumb />
      <Box display="flex" alignItems="center" gap={2} mb={3}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/interventions')}>Retour</Button>
        <Box flex={1}>
          <Typography variant="h6" fontWeight={700}>{data.numeroIntervention} — {data.objet}</Typography>
        </Box>
        {canEdit && (
          <Button variant="outlined" startIcon={<EditIcon />} onClick={() => navigate(`/interventions/${id}/edit`)}>
            Modifier
          </Button>
        )}
      </Box>

      <Grid container spacing={3}>
        {/* Main info */}
        <Grid item xs={12} md={8}>
          <Card sx={{ mb: 2 }}>
            <CardContent>
              <Typography variant="subtitle1" fontWeight={700} mb={2}>Informations générales</Typography>
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6}><InfoRow label="Type" value={data.typeIntervention} /></Grid>
                <Grid item xs={12} sm={6}><InfoRow label="Catégorie" value={data.categorie} /></Grid>
                <Grid item xs={12}><InfoRow label="Description" value={data.description} /></Grid>
                <Grid item xs={12} sm={6}><InfoRow label="Localisation" value={data.localisation} /></Grid>
                <Grid item xs={12} sm={6}><InfoRow label="Équipement" value={data.equipement} /></Grid>
                <Grid item xs={12} sm={6}><InfoRow label="Date création" value={formatDateTime(data.createdAt)} /></Grid>
                <Grid item xs={12} sm={6}><InfoRow label="Date prévue" value={formatDate(data.datePrevue)} /></Grid>
                {data.dateCloture && <Grid item xs={12} sm={6}><InfoRow label="Date clôture" value={formatDateTime(data.dateCloture)} /></Grid>}
                <Grid item xs={12} sm={6}><InfoRow label="Service" value={data.service?.name} /></Grid>
                <Grid item xs={12} sm={6}><InfoRow label="Équipe" value={data.equipe?.name} /></Grid>
              </Grid>
            </CardContent>
          </Card>

          {/* Tabs */}
          <Card>
            <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ borderBottom: 1, borderColor: 'divider' }}>
              <Tab label={`Commentaires (${data.comments?.length || 0})`} />
              <Tab label={`Pièces jointes (${data.attachments?.length || 0})`} />
              <Tab label="Compte rendu" />
              <Tab label={`Historique (${history.length})`} />
            </Tabs>

            <CardContent>
              {/* Comments */}
              {tab === 0 && (
                <Box>
                  <List>
                    {data.comments?.map(c => (
                      <ListItem key={c.id} alignItems="flex-start" sx={{ px: 0 }}>
                        <ListItemAvatar>
                          <Avatar sx={{ width: 32, height: 32, fontSize: 13, bgcolor: 'primary.main' }}>
                            {c.author?.fullName?.[0]}
                          </Avatar>
                        </ListItemAvatar>
                        <ListItemText
                          primary={<><strong>{c.author?.fullName}</strong> <Typography component="span" variant="caption" color="text.secondary">— {formatDateTime(c.createdAt)}</Typography></>}
                          secondary={c.content}
                        />
                      </ListItem>
                    ))}
                  </List>
                  {[ROLES.ADMIN, ROLES.CHEF, ROLES.TECH].includes(role) && (
                    <Box display="flex" gap={1} mt={2}>
                      <TextField fullWidth size="small" placeholder="Ajouter un commentaire…"
                        value={comment} onChange={e => setComment(e.target.value)}
                        onKeyDown={e => e.key === 'Enter' && !e.shiftKey && handleComment()} />
                      <Button variant="contained" onClick={handleComment} disabled={saving || !comment.trim()}>
                        <SendIcon />
                      </Button>
                    </Box>
                  )}
                </Box>
              )}

              {/* Attachments */}
              {tab === 1 && (
                <Box>
                  {data.attachments?.map(a => (
                    <Box key={a.id} sx={{ display: 'flex', alignItems: 'center', p: 1.5, mb: 1, bgcolor: 'grey.50', borderRadius: 2 }}>
                      <AttachFileIcon sx={{ mr: 1, color: 'text.secondary' }} />
                      <Box flex={1}>
                        <Typography variant="body2" fontWeight={500}>{a.originalFileName}</Typography>
                        <Typography variant="caption" color="text.secondary">
                          {(a.fileSize/1024).toFixed(0)} KB — {formatDate(a.createdAt)}
                        </Typography>
                      </Box>
                      <IconButton size="small" onClick={() => handleDownload(a.id, a.originalFileName)}>
                        <DownloadIcon fontSize="small" />
                      </IconButton>
                    </Box>
                  ))}
                  {data.attachments?.length === 0 && <Typography color="text.secondary" variant="body2">Aucune pièce jointe</Typography>}
                  <Button component="label" variant="outlined" startIcon={<AttachFileIcon />} sx={{ mt: 2 }} disabled={saving}>
                    Ajouter un fichier
                    <input type="file" hidden onChange={handleFile} accept=".jpg,.jpeg,.png,.webp,.pdf" />
                  </Button>
                </Box>
              )}

              {/* Compte rendu */}
              {tab === 2 && (
                <Box>
                  <TextField fullWidth multiline rows={8} label="Compte rendu d'intervention"
                    value={compteRendu} onChange={e => setCR(e.target.value)}
                    disabled={role !== ROLES.TECH} />
                  {role === ROLES.TECH && (
                    <Button variant="contained" sx={{ mt: 2 }} onClick={handleCR} disabled={saving}>
                      {saving ? <CircularProgress size={20} color="inherit" /> : 'Enregistrer'}
                    </Button>
                  )}
                </Box>
              )}

              {/* History */}
              {tab === 3 && (
                <List>
                  {history.map(h => (
                    <ListItem key={h.id} alignItems="flex-start" sx={{ px: 0 }}>
                      <ListItemAvatar>
                        <Avatar sx={{ width: 32, height: 32, fontSize: 12, bgcolor: 'secondary.main' }}>
                          {h.author?.fullName?.[0]}
                        </Avatar>
                      </ListItemAvatar>
                      <ListItemText
                        primary={<><strong>{ACTION_LABELS[h.actionType] || h.actionType}</strong> — <Typography component="span" variant="caption" color="text.secondary">{h.author?.fullName} · {formatDateTime(h.createdAt)}</Typography></>}
                        secondary={h.description || (h.oldValue && h.newValue ? `${h.oldValue} → ${h.newValue}` : null)}
                      />
                    </ListItem>
                  ))}
                </List>
              )}
            </CardContent>
          </Card>
        </Grid>

        {/* Side panel */}
        <Grid item xs={12} md={4}>
          <Card sx={{ mb: 2 }}>
            <CardContent>
              <Typography variant="subtitle1" fontWeight={700} mb={2}>Statut & Priorité</Typography>
              <Box mb={2}>
                <Typography variant="caption" color="text.secondary">Statut actuel</Typography>
                <Box mt={0.5} mb={1}><StatusChip status={data.status} /></Box>
                {canEdit && statusOptions.length > 0 && (
                  <FormControl fullWidth size="small">
                    <InputLabel>Changer le statut</InputLabel>
                    <Select value="" label="Changer le statut" onChange={handleStatusChange}>
                      {statusOptions.map(s => <MenuItem key={s} value={s}>{STATUS_CONFIG[s]?.label || s}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
                {role === ROLES.TECH && data.technicienId === user?.id && statusOptions.length > 0 && (
                  <FormControl fullWidth size="small" sx={{ mt: 1 }}>
                    <InputLabel>Changer le statut</InputLabel>
                    <Select value="" label="Changer le statut" onChange={handleStatusChange}>
                      {statusOptions.filter(s => ['EnCours','Suspendue','Terminee'].includes(s)).map(s => <MenuItem key={s} value={s}>{STATUS_CONFIG[s]?.label || s}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              </Box>
              <Divider sx={{ my: 1.5 }} />
              <Box>
                <Typography variant="caption" color="text.secondary">Priorité actuelle</Typography>
                <Box mt={0.5} mb={1}><PriorityChip priority={data.priority} /></Box>
                {canEdit && (
                  <FormControl fullWidth size="small">
                    <InputLabel>Changer la priorité</InputLabel>
                    <Select value={data.priority} label="Changer la priorité" onChange={handlePriorityChange}>
                      {['Faible','Normale','Urgente','Critique'].map(p => <MenuItem key={p} value={p}>{PRIORITY_CONFIG[p]?.label || p}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              </Box>
            </CardContent>
          </Card>

          <Card>
            <CardContent>
              <Typography variant="subtitle1" fontWeight={700} mb={2}>Personnes impliquées</Typography>
              <InfoRow label="Responsable" value={data.responsable?.fullName} />
              <InfoRow label="Chef de service" value={data.chefService?.fullName} />
              <Box sx={{ py: 1, borderBottom: '1px solid', borderColor: 'divider' }}>
                <Typography variant="caption" color="text.secondary">Technicien affecté</Typography>
                <Box mt={0.5} mb={1}>
                  <Typography variant="body2" fontWeight={500}>{data.technicien?.fullName || '— Non affecté —'}</Typography>
                </Box>
                {canEdit && (
                  <FormControl fullWidth size="small">
                    <InputLabel>Affecter un technicien</InputLabel>
                    <Select value={data.technicien?.id || ''} label="Affecter un technicien" onChange={handleAssign}>
                      <MenuItem value="">— Retirer l'affectation —</MenuItem>
                      {techs.map(t => <MenuItem key={t.id} value={t.id}>{t.firstName} {t.lastName}</MenuItem>)}
                    </Select>
                  </FormControl>
                )}
              </Box>
              <InfoRow label="Créé par" value={data.createdBy?.fullName} />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  )
}
