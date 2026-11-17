import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Check, RefreshCw, UserX } from 'lucide-react'
import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { api } from '../lib/api'
import { formatDateTime } from '../lib/format'
import type { StaffPassengerDto, StaffTripDetailDto, StaffTripDto } from '../lib/types'
import {
  Badge,
  Button,
  Card,
  Empty,
  Field,
  Input,
  Loading,
  PageHeader,
  Table,
  Td,
  Th,
  cx,
} from '../components/ui'

const tripStatusLabels = ['Scheduled', 'Delayed', 'Cancelled', 'Completed']
const bookingStatusLabels = ['Reserved', 'Confirmed', 'Cancelled', 'Expired', 'Boarded']
const offlineStatusLabels = ['Issued', 'Boarded', 'Cancelled']
const methodLabels = ['Online', 'Cash']
const paymentStatusLabels = ['Pending', 'Paid', 'Refunded', 'Failed', 'Approved']

function tripTone(status: number) {
  if (status === 1) return 'warning' as const
  if (status === 2) return 'danger' as const
  if (status === 3) return 'success' as const
  return 'neutral' as const
}

function passengerTone(passenger: StaffPassengerDto) {
  if (passenger.source === 'Offline') {
    if (passenger.status === 1) return 'success' as const
    if (passenger.status === 2) return 'danger' as const
    return 'warning' as const
  }

  if (passenger.status === 4) return 'success' as const
  if (passenger.status === 2) return 'danger' as const
  if (passenger.status === 3) return 'neutral' as const
  return 'warning' as const
}

function paymentTone(status: number) {
  if (status === 1 || status === 4) return 'success' as const
  if (status === 3) return 'danger' as const
  if (status === 2) return 'neutral' as const
  return 'warning' as const
}

function passengerStatusLabel(passenger: StaffPassengerDto) {
  if (passenger.source === 'Offline') {
    return offlineStatusLabels[passenger.status] ?? 'Unknown'
  }
  return bookingStatusLabels[passenger.status] ?? 'Unknown'
}

