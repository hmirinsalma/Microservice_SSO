import { useEffect, useState } from 'react';
import Modal from '../../components/ui/Modal';
import { Input, Select } from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import servicesApi from '../../api/servicesApi';
import directionsApi from '../../api/directionsApi';
import { extractErrorMessage } from '../../api/apiHelpers';

export default function ServiceFormModal({ service, onClose, onSuccess }) {
  const [form, setForm]     = useState({ nom: '', description: '', directionId: '' });
  const [dirs, setDirs]     = useState([]);
  const [errs, setErrs]     = useState({});
  const [err, setErr]       = useState('');
  const [loading, setLoading] = useState(false);
  const isEdit = !!service;

  useEffect(() => {
    directionsApi.getAll().then(({ data }) => setDirs(data)).catch(() => {});
  }, []);

  useEffect(() => {
    setForm(service
      ? { nom: service.nom, description: service.description || '', directionId: service.directionId }
      : { nom: '', description: '', directionId: '' });
    setErrs({}); setErr('');
  }, [service]);

  const validate = () => {
    const e = {};
    if (!form.nom.trim())  e.nom = 'Nom requis';
    if (!form.directionId) e.directionId = 'Direction requise';
    setErrs(e);
    return !Object.keys(e).length;
  };

  const submit = async () => {
    if (!validate()) return;
    setLoading(true); setErr('');
    try {
      const payload = { ...form, directionId: Number(form.directionId) };
      if (isEdit) await servicesApi.update(service.id, payload);
      else await servicesApi.create(payload);
      onSuccess(isEdit ? 'Service modifié.' : 'Service créé.');
      onClose();
    } catch (e) { setErr(extractErrorMessage(e)); }
    finally { setLoading(false); }
  };

  return (
    <Modal open title={isEdit ? 'Modifier le service' : 'Nouveau service'} onClose={onClose} size="sm"
      footer={<>
        <Button variant="secondary" onClick={onClose} disabled={loading}>Annuler</Button>
        <Button onClick={submit} loading={loading}>{isEdit ? 'Enregistrer' : 'Créer'}</Button>
      </>}>
      {err && <div className="mb-4 p-3 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700">{err}</div>}
      <div className="space-y-4">
        <Select label="Direction" value={form.directionId}
          onChange={e => setForm({ ...form, directionId: e.target.value })} error={errs.directionId}>
          <option value="">— Sélectionner une direction —</option>
          {dirs.map(d => <option key={d.id} value={d.id}>{d.nom}</option>)}
        </Select>
        <Input label="Nom du service" placeholder="ex: Service Développement" required
          value={form.nom} onChange={e => setForm({ ...form, nom: e.target.value })}
          error={errs.nom} autoFocus />
        <div className="flex flex-col gap-1">
          <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">Description (optionnelle)</label>
          <textarea rows={3} placeholder="Décrivez ce service..."
            value={form.description} onChange={e => setForm({ ...form, description: e.target.value })}
            className="w-full rounded-lg border border-slate-200 hover:border-slate-300 text-sm px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all resize-none" />
        </div>
      </div>
    </Modal>
  );
}
