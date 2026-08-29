import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Eye, EyeOff, Lock, Shield } from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import Button from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import authService from '../../auth/authService';

export default function LoginPage() {
  const { login, loading, error } = useAuth();
  const navigate = useNavigate();
  const [form, setForm]         = useState({ email: '', password: '' });
  const [showPass, setShowPass] = useState(false);
  const [errs, setErrs]         = useState({});

  const validate = () => {
    const e = {};
    if (!form.email)    e.email    = 'Email requis';
    else if (!/\S+@\S+\.\S+/.test(form.email)) e.email = 'Email invalide';
    if (!form.password) e.password = 'Mot de passe requis';
    setErrs(e);
    return !Object.keys(e).length;
  };

  const onSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;
    if (await login(form)) navigate('/', { replace: true });
  };

  // Nouvelle fonction pour login SSO
  const handleSSOLogin = () => {
    authService.login();
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-blue-950 to-slate-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Card */}
        <div className="bg-white rounded-2xl shadow-2xl overflow-hidden">
          {/* Top accent */}
          <div className="h-1 bg-gradient-to-r from-blue-500 via-blue-600 to-indigo-600" />

          <div className="p-8">
            {/* Logo */}
            <div className="flex flex-col items-center mb-8">
              <div className="w-14 h-14 rounded-2xl bg-blue-600 flex items-center justify-center mb-4 shadow-lg shadow-blue-200">
                <Lock size={24} className="text-white" />
              </div>
              <h1 className="text-xl font-bold text-slate-900">Gestion du Personnel</h1>
              <p className="text-sm text-slate-500 mt-1">Connectez-vous à votre espace</p>
            </div>

            {/* Erreur API */}
            {error && (
              <div className="mb-4 p-3 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700 font-medium">
                {error}
              </div>
            )}

            {/* Bouton SSO Principal */}
            <button
              onClick={handleSSOLogin}
              className="w-full mb-6 h-12 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white rounded-lg font-semibold flex items-center justify-center gap-2 transition-all shadow-lg shadow-blue-200 hover:shadow-xl hover:shadow-blue-300"
            >
              <Shield size={20} />
              Se connecter avec ONEE SSO
            </button>

            <div className="relative mb-6">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-slate-200"></div>
              </div>
              <div className="relative flex justify-center text-xs uppercase">
                <span className="bg-white px-2 text-slate-500">ou</span>
              </div>
            </div>

            <form onSubmit={onSubmit} className="space-y-4">
              <Input
                label="Adresse email"
                type="email"
                placeholder="votre@email.ma"
                value={form.email}
                onChange={e => setForm({ ...form, email: e.target.value })}
                error={errs.email}
                disabled={loading}
              />

              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">
                  Mot de passe
                </label>
                <div className="relative">
                  <input
                    type={showPass ? 'text' : 'password'}
                    placeholder="••••••••"
                    value={form.password}
                    onChange={e => setForm({ ...form, password: e.target.value })}
                    disabled={loading}
                    className="w-full h-9 rounded-lg border border-slate-200 hover:border-slate-300 text-sm px-3 pr-10 focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all"
                  />
                  <button type="button" onClick={() => setShowPass(!showPass)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600">
                    {showPass ? <EyeOff size={15} /> : <Eye size={15} />}
                  </button>
                </div>
                {errs.password && <p className="text-xs text-red-500">{errs.password}</p>}
              </div>

              <Button type="submit" className="w-full mt-2 h-10" loading={loading}>
                {loading ? 'Connexion...' : 'Connexion locale (développement)'}
              </Button>
            </form>

            <div className="mt-6 pt-4 border-t border-slate-100 text-center">
              <p className="text-xs text-emerald-600 font-medium">
                ✅ SSO ONEE Intégré - Authentification sécurisée
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
