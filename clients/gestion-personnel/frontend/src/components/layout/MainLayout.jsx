import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Topbar from './Topbar';

export default function MainLayout() {
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <div className="flex min-h-screen bg-slate-50">
      <Sidebar mobileOpen={mobileOpen} onClose={() => setMobileOpen(false)} />

      {/* Zone droite */}
      <div className="flex-1 flex flex-col min-w-0 md:ml-64">
        <Topbar onMenuClick={() => setMobileOpen(true)} />

        {/* Contenu */}
        <main className="flex-1 overflow-auto">
          <div className="max-w-[1400px] mx-auto px-6 py-8">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}
