import { useQuery } from '@tanstack/react-query'
import { ArrowRight } from 'lucide-react'
import { api } from '../lib/api'
import {
  bookingStatusLabels,
  bookingStatusTone,
  paymentMethodLabels,
  paymentStatusLabel,
} from '../lib/bookingLabels'
import { formatDateTime } from '../lib/format'
import type { BookingDto } from '../lib/types'
import { Badge, Loading, PageHeader, Table, Td, Th } from '../components/ui'

export function BookingsPage() {
  const bookings = useQuery({ queryKey: ['bookings'], queryFn: () => api.get<BookingDto[]>('/api/bookings') })

  return (
    <>
      <PageHeader
        title="Bookings"
        subtitle="All reservations, payment methods and capacity usage across trips."
      />

      {bookings.isLoading ? (
        <Loading />
      ) : (bookings.data?.length ?? 0) === 0 ? (
        <div className="rounded-xl border border-dashed border-line p-8 text-center text-sm text-muted">
          No bookings have been made yet.
        </div>
      ) : (
        <Table>
          <thead>
            <tr>
              <Th>Reference</Th>
              <Th>Passenger</Th>
              <Th>Trip</Th>
              <Th>Departure</Th>
              <Th>Amount</Th>
              <Th>Payment</Th>
              <Th>Status</Th>
              <Th>Booked</Th>
            </tr>
          </thead>
          <tbody>
            {(bookings.data ?? []).map((booking) => (
              <tr key={booking.id} className="border-b border-line last:border-0">
                <Td>
                  <div className="whitespace-nowrap font-mono text-xs font-semibold text-ink">{booking.reference}</div>
                </Td>
                <Td>
                  <div className="font-medium text-ink">{booking.passengerName}</div>
                  <div className="text-xs text-muted">{booking.passengerEmail}</div>
                </Td>
                <Td>
                  <div className="font-medium text-ink">
                    {booking.routeCode} · {booking.routeName}
                  </div>
                  {booking.origin && booking.destination ? (
                    <div className="mt-1 flex flex-wrap items-center gap-1.5 text-xs text-muted">
                      <span>{booking.origin}</span>
                      <ArrowRight className="size-3" />
                      <span>{booking.destination}</span>
                    </div>
                  ) : (
                    <div className="mt-1 text-xs text-muted">Stops not set</div>
                  )}
                </Td>
                <Td>{formatDateTime(booking.departureAt)}</Td>
                <Td>Rs {booking.amount.toFixed(2)}</Td>
                <Td>
                  <div className="text-sm">{paymentMethodLabels[booking.method] ?? 'Unknown'}</div>
                  <div className="text-xs text-muted">{paymentStatusLabel(booking)}</div>
                </Td>
                <Td>
                  <Badge tone={bookingStatusTone(booking.status)}>
                    {bookingStatusLabels[booking.status] ?? 'Unknown'}
                  </Badge>
                </Td>
                <Td>
                  <span className="text-xs text-muted">{formatDateTime(booking.createdAt)}</span>
                </Td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </>
  )
}
