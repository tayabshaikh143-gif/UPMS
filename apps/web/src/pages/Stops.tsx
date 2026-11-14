import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ArrowRight, Plus, Trash2 } from 'lucide-react'
import { useState } from 'react'
import { useAuth } from '../auth/AuthContext'
import { api } from '../lib/api'
import type { RouteDetailDto, RouteDto } from '../lib/types'
import { Badge, Button, Field, Input, Loading, PageHeader, cx } from '../components/ui'

type RouteBoard = RouteDto & { stops: RouteDetailDto['stops'] }

export function StopsPage() {
  const { user } = useAuth()
  const isAdmin = user?.roles.includes('Admin') ?? false
  const queryClient = useQueryClient()
  const [addingTo, setAddingTo] = useState<string | null>(null)
  const [stopName, setStopName] = useState('')
  const [error, setError] = useState('')

  const board = useQuery({
    queryKey: ['route-stops'],
    queryFn: async () => {
      const routes = await api.get<RouteDto[]>('/api/routes')
      const details = await Promise.all(
        routes.map((route) => api.get<RouteDetailDto>(`/api/routes/${route.id}`)),
      )
      return routes.map<RouteBoard>((route, index) => ({
        ...route,
        stops: details[index].stops,
      }))
    },
  })

  async function refresh() {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: ['route-stops'] }),
      queryClient.invalidateQueries({ queryKey: ['routes'] }),
      queryClient.invalidateQueries({ queryKey: ['trips'] }),
    ])
  }

  const addStop = useMutation({
    mutationFn: ({ routeId, name }: { routeId: string; name: string }) =>
      api.post<void>(`/api/routes/${routeId}/stops/by-name`, { name }),
    onSuccess: async () => {
      setAddingTo(null)
      setStopName('')
      setError('')
      await refresh()
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not add stop'),
  })

  const removeStop = useMutation({
    mutationFn: ({ routeId, routeStopId }: { routeId: string; routeStopId: string }) =>
      api.delete<void>(`/api/routes/${routeId}/stops/${routeStopId}`),
    onSuccess: refresh,
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not remove stop'),
  })

  return (
    <>
      <PageHeader
        title="Stops"
        subtitle="The ordered stops that make up each route, from origin to destination."
      />

      {error && <p className="mb-3 text-sm text-danger">{error}</p>}

      {board.isLoading ? (
        <Loading />
      ) : (board.data?.length ?? 0) === 0 ? (
        <div className="rounded-xl border border-dashed border-line p-8 text-center text-sm text-muted">
          No routes yet. Create a route first, then add its stops here.
        </div>
      ) : (
        <div className="space-y-5">
          {(board.data ?? []).map((route) => {
            const stops = route.stops
            const origin = stops[0]?.stopName
            const destination = stops[stops.length - 1]?.stopName
            const isAdding = addingTo === route.id

            return (
              <div key={route.id} className="rounded-xl border border-line bg-surface">
                <div className="flex flex-wrap items-start justify-between gap-3 border-b border-line p-5">
                  <div>
                    <div className="flex flex-wrap items-center gap-2">
                      <Badge>{route.code}</Badge>
                      <h2 className="text-lg font-semibold text-ink">{route.name}</h2>
                    </div>
                    {origin && destination ? (
                      <div className="mt-2 flex flex-wrap items-center gap-1.5 text-sm text-muted">
                        <span>{origin}</span>
                        <ArrowRight className="size-3.5 shrink-0" />
                        <span>{destination}</span>
                      </div>
                    ) : (
                      <p className="mt-2 text-sm text-muted">No stops added yet.</p>
                    )}
                  </div>

                  {isAdmin && (
                    <Button
                      variant="secondary"
                      onClick={() => {
                        setAddingTo(isAdding ? null : route.id)
                        setStopName('')
                        setError('')
                      }}
                    >
                      <Plus className="size-4" />
                      Add stop
                    </Button>
                  )}
                </div>

                <div className="p-5">
                  {stops.length === 0 ? (
                    <div className="rounded-lg border border-dashed border-line p-6 text-center text-sm text-muted">
                      Add the first stop for this route.
                    </div>
                  ) : (
                    <ol>
                      {stops.map((stop, index) => {
                        const isOrigin = index === 0
                        const isDestination = index === stops.length - 1
                        return (
                          <li key={stop.id} className="flex gap-3">
                            <div className="flex flex-col items-center">
                              <span
                                className={cx(
                                  'grid size-7 shrink-0 place-items-center rounded-full text-xs font-semibold',
                                  isOrigin || isDestination
                                    ? 'bg-brand text-white'
                                    : 'bg-brand-soft text-brand',
                                )}
                              >
                                {index + 1}
                              </span>
                              {!isDestination && <span className="my-1 h-7 w-px bg-line" />}
                            </div>

                            <div className="flex min-w-0 flex-1 items-start justify-between gap-3 pb-4">
                              <div>
                                <div className="flex flex-wrap items-center gap-2">
                                  <span className="text-sm font-medium text-ink">{stop.stopName}</span>
                                  {isOrigin && <Badge>Origin</Badge>}
                                  {isDestination && <Badge tone="success">Destination</Badge>}
                                </div>
                                <div className="mt-0.5 text-xs text-muted">
                                  {stop.offsetMinutes === 0
                                    ? 'Starts here'
                                    : `+${stop.offsetMinutes} min from origin`}
                                </div>
                              </div>

                              {isAdmin && (
                                <Button
                                  variant="ghost"
                                  aria-label={`Remove ${stop.stopName} from ${route.code}`}
                                  onClick={() => {
                                    if (confirm(`Remove ${stop.stopName} from route ${route.code}?`)) {
                                      removeStop.mutate({ routeId: route.id, routeStopId: stop.id })
                                    }
                                  }}
                                >
                                  <Trash2 className="size-4" />
                                </Button>
                              )}
                            </div>
                          </li>
                        )
                      })}
                    </ol>
                  )}

                  {isAdding && isAdmin && (
                    <form
                      className="mt-4 grid gap-3 rounded-lg bg-paper p-4 sm:grid-cols-[1fr_auto]"
                      onSubmit={(event) => {
                        event.preventDefault()
                        setError('')
                        addStop.mutate({ routeId: route.id, name: stopName })
                      }}
                    >
                      <Field label="Stop name">
                        <Input
                          value={stopName}
                          onChange={(event) => setStopName(event.target.value)}
                          placeholder="e.g. Kotri Bridge"
                          required
                        />
                      </Field>
                      <div className="flex items-end gap-2">
                        <Button type="submit" disabled={addStop.isPending}>
                          {addStop.isPending ? 'Adding…' : 'Add stop'}
                        </Button>
                        <Button
                          type="button"
                          variant="ghost"
                          onClick={() => {
                            setAddingTo(null)
                            setStopName('')
                          }}
                        >
                          Cancel
                        </Button>
                      </div>
                    </form>
                  )}
                </div>
              </div>
            )
          })}
        </div>
      )}
    </>
  )
}
