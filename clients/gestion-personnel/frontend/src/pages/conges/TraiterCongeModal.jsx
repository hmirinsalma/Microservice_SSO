import { useState } from 'react';
import { CheckCircle, XCircle } from 'lucide-react';
import Modal from '../../components/ui/Modal';
import Button from '../../components/ui/Button';
import CongeBadge from '../../components/ui/CongeBadge';
import congesApi from '../../api/congesApi';
import { extractErrorMessage } from '../../api/apiHelpers';

export default function TraiterCongeModal({ conge, role, onClose, onSuccess }) {
  const [commentaire, setCommentaire] = useState('');
  const [err,  setErr]  = useState('');
  const [loading, setLoading] = useState(false);

  const handle = async (accepter) => {
    setLoading(true); setErr('');
    try {
      const data = { accepter, commentaire: commentaire || null };
      if (role === 'ChefDeService')
        await congesApi.traiterChef(conge.id, data);
      else
        await congesApi.traiterDirecteur(conge.id, data);

      onSuccess(accepter ? 'Demande approuvée.' : 'Demande refusée.');
      onClose();
    } catch (e) { setErr(extractErrorMessage(e)); }
    finally { setLoading(false); }
  };

  const fmtDate = d => new Date(d).toLocaleDateString('fr-FR');

  return (
    <Modal open size="sm"
      title={`Traiter la demande de ${conge.employeNom} ${conge.employePrenom}`}
      onClose={onClose}
      footer={<>
        <Button variant="secondary" onClick={onClose} disabled={loading}>Fermer</Button>
        <Button variant="danger"    onClick={() => handle(false)} loading={loading} icon={<XCircle size={15} />}>Refuser</Button>
        <Button                     onClick={() => handle(true)}  loading={loading} icon={<CheckCircle size={15} />}>Approuver</Button>
      </>}
    >
      {err && <div className="mb-3 p-3 rounded-lg bg-red-50 border border-red-200 text-sm text-red-700">{err}</div>}
      <div className="space-y-4">
        {/* Résumé */}
        <div className="p-4 rounded-xl bg-slate-50 border border-slate-200 space-y-2">
          <div className="flex items-center justify-between">
            <span className="text-sm font-semibold text-slate-900">{conge.employeNom} {conge.employePrenom}</span>
            <CongeBadge statut={conge.statut} />
          </div>
          <p className="text-xs text-slate-500">{conge.matricule} · {conge.serviceNom}</p>
          <div className="flex items-center gap-2 text-sm text-slate-700 font-medium">
            📅 {fmtDate(conge.dateDebut)} → {fmtDate(conge.dateFin)}
            <span className="text-xs text-slate-500">({conge.nombreJours}j)</span>
          </div>
          <p className="text-sm text-slate-600 italic">"{conge.motif}"</p>
        </div>

        {/* Commentaire */}
        <div className="flex flex-col gap-1">
          <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">Commentaire (optionnel)</label>
          <textarea rows={3} placeholder="Ajoutez un commentaire..."
            value={commentaire} onChange={e => setCommentaire(e.target.value)}
            className="w-full rounded-lg border border-slate-200 hover:border-slate-300 text-sm px-3 py-2 bg-white resize-none focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all" />
        </div>
      </div>
    </Modal>
  );
}