export function StaffPortalPage() {
  const queryClient = useQueryClient()
  const [selectedTripId, setSelectedTripId] = useState<string | null>(null)
  const [passengerName, setPassengerName] = useState('')
  const [passengerPhone, setPassengerPhone] = useState('')
  const [amount, setAmount] = useState('')
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')

  const trips = useQuery({
    queryKey: ['staff', 'trips'],
    queryFn: () => api.get<StaffTripDto[]>('/api/staff/trips'),
  })
  const detail = useQuery({
    queryKey: ['staff', 'trip', selectedTripId],
    queryFn: () => api.get<StaffTripDetailDto>(`/api/staff/trips/${selectedTripId}/passengers`),
    enabled: Boolean(selectedTripId),
  })

  useEffect(() => {
    if (selectedTripId || !trips.data?.length) return
    const firstOpen = trips.data.find((trip) => trip.boardingOpen) ?? trips.data[0]
    setSelectedTripId(firstOpen.id)
  }, [selectedTripId, trips.data])

  const refresh = async () => {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: ['staff', 'trips'] }),
      queryClient.invalidateQueries({ queryKey: ['staff', 'trip', selectedTripId] }),
    ])
  }

  const boardBooking = useMutation({
    mutationFn: (bookingId: string) =>
      api.post<void>(`/api/staff/trips/${selectedTripId}/bookings/${bookingId}/board`, {}),
    onSuccess: async () => {
      setError('')
      setNotice('Passenger boarded.')
      await refresh()
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not board passenger'),
  })

  const noShowBooking = useMutation({
    mutationFn: (bookingId: string) =>
      api.post<void>(`/api/staff/trips/${selectedTripId}/bookings/${bookingId}/no-show`, {}),
    onSuccess: async () => {
      setError('')
      setNotice('Passenger marked no-show.')
      await refresh()
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not mark no-show'),
  })

  const boardOfflineTicket = useMutation({
    mutationFn: (ticketId: string) =>
      api.post<void>(`/api/staff/trips/${selectedTripId}/offline-tickets/${ticketId}/board`, {}),
    onSuccess: async () => {
      setError('')
      setNotice('Offline ticket boarded.')
      await refresh()
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not board ticket'),
  })

  const issueOfflineTicket = useMutation({
    mutationFn: async () => {
      await api.post<StaffPassengerDto>(`/api/staff/trips/${selectedTripId}/offline-tickets`, {
        passengerName,
        passengerPhone,
        amount: amount.trim() ? Number(amount) : null,
      })
    },
    onSuccess: async () => {
      setError('')
      setNotice('Offline ticket issued.')
      setPassengerName('')
      setPassengerPhone('')
      setAmount('')
      await refresh()
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not issue ticket'),
  })

  function issueTicket(event: FormEvent) {
    event.preventDefault()
    setNotice('')
    issueOfflineTicket.mutate()
  }

  const trip = detail.data?.trip
  const passengers = detail.data?.passengers ?? []
  const busy = boardBooking.isPending || noShowBooking.isPending || boardOfflineTicket.isPending

  return (
    <>
      <PageHeader
        title="Boarding console"
        subtitle="Check in booked passengers and issue offline cash tickets."
        action={
          <Button
            variant="secondary"
            onClick={() => {
              void refresh()
            }}
            disabled={trips.isFetching || detail.isFetching}
          >
            <RefreshCw className={cx('size-4', (trips.isFetching || detail.isFetching) && 'animate-spin')} />
            Refresh
          </Button>
        }
      />

      {(error || notice) && (
        <div className="mb-4 space-y-1">
          {error && <p className="text-sm text-danger">{error}</p>}
          {notice && <p className="text-sm text-accent">{notice}</p>}
        </div>
      )}

      {trips.isLoading ? (
        <Loading />
      ) : !trips.data?.length ? (
        <Empty>No trips are available for boarding in the next 7 days.</Empty>
      ) : (
        <div className="grid gap-5 lg:grid-cols-[320px_1fr]">
          <div>
            <h2 className="mb-3 text-sm font-semibold text-ink">Trips</h2>
            <div className="space-y-2">
              {trips.data.map((item) => (
                <button
                  key={item.id}
                  type="button"
                  onClick={() => {
                    setSelectedTripId(item.id)
                    setNotice('')
                    setError('')
                  }}
                  className={cx(
                    'w-full rounded-xl border p-3 text-left transition',
                    item.id === selectedTripId
                      ? 'border-brand bg-brand-soft/60'
                      : 'border-line bg-surface hover:border-brand/40',
                  )}
                >
                  <div className="flex items-center justify-between gap-2">
                    <span className="text-xs font-semibold uppercase tracking-wide text-muted">
                      {item.routeCode}
                    </span>
                    <Badge tone={item.boardingOpen ? 'success' : 'neutral'}>
                      {item.boardingOpen ? 'Boarding open' : 'Opens later'}
                    </Badge>
                  </div>
                  <div className="mt-1 text-sm font-medium text-ink">{item.routeName}</div>
                  <div className="mt-1 text-xs text-muted">
                    {formatDateTime(item.departureAt)} · {item.busRegistration ?? 'No bus'}
                  </div>
                  <div className="mt-1 text-xs text-muted">
                    {item.bookedCount}/{item.capacity} seats · {item.remainingSeats} left
                  </div>
                </button>
              ))}
            </div>
          </div>

          <div className="min-w-0">
            {!selectedTripId ? (
              <Empty>Select a trip to open its passenger list.</Empty>
            ) : detail.isLoading ? (
              <Loading />
            ) : detail.isError || !trip ? (
              <Card>
                <p className="text-sm text-danger">
                  {detail.error instanceof Error ? detail.error.message : 'Trip not found.'}
                </p>
              </Card>
            ) : (
              <>
                <Card className="mb-5">
                  <div className="flex flex-wrap items-start justify-between gap-3">
                    <div>
                      <div className="text-xs font-semibold uppercase tracking-wide text-muted">
                        {trip.routeCode}
                      </div>
                      <h2 className="mt-1 text-lg font-semibold text-ink">{trip.routeName}</h2>
                      <p className="mt-1 text-sm text-muted">
                        {trip.origin} → {trip.destination}
                      </p>
                      <p className="mt-2 text-sm text-ink">
                        {formatDateTime(trip.departureAt)} · {trip.busRegistration ?? 'No bus'} ·{' '}
                        {trip.bookedCount}/{trip.capacity} seats
                      </p>
                    </div>
                    <div className="text-right">
                      <Badge tone={tripTone(trip.status)}>
                        {tripStatusLabels[trip.status] ?? 'Unknown'}
                      </Badge>
                      <div className="mt-2 text-xs text-muted">
                        Boarding from {formatDateTime(trip.boardingStartsAt)}
                      </div>
                    </div>
                  </div>
                </Card>

                <Card className="mb-5">
                  <div className="flex flex-wrap items-center justify-between gap-2">
                    <h2 className="text-sm font-semibold text-ink">Issue offline ticket</h2>
                    <Badge tone={trip.boardingOpen ? 'success' : 'warning'}>
                      {trip.boardingOpen ? 'Boarding open' : 'Boarding not open yet'}
                    </Badge>
                  </div>
                  <p className="mt-1 text-xs text-muted">
                    Cash ticket for a walk-up passenger. The trip fare is used when the amount is
                    left blank.
                  </p>

                  <form
                    className="mt-4 grid gap-3 sm:grid-cols-[1fr_1fr_150px_auto]"
                    onSubmit={issueTicket}
                  >
                    <Field label="Passenger name">
                      <Input
                        value={passengerName}
                        onChange={(e) => setPassengerName(e.target.value)}
                        placeholder="Walk-up passenger"
                        disabled={!trip.boardingOpen || issueOfflineTicket.isPending}
                        required
                      />
                    </Field>
                    <Field label="Phone">
                      <Input
                        value={passengerPhone}
                        onChange={(e) => setPassengerPhone(e.target.value)}
                        placeholder="0300 0000000"
                        disabled={!trip.boardingOpen || issueOfflineTicket.isPending}
                        required
                      />
                    </Field>
                    <Field label={`Amount (fare Rs ${trip.fare.toFixed(2)})`}>
                      <Input
                        type="number"
                        min={1}
                        step="0.01"
                        value={amount}
                        onChange={(e) => setAmount(e.target.value)}
                        placeholder={String(trip.fare)}
                        disabled={!trip.boardingOpen || issueOfflineTicket.isPending}
                      />
                    </Field>
                    <div className="flex items-end">
                      <Button
                        type="submit"
                        className="w-full"
                        disabled={!trip.boardingOpen || issueOfflineTicket.isPending}
                      >
                        {issueOfflineTicket.isPending ? 'Issuing…' : 'Issue ticket'}
                      </Button>
                    </div>
                  </form>
                </Card>

                <div className="mb-3 flex items-center justify-between">
                  <h2 className="text-sm font-semibold text-ink">
                    Passengers ({passengers.length})
                  </h2>
                  <span className="text-xs text-muted">
                    {trip.remainingSeats} seat{trip.remainingSeats === 1 ? '' : 's'} remaining
                  </span>
                </div>

                {!passengers.length ? (
                  <Empty>No passengers are booked on this trip yet.</Empty>
                ) : (
                  <Table>
                    <thead>
                      <tr>
                        <Th>Reference</Th>
                        <Th>Passenger</Th>
                        <Th>Ticket</Th>
                        <Th>Status</Th>
                        <Th>Action</Th>
                      </tr>
                    </thead>
                    <tbody>
                      {passengers.map((passenger) => {
                        const activeBooking =
                          passenger.source === 'Booking' &&
                          (passenger.status === 0 || passenger.status === 1)
                        const issuedOffline =
                          passenger.source === 'Offline' && passenger.status === 0

                        return (
                          <tr key={`${passenger.source}-${passenger.id}`} className="border-b border-line last:border-0">
                            <Td className="whitespace-nowrap font-medium">{passenger.reference}</Td>
                            <Td>
                              <div className="font-medium">{passenger.passengerName}</div>
                              <div className="text-xs text-muted">{passenger.contact ?? '—'}</div>
                            </Td>
                            <Td>
                              <div className="text-sm">
                                {passenger.source === 'Offline'
                                  ? 'Offline cash'
                                  : methodLabels[passenger.method] ?? 'Unknown'}
                              </div>
                              <div className="mt-1">
                                <Badge tone={paymentTone(passenger.paymentStatus)}>
                                  {paymentStatusLabels[passenger.paymentStatus] ?? 'Unknown'}
                                </Badge>
                              </div>
                            </Td>
                            <Td>
                              <Badge tone={passengerTone(passenger)}>
                                {passengerStatusLabel(passenger)}
                              </Badge>
                            </Td>
                            <Td>
                              <div className="flex items-center gap-1">
                                {(activeBooking || issuedOffline) && (
                                  <Button
                                    variant="secondary"
                                    className="whitespace-nowrap px-2.5 py-1.5 text-xs"
                                    onClick={() =>
                                      passenger.source === 'Offline'
                                        ? boardOfflineTicket.mutate(passenger.id)
                                        : boardBooking.mutate(passenger.id)
                                    }
                                    disabled={!trip.boardingOpen || busy}
                                  >
                                    <Check className="size-3.5" />
                                    Board
                                  </Button>
                                )}
                                {activeBooking && (
                                  <Button
                                    variant="ghost"
                                    className="whitespace-nowrap px-2.5 py-1.5 text-xs text-danger"
                                    onClick={() => noShowBooking.mutate(passenger.id)}
                                    disabled={!trip.boardingOpen || busy}
                                  >
                                    <UserX className="size-3.5" />
                                    No-show
                                  </Button>
                                )}
                                {!activeBooking && !issuedOffline && (
                                  <span className="text-xs text-muted">—</span>
                                )}
                              </div>
                            </Td>
                          </tr>
                        )
                      })}
                    </tbody>
                  </Table>
                )}
              </>
            )}
          </div>
        </div>
      )}
    </>
  )
}
