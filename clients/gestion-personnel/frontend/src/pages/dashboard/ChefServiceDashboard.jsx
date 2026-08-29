import { Users, Clock, CheckCircle, XCircle } from 'lucide-react';
import Badge from '../../components/ui/Badge';

export default function ChefServiceDashboard({ data }) {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-lg font-bold text-slate-900">Tableau de bord</h2>
        <p className="text-sm text-slate-500">{data.serviceNom} — Chef de service</p>
      </div>

      <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
        {[
          { label: 'Employés du service', value: data.totalEmployes,   icon: Users,        bg: 'bg-blue-50',    color: 'text-blue-600' },
          { label: 'Congés en attente',   value: data.congesEnAttente, icon: Clock,        bg: 'bg-amber-50',   color: 'text-amber-600' },
          { label: 'Congés acceptés',     value: data.congesAcceptes,  icon: CheckCircle,  bg: 'bg-emerald-50', color: 'text-emerald-600' },
          { label: 'Congés refusés',      value: data.congesRefuses,   icon: XCircle,      bg: 'bg-red-50',     color: 'text-red-500' },
        ].map(s => (
          <div key={s.label} className="bg-white rounded-2xl border border-slate-200 p-5 flex items-center gap-3">
            <div className={`w-10 h-10 rounded-xl flex items-center justify-center shrink-0 ${s.bg}`}>
              <s.icon size={18} className={s.color} />
            </div>
            <div>
              <p className="text-xs font-medium text-slate-500 leading-tight">{s.label}</p>
              <p className="text-xl font-black text-slate-900">{s.value ?? 0}</p>
            </div>
          </div>
        ))}
      </div>

      {/* Liste employés du service */}
      {data.employes?.length > 0 && (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          <div className="px-5 py-3.5 border-b border-slate-100">
            <h3 className="text-sm font-semibold text-slate-900">Employés du service</h3>
          </div>
          <div className="divide-y divide-slate-50">
            {data.employes.map(e => (
              <div key={e.id} className="px-5 py-3 flex items-center gap-3">
                <div className="w-8 h-8 rounded-full bg-blue-100 text-blue-700 flex items-center justify-center text-xs font-bold shrink-0">
                  {e.nom[0]}{e.prenom[0]}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-semibold text-slate-900 truncate">{e.nom} {e.prenom}</p>
                  <p className="text-xs text-slate-500 truncate">{e.poste} · {e.matricule}</p>
                </div>
                <Badge variant={e.statut === 'Actif' ? 'success' : e.statut === 'Suspendu' ? 'warning' : 'neutral'}>
                  {e.statut}
                </Badge>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
