import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import type { ReactNode } from 'react'
import { api, getStoredUser, getToken, setStoredUser, setToken } from '../lib/api'
import type { AuthResponse, AuthUser } from '../lib/types'

type AuthState = {
  user: AuthUser | null
  loading: boolean
  login: (email: string, password: string) => Promise<void>
  register: (fullName: string, email: string, password: string, phone?: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthState | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => getStoredUser<AuthUser>())
  const [loading, setLoading] = useState(() => Boolean(getToken()))

  useEffect(() => {
    if (!getToken()) {
      setLoading(false)
      return
    }
    api
      .get<AuthUser>('/api/auth/me')
      .then((me) => {
        setUser(me)
        setStoredUser(me)
      })
      .catch(() => {
        setToken(null)
        setStoredUser(null)
        setUser(null)
      })
      .finally(() => setLoading(false))
  }, [])

  const value = useMemo<AuthState>(
    () => ({
      user,
      loading,
      async login(email: string, password: string) {
        const result = await api.post<AuthResponse>('/api/auth/login', { email, password })
        const nextUser: AuthUser = {
          id: result.userId,
          email: result.email,
          fullName: result.fullName,
          roles: result.roles,
        }
        setToken(result.token)
        setStoredUser(nextUser)
        setUser(nextUser)
      },
      async register(fullName: string, email: string, password: string, phone?: string) {
        const result = await api.post<AuthResponse>('/api/auth/register', {
          fullName,
          email,
          password,
          phone,
        })
        const nextUser: AuthUser = {
          id: result.userId,
          email: result.email,
          fullName: result.fullName,
          roles: result.roles,
        }
        setToken(result.token)
        setStoredUser(nextUser)
        setUser(nextUser)
      },
      logout() {
        setToken(null)
        setStoredUser(null)
        setUser(null)
      },
    }),
    [user, loading],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth must be used inside AuthProvider')
  return context
}
