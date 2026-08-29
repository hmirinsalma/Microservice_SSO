import { useState } from 'react';
import Modal from '../../components/ui/Modal';
import Button from '../../components/ui/Button';
import congesApi from '../../api/congesApi';
import { extractErrorMessage } from '../../api/apiHelpers';

export default function DemandeCongeModal({ onClose, onSuccess }) {
  const [form, setForm] = useState({ dateDebut: '', dateFin: '', motif: '' });
  const [errs, setErrs] = useState({});
  const [err,  setErr]  = useState('');
  const [loading, setLoading] = useState(false);

  const nb = form.dateDebut && form.dateFin
    ? Math.max(0, Math.floor((new Date(form.dateFin) - new Date(form.dateDebut)) / 86400000) + 1)
    : 0;

  const validate = () => {
    const e = {};
    if (!form.dateDebut) e.dateDebut = 'Requise';
    if (!form.dateFin)   e.dateFin   = 'Requise';
    else if (form.dateFin < form.dateDebut) e.dateFin = 'Doit être après la date de début';
    if (!form.motif.trim()) e.motif  = 'Motif requis';
    setErrs(e);
    return !Object.keys(e).length;
  };

  const submit = async () => {
    if (!validate()) return;
    setLoading(true); setErr('');
    try {
      await congesApi.create({
        dateDebut: new Date(form.dateDebut).toISOString(),
        dateFin:   new Date(form.dateFin).toISOString(),
        motif:     form.motif,
      });
      onSuccess('Demande de congé envoyée avec succès.');
      onClose();
    } catch (e) { setErr(extractErrorMessage(e)); }
    finally { setLoading(false); }
  };

  const Field = ({ label, field, type = 'text' }) => (
    <div className="flex flex-col gap-1">
      <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">{label}</label>
      <input type={type} value={form[field]}
        onChange={e => setForm(p => ({ ...p, [field]: e.target.value }))}
        className={`h-9 w-full rounded-lg border text-sm px-3 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all
          ${errs[field] ? 'border-red-400' : 'border-slate-200 hover:border-slate-300'}`} />
      {errs[field] && <p className="text-xs text-red-500">{errs[field]}</p>}
    </div>
  );

  return (
    <Modal open size="sm" title="Nouvelle demande de congé" onClose={onClose}
      footer={<>
        <Button variant="secondary" onClick={onClose} disabled={loading}>Annuler</Button>
        <Button onClick={submit} loading={loading}>Envoyer la demande</Button>
      </>}
    >
      {err && <div className="mb-4 p-3 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700">{err}</div>}
      <div className="space-y-4">
        <div className="grid grid-cols-2 gap-3">
          <Field label="Date de début" field="dateDebut" type="date" />
          <Field label="Date de fin"   field="dateFin"   type="date" />
        </div>
        {nb > 0 && (
          <div className="p-2.5 rounded-lg bg-blue-50 border border-blue-100 text-xs text-blue-700 font-medium">
            📅 Durée : {nb} jour{nb > 1 ? 's' : ''} calendaire{nb > 1 ? 's' : ''}
          </div>
        )}
        <div className="flex flex-col gap-1">
          <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">Motif</label>
          <textarea rows={3} placeholder="Décrivez le motif de votre congé..."
            value={form.motif} onChange={e => setForm(p => ({ ...p, motif: e.target.value }))}
            className={`w-full rounded-lg border text-sm px-3 py-2 bg-white resize-none focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all
              ${errs.motif ? 'border-red-400' : 'border-slate-200 hover:border-slate-300'}`} />
          {errs.motif && <p className="text-xs text-red-500">{errs.motif}</p>}
        </div>
      </div>
    </Modal>
  );
}
