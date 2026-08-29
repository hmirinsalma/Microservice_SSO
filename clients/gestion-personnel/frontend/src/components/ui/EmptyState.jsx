import Button from './Button';
import { Plus } from 'lucide-react';

export default function EmptyState({ icon: Icon, title, description, actionLabel, onAction }) {
  return (
    <div className="flex flex-col items-center justify-center py-16 px-4 text-center">
      <div className="w-16 h-16 rounded-2xl bg-slate-100 flex items-center justify-center mb-4">
        {Icon && <Icon size={28} className="text-slate-400" />}
      </div>
      <h3 className="text-sm font-semibold text-slate-900 mb-1">{title}</h3>
      <p className="text-sm text-slate-500 max-w-xs mb-6">{description}</p>
      {actionLabel && onAction && (
        <Button onClick={onAction} icon={<Plus size={16} />}>{actionLabel}</Button>
      )}
    </div>
  );
}
