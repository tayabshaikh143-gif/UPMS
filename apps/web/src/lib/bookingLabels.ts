import type { BookingDto } from './types'

export const bookingStatusLabels = ['Reserved', 'Confirmed', 'Cancelled', 'Expired', 'Boarded']
export const paymentMethodLabels = ['Online', 'Cash']
export const paymentStatusLabels = ['Pending', 'Paid', 'Refunded', 'Failed']

export function bookingStatusTone(status: number) {
  if (status === 0) return 'warning' as const
  if (status === 1) return 'success' as const
  if (status === 3) return 'danger' as const
  return 'neutral' as const
}

export function paymentStatusLabel(booking: BookingDto) {
  if (booking.paymentStatus === 0 && (booking.status === 2 || booking.status === 3)) return '—'
  return paymentStatusLabels[booking.paymentStatus] ?? 'Unknown'
}
