import { useEffect, useState } from 'react';
import Modal from '../../components/ui/Modal';
import { Input, Select } from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import employesApi from '../../api/employesApi';
import directionsApi from '../../api/directionsApi';
import servicesApi from '../../api/servicesApi';
import { extractErrorMessage } from '../../api/apiHelpers';

const STATUTS = ['Actif', 'Inactif', 'Suspendu'];
const POSTES_PAR_DIRECTION = {
  'Direction RH': ['Responsable RH','Chargé de Recrutement','Chargé de Formation','Gestionnaire de Paie','Responsable Paie','Assistant RH'],
  'Direction Technique': ['Ingénieur Technique','Technicien Maintenance','Chef de Chantier','Responsable Exploitation','Ingénieur Exploitation'],
  'Direction Informatique': ['Développeur Full Stack','Développeur Frontend','Développeur Backend','Ingénieur DevOps','Architecte Logiciel','Chef de Projet IT','Analyste Systèmes','Administrateur Réseau','Administrateur Base de Données'],
  'Direction Patrimoine': ['Gestionnaire Immobilier','Responsable Logistique','Chargé des Achats','Agent de Patrimoine'],
};
const POSTES_COMMUNS = ['Directeur','Directeur Adjoint','Chef de Service','Manager','Assistant Administratif','Comptable','Juriste','Auditeur Interne'];

const INIT = { matricule:'', nom:'', prenom:'', email:'', telephone:'', dateEmbauche:'', poste:'', statut:'Actif', directionId:'', serviceId:'' };

