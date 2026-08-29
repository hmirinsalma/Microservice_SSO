import { useEffect, useState, useCallback } from 'react';
import { Plus, Pencil, Trash2, Building2 } from 'lucide-react';
import directionsApi from '../../api/directionsApi';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import Spinner from '../../components/ui/Spinner';
import EmptyState from '../../components/ui/EmptyState';
import Toast from '../../components/ui/Toast';
import DirectionFormModal from './DirectionFormModal';
import ConfirmModal from '../../components/common/ConfirmModal';
import { useAuth } from '../../context/AuthContext';

export default function DirectionsPage() {
  const { isAdmin } = useAuth();
  const [rows, setRows]   = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [modal, setModal] = useState(null); // null | { type: 'form'|'delete', data? }
  const [toast, setToast] = useState({ open: false, message: '', type: 'success' });

  const load = useCallback(async () => {
    setLoading(true);
    try { const { data } = await directionsApi.getAll(); setRows(data); setError(null); }
    catch { setError(true); }
    finally { setLoading(false); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const notify = (message, type = 'success') => setToast({ open: true, message, type });

  const handleDelete = async () => {
    try {
      await directionsApi.delete(modal.data.id);
      notify('Direction supprimée.');
      load();
    } catch (e) {
      notify(e.response?.data?.message || 'Erreur.', 'error');
    } finally { setModal(null); }
  };

  if (loading) return <Spinner />;
  if (error)   return <div className="text-center py-20 text-red-500">Erreur de chargement.</div>;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-slate-900">Directions</h2>
          <p className="text-sm text-slate-500">{rows.length} direction(s) enregistrée(s)</p>
        </div>
        {isAdmin && (
          <Button icon={<Plus size={16} />} onClick={() => setModal({ type: 'form' })}>
            Nouvelle direction
          </Button>
        )}
      </div>

      {/* Table */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
        {rows.length === 0 ? (
          <EmptyState icon={Building2} title="Aucune direction" description="Créez votre première direction pour structurer votre organisation."
            actionLabel={isAdmin ? "Créer une direction" : undefined}
            onAction={isAdmin ? () => setModal({ type: 'form' }) : undefined} />
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-100">
                  {['Direction', 'Description', 'Services', 'Employés', isAdmin ? 'Actions' : null]
                    .filter(Boolean).map(h => (
                    <th key={h} className={`px-6 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider ${h === 'Actions' ? 'text-right' : 'text-left'}`}>{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {rows.map(d => (
                  <tr key={d.id} className="hover:bg-slate-50/60 transition-colors group">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-lg bg-blue-50 flex items-center justify-center shrink-0">
                          <Building2 size={15} className="text-blue-600" />
                        </div>
                        <span className="text-sm font-semibold text-slate-900">{d.nom}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 text-sm text-slate-500">{d.description || '—'}</td>
                    <td className="px-6 py-4">
                      <Badge variant="success">{d.nombreServices} service{d.nombreServices !== 1 ? 's' : ''}</Badge>
                    </td>
                    <td className="px-6 py-4">
                      <Badge variant="info">{d.nombreEmployes} employé{d.nombreEmployes !== 1 ? 's' : ''}</Badge>
                    </td>
                    {isAdmin && (
                      <td className="px-6 py-4">
                        <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button onClick={() => setModal({ type: 'form', data: d })}
                            className="p-1.5 rounded-lg hover:bg-blue-50 text-slate-400 hover:text-blue-600 transition-colors">
                            <Pencil size={14} />
                          </button>
                          <button
                            onClick={() => setModal({ type: 'delete', data: d })}
                            disabled={d.nombreServices > 0 || d.nombreEmployes > 0}
                            title={d.nombreServices > 0 || d.nombreEmployes > 0 ? 'Contient des données liées' : 'Supprimer'}
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

      {modal?.type === 'form' && (
        <DirectionFormModal direction={modal.data} onClose={() => setModal(null)}
          onSuccess={(m) => { notify(m); load(); }} />
      )}
      {modal?.type === 'delete' && (
        <ConfirmModal
          title="Supprimer la direction"
          message={`Supprimer définitivement "${modal.data.nom}" ?`}
          onConfirm={handleDelete} onCancel={() => setModal(null)} />
      )}
      <Toast {...toast} onClose={() => setToast(t => ({ ...t, open: false }))} />
    </div>
  );
}
