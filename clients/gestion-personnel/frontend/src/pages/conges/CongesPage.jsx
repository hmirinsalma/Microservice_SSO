import { useEffect, useState, useCallback } from 'react';
import { Plus, Eye, CheckCircle, XCircle, Trash2, FileText } from 'lucide-react';
import congesApi from '../../api/congesApi';
import directionsApi from '../../api/directionsApi';
import servicesApi from '../../api/servicesApi';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import CongeBadge from '../../components/ui/CongeBadge';
import Spinner from '../../components/ui/Spinner';
import EmptyState from '../../components/ui/EmptyState';
import Toast from '../../components/ui/Toast';
import ConfirmModal from '../../components/common/ConfirmModal';
import DemandeCongeModal from './DemandeCongeModal';
import TraiterCongeModal from './TraiterCongeModal';
import { useAuth } from '../../context/AuthContext';

const STATUTS = ['', 'EnAttente', 'ValideChef', 'ValideDirecteur', 'Refuse', 'Annule'];
const STATUTS_LABELS = {
  '': 'Tous', EnAttente: 'En attente', ValideChef: 'Validé chef',
  ValideDirecteur: 'Approuvé', Refuse: 'Refusé', Annule: 'Annulé',
};

export default function CongesPage() {
  const { user } = useAuth();
  const role = user?.role;
  const isAdmin = role === 'AdministrateurRH';
  const isChef  = role === 'ChefDeService';
  const isDir   = role === 'Directeur';
  const isEmp   = role === 'Employe';

  const canTraiter = isChef || isDir;
  const canCreate  = isEmp || isChef || isDir; // Admin RH ne crée pas de congé pour lui-même

  const [conges,  setConges]  = useState([]);
  const [dirs,    setDirs]    = useState([]);
  const [svcs,    setSvcs]    = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter,  setFilter]  = useState({ statut: '', directionId: '', serviceId: '' });
  const [modal,   setModal]   = useState(null);
  const [toast,   setToast]   = useState({ open: false, message: '', type: 'success' });

  const notify = (msg, type = 'success') => setToast({ open: true, message: msg, type });

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const { data } = await congesApi.getAll({
        statut:      filter.statut      || undefined,
        directionId: filter.directionId || undefined,
        serviceId:   filter.serviceId   || undefined,
      });
      // L'API retourne soit PagedResultDto (admin) soit tableau direct
      setConges(Array.isArray(data) ? data : (data.data ?? []));
    } catch {}
    finally { setLoading(false); }
  }, [filter]);

  useEffect(() => { load(); }, [load]);

  useEffect(() => {
    if (isAdmin) {
      directionsApi.getAll().then(({ data }) => setDirs(data)).catch(() => {});
    }
  }, [isAdmin]);

  useEffect(() => {
    if (isAdmin && filter.directionId)
      servicesApi.getByDirection(filter.directionId).then(({ data }) => setSvcs(data)).catch(() => {});
    else setSvcs([]);
  }, [filter.directionId, isAdmin]);

  const handleAnnuler = async () => {
    try {
      await congesApi.annuler(modal.data.id);
      notify('Demande annulée.');
      load();
    } catch (e) { notify(e.response?.data?.message || 'Erreur.', 'error'); }
    finally { setModal(null); }
  };

  const fmtDate = d => d ? new Date(d).toLocaleDateString('fr-FR') : '—';

  const canTraiterRow = (c) => {
    if (isChef) return c.statut === 'EnAttente';
    if (isDir)  return c.statut === 'ValideChef';
    return false;
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-slate-900">Congés</h2>
          <p className="text-sm text-slate-500">{conges.length} demande(s)</p>
        </div>
        {canCreate && (
          <Button icon={<Plus size={16} />} onClick={() => setModal({ type: 'create' })}>
            Nouvelle demande
          </Button>
        )}
      </div>

      {/* Filtres */}
      <div className="bg-white rounded-2xl border border-slate-200 p-4">
        <div className="flex flex-wrap gap-3 items-center">
          <span className="text-xs font-bold text-slate-400 uppercase tracking-widest">Filtres :</span>
          {/* Statut */}
          <select value={filter.statut} onChange={e => setFilter(p => ({ ...p, statut: e.target.value }))}
            className="h-8 px-3 rounded-lg border border-slate-200 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 cursor-pointer">
            {STATUTS.map(s => <option key={s} value={s}>{STATUTS_LABELS[s]}</option>)}
          </select>
          {/* Direction — admin seulement */}
          {isAdmin && (
            <select value={filter.directionId} onChange={e => setFilter(p => ({ ...p, directionId: e.target.value, serviceId: '' }))}
              className="h-8 px-3 rounded-lg border border-slate-200 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 cursor-pointer">
              <option value="">Toutes directions</option>
              {dirs.map(d => <option key={d.id} value={d.id}>{d.nom}</option>)}
            </select>
          )}
          {isAdmin && filter.directionId && (
            <select value={filter.serviceId} onChange={e => setFilter(p => ({ ...p, serviceId: e.target.value }))}
              className="h-8 px-3 rounded-lg border border-slate-200 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 cursor-pointer">
              <option value="">Tous services</option>
              {svcs.map(s => <option key={s.id} value={s.id}>{s.nom}</option>)}
            </select>
          )}
        </div>
      </div>

      {/* Table */}
      {loading ? <Spinner /> : (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          {conges.length === 0 ? (
            <EmptyState icon={FileText} title="Aucune demande de congé"
              description="Aucune demande ne correspond à vos critères."
              actionLabel={canCreate ? "Créer une demande" : undefined}
              onAction={canCreate ? () => setModal({ type: 'create' }) : undefined} />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="bg-slate-50 border-b border-slate-100">
                    {['Employé','Direction','Service','Période','Durée','Motif','Statut','Actions']
                      .filter(h => h !== 'Employé' || !isEmp)
                      .map(h => (
                      <th key={h} className={`px-4 py-2.5 text-xs font-semibold text-slate-500 uppercase tracking-wider ${h === 'Actions' ? 'text-right' : 'text-left'}`}>{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {conges.map(c => (
                    <tr key={c.id} className="hover:bg-slate-50/60 transition-colors group">
                      {!isEmp && (
                        <td className="px-4 py-3">
                          <p className="text-sm font-semibold text-slate-900">{c.employeNom} {c.employePrenom}</p>
                          <p className="text-xs text-blue-600">{c.employeMatricule}</p>
                        </td>
                      )}
                      <td className="px-4 py-3"><Badge variant="info">{c.directionNom}</Badge></td>
                      <td className="px-4 py-3 text-sm text-slate-500">{c.serviceNom}</td>
                      <td className="px-4 py-3 text-sm text-slate-700 whitespace-nowrap">
                        {fmtDate(c.dateDebut)} → {fmtDate(c.dateFin)}
                      </td>
                      <td className="px-4 py-3 text-sm font-semibold text-slate-700">{c.nombreJours}j</td>
                      <td className="px-4 py-3 text-sm text-slate-500 max-w-[150px] truncate">{c.motif}</td>
                      <td className="px-4 py-3"><CongeBadge statut={c.statut} /></td>
                      <td className="px-4 py-3">
                        <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                          {canTraiterRow(c) && (
                            <button onClick={() => setModal({ type: 'traiter', data: c })}
                              className="p-1.5 rounded-lg hover:bg-blue-50 text-slate-400 hover:text-blue-600 transition-colors"
                              title="Traiter">
                              <CheckCircle size={14} />
                            </button>
                          )}
                          {c.statut === 'EnAttente' && (isEmp || isAdmin) && (
                            <button onClick={() => setModal({ type: 'annuler', data: c })}
                              className="p-1.5 rounded-lg hover:bg-red-50 text-slate-400 hover:text-red-600 transition-colors"
                              title="Annuler">
                              <Trash2 size={14} />
                            </button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* Modals */}
      {modal?.type === 'create' && (
        <DemandeCongeModal onClose={() => setModal(null)}
          onSuccess={(m) => { notify(m); load(); }} />
      )}
      {modal?.type === 'traiter' && (
        <TraiterCongeModal conge={modal.data} role={role}
          onClose={() => setModal(null)}
          onSuccess={(m) => { notify(m); load(); }} />
      )}
      {modal?.type === 'annuler' && (
        <ConfirmModal title="Annuler la demande"
          message="Êtes-vous sûr de vouloir annuler cette demande de congé ?"
          onConfirm={handleAnnuler} onCancel={() => setModal(null)} />
      )}

      <Toast {...toast} onClose={() => setToast(t => ({ ...t, open: false }))} />
    </div>
  );
}
