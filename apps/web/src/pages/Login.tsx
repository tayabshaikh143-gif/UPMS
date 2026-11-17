import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, Navigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { Button, Card, Field, Input } from '../components/ui'

export function Login() {
  const { user, login } = useAuth()
  const [email, setEmail] = useState('admin@upms.local')
  const [password, setPassword] = useState('Admin@12345')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  if (user) return <Navigate to="/" replace />

  async function onSubmit(event: FormEvent) {
    event.preventDefault()
    setBusy(true)
    setError('')
    try {
      await login(email, password)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed')
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-paper px-5 py-10">
      <div className="w-full max-w-sm">
        <div className="mb-6 text-center">
          <div className="text-sm font-semibold uppercase tracking-[0.2em] text-accent">UPMS</div>
          <h1 className="mt-2 text-2xl font-semibold text-ink">University Transport</h1>
          <p className="mt-1 text-sm text-muted">Sign in to book trips or manage the network.</p>
        </div>

        <Card>
          <form className="space-y-4" onSubmit={onSubmit}>
            <Field label="Email">
              <Input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                autoComplete="email"
                required
              />
            </Field>
            <Field label="Password">
              <Input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                autoComplete="current-password"
                required
              />
            </Field>
            {error && <p className="text-sm text-danger">{error}</p>}
            <Button type="submit" className="w-full" disabled={busy}>
              {busy ? 'Signing in…' : 'Sign in'}
            </Button>
          </form>
        </Card>

        <div className="mt-4 space-y-1 text-center text-xs text-muted">
          <p>
            Demo admin: <span className="font-medium text-ink">admin@upms.local</span> / Admin@12345
          </p>
          <p>
            Demo passenger: <span className="font-medium text-ink">student@upms.local</span> / Student@123
          </p>
          <p>
            Demo vendor: <span className="font-medium text-ink">vendor@upms.local</span> / Vendor@123
          </p>
          <p>
            Demo staff: <span className="font-medium text-ink">staff@upms.local</span> / Staff@123
          </p>
          <p className="pt-2">
            New here?{' '}
            <Link to="/register" className="font-medium text-accent hover:underline">
              Create an account
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}
