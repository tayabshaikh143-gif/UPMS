import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Pencil, Plus } from 'lucide-react'
import { useState } from 'react'
import { useAuth } from '../auth/AuthContext'
import { api } from '../lib/api'
import type { VendorDto } from '../lib/types'
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

const statuses = ['Pending', 'Approved', 'Rejected']

export function VendorsPage() {
  const { user } = useAuth()
  const isAdmin = user?.roles.includes('Admin') ?? false
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [name, setName] = useState('')
  const [status, setStatus] = useState('0')
  const [error, setError] = useState('')

  const vendors = useQuery({ queryKey: ['vendors'], queryFn: () => api.get<VendorDto[]>('/api/vendors') })

  function startCreate() {
    setEditingId(null)
    setName('')
    setStatus('0')
    setError('')
    setOpen(true)
  }

  function startEdit(vendor: VendorDto) {
    setEditingId(vendor.id)
    setName(vendor.name)
    setStatus(String(vendor.status))
    setError('')
    setOpen(true)
  }

  function closeForm() {
    setEditingId(null)
    setName('')
    setStatus('0')
    setError('')
    setOpen(false)
  }

  const save = useMutation({
    mutationFn: async () => {
      const body = { name, status: Number(status) }
      if (editingId) {
        await api.put<void>(`/api/vendors/${editingId}`, body)
      } else {
        await api.post<VendorDto>('/api/vendors', body)
      }
    },
    onSuccess: async () => {
      closeForm()
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['vendors'] }),
        queryClient.invalidateQueries({ queryKey: ['buses'] }),
      ])
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not save vendor'),
  })

  return (
    <>
      <PageHeader
        title="Vendors"
        subtitle="Private operators approved to run services on university routes."
        action={
          isAdmin && (
            <Button onClick={() => (open && !editingId ? closeForm() : startCreate())}>
              <Plus className="size-4" />
              Add vendor
            </Button>
          )
        }
      />

      {open && isAdmin && (
        <Card className="mb-5">
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-sm font-semibold text-ink">
              {editingId ? 'Edit vendor' : 'New vendor'}
            </h2>
            <Button type="button" variant="ghost" onClick={closeForm}>
              Cancel
            </Button>
          </div>
          <form
            className="grid gap-4 sm:grid-cols-[1fr_200px_auto]"
            onSubmit={(event) => {
              event.preventDefault()
              setError('')
              save.mutate()
            }}
          >
            <Field label="Name">
              <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="Indus Transport" required />
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

      {vendors.isLoading ? (
        <Loading />
      ) : (
        <Table>
          <thead>
            <tr>
              <Th>Name</Th>
              <Th>Status</Th>
              <Th>Buses</Th>
              {isAdmin && <Th> </Th>}
            </tr>
          </thead>
          <tbody>
            {(vendors.data ?? []).map((vendor) => (
              <tr key={vendor.id} className="border-b border-line last:border-0">
                <Td className="font-medium">{vendor.name}</Td>
                <Td>
                  <Badge tone={vendor.status === 1 ? 'success' : vendor.status === 2 ? 'danger' : 'warning'}>
                    {statuses[vendor.status] ?? 'Unknown'}
                  </Badge>
                </Td>
                <Td>{vendor.busCount}</Td>
                {isAdmin && (
                  <Td>
                    <Button
                      variant="ghost"
                      onClick={() => startEdit(vendor)}
                      aria-label={`Edit ${vendor.name}`}
                    >
                      <Pencil className="size-4" />
                    </Button>
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
