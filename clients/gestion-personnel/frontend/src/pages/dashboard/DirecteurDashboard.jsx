import { Users, GitBranch, Clock } from 'lucide-react';
import Badge from '../../components/ui/Badge';

export default function DirecteurDashboard({ data }) {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-lg font-bold text-slate-900">Tableau de bord</h2>
        <p className="text-sm text-slate-500">{data.directionNom} — Directeur</p>
      </div>

      <div className="grid grid-cols-3 gap-3">
        {[
          { label: 'Employés',         value: data.totalEmployes,   icon: Users,    bg: 'bg-blue-50',    color: 'text-blue-600' },
          { label: 'Services',         value: data.totalServices,   icon: GitBranch, bg: 'bg-emerald-50', color: 'text-emerald-600' },
          { label: 'Congés à valider', value: data.congesEnAttente, icon: Clock,    bg: 'bg-amber-50',   color: 'text-amber-600' },
        ].map(s => (
          <div key={s.label} className="bg-white rounded-2xl border border-slate-200 p-5 flex items-center gap-4">
            <div className={`w-11 h-11 rounded-xl flex items-center justify-center shrink-0 ${s.bg}`}>
              <s.icon size={20} className={s.color} />
            </div>
            <div>
              <p className="text-xs font-medium text-slate-500">{s.label}</p>
              <p className="text-2xl font-black text-slate-900">{s.value ?? 0}</p>
            </div>
          </div>
        ))}
      </div>

      {/* Employés par service */}
      {data.employesParService?.length > 0 && (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          <div className="px-5 py-3.5 border-b border-slate-100">
            <h3 className="text-sm font-semibold text-slate-900">Répartition par service</h3>
          </div>
          <div className="divide-y divide-slate-50">
            {data.employesParService.map(s => (
              <div key={s.nom} className="px-5 py-3 flex items-center justify-between">
                <span className="text-sm text-slate-700 font-medium">{s.nom}</span>
                <Badge variant="info">{s.nombreEmployes} employé{s.nombreEmployes !== 1 ? 's' : ''}</Badge>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Derniers recrutés */}
      {data.derniersRecrutes?.length > 0 && (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          <div className="px-5 py-3.5 border-b border-slate-100">
            <h3 className="text-sm font-semibold text-slate-900">Derniers recrutés</h3>
          </div>
          <div className="divide-y divide-slate-50">
            {data.derniersRecrutes.map(e => (
              <div key={e.id} className="px-5 py-3 flex items-center gap-3">
                <div className="w-8 h-8 rounded-full bg-blue-100 text-blue-700 flex items-center justify-center text-xs font-bold shrink-0">
                  {e.nom[0]}{e.prenom[0]}
                </div>
                <div className="flex-1">
                  <p className="text-sm font-semibold text-slate-900">{e.nom} {e.prenom}</p>
                  <p className="text-xs text-slate-500">{e.poste} · {e.serviceNom}</p>
                </div>
                <Badge variant={e.statut === 'Actif' ? 'success' : 'neutral'}>{e.statut}</Badge>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
