import { useEffect, useState, useCallback } from 'react';
import { Plus, Pencil, Trash2, GitBranch, ChevronDown } from 'lucide-react';
import servicesApi from '../../api/servicesApi';
import directionsApi from '../../api/directionsApi';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import Spinner from '../../components/ui/Spinner';
import EmptyState from '../../components/ui/EmptyState';
import Toast from '../../components/ui/Toast';
import ServiceFormModal from './ServiceFormModal';
import ConfirmModal from '../../components/common/ConfirmModal';
import { useAuth } from '../../context/AuthContext';

export default function ServicesPage() {
  const { isAdmin } = useAuth();
  const [rows, setRows]   = useState([]);
  const [dirs, setDirs]   = useState([]);
  const [filterDir, setFilterDir] = useState('');
  const [loading, setLoading] = useState(true);
  const [modal, setModal] = useState(null);
  const [toast, setToast] = useState({ open: false, message: '', type: 'success' });

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [sR, dR] = await Promise.all([servicesApi.getAll(), directionsApi.getAll()]);
      setRows(sR.data); setDirs(dR.data);
    } catch {}
    finally { setLoading(false); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const notify = (msg, type = 'success') => setToast({ open: true, message: msg, type });

  const handleDelete = async () => {
    try {
      await servicesApi.delete(modal.data.id);
      notify('Service supprimé.');
      load();
    } catch (e) { notify(e.response?.data?.message || 'Erreur.', 'error'); }
    finally { setModal(null); }
  };

  const filtered = filterDir ? rows.filter(s => s.directionId === Number(filterDir)) : rows;

  if (loading) return <Spinner />;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-slate-900">Services</h2>
          <p className="text-sm text-slate-500">{rows.length} service(s) enregistré(s)</p>
        </div>
        {isAdmin && (
          <Button icon={<Plus size={16} />} onClick={() => setModal({ type: 'form' })}>
            Nouveau service
          </Button>
        )}
      </div>

      {/* Filtre */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-4">
        <div className="flex items-center gap-4 flex-wrap">
          <span className="text-xs font-semibold text-slate-500 uppercase tracking-wide">Filtrer par :</span>
          <div className="relative">
            <select value={filterDir} onChange={e => setFilterDir(e.target.value)}
              className="h-9 pl-3 pr-8 rounded-lg border border-slate-200 text-sm text-slate-700 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 appearance-none cursor-pointer min-w-[200px]">
              <option value="">Toutes les directions</option>
              {dirs.map(d => <option key={d.id} value={d.id}>{d.nom}</option>)}
            </select>
            <ChevronDown size={14} className="absolute right-2.5 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none" />
          </div>
          {filterDir && (
            <button onClick={() => setFilterDir('')} className="text-xs text-blue-600 hover:text-blue-800 font-medium">
              × Effacer
            </button>
          )}
        </div>
      </div>

      {/* Table */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
        {filtered.length === 0 ? (
          <EmptyState icon={GitBranch} title="Aucun service" description="Ajoutez des services à vos directions."
            actionLabel={isAdmin ? "Créer un service" : undefined}
            onAction={isAdmin ? () => setModal({ type: 'form' }) : undefined} />
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-100">
                  {['Service','Direction','Description','Employés', isAdmin ? 'Actions' : null].filter(Boolean).map(h => (
                    <th key={h} className={`px-6 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider ${h==='Actions' ? 'text-right' : 'text-left'}`}>{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {filtered.map(s => (
                  <tr key={s.id} className="hover:bg-slate-50/60 transition-colors group">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-lg bg-amber-50 flex items-center justify-center shrink-0">
                          <GitBranch size={14} className="text-amber-600" />
                        </div>
                        <span className="text-sm font-semibold text-slate-900">{s.nom}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <Badge variant="info">{s.directionNom}</Badge>
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-500">{s.description || '—'}</td>
                    <td className="px-6 py-4">
                      <Badge variant="neutral">{s.nombreEmployes} employé{s.nombreEmployes !== 1 ? 's' : ''}</Badge>
                    </td>
                    {isAdmin && (
                      <td className="px-6 py-4">
                        <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button onClick={() => setModal({ type: 'form', data: s })}
                            className="p-1.5 rounded-lg hover:bg-blue-50 text-slate-400 hover:text-blue-600 transition-colors">
                            <Pencil size={14} />
                          </button>
                          <button onClick={() => setModal({ type: 'delete', data: s })}
                            disabled={s.nombreEmployes > 0}
                            className="p-1.5 rounded-lg hover:bg-red-50 text-slate-400 hover:text-red-600 transition-colors disabled:opacity-30 disabled:cursor-not-allowed">
                            <Trash2 size={14} />
                          </button>
                        </div>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {modal?.type === 'form' && <ServiceFormModal service={modal.data} onClose={() => setModal(null)} onSuccess={(m) => { notify(m); load(); }} />}
      {modal?.type === 'delete' && <ConfirmModal title="Supprimer le service" message={`Supprimer "${modal.data.nom}" ?`} onConfirm={handleDelete} onCancel={() => setModal(null)} />}
      <Toast {...toast} onClose={() => setToast(t => ({ ...t, open: false }))} />
    </div>
  );
}
