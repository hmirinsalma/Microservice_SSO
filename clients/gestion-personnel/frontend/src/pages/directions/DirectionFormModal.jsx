import { useEffect, useState } from 'react';
import Modal from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import Button from '../../components/ui/Button';
import directionsApi from '../../api/directionsApi';
import { extractErrorMessage } from '../../api/apiHelpers';

export default function DirectionFormModal({ direction, onClose, onSuccess }) {
  const [form, setForm]   = useState({ nom: '', description: '' });
  const [errs, setErrs]   = useState({});
  const [err, setErr]     = useState('');
  const [loading, setLoading] = useState(false);
  const isEdit = !!direction;

  useEffect(() => {
    setForm(direction ? { nom: direction.nom, description: direction.description || '' } : { nom: '', description: '' });
    setErrs({}); setErr('');
  }, [direction]);

  const validate = () => {
    const e = {};
    if (!form.nom.trim()) e.nom = 'Nom requis';
    setErrs(e);
    return !Object.keys(e).length;
  };

  const submit = async () => {
    if (!validate()) return;
    setLoading(true); setErr('');
    try {
      if (isEdit) await directionsApi.update(direction.id, form);
      else await directionsApi.create(form);
      onSuccess(isEdit ? 'Direction modifiée.' : 'Direction créée.');
      onClose();
    } catch (e) { setErr(extractErrorMessage(e)); }
    finally { setLoading(false); }
  };

  return (
    <Modal open title={isEdit ? 'Modifier la direction' : 'Nouvelle direction'} onClose={onClose} size="sm"
      footer={<>
        <Button variant="secondary" onClick={onClose} disabled={loading}>Annuler</Button>
        <Button onClick={submit} loading={loading}>{isEdit ? 'Enregistrer' : 'Créer'}</Button>
      </>}>
      {err && <div className="mb-4 p-3 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700">{err}</div>}
      <div className="space-y-4">
        <Input label="Nom de la direction" placeholder="ex: Direction Informatique" required
          value={form.nom} onChange={e => setForm({ ...form, nom: e.target.value })}
          error={errs.nom} autoFocus />
        <div className="flex flex-col gap-1">
          <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">Description (optionnelle)</label>
          <textarea rows={3} placeholder="Décrivez cette direction..."
            value={form.description} onChange={e => setForm({ ...form, description: e.target.value })}
            className="w-full rounded-lg border border-slate-200 hover:border-slate-300 text-sm px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all resize-none" />
        </div>
      </div>
    </Modal>
  );
}
