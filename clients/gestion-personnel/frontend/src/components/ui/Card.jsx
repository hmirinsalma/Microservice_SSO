import { clsx } from 'clsx';

export default function Card({ children, className, padding = true }) {
  return (
    <div className={clsx(
      'bg-white rounded-2xl border border-slate-200 shadow-sm',
      padding && 'p-6',
      className
    )}>
      {children}
    </div>
  );
}

export function CardHeader({ title, subtitle, action }) {
  return (
    <div className="flex items-start justify-between mb-5">
      <div>
        <h2 className="text-base font-semibold text-slate-900">{title}</h2>
        {subtitle && <p className="text-sm text-slate-500 mt-0.5">{subtitle}</p>}
      </div>
      {action && <div>{action}</div>}
    </div>
  );
}
