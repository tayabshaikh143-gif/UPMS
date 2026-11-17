import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Pencil, Plus } from 'lucide-react'
import { useState } from 'react'
import { useAuth } from '../auth/AuthContext'
import { api } from '../lib/api'
import type { BusDto, VendorDto } from '../lib/types'
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

export function BusesPage() {
  const { user } = useAuth()
  const isAdmin = user?.roles.includes('Admin') ?? false
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [registration, setRegistration] = useState('')
  const [capacity, setCapacity] = useState('45')
  const [ownership, setOwnership] = useState('0')
  const [vendorId, setVendorId] = useState('')
  const [active, setActive] = useState('true')
  const [error, setError] = useState('')

  const buses = useQuery({ queryKey: ['buses'], queryFn: () => api.get<BusDto[]>('/api/buses') })
  const vendors = useQuery({ queryKey: ['vendors'], queryFn: () => api.get<VendorDto[]>('/api/vendors') })

  function startCreate() {
    setEditingId(null)
    setRegistration('')
    setCapacity('45')
    setOwnership('0')
    setVendorId('')
    setActive('true')
    setError('')
    setOpen(true)
  }

  function startEdit(bus: BusDto) {
    setEditingId(bus.id)
    setRegistration(bus.registration)
    setCapacity(String(bus.capacity))
    setOwnership(String(bus.ownership))
    setVendorId(bus.vendorId ?? '')
    setActive(bus.active ? 'true' : 'false')
    setError('')
    setOpen(true)
  }

  function closeForm() {
    setEditingId(null)
    setRegistration('')
    setCapacity('45')
    setOwnership('0')
    setVendorId('')
    setActive('true')
    setError('')
    setOpen(false)
  }

  const save = useMutation({
    mutationFn: async () => {
      const body = {
        registration,
        capacity: Number(capacity),
        ownership: Number(ownership),
        vendorId: Number(ownership) === 1 && vendorId ? vendorId : null,
        active: active === 'true',
      }
      if (editingId) {
        await api.put<void>(`/api/buses/${editingId}`, body)
      } else {
        await api.post<BusDto>('/api/buses', body)
      }
    },
    onSuccess: async () => {
      closeForm()
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['buses'] }),
        queryClient.invalidateQueries({ queryKey: ['vendors'] }),
      ])
    },
    onError: (err) => setError(err instanceof Error ? err.message : 'Could not save bus'),
  })

  return (
    <>
      <PageHeader
        title="Buses"
        subtitle="University-owned and vendor-owned vehicles."
        action={
          isAdmin && (
            <Button onClick={() => (open && !editingId ? closeForm() : startCreate())}>
              <Plus className="size-4" />
              Add bus
            </Button>
          )
        }
      />

      {open && isAdmin && (
        <Card className="mb-5">
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-sm font-semibold text-ink">
              {editingId ? 'Edit bus' : 'New bus'}
            </h2>
            <Button type="button" variant="ghost" onClick={closeForm}>
              Cancel
            </Button>
          </div>
          <form
            className="grid gap-4 sm:grid-cols-2 lg:grid-cols-[1fr_100px_150px_1fr_130px_auto]"
            onSubmit={(event) => {
              event.preventDefault()
              setError('')
              save.mutate()
            }}
          >
            <Field label="Registration">
              <Input
                value={registration}
                onChange={(e) => setRegistration(e.target.value)}
                placeholder="SND-3003"
                required
              />
            </Field>
            <Field label="Capacity">
              <Input
                type="number"
                min={1}
                value={capacity}
                onChange={(e) => setCapacity(e.target.value)}
                required
              />
            </Field>
            <Field label="Ownership">
              <Select value={ownership} onChange={(e) => setOwnership(e.target.value)}>
                <option value="0">University</option>
                <option value="1">Vendor</option>
              </Select>
            </Field>
            <Field label="Vendor">
              <Select value={vendorId} onChange={(e) => setVendorId(e.target.value)} disabled={ownership === '0'}>
                <option value="">Select a vendor</option>
                {(vendors.data ?? []).map((vendor) => (
                  <option key={vendor.id} value={vendor.id}>
                    {vendor.name}
                  </option>
                ))}
              </Select>
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

      {buses.isLoading ? (
        <Loading />
      ) : (
        <Table>
          <thead>
            <tr>
              <Th>Registration</Th>
              <Th>Capacity</Th>
              <Th>Ownership</Th>
              <Th>Vendor</Th>
              <Th>Status</Th>
              {isAdmin && <Th> </Th>}
            </tr>
          </thead>
          <tbody>
            {(buses.data ?? []).map((bus) => (
              <tr key={bus.id} className="border-b border-line last:border-0">
                <Td className="font-medium">{bus.registration}</Td>
                <Td>{bus.capacity}</Td>
                <Td>
                  <Badge tone={bus.ownership === 0 ? 'neutral' : 'success'}>
                    {bus.ownership === 0 ? 'University' : 'Vendor'}
                  </Badge>
                </Td>
                <Td>{bus.vendorName ?? '—'}</Td>
                <Td>
                  <Badge tone={bus.active ? 'success' : 'neutral'}>
                    {bus.active ? 'Active' : 'Inactive'}
                  </Badge>
                </Td>
                {isAdmin && (
                  <Td>
                    <Button
                      variant="ghost"
                      onClick={() => startEdit(bus)}
                      aria-label={`Edit ${bus.registration}`}
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
