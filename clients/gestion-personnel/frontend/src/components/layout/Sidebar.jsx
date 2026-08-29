import { NavLink, useNavigate } from 'react-router-dom';
import {
  LayoutDashboard, Users, Building2,
  GitBranch, LogOut, UserCog, CalendarDays
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { clsx } from 'clsx';

const ROLE_LABELS = {
  AdministrateurRH: 'Admin RH',
  Directeur:        'Directeur',
  ChefDeService:    'Chef de Service',
  Employe:          'Employé',
};

function SidebarContent({ onClose }) {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  const NAV = [
    { label: 'Dashboard',    path: '/',             icon: LayoutDashboard, always: true },
    { label: 'Employés',     path: '/employes',     icon: Users,           always: true },
    { label: 'Directions',   path: '/directions',   icon: Building2,       always: true },
    { label: 'Services',     path: '/services',     icon: GitBranch,      always: true },
    { label: 'Congés',       path: '/conges',       icon: CalendarDays,   always: true },
    { label: 'Utilisateurs', path: '/utilisateurs', icon: UserCog,        adminOnly: true },
  ].filter(item => !item.adminOnly || isAdmin);

  const go = (path) => { navigate(path); if (onClose) onClose(); };

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <div className="flex flex-col h-full bg-slate-900 select-none">

      {/* ── Logo ── */}
      <div className="flex items-center gap-3 px-5 h-16 border-b border-slate-800 shrink-0">
        <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center font-black text-white text-sm shrink-0 shadow-md">
          GP
        </div>
        <div className="leading-tight">
          <p className="text-sm font-bold text-white">Gestion Personnel</p>
          <p className="text-[10px] text-slate-400 font-medium uppercase tracking-wider">ONEE · RH</p>
        </div>
      </div>

      {/* ── Navigation ── */}
      <nav className="flex-1 overflow-y-auto px-3 py-5 space-y-0.5">
        <p className="px-3 mb-3 text-[10px] font-bold text-slate-500 uppercase tracking-widest">
          Navigation
        </p>

        {NAV.map(({ label, path, icon: Icon, adminOnly }) => (
          <NavLink
            key={path}
            to={path}
            end={path === '/'}
            onClick={() => onClose?.()}
            className={({ isActive }) => clsx(
              'flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-medium transition-all duration-150',
              isActive
                ? 'bg-blue-600 text-white shadow-sm shadow-blue-900/40'
                : 'text-slate-400 hover:text-white hover:bg-slate-800'
            )}
          >
            <Icon size={17} className="shrink-0" />
            <span>{label}</span>
            {adminOnly && (
              <span className="ml-auto text-[9px] font-bold bg-blue-500/30 text-blue-300 px-1.5 py-0.5 rounded uppercase tracking-wide">
                Admin
              </span>
            )}
          </NavLink>
        ))}
      </nav>

      {/* ── Pied de sidebar ── */}
      <div className="shrink-0 border-t border-slate-800 px-3 py-4 space-y-1">
        {/* Profil */}
        <NavLink to="/profil" onClick={() => onClose?.()}
          className={({ isActive }) => clsx(
            'flex items-center gap-3 px-3 py-2.5 rounded-xl w-full transition-all duration-150',
            isActive ? 'bg-slate-700' : 'hover:bg-slate-800'
          )}>
          <div className="w-8 h-8 rounded-full bg-blue-600 flex items-center justify-center text-white font-bold text-sm shrink-0">
            {user?.username?.charAt(0).toUpperCase()}
          </div>
          <div className="flex-1 min-w-0 text-left">
            <p className="text-sm font-semibold text-white truncate leading-tight">{user?.username}</p>
            <p className="text-xs text-slate-400 truncate leading-tight">
              {ROLE_LABELS[user?.role] || user?.role}
            </p>
          </div>
        </NavLink>

        {/* Déconnexion */}
        <button
          onClick={handleLogout}
          className="flex items-center gap-3 px-3 py-2.5 rounded-xl w-full text-sm font-medium text-slate-400 hover:text-red-400 hover:bg-slate-800 transition-all duration-150"
        >
          <LogOut size={16} className="shrink-0" />
          Se déconnecter
        </button>
      </div>
    </div>
  );
}

export default function Sidebar({ mobileOpen, onClose }) {
  return (
    <>
      {/* ── Mobile overlay ── */}
      {mobileOpen && (
        <div className="fixed inset-0 z-40 md:hidden">
          <div className="absolute inset-0 bg-black/50 backdrop-blur-sm" onClick={onClose} />
          <div className="absolute left-0 top-0 bottom-0 w-64" onClick={e => e.stopPropagation()}>
            <SidebarContent onClose={onClose} />
          </div>
        </div>
      )}

      {/* ── Desktop permanent ── */}
      <div className="hidden md:flex flex-col w-64 shrink-0 fixed top-0 left-0 bottom-0 z-30">
        <SidebarContent />
      </div>
    </>
  );
}
