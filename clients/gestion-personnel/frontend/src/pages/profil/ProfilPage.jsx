import { useEffect, useState } from 'react';
import {
  Shield, Mail, Phone, MapPin, Briefcase,
  Building2, GitBranch, Calendar, User, Save
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import Button from '../../components/ui/Button';
import Toast from '../../components/ui/Toast';
import axiosInstance from '../../api/axiosInstance';

const ROLES = {
  AdministrateurRH: 'Administrateur RH',
  Directeur:        'Directeur',
  ChefDeService:    'Chef de Service',
  Employe:          'Employé',
};

const COLORS = {
  AdministrateurRH: 'bg-blue-600',
  Directeur:        'bg-emerald-600',
  ChefDeService:    'bg-amber-600',
  Employe:          'bg-purple-600',
};

export default function ProfilPage() {
  const { user }                            = useAuth();
  const [employe,  setEmploye]              = useState(null);
  const [form,     setForm]                 = useState({ telephone: '', adresse: '' });
  const [loading,  setLoading]              = useState(false);
  const [saving,   setSaving]               = useState(false);
  const [toast,    setToast]                = useState({ open: false, message: '', type: 'success' });

  const colorClass = COLORS[user?.role] || 'bg-blue-600';
  const notify = (msg, type = 'success') => setToast({ open: true, message: msg, type });

  useEffect(() => {
    setLoading(true);
    axiosInstance.get('/employes', { params: { pageSize: 1000 } })
      .then(({ data }) => {
        const me = (data.data ?? [])[0] ?? null;
        if (me) {
          setEmploye(me);
          setForm({ telephone: me.telephone || '', adresse: me.adresse || '' });
        }
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  const handleSave = async () => {
    if (!employe) return;
    setSaving(true);
    try {
      await axiosInstance.patch(`/employes/${employe.id}/profil`, {
        telephone: form.telephone || null,
        adresse:   form.adresse   || null,
      });
      notify('Profil mis à jour avec succès.');
    } catch (e) {
      notify(e.response?.data?.message || 'Erreur lors de la mise à jour.', 'error');
    } finally { setSaving(false); }
  };

  const fmtDate = d => d ? new Date(d).toLocaleDateString('fr-FR') : '—';

  return (
    <div className="max-w-2xl space-y-5">
      {/* Header */}
      <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
        <div className="h-20 bg-gradient-to-r from-blue-600 to-indigo-600" />
        <div className="px-6 pb-5">
          <div className="flex items-end gap-4 -mt-7 mb-4">
            <div className={`w-14 h-14 rounded-2xl ${colorClass} flex items-center justify-center text-white text-xl font-black border-4 border-white shadow-md`}>
              {user?.username?.charAt(0).toUpperCase()}
            </div>
            <div className="pb-1">
              <p className="text-base font-bold text-slate-900">{user?.username}</p>
              <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-semibold bg-blue-50 text-blue-700 border border-blue-100">
                <Shield size={10} /> {ROLES[user?.role] || user?.role}
              </span>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-2">
            {[
              { icon: Mail,      label: 'Email',         value: user?.email },
              { icon: Briefcase, label: 'Poste',          value: employe?.poste },
              { icon: Building2, label: 'Direction',      value: employe?.directionNom },
              { icon: GitBranch, label: 'Service',        value: employe?.serviceNom },
              { icon: Calendar,  label: "Date d'embauche",value: fmtDate(employe?.dateEmbauche) },
              { icon: User,      label: 'Responsable',    value: employe?.responsableNom || '—' },
            ].map(f => (
              <div key={f.label} className="flex items-center gap-2.5 p-3 rounded-xl bg-slate-50 border border-slate-100">
                <f.icon size={14} className="text-slate-400 shrink-0" />
                <div className="min-w-0">
                  <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">{f.label}</p>
                  <p className="text-sm font-semibold text-slate-800 truncate">{f.value || '—'}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Champs modifiables */}
      <div className="bg-white rounded-2xl border border-slate-200 p-5">
        <div className="mb-4">
          <h3 className="text-sm font-semibold text-slate-900">Informations personnelles</h3>
          <p className="text-xs text-slate-500">Modifiables par vous-même</p>
        </div>
        <div className="space-y-3">
          {[
            { icon: Phone,  label: 'Téléphone', field: 'telephone', placeholder: '06 00 00 00 00' },
            { icon: MapPin, label: 'Adresse',   field: 'adresse',   placeholder: '12 Rue Hassan II, Casablanca' },
          ].map(f => (
            <div key={f.field} className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide flex items-center gap-1.5">
                <f.icon size={12} /> {f.label}
              </label>
              <input
                value={form[f.field]}
                onChange={e => setForm(p => ({ ...p, [f.field]: e.target.value }))}
                placeholder={f.placeholder}
                className="h-9 w-full rounded-lg border border-slate-200 hover:border-slate-300 text-sm px-3 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all"
              />
            </div>
          ))}
        </div>

        <div className="mt-5">
          <Button onClick={handleSave} loading={saving} icon={<Save size={15} />} disabled={!employe}>
            Enregistrer les modifications
          </Button>
        </div>

        {/* ──────────────────────────────────────────────────────
            NOTE : "Changer le mot de passe" supprimé.
            Les mots de passe sont gérés par le microservice SSO.
            ────────────────────────────────────────────────────── */}
      </div>

      {/* Note SSO */}
      <div className="p-3 rounded-xl bg-blue-50 border border-blue-100">
        <p className="text-xs text-blue-700 font-medium">
          🔒 Le rôle, la direction et le service sont gérés par l'Administrateur RH.
          Après intégration du SSO, la gestion des comptes et des mots de passe sera
          centralisée dans le portail SSO.
        </p>
      </div>

      <Toast {...toast} onClose={() => setToast(t => ({ ...t, open: false }))} />
    </div>
  );
}
