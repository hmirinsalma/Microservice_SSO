import { useEffect } from 'react';
import { CheckCircle, XCircle, X } from 'lucide-react';
import { clsx } from 'clsx';

export default function Toast({ open, message, type = 'success', onClose }) {
  useEffect(() => {
    if (open) {
      const t = setTimeout(onClose, 4000);
      return () => clearTimeout(t);
    }
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div className={clsx(
      'fixed bottom-6 right-6 z-50 flex items-center gap-3 px-4 py-3 rounded-xl shadow-lg border max-w-sm',
      'animate-in slide-in-from-bottom-2 duration-200',
      type === 'success' ? 'bg-white border-emerald-200 text-slate-900' : 'bg-white border-red-200 text-slate-900'
    )}>
      {type === 'success'
        ? <CheckCircle size={18} className="text-emerald-500 shrink-0" />
        : <XCircle size={18} className="text-red-500 shrink-0" />}
      <p className="text-sm font-medium flex-1">{message}</p>
      <button onClick={onClose} className="text-slate-400 hover:text-slate-600">
        <X size={14} />
      </button>
    </div>
  );
}
