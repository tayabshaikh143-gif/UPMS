export type Role = 'Admin' | 'Passenger' | 'Staff' | 'Vendor'

export type AuthUser = {
  id: string
  email: string
  fullName: string
  roles: Role[]
}

export type AuthResponse = {
  token: string
  userId: string
  email: string
  fullName: string
  roles: Role[]
}

export type RouteDto = {
  id: string
  code: string
  name: string
  active: boolean
  stopCount: number
}

export type RouteStopDto = {
  id: string
  stopId: string
  stopName: string
  sequence: number
  offsetMinutes: number
}

export type RouteDetailDto = {
  id: string
  code: string
  name: string
  active: boolean
  stops: RouteStopDto[]
}

export type StopDto = {
  id: string
  name: string
  latitude: number
  longitude: number
}

export type VendorDto = {
  id: string
  name: string
  status: number
  busCount: number
  userId: string | null
  contact: string | null
  monthlySubscription: number
  subscriptionStatus: number
}

export type VendorProfileDto = {
  id: string
  name: string
  status: number
  contact: string | null
  monthlySubscription: number
  subscriptionStatus: number
  busCount: number
  contactPerson: string
  email: string
}

export type VendorTripDto = {
  id: string
  routeCode: string
  routeName: string
  origin: string
  destination: string
  departureAt: string
  fare: number
  status: number
  busRegistration: string | null
  capacity: number
  bookedCount: number
  remainingSeats: number
}

export type VendorSettlementDto = {
  id: string
  vendorId: string
  period: string
  grossFares: number
  subscriptionDue: number
  status: number
  paidAt: string | null
  createdAt: string
}

export type BusDto = {
  id: string
  registration: string
  capacity: number
  ownership: number
  vendorId: string | null
  vendorName: string | null
  active: boolean
}

export type TripDto = {
  id: string
  routeId: string
  routeCode: string
  routeName: string
  origin: string
  destination: string
  departureAt: string
  fare: number
  status: number
  busId: string | null
  busRegistration: string | null
  capacity: number
  bookedCount: number
  remainingSeats: number
  stops: RouteStopDto[]
  cancelledAt: string | null
  cancellationReason: string | null
}

export type BookingDto = {
  id: string
  reference: string
  tripId: string
  routeCode: string
  routeName: string
  origin: string
  destination: string
  departureAt: string
  busRegistration: string | null
  amount: number
  status: number
  method: number
  paymentStatus: number
  createdAt: string
  cancelledAt: string | null
  passengerName: string
  passengerEmail: string
}
