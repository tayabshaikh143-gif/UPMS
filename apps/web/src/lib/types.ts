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
}

export type BusDto = {
  id: string
  registration: string
  capacity: number
  ownership: number
  vendorId: string | null
  vendorName: string | null
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
}
