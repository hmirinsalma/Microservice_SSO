import { useEffect, useState } from 'react';
import { Eye, EyeOff, User } from 'lucide-react';
import Modal from '../../components/ui/Modal';
import { Input, Select } from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import usersApi from '../../api/usersApi';
import axiosInstance from '../../api/axiosInstance';
import { extractErrorMessage } from '../../api/apiHelpers';

const INIT = { username: '', email: '', password: '', roleId: '', employeId: '' };

const ROLE_DESCRIPTIONS = {
  AdministrateurRH: 'Accès complet — CRUD sur tout le système',
  Directeur:        'Consultation des employés de sa direction uniquement',
  ChefDeService:    'Consultation des employés de son service uniquement',
  Employe:          'Consultation de ses propres informations',
};

export default function UserFormModal({ user, onClose, onSuccess }) {
  const [form,     setForm]     = useState(INIT);
  const [roles,    setRoles]    = useState([]);
  const [employes, setEmployes] = useState([]);
  const [errs,     setErrs]     = useState({});
  const [err,      setErr]      = useState('');
  const [loading,  setLoading]  = useState(false);
  const [showPass, setShowPass] = useState(false);
  const isEdit = !!user;

  useEffect(() => {
    usersApi.getRoles().then(({ data }) => setRoles(data)).catch(() => {});
    if (!isEdit) {
      axiosInstance.get('/users/employes-sans-compte')
        .then(({ data }) => setEmployes(data)).catch(() => {});
    }
  }, [isEdit]);

  useEffect(() => {
    setForm(user
      ? { username: user.username, email: user.email, password: '', roleId: '', employeId: '' }
      : INIT);
    setErrs({}); setErr('');
  }, [user]);

  const handleEmployeChange = (e) => {
    const empId = e.target.value;
    const emp   = employes.find(em => em.id === Number(empId));
    setForm(prev => ({
      ...prev,
      employeId: empId,
      username:  emp ? `${emp.prenom.toLowerCase()}.${emp.nom.toLowerCase()}` : prev.username,
      email:     emp ? emp.email : prev.email,
    }));
  };

  const validate = () => {
    const e = {};
    if (!form.username.trim() || form.username.length < 3) e.username = 'Minimum 3 caractères';
    if (!form.email.trim() || !/\S+@\S+\.\S+/.test(form.email)) e.email = 'Email invalide';
    if (!form.roleId) e.roleId = 'Rôle requis';
    // Mot de passe requis seulement en création (stub temporaire)
    if (!isEdit && !form.password) e.password = 'Requis (stub temporaire)';
    setErrs(e);
    return !Object.keys(e).length;
  };

  const submit = async () => {
    if (!validate()) return;
    setLoading(true); setErr('');
    try {
      if (isEdit) {
        await usersApi.update(user.id, {
          username: form.username, email: form.email,
          roleId: Number(form.roleId), isActive: user.isActive,
        });
      } else {
        await usersApi.create({
          username:  form.username,
          email:     form.email,
          password:  form.password,   // stub temporaire
          roleId:    Number(form.roleId),
          employeId: form.employeId ? Number(form.employeId) : null,
        });
      }
      onSuccess(isEdit ? 'Compte modifié.' : 'Compte créé et lié à la fiche employé.');
      onClose();
    } catch (e) { setErr(extractErrorMessage(e)); }
    finally { setLoading(false); }
  };

  const selectedRole = roles.find(r => r.id === Number(form.roleId));

  return (
    <Modal open size="md"
      title={isEdit ? `Modifier — ${user.username}` : 'Nouveau compte utilisateur'}
      onClose={onClose}
      footer={<>
        <Button variant="secondary" onClick={onClose} disabled={loading}>Annuler</Button>
        <Button onClick={submit} loading={loading}>
          {isEdit ? 'Enregistrer' : 'Créer le compte'}
        </Button>
      </>}
    >
      {err && <div className="mb-4 p-3 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700">{err}</div>}

      <div className="space-y-4">
        {/* Info SSO */}
        <div className="p-3 rounded-xl bg-blue-50 border border-blue-100">
          <p className="text-xs text-blue-700 font-medium">
            ℹ️ Compte provisoire (Stub). Après intégration SSO, la création de comptes sera gérée
            directement par le serveur SSO — ce formulaire sera remplacé par la console SSO.
          </p>
        </div>

        {/* Sélection employé (création uniquement) */}
        {!isEdit && (
          <div className="flex flex-col gap-1">
            <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide flex items-center gap-1.5">
              <User size={11} /> Lier à une fiche employé
              <span className="text-slate-400 font-normal normal-case tracking-normal">(recommandé)</span>
            </label>
            <select value={form.employeId} onChange={handleEmployeChange}
              className="h-9 w-full rounded-lg border border-slate-200 hover:border-slate-300 text-sm px-3 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all cursor-pointer">
              <option value="">— Aucun employé sélectionné —</option>
              {employes.map(e => (
                <option key={e.id} value={e.id}>
                  {e.nom} {e.prenom} · {e.matricule} · {e.directionNom}
                </option>
              ))}
            </select>
            {employes.length === 0 && (
              <p className="text-xs text-amber-600">Tous les employés ont déjà un compte.</p>
            )}
          </div>
        )}

        <div className="grid grid-cols-2 gap-3">
          <Input label="Nom d'utilisateur" placeholder="ex: salma.hmirin"
            value={form.username} onChange={e => setForm(p => ({ ...p, username: e.target.value }))}
            error={errs.username} />
          <Input label="Email" type="email" placeholder="nom@onee.ma"
            value={form.email} onChange={e => setForm(p => ({ ...p, email: e.target.value }))}
            error={errs.email} />
        </div>

        {/* Mot de passe STUB — création uniquement, supprimé avec SSO */}
        {!isEdit && (
          <div className="flex flex-col gap-1">
            <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">
              Mot de passe provisoire
              <span className="ml-1 text-amber-500 font-normal normal-case">(supprimé avec SSO)</span>
            </label>
            <div className="relative">
              <input type={showPass ? 'text' : 'password'} placeholder="Minimum 6 caractères"
                value={form.password}
                onChange={e => setForm(p => ({ ...p, password: e.target.value }))}
                className={`w-full h-9 rounded-lg border text-sm px-3 pr-10 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all
                  ${errs.password ? 'border-red-400' : 'border-slate-200 hover:border-slate-300'}`}
              />
              <button type="button" onClick={() => setShowPass(!showPass)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600">
                {showPass ? <EyeOff size={14} /> : <Eye size={14} />}
              </button>
            </div>
            {errs.password && <p className="text-xs text-red-500">{errs.password}</p>}
          </div>
        )}

        <Select label="Rôle *" value={form.roleId}
          onChange={e => setForm(p => ({ ...p, roleId: e.target.value }))} error={errs.roleId}>
          <option value="">— Sélectionner un rôle —</option>
          {roles.map(r => <option key={r.id} value={r.id}>{r.nom}</option>)}
        </Select>

        {selectedRole && (
          <div className="p-3 rounded-xl bg-slate-50 border border-slate-100">
            <p className="text-xs font-semibold text-slate-700">{selectedRole.nom}</p>
            <p className="text-xs text-slate-500 mt-0.5">{ROLE_DESCRIPTIONS[selectedRole.nom]}</p>
          </div>
        )}
      </div>
    </Modal>
  );
}
