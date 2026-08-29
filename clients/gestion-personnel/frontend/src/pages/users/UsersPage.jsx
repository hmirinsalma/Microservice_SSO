import { useEffect, useState, useCallback } from 'react';
import { Plus, Pencil, Trash2, ShieldCheck, ShieldOff, Users, CheckCircle, XCircle } from 'lucide-react';
import usersApi from '../../api/usersApi';
import Button from '../../components/ui/Button';
import Badge from '../../components/ui/Badge';
import Spinner from '../../components/ui/Spinner';
import EmptyState from '../../components/ui/EmptyState';
import Toast from '../../components/ui/Toast';
import UserFormModal from './UserFormModal';
import ConfirmModal from '../../components/common/ConfirmModal';

const ROLE_BADGE = {
  AdministrateurRH: <Badge variant="info">Admin RH</Badge>,
  Directeur:        <Badge variant="success">Directeur</Badge>,
  ChefDeService:    <Badge variant="warning">Chef de Service</Badge>,
  Employe:          <Badge variant="neutral">Employé</Badge>,
};

export default function UsersPage() {
  const [users,   setUsers]   = useState([]);
  const [loading, setLoading] = useState(true);
  const [modal,   setModal]   = useState(null);
  const [toast,   setToast]   = useState({ open: false, message: '', type: 'success' });

  const load = useCallback(async () => {
    setLoading(true);
    try { const { data } = await usersApi.getAll(); setUsers(data); }
    catch {}
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  const notify = (msg, type = 'success') => setToast({ open: true, message: msg, type });

  const handleToggle = async (user) => {
    try {
      await usersApi.toggleActive(user.id);
      notify(user.isActive ? 'Compte désactivé.' : 'Compte activé.');
      load();
    } catch { notify('Erreur.', 'error'); }
  };

  const handleDelete = async () => {
    try {
      await usersApi.delete(modal.data.id);
      notify('Compte supprimé.');
      load();
    } catch (e) {
      notify(e.response?.data?.message || 'Erreur.', 'error');
    } finally { setModal(null); }
  };

  const fmtDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR') : '—';
  const actifs   = users.filter(u => u.isActive).length;
  const inactifs = users.length - actifs;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-bold text-slate-900">Gestion des utilisateurs</h2>
          <p className="text-sm text-slate-500">{users.length} compte(s) enregistré(s)</p>
        </div>
        <Button icon={<Plus size={16} />} onClick={() => setModal({ type: 'form' })}>
          Nouveau compte
        </Button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-3 gap-4">
        {[
          { label: 'Total',    value: users.length, icon: Users,       bg: 'bg-slate-50',   color: 'text-slate-600',   border: 'border-slate-200' },
          { label: 'Actifs',   value: actifs,       icon: CheckCircle, bg: 'bg-emerald-50', color: 'text-emerald-600', border: 'border-emerald-200' },
          { label: 'Inactifs', value: inactifs,     icon: XCircle,     bg: 'bg-red-50',     color: 'text-red-500',     border: 'border-red-200' },
        ].map(s => (
          <div key={s.label} className={`bg-white rounded-2xl border ${s.border} p-4 flex items-center gap-4`}>
            <div className={`w-10 h-10 rounded-xl ${s.bg} flex items-center justify-center shrink-0`}>
              <s.icon size={20} className={s.color} />
            </div>
            <div>
              <p className="text-xs text-slate-500 font-medium">{s.label}</p>
              <p className="text-2xl font-black text-slate-900 leading-none">{s.value}</p>
            </div>
          </div>
        ))}
      </div>

      {/* Note SSO */}
      <div className="p-4 rounded-xl bg-amber-50 border border-amber-200 flex gap-3">
        <span className="text-lg">⚠️</span>
        <div>
          <p className="text-sm font-semibold text-amber-800">Authentification locale temporaire (Stub)</p>
          <p className="text-xs text-amber-700 mt-0.5">
            Ces comptes seront remplacés par le microservice SSO. Le module Gestion des mots de passe
            sera supprimé lors de l'intégration SSO — il sera géré directement par le SSO.
          </p>
        </div>
      </div>

      {/* Tableau */}
      {loading ? <Spinner /> : (
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          {users.length === 0 ? (
            <EmptyState icon={Users} title="Aucun compte utilisateur"
              description="Créez le premier compte pour permettre l'accès à l'application."
              actionLabel="Créer un compte" onAction={() => setModal({ type: 'form' })} />
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="bg-slate-50 border-b border-slate-100">
                    {['Utilisateur','Email','Rôle','Statut','Créé le','Actions'].map(h => (
                      <th key={h} className={`px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider ${h === 'Actions' ? 'text-right' : 'text-left'}`}>
                        {h}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {users.map(u => (
                    <tr key={u.id} className={`hover:bg-slate-50/60 transition-colors group ${!u.isActive ? 'opacity-60' : ''}`}>
                      <td className="px-5 py-3.5">
                        <div className="flex items-center gap-3">
                          <div className={`w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold shrink-0
                            ${u.isActive ? 'bg-blue-100 text-blue-700' : 'bg-slate-100 text-slate-500'}`}>
                            {u.username?.charAt(0).toUpperCase()}
                          </div>
                          <span className="text-sm font-semibold text-slate-900">{u.username}</span>
                        </div>
                      </td>
                      <td className="px-5 py-3.5 text-sm text-slate-500">{u.email}</td>
                      <td className="px-5 py-3.5">{ROLE_BADGE[u.role] || <Badge>{u.role}</Badge>}</td>
                      <td className="px-5 py-3.5">
                        {u.isActive
                          ? <Badge variant="success">Actif</Badge>
                          : <Badge variant="danger">Inactif</Badge>}
                      </td>
                      <td className="px-5 py-3.5 text-sm text-slate-500">{fmtDate(u.createdAt)}</td>
                      <td className="px-5 py-3.5">
                        <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button onClick={() => setModal({ type: 'form', data: u })} title="Modifier"
                            className="p-1.5 rounded-lg hover:bg-blue-50 text-slate-400 hover:text-blue-600 transition-colors">
                            <Pencil size={14} />
                          </button>
                          <button onClick={() => handleToggle(u)}
                            title={u.isActive ? 'Désactiver' : 'Activer'}
                            className="p-1.5 rounded-lg hover:bg-amber-50 text-slate-400 hover:text-amber-600 transition-colors">
                            {u.isActive ? <ShieldOff size={14} /> : <ShieldCheck size={14} />}
                          </button>
                          <button onClick={() => setModal({ type: 'delete', data: u })} title="Supprimer"
                            className="p-1.5 rounded-lg hover:bg-red-50 text-slate-400 hover:text-red-600 transition-colors">
                            <Trash2 size={14} />
                          </button>
                          {/* ────────────────────────────────────────────
                              NOTE : Bouton "Changer mot de passe" supprimé.
                              Les mots de passe seront gérés par le SSO.
                              ──────────────────────────────────────────── */}
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

      {modal?.type === 'form' && (
        <UserFormModal user={modal.data} onClose={() => setModal(null)}
          onSuccess={(m) => { notify(m); load(); }} />
      )}
      {modal?.type === 'delete' && (
        <ConfirmModal title="Supprimer le compte"
          message={`Supprimer définitivement le compte "${modal.data?.username}" ?`}
          onConfirm={handleDelete} onCancel={() => setModal(null)} />
      )}
      <Toast {...toast} onClose={() => setToast(t => ({ ...t, open: false }))} />
    </div>
  );
}
