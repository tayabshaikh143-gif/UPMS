import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useState } from 'react'
import { useAuth } from '../auth/AuthContext'
import { api } from '../lib/api'
import type { RouteDto } from '../lib/types'
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

export function RoutesPage() {
  const { user } = useAuth()
  const isAdmin = user?.roles.includes('Admin') ?? false
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [code, setCode] = useState('')
  const [name, setName] = useState('')
  const [active, setActive] = useState('true')
  const [error, setError] = useState('')

  const routes = useQuery({ queryKey: ['routes'], queryFn: () => api.get<RouteDto[]>('/api/routes') })

  function startCreate() {
    setEditingId(null)
    setCode('')
    setName('')
    setActive('true')
    setError('')
    setOpen(true)
  }

  function startEdit(route: RouteDto) {
    setEditingId(route.id)
    setCode(route.code)
    setName(route.name)
    setActive(route.active ? 'true' : 'false')
    setError('')
    setOpen(true)
  }

  function closeForm() {
    setEditingId(null)
    setCode('')
    setName('')
    setActive('true')
    setError('')
    setOpen(false)
  }

  const save = useMutation({
    mutationFn: async () => {
      const body = { code, name, active: active === 'true' }
      if (editingId) {
        await api.put<void>(`/api/routes/${editingId}`, body)
      } else {
        await api.post<RouteDto>('/api/routes', body)
      }
    },
    onSuccess: async () => {
      closeForm()
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['routes'] }),
        queryClient.invalidateQueries({ queryKey: ['route-stops'] }),
        queryClient.invalidateQueries({ queryKey: ['trips'] }),
      ])
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not save route'),
  })

  const remove = useMutation({
    mutationFn: (id: string) => api.delete<void>(`/api/routes/${id}`),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['routes'] }),
        queryClient.invalidateQueries({ queryKey: ['route-stops'] }),
      ])
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not delete route'),
  })

  return (
    <>
      <PageHeader
        title="Routes"
        subtitle="Admin-approved paths with ordered stops."
        action={
          isAdmin && (
            <Button onClick={() => (open && !editingId ? closeForm() : startCreate())}>
              <Plus className="size-4" />
              Add route
            </Button>
          )
        }
      />

      {open && isAdmin && (
        <Card className="mb-5">
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-sm font-semibold text-ink">
              {editingId ? 'Edit route' : 'New route'}
            </h2>
            <Button type="button" variant="ghost" onClick={closeForm}>
              Cancel
            </Button>
          </div>
          <form
            className="grid gap-4 sm:grid-cols-[160px_1fr_160px_auto]"
            onSubmit={(event) => {
              event.preventDefault()
              setError('')
              save.mutate()
            }}
          >
            <Field label="Code">
              <Input value={code} onChange={(e) => setCode(e.target.value)} placeholder="JS-02" required />
            </Field>
            <Field label="Name">
              <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="Campus to Kotri" required />
            </Field>
            <Field label="Status">
              <Select value={active} onChange={(e) => setActive(e.target.value)}>
                <option value="true">Active</option>
                <option value="false">Inactive</option>
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

      {routes.isLoading ? (
        <Loading />
      ) : (
        <Table>
          <thead>
            <tr>
              <Th>Code</Th>
              <Th>Name</Th>
              <Th>Stops</Th>
              <Th>Status</Th>
              {isAdmin && <Th> </Th>}
            </tr>
          </thead>
          <tbody>
            {(routes.data ?? []).map((route) => (
              <tr key={route.id} className="border-b border-line last:border-0">
                <Td className="font-medium">{route.code}</Td>
                <Td>{route.name}</Td>
                <Td>{route.stopCount}</Td>
                <Td>
                  <Badge tone={route.active ? 'success' : 'neutral'}>
                    {route.active ? 'Active' : 'Inactive'}
                  </Badge>
                </Td>
                {isAdmin && (
                  <Td>
                    <div className="flex items-center gap-1">
                      <Button
                        variant="ghost"
                        onClick={() => startEdit(route)}
                        aria-label={`Edit ${route.code}`}
                      >
                        <Pencil className="size-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        onClick={() => {
                          if (confirm(`Delete route ${route.code}? This cannot be undone.`)) {
                            remove.mutate(route.id)
                          }
                        }}
                        aria-label={`Delete ${route.code}`}
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
