import { Menu } from 'lucide-react';
import { useLocation } from 'react-router-dom';

const TITLES = {
  '/':               { title: 'Tableau de bord',        sub: 'Vue d\'ensemble de votre organisation' },
  '/employes':       { title: 'Employés',               sub: 'Gérer les membres de votre équipe' },
  '/directions':     { title: 'Directions',             sub: 'Gérer les unités organisationnelles' },
  '/services':       { title: 'Services',               sub: 'Gérer les sous-unités de chaque direction' },
  '/conges':         { title: 'Congés',               sub: 'Gérer les demandes de congé' },
  '/utilisateurs':   { title: 'Utilisateurs',           sub: 'Gérer les comptes d\'accès à l\'application' },
  '/profil':         { title: 'Mon profil',             sub: 'Informations de votre compte' },
};

export default function Topbar({ onMenuClick }) {
  const { pathname } = useLocation();
  const info = TITLES[pathname] || { title: 'Gestion du Personnel', sub: '' };

  return (
    <header className="h-16 bg-white border-b border-slate-200 flex items-center px-6 gap-4 shrink-0">
      {/* Burger mobile */}
      <button
        onClick={onMenuClick}
        className="md:hidden p-2 rounded-lg hover:bg-slate-100 text-slate-500 transition-colors"
      >
        <Menu size={20} />
      </button>

      {/* Titre de la page courante */}
      <div className="flex-1 min-w-0">
        <h1 className="text-base font-bold text-slate-900 leading-tight truncate">
          {info.title}
        </h1>
        {info.sub && (
          <p className="text-xs text-slate-500 leading-tight hidden sm:block">{info.sub}</p>
        )}
      </div>
    </header>
  );
}
