import { Clock, CheckCircle, XCircle, User, Building2, GitBranch, Calendar } from 'lucide-react';
import CongeBadge from '../../components/ui/CongeBadge';

export default function EmployeDashboard({ data }) {
  const p = data.profil;
  const fmtDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR') : '—';

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-lg font-bold text-slate-900">Mon espace</h2>
        <p className="text-sm text-slate-500">Bienvenue, {p?.prenom} {p?.nom}</p>
      </div>

      {/* Profil card */}
      {p && (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          <div className="h-16 bg-gradient-to-r from-purple-600 to-indigo-600" />
          <div className="px-5 pb-5">
            <div className="flex items-end gap-4 -mt-6 mb-4">
              <div className="w-14 h-14 rounded-2xl bg-purple-600 flex items-center justify-center text-white text-xl font-black border-4 border-white shadow">
                {p.nom[0]}{p.prenom[0]}
              </div>
              <div className="pb-1">
                <p className="text-base font-bold text-slate-900">{p.nom} {p.prenom}</p>
                <p className="text-sm text-slate-500">{p.poste}</p>
              </div>
            </div>
            <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
              {[
                { icon: Building2,  label: 'Direction', value: p.directionNom },
                { icon: GitBranch,  label: 'Service',   value: p.serviceNom },
                { icon: Calendar,   label: 'Embauche',  value: fmtDate(p.dateEmbauche) },
                { icon: User,       label: 'Matricule', value: p.matricule },
                { icon: User,       label: 'Responsable', value: p.responsableNom || '—' },
              ].map(f => (
                <div key={f.label} className="flex items-center gap-2.5 p-3 rounded-xl bg-slate-50 border border-slate-100">
                  <f.icon size={14} className="text-slate-400 shrink-0" />
                  <div>
                    <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wide">{f.label}</p>
                    <p className="text-sm font-semibold text-slate-800">{f.value}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* Stats congés */}
      <div className="grid grid-cols-3 gap-3">
        {[
          { label: 'En attente', value: data.congesEnAttente, icon: Clock,       bg: 'bg-amber-50',   color: 'text-amber-600' },
          { label: 'Acceptés',   value: data.congesAcceptes,  icon: CheckCircle, bg: 'bg-emerald-50', color: 'text-emerald-600' },
          { label: 'Refusés',    value: data.congesRefuses,   icon: XCircle,     bg: 'bg-red-50',     color: 'text-red-500' },
        ].map(s => (
          <div key={s.label} className="bg-white rounded-2xl border border-slate-200 p-4 flex items-center gap-3">
            <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 ${s.bg}`}>
              <s.icon size={18} className={s.color} />
            </div>
            <div>
              <p className="text-xs text-slate-500 font-medium">{s.label}</p>
              <p className="text-xl font-black text-slate-900">{s.value ?? 0}</p>
            </div>
          </div>
        ))}
      </div>

      {/* Dernières demandes */}
      {data.dernieresDemandesConge?.length > 0 && (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          <div className="px-5 py-3.5 border-b border-slate-100">
            <h3 className="text-sm font-semibold text-slate-900">Mes dernières demandes de congé</h3>
          </div>
          <div className="divide-y divide-slate-50">
            {data.dernieresDemandesConge.map(c => (
              <div key={c.id} className="px-5 py-3 flex items-center justify-between gap-4">
                <div>
                  <p className="text-sm font-semibold text-slate-900">{c.motif}</p>
                  <p className="text-xs text-slate-500">
                    {new Date(c.dateDebut).toLocaleDateString('fr-FR')} →{' '}
                    {new Date(c.dateFin).toLocaleDateString('fr-FR')} · {c.nombreJours}j
                  </p>
                </div>
                <CongeBadge statut={c.statut} />
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