export default function EmployeFormModal({ employe, onClose, onSuccess }) {
  const [form, setForm]   = useState(INIT);
  const [dirs, setDirs]   = useState([]);
  const [svcs, setSvcs]   = useState([]);
  const [errs, setErrs]   = useState({});
  const [err, setErr]     = useState('');
  const [loading, setLoading] = useState(false);
  const isEdit = !!employe;

  useEffect(() => { directionsApi.getAll().then(({ data }) => setDirs(data)).catch(() => {}); }, []);

  useEffect(() => {
    if (form.directionId) servicesApi.getByDirection(form.directionId).then(({ data }) => setSvcs(data)).catch(() => setSvcs([]));
    else setSvcs([]);
  }, [form.directionId]);

  useEffect(() => {
    setForm(employe ? {
      matricule: employe.matricule, nom: employe.nom, prenom: employe.prenom,
      email: employe.email, telephone: employe.telephone || '',
      dateEmbauche: employe.dateEmbauche?.split('T')[0] || '',
      poste: employe.poste, statut: employe.statut,
      directionId: employe.directionId, serviceId: employe.serviceId,
    } : INIT);
    setErrs({}); setErr('');
  }, [employe]);

  const getPostes = () => {
    const dir = dirs.find(d => d.id === Number(form.directionId));
    const sp = dir ? (POSTES_PAR_DIRECTION[dir.nom] || []) : [];
    return [...sp, ...POSTES_COMMUNS.filter(p => !sp.includes(p))];
  };

  const validate = () => {
    const e = {};
    if (!isEdit && !form.matricule.trim()) e.matricule = 'Requis';
    if (!form.nom.trim())    e.nom    = 'Requis';
    if (!form.prenom.trim()) e.prenom = 'Requis';
    if (!form.email.trim())  e.email  = 'Requis';
    else if (!/\S+@\S+\.\S+/.test(form.email)) e.email = 'Email invalide';
    if (!form.dateEmbauche)  e.dateEmbauche = 'Requise';
    if (!form.poste)         e.poste  = 'Requis';
    if (!form.directionId)   e.directionId = 'Requise';
    if (!form.serviceId)     e.serviceId   = 'Requis';
    setErrs(e); return !Object.keys(e).length;
  };

  const submit = async () => {
    if (!validate()) return;
    setLoading(true); setErr('');
    try {
      if (isEdit) {
        await employesApi.update(employe.id, {
          nom: form.nom, prenom: form.prenom, email: form.email,
          telephone: form.telephone || null,
          dateEmbauche: new Date(form.dateEmbauche).toISOString(),
          poste: form.poste, statut: form.statut,
          directionId: Number(form.directionId), serviceId: Number(form.serviceId),
        });
      } else {
        await employesApi.create({
          ...form, telephone: form.telephone || null,
          dateEmbauche: new Date(form.dateEmbauche).toISOString(),
          directionId: Number(form.directionId), serviceId: Number(form.serviceId),
        });
      }
      onSuccess(isEdit ? 'Employé modifié.' : 'Employé créé.');
      onClose();
    } catch (e) { setErr(extractErrorMessage(e)); }
    finally { setLoading(false); }
  };

  const set = (f) => (e) => setForm(p => {
    const n = { ...p, [f]: e.target.value };
    if (f === 'directionId') { n.serviceId = ''; n.poste = ''; }
    return n;
  });

  const Section = ({ label }) => (
    <p className="text-[10px] font-bold text-slate-400 uppercase tracking-widest mt-5 mb-3 pb-1 border-b border-slate-100">{label}</p>
  );

  return (
    <Modal open size="lg" title={isEdit ? `Modifier — ${employe.nom} ${employe.prenom}` : 'Nouvel employé'} onClose={onClose}
      footer={<>
        <Button variant="secondary" onClick={onClose} disabled={loading}>Annuler</Button>
        <Button onClick={submit} loading={loading}>{isEdit ? 'Enregistrer les modifications' : 'Créer l\'employé'}</Button>
      </>}>
      {err && <div className="mb-4 p-3 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700">{err}</div>}

      <Section label="Identité" />
      <div className="grid grid-cols-3 gap-3">
        <Input label="Matricule" placeholder="ex: EMP-001" value={form.matricule} onChange={set('matricule')}
          error={errs.matricule} disabled={loading || isEdit} required />
        <Input label="Nom" placeholder="Nom de famille" value={form.nom} onChange={set('nom')} error={errs.nom} disabled={loading} required />
        <Input label="Prénom" placeholder="Prénom" value={form.prenom} onChange={set('prenom')} error={errs.prenom} disabled={loading} required />
      </div>

      <Section label="Contact" />
      <div className="grid grid-cols-2 gap-3">
        <Input label="Email professionnel" type="email" placeholder="nom@onee.ma" value={form.email} onChange={set('email')} error={errs.email} disabled={loading} required />
        <Input label="Téléphone (optionnel)" type="tel" placeholder="0600000000" value={form.telephone} onChange={set('telephone')} disabled={loading} />
      </div>

      <Section label="Organisation" />
      <div className="grid grid-cols-2 gap-3">
        <Select label="Direction" value={form.directionId} onChange={set('directionId')} error={errs.directionId} disabled={loading}>
          <option value="">— Sélectionner —</option>
          {dirs.map(d => <option key={d.id} value={d.id}>{d.nom}</option>)}
        </Select>
        <Select label="Service" value={form.serviceId} onChange={set('serviceId')} error={errs.serviceId} disabled={loading || !form.directionId}>
          <option value="">{!form.directionId ? '← Choisir une direction' : '— Sélectionner —'}</option>
          {svcs.map(s => <option key={s.id} value={s.id}>{s.nom}</option>)}
        </Select>
      </div>

      <Section label="Poste & Statut" />
      <div className="grid grid-cols-3 gap-3">
        <div className="flex flex-col gap-1">
          <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">
            Date d'embauche <span className="text-red-500">*</span>
          </label>
          <input type="date" value={form.dateEmbauche} onChange={e => setForm(p => ({ ...p, dateEmbauche: e.target.value }))}
            disabled={loading}
            className={`h-9 rounded-lg border text-sm px-3 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all
              ${errs.dateEmbauche ? 'border-red-400' : 'border-slate-200 hover:border-slate-300'}
              disabled:bg-slate-50 disabled:cursor-not-allowed`}
          />
          {errs.dateEmbauche && <p className="text-xs text-red-500">{errs.dateEmbauche}</p>}
        </div>
        <Select label="Poste / Fonction" value={form.poste} onChange={set('poste')} error={errs.poste} disabled={loading || !form.directionId}>
          <option value="">{!form.directionId ? '← Choisir une direction' : '— Sélectionner —'}</option>
          {getPostes().map(p => <option key={p} value={p}>{p}</option>)}
        </Select>
        <Select label="Statut" value={form.statut} onChange={set('statut')} disabled={loading}>
          {STATUTS.map(s => <option key={s} value={s}>{s}</option>)}
        </Select>
      </div>
    </Modal>
  );
}
