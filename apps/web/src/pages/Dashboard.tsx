import { useQuery } from '@tanstack/react-query'
import { ArrowRight, Bus, MapPin, Navigation, Route as RouteIcon, Ticket, Users } from 'lucide-react'
import { api } from '../lib/api'
import { formatDateTime } from '../lib/format'
import type { BookingDto, BusDto, RouteDto, StopDto, TripDto, VendorDto } from '../lib/types'
import { Card, Loading, PageHeader, Table, Td, Th } from '../components/ui'

const tripStatus = ['Scheduled', 'Delayed', 'Cancelled', 'Completed']

export function Dashboard() {
  const routes = useQuery({ queryKey: ['routes'], queryFn: () => api.get<RouteDto[]>('/api/routes') })
  const stops = useQuery({ queryKey: ['stops'], queryFn: () => api.get<StopDto[]>('/api/stops') })
  const buses = useQuery({ queryKey: ['buses'], queryFn: () => api.get<BusDto[]>('/api/buses') })
  const vendors = useQuery({ queryKey: ['vendors'], queryFn: () => api.get<VendorDto[]>('/api/vendors') })
  const trips = useQuery({ queryKey: ['trips'], queryFn: () => api.get<TripDto[]>('/api/trips') })
  const bookings = useQuery({ queryKey: ['bookings'], queryFn: () => api.get<BookingDto[]>('/api/bookings') })

  const loading =
    routes.isLoading ||
    stops.isLoading ||
    buses.isLoading ||
    vendors.isLoading ||
    trips.isLoading ||
    bookings.isLoading

  if (loading) return <Loading />

  const stats = [
    { label: 'Routes', value: routes.data?.length ?? 0, icon: RouteIcon },
    { label: 'Stops', value: stops.data?.length ?? 0, icon: MapPin },
    { label: 'Buses', value: buses.data?.length ?? 0, icon: Bus },
    { label: 'Vendors', value: vendors.data?.length ?? 0, icon: Users },
    { label: 'Trips', value: trips.data?.length ?? 0, icon: Navigation },
    { label: 'Bookings', value: bookings.data?.length ?? 0, icon: Ticket },
  ]

  const upcoming = [...(trips.data ?? [])]
    .filter((trip) => new Date(trip.departureAt) >= new Date())
    .sort((a, b) => a.departureAt.localeCompare(b.departureAt))
    .slice(0, 5)

  return (
    <>
      <PageHeader title="Dashboard" subtitle="A quick view of the transport network." />

      <div className="grid grid-cols-2 gap-4 md:grid-cols-3 xl:grid-cols-6">
        {stats.map(({ label, value, icon: Icon }) => (
          <Card key={label} className="flex items-center gap-3">
            <div className="rounded-lg bg-brand-soft p-2 text-brand">
              <Icon className="size-5" />
            </div>
            <div>
              <div className="text-2xl font-semibold text-ink">{value}</div>
              <div className="text-xs uppercase tracking-wide text-muted">{label}</div>
            </div>
          </Card>
        ))}
      </div>

      <div className="mt-8">
        <h2 className="mb-3 text-lg font-semibold text-ink">Upcoming trips</h2>
        {upcoming.length === 0 ? (
          <Card>
            <p className="text-sm text-muted">No upcoming trips yet. Add one from the Trips page.</p>
          </Card>
        ) : (
          <Table>
            <thead>
              <tr>
                <Th>Route</Th>
                <Th>Journey</Th>
                <Th>Departure</Th>
                <Th>Fare</Th>
                <Th>Seats</Th>
                <Th>Status</Th>
              </tr>
            </thead>
            <tbody>
              {upcoming.map((trip) => (
                <tr key={trip.id} className="border-b border-line last:border-0">
                  <Td>
                    <div className="font-medium">{trip.routeCode}</div>
                    <div className="text-xs text-muted">{trip.routeName}</div>
                  </Td>
                  <Td>
                    {trip.origin && trip.destination ? (
                      <div className="flex flex-wrap items-center gap-1.5">
                        <span>{trip.origin}</span>
                        <ArrowRight className="size-3.5 shrink-0 text-muted" />
                        <span>{trip.destination}</span>
                      </div>
                    ) : (
                      <span className="text-sm text-muted">Stops not set</span>
                    )}
                  </Td>
                  <Td>{formatDateTime(trip.departureAt)}</Td>
                  <Td>Rs {trip.fare.toFixed(2)}</Td>
                  <Td>
                    <span className="font-medium text-ink">{trip.remainingSeats}</span>
                    <span className="text-xs text-muted"> / {trip.capacity}</span>
                  </Td>
                  <Td>{tripStatus[trip.status] ?? 'Unknown'}</Td>
                </tr>
              ))}
            </tbody>
          </Table>
        )}
      </div>
    </>
  )
}
