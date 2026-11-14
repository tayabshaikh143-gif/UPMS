import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ArrowRight, Pencil, Plus, Trash2 } from 'lucide-react'
import { useState } from 'react'
import { useAuth } from '../auth/AuthContext'
import { api } from '../lib/api'
import { formatDateTime } from '../lib/format'
import type { RouteDto, TripDto } from '../lib/types'
import {
  Badge,
  Button,
  Card,
  Field,
  Input,
  Loading,
  PageHeader,
  Select,
  Table,
  Td,
  Th,
} from '../components/ui'

const statuses = ['Scheduled', 'Delayed', 'Cancelled', 'Completed']

function toInputValue(value: string) {
  return value.length >= 16 ? value.slice(0, 16) : value
}

export function TripsPage() {
  const { user } = useAuth()
  const isAdmin = user?.roles.includes('Admin') ?? false
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [routeId, setRouteId] = useState('')
  const [departureAt, setDepartureAt] = useState('')
  const [fare, setFare] = useState('120')
  const [status, setStatus] = useState('0')
  const [error, setError] = useState('')

  const trips = useQuery({ queryKey: ['trips'], queryFn: () => api.get<TripDto[]>('/api/trips') })
  const routes = useQuery({ queryKey: ['routes'], queryFn: () => api.get<RouteDto[]>('/api/routes') })

  function startCreate() {
    setEditingId(null)
    setRouteId('')
    setDepartureAt('')
    setFare('120')
    setStatus('0')
    setError('')
    setOpen(true)
  }

  function startEdit(trip: TripDto) {
    setEditingId(trip.id)
    setRouteId(trip.routeId)
    setDepartureAt(toInputValue(trip.departureAt))
    setFare(String(trip.fare))
    setStatus(String(trip.status))
    setError('')
    setOpen(true)
  }

  function closeForm() {
    setEditingId(null)
    setRouteId('')
    setDepartureAt('')
    setFare('120')
    setStatus('0')
    setError('')
    setOpen(false)
  }

  const save = useMutation({
    mutationFn: async () => {
      const body = {
        routeId,
        departureAt,
        fare: Number(fare),
        status: Number(status),
      }
      if (editingId) {
        await api.put<void>(`/api/trips/${editingId}`, body)
      } else {
        await api.post<TripDto>('/api/trips', body)
      }
    },
    onSuccess: async () => {
      closeForm()
      await queryClient.invalidateQueries({ queryKey: ['trips'] })
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not save trip'),
  })

  const remove = useMutation({
    mutationFn: (id: string) => api.delete<void>(`/api/trips/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['trips'] }),
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not delete trip'),
  })

  return (
    <>
      <PageHeader
        title="Trips"
        subtitle="Dated services on approved routes."
        action={
          isAdmin && (
            <Button onClick={() => (open && !editingId ? closeForm() : startCreate())}>
              <Plus className="size-4" />
              Add trip
            </Button>
          )
        }
      />

      {open && isAdmin && (
        <Card className="mb-5">
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-sm font-semibold text-ink">
              {editingId ? 'Edit trip' : 'New trip'}
            </h2>
            <Button type="button" variant="ghost" onClick={closeForm}>
              Cancel
            </Button>
          </div>
          <form
            className="grid gap-4 sm:grid-cols-2 lg:grid-cols-[1fr_1fr_120px_150px_auto]"
            onSubmit={(event) => {
              event.preventDefault()
              setError('')
              save.mutate()
            }}
          >
            <Field label="Route">
              <Select value={routeId} onChange={(e) => setRouteId(e.target.value)} required>
                <option value="">Select a route</option>
                {(routes.data ?? []).map((route) => (
                  <option key={route.id} value={route.id}>
                    {route.code} — {route.name}
                  </option>
                ))}
              </Select>
            </Field>
            <Field label="Departure">
              <Input
                type="datetime-local"
                value={departureAt}
                onChange={(e) => setDepartureAt(e.target.value)}
                required
              />
            </Field>
            <Field label="Fare (Rs)">
              <Input type="number" min={0} value={fare} onChange={(e) => setFare(e.target.value)} required />
            </Field>
            <Field label="Status">
              <Select value={status} onChange={(e) => setStatus(e.target.value)}>
                {statuses.map((label, index) => (
                  <option key={label} value={index}>
                    {label}
                  </option>
                ))}
              </Select>
            </Field>
            <div className="flex items-end">
              <Button type="submit" disabled={save.isPending}>
                {save.isPending ? 'Saving…' : editingId ? 'Update' : 'Save'}
              </Button>
            </div>
          </form>
        </Card>
      )}

      {error && <p className="mb-3 text-sm text-danger">{error}</p>}

      {trips.isLoading ? (
        <Loading />
      ) : (
        <Table>
          <thead>
            <tr>
              <Th>Route</Th>
              <Th>Journey</Th>
              <Th>Departure</Th>
              <Th>Fare</Th>
              <Th>Status</Th>
              {isAdmin && <Th> </Th>}
            </tr>
          </thead>
          <tbody>
            {(trips.data ?? []).map((trip) => (
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
                  <Badge
                    tone={
                      trip.status === 2
                        ? 'danger'
                        : trip.status === 1
                          ? 'warning'
                          : trip.status === 3
                            ? 'neutral'
                            : 'success'
                    }
                  >
                    {statuses[trip.status] ?? 'Unknown'}
                  </Badge>
                </Td>
                {isAdmin && (
                  <Td>
                    <div className="flex items-center gap-1">
                      <Button
                        variant="ghost"
                        onClick={() => startEdit(trip)}
                        aria-label="Edit trip"
                      >
                        <Pencil className="size-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        onClick={() => {
                          if (confirm('Delete this trip? This cannot be undone.')) {
                            remove.mutate(trip.id)
                          }
                        }}
                        aria-label="Delete trip"
                      >
                        <Trash2 className="size-4" />
                      </Button>
                    </div>
                  </Td>
                )}
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </>
  )
}
