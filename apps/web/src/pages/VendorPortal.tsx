import { useQuery } from '@tanstack/react-query'
import { formatDateTime } from '../lib/format'
import { api } from '../lib/api'
import type { BusDto, VendorProfileDto, VendorSettlementDto, VendorTripDto } from '../lib/types'
import {
  Badge,
  Card,
  Empty,
  Loading,
  PageHeader,
  Table,
  Td,
  Th,
} from '../components/ui'

const vendorStatuses = ['Pending', 'Approved', 'Rejected']
const tripStatuses = ['Scheduled', 'Delayed', 'Cancelled', 'Completed']

function vendorTone(status: number) {
  if (status === 1) return 'success' as const
  if (status === 2) return 'danger' as const
  return 'warning' as const
}

function tripTone(status: number) {
  if (status === 1) return 'warning' as const
  if (status === 2) return 'danger' as const
  return 'neutral' as const
}

export function VendorPortalPage() {
  const profile = useQuery({
    queryKey: ['vendor', 'me'],
    queryFn: () => api.get<VendorProfileDto>('/api/vendors/me'),
  })
  const approved = profile.data?.status === 1
  const buses = useQuery({
    queryKey: ['vendor', 'buses'],
    queryFn: () => api.get<BusDto[]>('/api/vendors/me/buses'),
    enabled: approved,
  })
  const trips = useQuery({
    queryKey: ['vendor', 'trips'],
    queryFn: () => api.get<VendorTripDto[]>('/api/vendors/me/trips'),
    enabled: approved,
  })
  const settlements = useQuery({
    queryKey: ['vendor', 'settlements'],
    queryFn: () => api.get<VendorSettlementDto[]>('/api/vendors/me/settlements'),
    enabled: approved,
  })

  if (profile.isLoading) return <Loading />

  if (profile.isError || !profile.data) {
    return (
      <>
        <PageHeader title="Vendor portal" subtitle="Your organization and assigned services." />
        <Card>
          <h2 className="text-sm font-semibold text-ink">Application not linked</h2>
          <p className="mt-2 text-sm text-muted">
            {profile.error instanceof Error
              ? profile.error.message
              : 'No vendor application is linked to this account.'}
          </p>
        </Card>
      </>
    )
  }

  const vendor = profile.data
  if (!approved) {
    return (
      <>
        <PageHeader title="Vendor portal" subtitle={vendor.name} />
        <Card>
          <div className="flex flex-wrap items-center justify-between gap-3">
            <div>
              <h2 className="text-lg font-semibold text-ink">
                {vendor.status === 2 ? 'Application not approved' : 'Application under review'}
              </h2>
              <p className="mt-1 text-sm text-muted">
                {vendor.status === 2
                  ? 'The university did not approve this application. Contact the transport office for details.'
                  : 'The university administrator will review your organization. Buses, trips and settlements unlock after approval.'}
              </p>
            </div>
            <Badge tone={vendorTone(vendor.status)}>
              {vendorStatuses[vendor.status] ?? 'Unknown'}
            </Badge>
          </div>

          <dl className="mt-5 grid gap-4 border-t border-line pt-5 sm:grid-cols-2">
            <div>
              <dt className="text-xs font-medium uppercase tracking-wide text-muted">Organization</dt>
              <dd className="mt-1 text-sm font-medium text-ink">{vendor.name}</dd>
            </div>
            <div>
              <dt className="text-xs font-medium uppercase tracking-wide text-muted">Contact person</dt>
              <dd className="mt-1 text-sm text-ink">{vendor.contactPerson || '—'}</dd>
            </div>
            <div>
              <dt className="text-xs font-medium uppercase tracking-wide text-muted">Email</dt>
              <dd className="mt-1 text-sm text-ink">{vendor.email || '—'}</dd>
            </div>
            <div>
              <dt className="text-xs font-medium uppercase tracking-wide text-muted">Contact phone</dt>
              <dd className="mt-1 text-sm text-ink">{vendor.contact || '—'}</dd>
            </div>
          </dl>
        </Card>
      </>
    )
  }

  const vendorTrips = trips.data ?? []
  const scheduledTrips = vendorTrips.filter((trip) => trip.status === 0).length

  return (
    <>
      <PageHeader
        title="Vendor portal"
        subtitle={vendor.name}
        action={<Badge tone="success">Approved vendor</Badge>}
      />

      <div className="mb-6 grid gap-4 sm:grid-cols-3">
        <Card>
          <div className="text-xs font-medium uppercase tracking-wide text-muted">Buses</div>
          <div className="mt-2 text-2xl font-semibold text-ink">{vendor.busCount}</div>
          <div className="mt-1 text-xs text-muted">Registered with your organization</div>
        </Card>
        <Card>
          <div className="text-xs font-medium uppercase tracking-wide text-muted">Scheduled trips</div>
          <div className="mt-2 text-2xl font-semibold text-ink">{scheduledTrips}</div>
          <div className="mt-1 text-xs text-muted">Assigned to your buses</div>
        </Card>
        <Card>
          <div className="text-xs font-medium uppercase tracking-wide text-muted">Monthly subscription</div>
          <div className="mt-2 text-2xl font-semibold text-ink">
            Rs {vendor.monthlySubscription.toFixed(2)}
          </div>
          <div className="mt-1">
            <Badge tone={vendor.subscriptionStatus === 1 ? 'success' : 'warning'}>
              {vendor.subscriptionStatus === 1 ? 'Paid' : 'Pending'}
            </Badge>
          </div>
        </Card>
      </div>

      <section className="mb-8">
        <h2 className="mb-3 text-sm font-semibold text-ink">My buses</h2>
        {buses.isLoading ? (
          <Loading />
        ) : !buses.data?.length ? (
          <Empty>No buses are registered to your organization yet.</Empty>
        ) : (
          <Table>
            <thead>
              <tr>
                <Th>Registration</Th>
                <Th>Capacity</Th>
                <Th>Status</Th>
              </tr>
            </thead>
            <tbody>
              {buses.data.map((bus) => (
                <tr key={bus.id} className="border-b border-line last:border-0">
                  <Td className="font-medium">{bus.registration}</Td>
                  <Td>{bus.capacity}</Td>
                  <Td>
                    <Badge tone={bus.active ? 'success' : 'danger'}>
                      {bus.active ? 'Active' : 'Inactive'}
                    </Badge>
                  </Td>
                </tr>
              ))}
            </tbody>
          </Table>
        )}
      </section>

      <section className="mb-8">
        <h2 className="mb-3 text-sm font-semibold text-ink">Assigned trips</h2>
        {trips.isLoading ? (
          <Loading />
        ) : !vendorTrips.length ? (
          <Empty>No trips are assigned to your buses yet.</Empty>
        ) : (
          <Table>
            <thead>
              <tr>
                <Th>Route</Th>
                <Th>Journey</Th>
                <Th>Departure</Th>
                <Th>Bus</Th>
                <Th>Seats</Th>
                <Th>Fare</Th>
                <Th>Status</Th>
              </tr>
            </thead>
            <tbody>
              {vendorTrips.map((trip) => (
                <tr key={trip.id} className="border-b border-line last:border-0">
                  <Td className="font-medium">{trip.routeCode}</Td>
                  <Td>
                    <div className="text-sm">{trip.routeName}</div>
                    <div className="text-xs text-muted">
                      {trip.origin} → {trip.destination}
                    </div>
                  </Td>
                  <Td>{formatDateTime(trip.departureAt)}</Td>
                  <Td>{trip.busRegistration ?? '—'}</Td>
                  <Td>
                    {trip.bookedCount}/{trip.capacity}
                  </Td>
                  <Td>Rs {trip.fare.toFixed(2)}</Td>
                  <Td>
                    <Badge tone={tripTone(trip.status)}>
                      {tripStatuses[trip.status] ?? 'Unknown'}
                    </Badge>
                  </Td>
                </tr>
              ))}
            </tbody>
          </Table>
        )}
      </section>

      <section>
        <h2 className="mb-3 text-sm font-semibold text-ink">Settlements</h2>
        {settlements.isLoading ? (
          <Loading />
        ) : !settlements.data?.length ? (
          <Empty>No monthly settlements have been recorded yet.</Empty>
        ) : (
          <Table>
            <thead>
              <tr>
                <Th>Period</Th>
                <Th>Gross fares</Th>
                <Th>Subscription due</Th>
                <Th>Status</Th>
                <Th>Paid at</Th>
              </tr>
            </thead>
            <tbody>
              {settlements.data.map((settlement) => (
                <tr key={settlement.id} className="border-b border-line last:border-0">
                  <Td className="font-medium">{settlement.period}</Td>
                  <Td>Rs {settlement.grossFares.toFixed(2)}</Td>
                  <Td>Rs {settlement.subscriptionDue.toFixed(2)}</Td>
                  <Td>
                    <Badge tone={settlement.status === 1 ? 'success' : 'warning'}>
                      {settlement.status === 1 ? 'Paid' : 'Pending'}
                    </Badge>
                  </Td>
                  <Td>{settlement.paidAt ? formatDateTime(settlement.paidAt) : '—'}</Td>
                </tr>
              ))}
            </tbody>
          </Table>
        )}
      </section>
    </>
  )
}
