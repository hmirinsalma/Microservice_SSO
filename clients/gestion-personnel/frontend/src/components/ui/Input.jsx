import { clsx } from 'clsx';

export function Input({ label, error, icon, className, ...props }) {
  return (
    <div className="flex flex-col gap-1">
      {label && <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">{label}</label>}
      <div className="relative">
        {icon && <div className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400">{icon}</div>}
        <input
          className={clsx(
            'w-full h-9 rounded-lg border text-sm text-slate-900 bg-white placeholder-slate-400',
            'focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500',
            'transition-all duration-150',
            icon ? 'pl-9 pr-3' : 'px-3',
            error ? 'border-red-400 bg-red-50' : 'border-slate-200 hover:border-slate-300',
            'disabled:bg-slate-50 disabled:text-slate-400 disabled:cursor-not-allowed',
            className
          )}
          {...props}
        />
      </div>
      {error && <p className="text-xs text-red-500">{error}</p>}
    </div>
  );
}

export function Select({ label, error, children, className, ...props }) {
  return (
    <div className="flex flex-col gap-1">
      {label && <label className="text-xs font-semibold text-slate-600 uppercase tracking-wide">{label}</label>}
      <select
        className={clsx(
          'w-full h-9 rounded-lg border text-sm text-slate-900 bg-white px-3',
          'focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500',
          'transition-all duration-150 cursor-pointer',
          error ? 'border-red-400' : 'border-slate-200 hover:border-slate-300',
          'disabled:bg-slate-50 disabled:text-slate-400 disabled:cursor-not-allowed',
          className
        )}
        {...props}
      >
        {children}
      </select>
      {error && <p className="text-xs text-red-500">{error}</p>}
    </div>
  );
}
