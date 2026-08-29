import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { CheckCircle, ArrowRight } from 'lucide-react';
import authService from '../auth/authService';

/**
 * Page de succès SSO (optionnelle)
 * Affichée après une authentification réussie
 */
export default function SsoSuccess() {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);

  useEffect(() => {
    const loadUser = async () => {
      const profile = await authService.getUserProfile();
      setUser(profile);
    };
    loadUser();
  }, []);

  const handleContinue = () => {
    navigate('/dashboard');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-emerald-50 via-blue-50 to-purple-50 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="bg-white rounded-2xl shadow-2xl overflow-hidden">
          <div className="h-1 bg-gradient-to-r from-emerald-500 via-blue-500 to-purple-500" />
          
          <div className="p-8 text-center">
            {/* Icône de succès */}
            <div className="flex justify-center mb-6">
              <div className="w-20 h-20 rounded-full bg-emerald-100 flex items-center justify-center">
                <CheckCircle size={48} className="text-emerald-600" />
              </div>
            </div>

            {/* Titre */}
            <h1 className="text-2xl font-bold text-slate-900 mb-2">
              Authentification réussie !
            </h1>
            
            <p className="text-slate-600 mb-6">
              Vous êtes maintenant connecté via ONEE SSO
            </p>

            {/* Informations utilisateur */}
            {user && (
              <div className="bg-slate-50 rounded-lg p-4 mb-6 text-left">
                <div className="space-y-2 text-sm">
                  <div>
                    <span className="font-semibold text-slate-700">Email : </span>
                    <span className="text-slate-600">{user.email}</span>
                  </div>
                  <div>
                    <span className="font-semibold text-slate-700">Nom : </span>
                    <span className="text-slate-600">{user.name || 'N/A'}</span>
                  </div>
                  {user.roles && user.roles.length > 0 && (
                    <div>
                      <span className="font-semibold text-slate-700">Rôle : </span>
                      <span className="text-slate-600">{user.roles[0]}</span>
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Bouton continuer */}
            <button
              onClick={handleContinue}
              className="w-full h-12 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white rounded-lg font-semibold flex items-center justify-center gap-2 transition-all shadow-lg shadow-blue-200 hover:shadow-xl"
            >
              Continuer vers le dashboard
              <ArrowRight size={20} />
            </button>

            {/* Badge SSO */}
            <div className="mt-6 pt-4 border-t border-slate-100">
              <p className="text-xs text-emerald-600 font-medium">
                🔐 Sécurisé par ONEE SSO
              </p>
            </div>
          </div>
        </div>

        <p className="text-center text-xs text-slate-500 mt-4">
          ONEE — Système de Gestion RH © 2024
        </p>
      </div>
    </div>
  );
}
