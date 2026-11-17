import {
  Bus,
  Building2,
  ClipboardCheck,
  ClipboardList,
  GraduationCap,
  LayoutDashboard,
  MapPin,
  Navigation,
  Route as RouteIcon,
  Ticket,
  Users,
} from 'lucide-react'
import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { Badge, cx } from './ui'

const adminNav = [
  { to: '/', label: 'Dashboard', icon: LayoutDashboard, end: true },
  { to: '/routes', label: 'Routes', icon: RouteIcon },
  { to: '/stops', label: 'Stops', icon: MapPin },
  { to: '/buses', label: 'Buses', icon: Bus },
  { to: '/vendors', label: 'Vendors', icon: Users },
  { to: '/trips', label: 'Trips', icon: Navigation },
  { to: '/bookings', label: 'Bookings', icon: Ticket },
]

const passengerNav = [
  { to: '/book', label: 'Book a trip', icon: Ticket, end: false },
  { to: '/my-bookings', label: 'My bookings', icon: ClipboardList, end: false },
]

const vendorNav = [
  { to: '/vendor', label: 'Vendor portal', icon: Building2, end: true },
]

const staffNav = [
  { to: '/staff', label: 'Boarding', icon: ClipboardCheck, end: true },
]

export function AppLayout() {
  const { user, logout } = useAuth()
  const isAdmin = user?.roles.includes('Admin') ?? false
  const isVendor = user?.roles.includes('Vendor') ?? false
  const isStaff = user?.roles.includes('Staff') ?? false
  const nav = isAdmin ? adminNav : isStaff ? staffNav : isVendor ? vendorNav : passengerNav

  return (
    <div className="min-h-screen bg-paper">
      <aside className="fixed inset-y-0 left-0 hidden w-64 border-r border-line bg-surface px-4 py-6 md:flex md:flex-col">
        <div className="mb-8 flex items-center gap-3 px-2">
          <div className="grid size-11 shrink-0 place-items-center rounded-xl bg-brand text-white">
            <GraduationCap className="size-6" />
          </div>
          <div>
            <div className="text-2xl font-semibold leading-none tracking-tight text-ink">UPMS</div>
            <div className="mt-1.5 text-xs font-medium text-muted">University Transport</div>
          </div>
        </div>

        <nav className="flex flex-1 flex-col gap-1">
          {nav.map(({ to, label, icon: Icon, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              className={({ isActive }) =>
                cx(
                  'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition',
                  isActive ? 'bg-brand text-white' : 'text-muted hover:bg-brand-soft hover:text-ink',
                )
              }
            >
              <Icon className="size-4" />
              {label}
            </NavLink>
          ))}
        </nav>

        <div className="mt-6 rounded-lg border border-line p-3">
          <div className="truncate text-sm font-medium text-ink">{user?.fullName}</div>
          <div className="mt-1 flex items-center justify-between gap-2">
            <span className="truncate text-xs text-muted">{user?.email}</span>
            <Badge tone="success">{user?.roles[0] ?? 'User'}</Badge>
          </div>
          <button
            onClick={logout}
            className="mt-3 w-full rounded-md border border-line px-2 py-1.5 text-xs font-medium text-muted hover:text-danger"
          >
            Sign out
          </button>
        </div>
      </aside>

      <div className="md:pl-64">
        <header className="sticky top-0 z-10 flex items-center justify-between gap-3 border-b border-line bg-paper/90 px-5 py-3 backdrop-blur md:hidden">
          <div className="text-lg font-semibold tracking-tight text-ink">UPMS</div>
          <button onClick={logout} className="text-xs font-medium text-muted">
            Sign out
          </button>
        </header>

        <nav className="flex gap-1 overflow-x-auto border-b border-line px-4 py-2 [scrollbar-width:none] [&::-webkit-scrollbar]:hidden md:hidden">
          {nav.map(({ to, label, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              className={({ isActive }) =>
                cx(
                  'whitespace-nowrap rounded-md px-3 py-1.5 text-xs font-medium',
                  isActive ? 'bg-brand text-white' : 'text-muted',
                )
              }
            >
              {label}
            </NavLink>
          ))}
        </nav>

        <main className="mx-auto max-w-6xl px-5 py-8">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
