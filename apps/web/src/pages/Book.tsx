import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  ArrowRight,
  Bus,
  CalendarDays,
  CheckCircle2,
  CreditCard,
  Ticket,
  Users,
  Wallet,
} from 'lucide-react'
import { useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../lib/api'
import { formatDateTime } from '../lib/format'
import type { BookingDto, TripDto } from '../lib/types'
import { Badge, Button, Card, Loading, PageHeader, cx } from '../components/ui'

export function BookPage() {
  const queryClient = useQueryClient()
  const [selected, setSelected] = useState<TripDto | null>(null)
  const [method, setMethod] = useState(0)
  const [success, setSuccess] = useState<BookingDto | null>(null)
  const [error, setError] = useState('')

  const trips = useQuery({ queryKey: ['trips'], queryFn: () => api.get<TripDto[]>('/api/trips') })

  const available = [...(trips.data ?? [])]
    .filter((trip) => trip.status === 0 && new Date(trip.departureAt) > new Date())
    .sort((a, b) => a.departureAt.localeCompare(b.departureAt))

  const create = useMutation({
    mutationFn: () =>
      api.post<BookingDto>('/api/bookings', {
        tripId: selected!.id,
        method,
      }),
    onSuccess: async (booking) => {
      setSuccess(booking)
      setSelected(null)
      setError('')
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['trips'] }),
        queryClient.invalidateQueries({ queryKey: ['my-bookings'] }),
      ])
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not book this trip'),
  })

  return (
    <>
      <PageHeader
        title="Book a trip"
        subtitle="One ticket reserves one seat. No numbered seat selection."
      />

      {success && (
        <div className="mb-6 rounded-xl border border-accent/30 bg-accent-soft p-5">
          <div className="flex items-start gap-3">
            <CheckCircle2 className="mt-0.5 size-5 shrink-0 text-accent" />
            <div>
              <div className="font-semibold text-ink">Booking confirmed</div>
              <p className="mt-1 text-sm text-muted">
                Reference <span className="font-mono font-semibold text-ink">{success.reference}</span>{' '}
                — {success.routeCode}, {formatDateTime(success.departureAt)}.
              </p>
              <Link to="/my-bookings" className="mt-2 inline-block text-sm font-medium text-accent hover:underline">
                View my bookings
              </Link>
            </div>
          </div>
        </div>
      )}

      {selected && (
        <Card className="mb-6 border-accent/40">
          <div className="flex flex-wrap items-start justify-between gap-3">
            <div>
              <h2 className="text-lg font-semibold text-ink">Confirm booking</h2>
              <p className="mt-1 text-sm text-muted">
                {selected.routeCode} · {selected.routeName}
              </p>
              <div className="mt-2 flex flex-wrap items-center gap-1.5 text-sm text-ink">
                <span>{selected.origin}</span>
                <ArrowRight className="size-3.5 shrink-0 text-muted" />
                <span>{selected.destination}</span>
              </div>
            </div>
            <Button variant="ghost" onClick={() => setSelected(null)}>
              Cancel
            </Button>
          </div>

          <div className="mt-4 grid gap-3 text-sm sm:grid-cols-3">
            <div>
              <div className="text-xs uppercase tracking-wide text-muted">Departure</div>
              <div className="mt-1 font-medium text-ink">{formatDateTime(selected.departureAt)}</div>
            </div>
            <div>
              <div className="text-xs uppercase tracking-wide text-muted">Bus</div>
              <div className="mt-1 font-medium text-ink">{selected.busRegistration ?? 'Not assigned'}</div>
            </div>
            <div>
              <div className="text-xs uppercase tracking-wide text-muted">Fare</div>
              <div className="mt-1 font-medium text-ink">Rs {selected.fare.toFixed(2)}</div>
            </div>
          </div>

          <div className="mt-5">
            <div className="mb-2 text-xs font-medium uppercase tracking-wide text-muted">Payment method</div>
            <div className="grid gap-3 sm:grid-cols-2">
              <button
                type="button"
                onClick={() => setMethod(0)}
                className={cx(
                  'rounded-lg border p-3 text-left transition',
                  method === 0 ? 'border-accent bg-accent-soft' : 'border-line bg-surface hover:bg-paper',
                )}
              >
                <div className="flex items-center gap-2 text-sm font-medium text-ink">
                  <CreditCard className="size-4" /> Pay online
                </div>
                <p className="mt-1 text-xs text-muted">Demo gateway — confirms immediately.</p>
              </button>
              <button
                type="button"
                onClick={() => setMethod(1)}
                className={cx(
                  'rounded-lg border p-3 text-left transition',
                  method === 1 ? 'border-accent bg-accent-soft' : 'border-line bg-surface hover:bg-paper',
                )}
              >
                <div className="flex items-center gap-2 text-sm font-medium text-ink">
                  <Wallet className="size-4" /> Cash at boarding
                </div>
                <p className="mt-1 text-xs text-muted">
                  Seat held until 5 minutes before departure.
                </p>
              </button>
            </div>
          </div>

          {error && <p className="mt-3 text-sm text-danger">{error}</p>}

          <Button className="mt-4" onClick={() => create.mutate()} disabled={create.isPending}>
            <Ticket className="size-4" />
            {create.isPending ? 'Booking…' : `Confirm booking · Rs ${selected.fare.toFixed(2)}`}
          </Button>
        </Card>
      )}

      {trips.isLoading ? (
        <Loading />
      ) : available.length === 0 ? (
        <div className="rounded-xl border border-dashed border-line p-8 text-center text-sm text-muted">
          No upcoming trips are open for booking right now.
        </div>
      ) : (
        <div className="grid gap-4 xl:grid-cols-2">
          {available.map((trip) => {
            const occupancy = trip.capacity > 0 ? Math.round((trip.bookedCount / trip.capacity) * 100) : 0
            const soldOut = trip.remainingSeats <= 0

            return (
              <div key={trip.id} className="rounded-xl border border-line bg-surface p-5">
                <div className="flex flex-wrap items-start justify-between gap-3">
                  <div>
                    <div className="flex flex-wrap items-center gap-2">
                      <Badge>{trip.routeCode}</Badge>
                      <h2 className="font-semibold text-ink">{trip.routeName}</h2>
                    </div>
                    <div className="mt-2 flex flex-wrap items-center gap-1.5 text-sm text-muted">
                      <span>{trip.origin || 'Origin not set'}</span>
                      <ArrowRight className="size-3.5 shrink-0" />
                      <span>{trip.destination || 'Destination not set'}</span>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="text-lg font-semibold text-ink">Rs {trip.fare.toFixed(2)}</div>
                    <div className="text-xs text-muted">{soldOut ? 'Sold out' : `${trip.remainingSeats} seats left`}</div>
                  </div>
                </div>

                <div className="mt-4 grid gap-2 text-sm text-muted sm:grid-cols-3">
                  <div className="flex items-center gap-2">
                    <CalendarDays className="size-4" />
                    {formatDateTime(trip.departureAt)}
                  </div>
                  <div className="flex items-center gap-2">
                    <Bus className="size-4" />
                    {trip.busRegistration ?? 'No bus'}
                  </div>
                  <div className="flex items-center gap-2">
                    <Users className="size-4" />
                    {trip.bookedCount}/{trip.capacity} booked
                  </div>
                </div>

                <div className="mt-3 h-1.5 overflow-hidden rounded-full bg-line">
                  <div
                    className={cx('h-full rounded-full', occupancy >= 100 ? 'bg-danger' : 'bg-accent')}
                    style={{ width: `${Math.min(100, occupancy)}%` }}
                  />
                </div>

                <Button
                  className="mt-4 w-full"
                  disabled={soldOut}
                  onClick={() => {
                    setSelected(trip)
                    setSuccess(null)
                    setError('')
                    setMethod(0)
                  }}
                >
                  {soldOut ? 'Fully booked' : 'Book a seat'}
                </Button>
              </div>
            )
          })}
        </div>
      )}
    </>
  )
}
