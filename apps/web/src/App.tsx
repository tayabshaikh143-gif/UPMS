import type { ReactElement } from 'react'
import { Navigate, Route, Routes, useLocation } from 'react-router-dom'
import { useAuth } from './auth/AuthContext'
import { AppLayout } from './components/AppLayout'
import { Login } from './pages/Login'
import { Register } from './pages/Register'
import { Dashboard } from './pages/Dashboard'
import { RoutesPage } from './pages/Routes'
import { StopsPage } from './pages/Stops'
import { VendorsPage } from './pages/Vendors'
import { BusesPage } from './pages/Buses'
import { TripsPage } from './pages/Trips'
import { BookPage } from './pages/Book'
import { MyBookingsPage } from './pages/MyBookings'
import { BookingsPage } from './pages/Bookings'
import { VendorPortalPage } from './pages/VendorPortal'

function Protected() {
  const { user, loading } = useAuth()
  const location = useLocation()

  if (loading) {
    return <div className="flex min-h-screen items-center justify-center text-sm text-muted">Loading…</div>
  }
  if (!user) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }
  return <AppLayout />
}

function Home() {
  const { user } = useAuth()
  if (!user) return null
  if (user.roles.includes('Admin')) return <Dashboard />
  if (user.roles.includes('Vendor')) return <Navigate to="/vendor" replace />
  return <Navigate to="/book" replace />
}

function AdminOnly({ children }: { children: ReactElement }) {
  const { user } = useAuth()
  if (!user) return null
  return user.roles.includes('Admin') ? children : <Navigate to="/" replace />
}

function VendorOnly({ children }: { children: ReactElement }) {
  const { user } = useAuth()
  if (!user) return null
  return user.roles.includes('Vendor') ? children : <Navigate to="/" replace />
}

export function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route element={<Protected />}>
        <Route index element={<Home />} />
        <Route
          path="routes"
          element={
            <AdminOnly>
              <RoutesPage />
            </AdminOnly>
          }
        />
        <Route
          path="stops"
          element={
            <AdminOnly>
              <StopsPage />
            </AdminOnly>
          }
        />
        <Route
          path="vendors"
          element={
            <AdminOnly>
              <VendorsPage />
            </AdminOnly>
          }
        />
        <Route
          path="buses"
          element={
            <AdminOnly>
              <BusesPage />
            </AdminOnly>
          }
        />
        <Route
          path="trips"
          element={
            <AdminOnly>
              <TripsPage />
            </AdminOnly>
          }
        />
        <Route
          path="bookings"
          element={
            <AdminOnly>
              <BookingsPage />
            </AdminOnly>
          }
        />
        <Route path="book" element={<BookPage />} />
        <Route path="my-bookings" element={<MyBookingsPage />} />
        <Route
          path="vendor"
          element={
            <VendorOnly>
              <VendorPortalPage />
            </VendorOnly>
          }
        />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
