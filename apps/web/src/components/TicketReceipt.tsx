import { ArrowRight, Printer, X } from 'lucide-react'
import { useEffect } from 'react'
import { createPortal } from 'react-dom'
import {
  bookingStatusLabels,
  paymentMethodLabels,
  paymentStatusLabel,
} from '../lib/bookingLabels'
import { formatDateTime } from '../lib/format'
import type { BookingDto } from '../lib/types'
import { Button } from './ui'

export function TicketReceipt({
  booking,
  onClose,
}: {
  booking: BookingDto
  onClose: () => void
}) {
  useEffect(() => {
    const previousOverflow = document.body.style.overflow
    document.body.style.overflow = 'hidden'

    function onKeyDown(event: KeyboardEvent) {
      if (event.key === 'Escape') onClose()
    }

    window.addEventListener('keydown', onKeyDown)
    return () => {
      document.body.style.overflow = previousOverflow
      window.removeEventListener('keydown', onKeyDown)
    }
  }, [onClose])

  const status = bookingStatusLabels[booking.status] ?? 'Unknown'

  return createPortal(
    <div
      id="ticket-receipt-overlay"
      className="fixed inset-0 z-50 overflow-y-auto bg-ink/60 p-4 backdrop-blur-sm sm:p-6 print:static print:overflow-visible print:bg-white print:p-0"
      role="dialog"
      aria-modal="true"
      aria-labelledby="ticket-receipt-title"
      onClick={(event) => {
        if (event.target === event.currentTarget) onClose()
      }}
    >
      <div
        id="ticket-receipt"
        className="mx-auto w-full max-w-xl overflow-hidden rounded-2xl bg-surface shadow-2xl print:max-w-none print:rounded-none print:shadow-none"
      >
        <div className="relative bg-brand px-6 py-5 text-white">
          <div className="flex items-start justify-between gap-3">
            <div>
              <div className="text-[10px] font-semibold uppercase tracking-[0.22em] text-white/60">
                UPMS · E-Ticket
              </div>
              <h2 id="ticket-receipt-title" className="mt-1 text-xl font-semibold">
                Ticket receipt
              </h2>
            </div>
            <button
              type="button"
              onClick={onClose}
              className="rounded-lg p-1 text-white/70 transition hover:bg-white/10 hover:text-white print:hidden"
              aria-label="Close ticket"
            >
              <X className="size-5" />
            </button>
          </div>

          <div className="mt-5 flex flex-wrap items-end justify-between gap-3">
            <div>
              <div className="text-[10px] uppercase tracking-wide text-white/60">Booking reference</div>
              <div className="font-mono text-2xl font-semibold tracking-[0.08em]">
                {booking.reference}
              </div>
            </div>
            <span className="rounded-full bg-white/15 px-3 py-1 text-xs font-medium text-white ring-1 ring-white/20">
              {status}
            </span>
          </div>
        </div>

        <div className="px-6 py-5">
          <div className="flex flex-wrap items-start justify-between gap-4">
            <div>
              <div className="text-[10px] uppercase tracking-wide text-muted">Passenger</div>
              <div className="mt-1 font-medium text-ink">{booking.passengerName}</div>
              <div className="text-xs text-muted">{booking.passengerEmail}</div>
            </div>
            <div className="text-right">
              <div className="text-[10px] uppercase tracking-wide text-muted">Booked</div>
              <div className="mt-1 text-sm text-ink">{formatDateTime(booking.createdAt)}</div>
            </div>
          </div>

          <div className="my-5 border-t border-dashed border-line" />

          <div className="flex flex-wrap items-center gap-2">
            <span className="rounded-md bg-brand-soft px-2 py-0.5 text-xs font-semibold text-brand">
              {booking.routeCode}
            </span>
            <span className="text-sm font-medium text-ink">{booking.routeName}</span>
          </div>
          <div className="mt-2 flex flex-wrap items-center gap-2 text-sm text-ink">
            <span>{booking.origin || 'Origin not set'}</span>
            <ArrowRight className="size-3.5 shrink-0 text-muted" />
            <span>{booking.destination || 'Destination not set'}</span>
          </div>

          <div className="mt-5 grid grid-cols-2 gap-4">
            <div>
              <div className="text-[10px] uppercase tracking-wide text-muted">Departure</div>
              <div className="mt-1 text-sm font-medium text-ink">
                {formatDateTime(booking.departureAt)}
              </div>
            </div>
            <div>
              <div className="text-[10px] uppercase tracking-wide text-muted">Bus</div>
              <div className="mt-1 text-sm font-medium text-ink">
                {booking.busRegistration ?? 'Not assigned'}
              </div>
            </div>
            <div>
              <div className="text-[10px] uppercase tracking-wide text-muted">Payment method</div>
              <div className="mt-1 text-sm font-medium text-ink">
                {paymentMethodLabels[booking.method] ?? 'Unknown'}
              </div>
            </div>
            <div>
              <div className="text-[10px] uppercase tracking-wide text-muted">Payment status</div>
              <div className="mt-1 text-sm font-medium text-ink">{paymentStatusLabel(booking)}</div>
            </div>
          </div>

          <div className="my-5 border-t border-dashed border-line" />

          <div className="flex items-center justify-between gap-3">
            <div>
              <div className="text-[10px] uppercase tracking-wide text-muted">Total fare</div>
              <div className="text-xs text-muted">One passenger · one journey</div>
            </div>
            <div className="text-xl font-semibold text-ink">Rs {booking.amount.toFixed(2)}</div>
          </div>

          <div className="mt-5 rounded-lg bg-paper px-4 py-3 text-center text-xs leading-5 text-muted">
            Present this booking reference at boarding. This ticket is valid for one passenger on
            the trip shown above.
          </div>

          {(booking.status === 2 || booking.status === 3) && (
            <div className="mt-3 text-center text-xs font-medium text-danger">
              This booking is {status.toLowerCase()} and is not valid for boarding.
            </div>
          )}

          <div className="mt-5 flex items-center justify-between border-t border-line pt-3">
            <div className="text-[10px] uppercase tracking-wide text-muted">UPMS · University Transport</div>
            <div className="flex items-center gap-2">
              <span className="text-[10px] uppercase tracking-wide text-muted">Status</span>
              <span className="text-xs font-medium text-ink">{status}</span>
            </div>
          </div>
        </div>

        <div className="flex justify-end gap-2 border-t border-line px-6 py-4 print:hidden">
          <Button type="button" variant="secondary" onClick={() => window.print()}>
            <Printer className="size-4" />
            Print receipt
          </Button>
          <Button type="button" onClick={onClose}>
            Close
          </Button>
        </div>
      </div>
    </div>,
    document.body,
  )
}
