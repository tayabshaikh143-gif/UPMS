import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ArrowRight, Ticket, XCircle } from 'lucide-react'
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../lib/api'
import {
  CANCELLATION_CUTOFF_MINUTES,
  bookingStatusLabels,
  bookingStatusTone,
  paymentMethodLabels,
  paymentStatusLabel,
} from '../lib/bookingLabels'
import { formatDateTime } from '../lib/format'
import type { BookingDto } from '../lib/types'
import { Badge, Button, Loading, PageHeader, Table, Td, Th } from '../components/ui'
import { TicketReceipt } from '../components/TicketReceipt'

function canCancel(booking: BookingDto) {
  const active = booking.status === 0 || booking.status === 1
  const cutoff = new Date(booking.departureAt).getTime() - CANCELLATION_CUTOFF_MINUTES * 60 * 1000
  return active && Date.now() < cutoff
}

export function MyBookingsPage() {
  const queryClient = useQueryClient()
  const [receipt, setReceipt] = useState<BookingDto | null>(null)
  const bookings = useQuery({
    queryKey: ['my-bookings'],
    queryFn: () => api.get<BookingDto[]>('/api/bookings/my'),
  })

  const cancel = useMutation({
    mutationFn: (id: string) => api.post<BookingDto>(`/api/bookings/${id}/cancel`, {}),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['my-bookings'] }),
        queryClient.invalidateQueries({ queryKey: ['trips'] }),
      ])
    },
  })

  return (
    <>
      <PageHeader
        title="My bookings"
        subtitle="Your ticket references and live booking status."
        action={
          <Link
            to="/book"
            className="inline-flex items-center gap-2 rounded-lg bg-brand px-3.5 py-2 text-sm font-medium text-white transition hover:bg-brand/90"
          >
            <Ticket className="size-4" />
            Book a trip
          </Link>
        }
      />

      <p className="mb-4 text-xs text-muted">
        Self-service cancellation is open until {CANCELLATION_CUTOFF_MINUTES} minutes before
        departure. Online refunds are simulated in this MVP.
      </p>

      {cancel.isError && (
        <p className="mb-3 text-sm text-danger">
          {cancel.error instanceof Error ? cancel.error.message : 'Could not cancel booking'}
        </p>
      )}

      {bookings.isLoading ? (
        <Loading />
      ) : (bookings.data?.length ?? 0) === 0 ? (
        <div className="rounded-xl border border-dashed border-line p-8 text-center text-sm text-muted">
          <p>You have no bookings yet.</p>
          <Link to="/book" className="mt-2 inline-block font-medium text-accent hover:underline">
            Browse upcoming trips
          </Link>
        </div>
      ) : (
        <Table>
          <thead>
            <tr>
              <Th>Reference</Th>
              <Th>Trip</Th>
              <Th>Departure</Th>
              <Th>Amount</Th>
              <Th>Payment</Th>
              <Th>Status</Th>
              <Th> </Th>
            </tr>
          </thead>
          <tbody>
            {(bookings.data ?? []).map((booking) => (
              <tr key={booking.id} className="border-b border-line last:border-0">
                <Td>
                  <div className="whitespace-nowrap font-mono text-xs font-semibold text-ink">{booking.reference}</div>
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
                  <div className="flex items-center gap-1">
                    <Button
                      variant="secondary"
                      onClick={() => setReceipt(booking)}
                      aria-label={`Show ticket ${booking.reference}`}
                    >
                      <Ticket className="size-4" />
                      Show ticket
                    </Button>
                    {canCancel(booking) && (
                      <Button
                        variant="ghost"
                        onClick={() => {
                          if (
                            confirm(
                              `Cancel booking ${booking.reference}? The seat will be released immediately.`,
                            )
                          ) {
                            cancel.mutate(booking.id)
                          }
                        }}
                        aria-label={`Cancel ${booking.reference}`}
                      >
                        <XCircle className="size-4" />
                        Cancel
                      </Button>
                    )}
                  </div>
                </Td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}

      {receipt && <TicketReceipt booking={receipt} onClose={() => setReceipt(null)} />}
    </>
  )
}
