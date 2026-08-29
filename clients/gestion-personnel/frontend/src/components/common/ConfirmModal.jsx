import { useState } from 'react';
import { AlertTriangle } from 'lucide-react';
import Modal from '../ui/Modal';
import Button from '../ui/Button';

export default function ConfirmModal({ title, message, onConfirm, onCancel }) {
  const [loading, setLoading] = useState(false);

  const handle = async () => {
    setLoading(true);
    await onConfirm();
    setLoading(false);
  };

  return (
    <Modal open title={title} onClose={onCancel} size="sm"
      footer={<>
        <Button variant="secondary" onClick={onCancel} disabled={loading}>Annuler</Button>
        <Button variant="danger" onClick={handle} loading={loading}>Supprimer</Button>
      </>}>
      <div className="flex gap-4">
        <div className="w-10 h-10 rounded-full bg-red-50 flex items-center justify-center shrink-0">
          <AlertTriangle size={18} className="text-red-500" />
        </div>
        <p className="text-sm text-slate-600 leading-relaxed pt-1">{message}</p>
      </div>
    </Modal>
  );
}
