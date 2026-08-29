import { useEffect, useState, useCallback } from 'react';
import { Plus, Search, Users, Pencil, Trash2, X, ChevronLeft, ChevronRight } from 'lucide-react';
import employesApi from '../../api/employesApi';
import directionsApi from '../../api/directionsApi';
import servicesApi from '../../api/servicesApi';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import Spinner from '../../components/ui/Spinner';
import EmptyState from '../../components/ui/EmptyState';
import Toast from '../../components/ui/Toast';
import EmployeFormModal from './EmployeFormModal';
import ConfirmModal from '../../components/common/ConfirmModal';
import { useAuth } from '../../context/AuthContext';

const STATUS_BADGE = {
  Actif:    <Badge variant="success">Actif</Badge>,
  Inactif:  <Badge variant="neutral">Inactif</Badge>,
  Suspendu: <Badge variant="warning">Suspendu</Badge>,
};

const PAGE_SIZE = 10;

export default function EmployesPage() {
  const { isAdmin } = useAuth();
  const [result, setResult]   = useState({ data: [], totalCount: 0, totalPages: 1 });
  const [dirs, setDirs]       = useState([]);
  const [svcs, setSvcs]       = useState([]);
  const [loading, setLoading] = useState(true);
  const [modal, setModal]     = useState(null);
  const [toast, setToast]     = useState({ open: false, message: '', type: 'success' });
  const [query, setQuery]     = useState({ page: 1, pageSize: PAGE_SIZE, search: '', directionId: '', serviceId: '', statut: '' });
  const [searchInput, setSearchInput] = useState('');

  const load = useCallback(async (q) => {
    setLoading(true);
    try {
      const p = { ...q };
      ['directionId','serviceId','statut','search'].forEach(k => { if (!p[k]) delete p[k]; });
      const { data } = await employesApi.getAll(p);
      setResult(data);
    } catch {}
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(query); }, [query, load]);
  useEffect(() => { directionsApi.getAll().then(({ data }) => setDirs(data)).catch(() => {}); }, []);
  useEffect(() => {
    if (query.directionId) servicesApi.getByDirection(query.directionId).then(({ data }) => setSvcs(data)).catch(() => {});
    else setSvcs([]);
  }, [query.directionId]);

  const upd = (u) => setQuery(p => ({ ...p, ...u, page: 1 }));
  const notify = (msg, type = 'success') => setToast({ open: true, message: msg, type });

  const handleDel = async () => {
    try {
      await employesApi.delete(modal.data.id);
      notify('Employé supprimé.');
      load(query);
    } catch (e) { notify(e.response?.data?.message || 'Erreur.', 'error'); }
    finally { setModal(null); }
  };

  const fmt = (d) => d ? new Date(d).toLocaleDateString('fr-FR') : '—';
  const init = (n, p) => `${(n||'').charAt(0)}${(p||'').charAt(0)}`.toUpperCase();

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-slate-900">Employés</h2>
          <p className="text-sm text-slate-500">{result.totalCount} employé(s) au total</p>
        </div>
        {isAdmin && (
          <Button icon={<Plus size={16} />} onClick={() => setModal({ type: 'form' })}>
            Nouvel employé
          </Button>
        )}
      </div>

      {/* Filtres */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-4">
        <div className="flex flex-wrap gap-3 items-end">
          {/* Recherche */}
          <div className="flex-1 min-w-[200px]">
            <label className="text-xs font-semibold text-slate-500 uppercase tracking-wide block mb-1.5">Rechercher</label>
            <div className="relative">
              <Search size={15} className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                placeholder="Nom, prénom, matricule..."
                value={searchInput}
                onChange={e => setSearchInput(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && upd({ search: searchInput })}
                className="w-full h-9 pl-9 pr-8 rounded-lg border border-slate-200 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all"
              />
              {searchInput && (
                <button onClick={() => { setSearchInput(''); upd({ search: '' }); }}
                  className="absolute right-2.5 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600">
                  <X size={13} />
                </button>
              )}
            </div>
          </div>

          {/* Direction */}
          <div className="min-w-[170px]">
            <label className="text-xs font-semibold text-slate-500 uppercase tracking-wide block mb-1.5">Direction</label>
            <select value={query.directionId} onChange={e => upd({ directionId: e.target.value, serviceId: '' })}
              className="w-full h-9 px-3 rounded-lg border border-slate-200 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all cursor-pointer">
              <option value="">Toutes</option>
              {dirs.map(d => <option key={d.id} value={d.id}>{d.nom}</option>)}
            </select>
          </div>

          {/* Service */}
          <div className="min-w-[160px]">
            <label className="text-xs font-semibold text-slate-500 uppercase tracking-wide block mb-1.5">Service</label>
            <select value={query.serviceId} onChange={e => upd({ serviceId: e.target.value })}
              disabled={!query.directionId}
              className="w-full h-9 px-3 rounded-lg border border-slate-200 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all cursor-pointer disabled:bg-slate-50 disabled:cursor-not-allowed">
              <option value="">Tous</option>
              {svcs.map(s => <option key={s.id} value={s.id}>{s.nom}</option>)}
            </select>
          </div>

          {/* Statut */}
          <div className="min-w-[130px]">
            <label className="text-xs font-semibold text-slate-500 uppercase tracking-wide block mb-1.5">Statut</label>
            <select value={query.statut} onChange={e => upd({ statut: e.target.value })}
              className="w-full h-9 px-3 rounded-lg border border-slate-200 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all cursor-pointer">
              {['','Actif','Inactif','Suspendu'].map(s => <option key={s} value={s}>{s||'Tous'}</option>)}
            </select>
          </div>

          <Button onClick={() => upd({ search: searchInput })} size="md">Filtrer</Button>
        </div>
      </div>

      {/* Table */}
      {loading ? <Spinner /> : (
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          {result.data.length === 0 ? (
            <EmptyState icon={Users} title="Aucun employé trouvé"
              description="Aucun employé ne correspond à vos critères de recherche."
              actionLabel={isAdmin ? "Ajouter un employé" : undefined}
              onAction={isAdmin ? () => setModal({ type: 'form' }) : undefined} />
          ) : (
            <>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="bg-slate-50 border-b border-slate-100">
                      {['Employé','Email','Poste','Direction','Service','Embauche','Statut', isAdmin ? 'Actions' : null]
                        .filter(Boolean).map(h => (
                        <th key={h} className={`px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider whitespace-nowrap ${h==='Actions'?'text-right':'text-left'}`}>{h}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-50">
                    {result.data.map(e => (
                      <tr key={e.id} className="hover:bg-slate-50/60 transition-colors group">
                        <td className="px-5 py-3.5">
                          <div className="flex items-center gap-3">
                            <div className="w-8 h-8 rounded-full bg-blue-100 text-blue-700 flex items-center justify-center text-xs font-bold shrink-0">
                              {init(e.nom, e.prenom)}
                            </div>
                            <div className="min-w-0">
                              <p className="text-sm font-semibold text-slate-900 truncate">{e.nom} {e.prenom}</p>
                              <p className="text-xs text-blue-600 font-medium">{e.matricule}</p>
                            </div>
                          </div>
                        </td>
                        <td className="px-5 py-3.5 text-sm text-slate-500 truncate max-w-[180px]">{e.email}</td>
                        <td className="px-5 py-3.5 text-sm text-slate-700 whitespace-nowrap">{e.poste}</td>
                        <td className="px-5 py-3.5">
                          <Badge variant="info">{e.directionNom}</Badge>
                        </td>
                        <td className="px-5 py-3.5 text-sm text-slate-500 whitespace-nowrap">{e.serviceNom}</td>
                        <td className="px-5 py-3.5 text-sm text-slate-500 whitespace-nowrap">{fmt(e.dateEmbauche)}</td>
                        <td className="px-5 py-3.5">{STATUS_BADGE[e.statut] || <Badge>{e.statut}</Badge>}</td>
                        {isAdmin && (
                          <td className="px-5 py-3.5">
                            <div className="flex items-center justify-end gap-1.5 opacity-0 group-hover:opacity-100 transition-opacity">
                              <button onClick={() => setModal({ type: 'form', data: e })}
                                className="p-1.5 rounded-lg hover:bg-blue-50 text-slate-400 hover:text-blue-600 transition-colors">
                                <Pencil size={14} />
                              </button>
                              <button onClick={() => setModal({ type: 'delete', data: e })}
                                className="p-1.5 rounded-lg hover:bg-red-50 text-slate-400 hover:text-red-600 transition-colors">
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

              {/* Pagination */}
              {result.totalPages > 1 && (
                <div className="flex items-center justify-between px-5 py-3 border-t border-slate-100 bg-slate-50/50">
                  <p className="text-xs text-slate-500">
                    Page {query.page} sur {result.totalPages} · {result.totalCount} résultats
                  </p>
                  <div className="flex items-center gap-1">
                    <button disabled={query.page === 1}
                      onClick={() => setQuery(p => ({ ...p, page: p.page - 1 }))}
                      className="p-1.5 rounded-lg hover:bg-white border border-transparent hover:border-slate-200 text-slate-500 disabled:opacity-30 disabled:cursor-not-allowed transition-all">
                      <ChevronLeft size={16} />
                    </button>
                    {Array.from({ length: Math.min(result.totalPages, 5) }, (_, i) => {
                      const p = i + 1;
                      return (
                        <button key={p} onClick={() => setQuery(prev => ({ ...prev, page: p }))}
                          className={`w-8 h-8 rounded-lg text-xs font-semibold transition-all ${query.page === p ? 'bg-blue-600 text-white' : 'hover:bg-white border border-transparent hover:border-slate-200 text-slate-600'}`}>
                          {p}
                        </button>
                      );
                    })}
                    <button disabled={query.page === result.totalPages}
                      onClick={() => setQuery(p => ({ ...p, page: p.page + 1 }))}
                      className="p-1.5 rounded-lg hover:bg-white border border-transparent hover:border-slate-200 text-slate-500 disabled:opacity-30 disabled:cursor-not-allowed transition-all">
                      <ChevronRight size={16} />
                    </button>
                  </div>
                </div>
              )}
            </>
          )}
        </div>
      )}

      {modal?.type === 'form' && <EmployeFormModal employe={modal.data} onClose={() => setModal(null)} onSuccess={(m) => { notify(m); load(query); }} />}
      {modal?.type === 'delete' && <ConfirmModal title="Supprimer l'employé" message={`Supprimer ${modal.data?.nom} ${modal.data?.prenom} ?`} onConfirm={handleDel} onCancel={() => setModal(null)} />}
      <Toast {...toast} onClose={() => setToast(t => ({ ...t, open: false }))} />
    </div>
  );
}
