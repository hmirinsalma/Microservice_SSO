import { Users, Building2, GitBranch, Clock, FileText } from 'lucide-react';
import Badge from '../../components/ui/Badge';

function Stat({ label, value, sub, icon: Icon, iconBg, iconColor }) {
  return (
    <div className="bg-white rounded-2xl border border-slate-200 p-5 flex items-center gap-4">
      <div className={`w-11 h-11 rounded-xl flex items-center justify-center shrink-0 ${iconBg}`}>
        <Icon size={20} className={iconColor} />
      </div>
      <div>
        <p className="text-xs font-medium text-slate-500">{label}</p>
        <p className="text-2xl font-black text-slate-900 leading-none mt-0.5">{value ?? 0}</p>
        {sub && <p className="text-xs text-slate-400 mt-0.5">{sub}</p>}
      </div>
    </div>
  );
}

export default function AdminDashboard({ data }) {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-lg font-bold text-slate-900">Tableau de bord</h2>
        <p className="text-sm text-slate-500">Vue globale — Administrateur RH</p>
      </div>

      {/* Stats globales */}
      <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3">
        <Stat label="Employés"         value={data.totalEmployes}        icon={Users}    iconBg="bg-blue-50"    iconColor="text-blue-600" />
        <Stat label="Directions"       value={data.totalDirections}      icon={Building2} iconBg="bg-emerald-50" iconColor="text-emerald-600" />
        <Stat label="Services"         value={data.totalServices}        icon={GitBranch} iconBg="bg-amber-50"   iconColor="text-amber-600" />
        <Stat label="Congés total"     value={data.totalConges}          icon={FileText}  iconBg="bg-slate-50"   iconColor="text-slate-600" />
        <Stat label="Congés en attente" value={data.totalCongesEnAttente} sub="À traiter" icon={Clock} iconBg="bg-red-50" iconColor="text-red-500" />
      </div>

      {/* Employés par direction */}
      {data.employesParDirection?.length > 0 && (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          <div className="px-5 py-3.5 border-b border-slate-100">
            <h3 className="text-sm font-semibold text-slate-900">Répartition par direction</h3>
          </div>
          <div className="divide-y divide-slate-50">
            {data.employesParDirection.map(d => (
              <div key={d.nom} className="px-5 py-3 flex items-center justify-between">
                <span className="text-sm font-medium text-slate-700">{d.nom}</span>
                <div className="flex items-center gap-3">
                  <div className="w-32 h-2 rounded-full bg-slate-100 overflow-hidden">
                    <div className="h-full rounded-full bg-blue-500 transition-all"
                      style={{ width: `${data.totalEmployes ? (d.nombreEmployes / data.totalEmployes) * 100 : 0}%` }} />
                  </div>
                  <span className="text-sm font-bold text-slate-900 w-6 text-right">{d.nombreEmployes}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Derniers employés */}
      {data.derniersEmployes?.length > 0 && (
        <div className="bg-white rounded-2xl border border-slate-200 overflow-hidden">
          <div className="px-5 py-3.5 border-b border-slate-100 flex items-center gap-2">
            <Clock size={15} className="text-blue-600" />
            <h3 className="text-sm font-semibold text-slate-900">Derniers employés ajoutés</h3>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-100">
                  {['Employé','Poste','Direction','Service','Statut'].map(h => (
                    <th key={h} className="px-5 py-2.5 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {data.derniersEmployes.map(e => (
                  <tr key={e.id} className="hover:bg-slate-50/50">
                    <td className="px-5 py-3">
                      <div className="flex items-center gap-2.5">
                        <div className="w-7 h-7 rounded-full bg-blue-100 text-blue-700 flex items-center justify-center text-xs font-bold shrink-0">
                          {e.nom[0]}{e.prenom[0]}
                        </div>
                        <div>
                          <p className="text-sm font-semibold text-slate-900">{e.nom} {e.prenom}</p>
                          <p className="text-xs text-blue-600">{e.matricule}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-5 py-3 text-sm text-slate-600">{e.poste}</td>
                    <td className="px-5 py-3"><Badge variant="info">{e.directionNom}</Badge></td>
                    <td className="px-5 py-3 text-sm text-slate-500">{e.serviceNom}</td>
                    <td className="px-5 py-3">
                      <Badge variant={e.statut === 'Actif' ? 'success' : e.statut === 'Suspendu' ? 'warning' : 'neutral'}>
                        {e.statut}
                      </Badge>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
