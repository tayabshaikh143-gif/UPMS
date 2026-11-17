import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, Navigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { Button, Card, Field, Input, cx } from '../components/ui'

type AccountType = 'student' | 'vendor'

export function Register() {
  const { user, register, registerVendor } = useAuth()
  const [accountType, setAccountType] = useState<AccountType>('student')
  const [fullName, setFullName] = useState('')
  const [organizationName, setOrganizationName] = useState('')
  const [contactPerson, setContactPerson] = useState('')
  const [email, setEmail] = useState('')
  const [phone, setPhone] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  if (user) return <Navigate to="/" replace />

  const isVendor = accountType === 'vendor'

  async function onSubmit(event: FormEvent) {
    event.preventDefault()
    setBusy(true)
    setError('')
    try {
      if (isVendor) {
        await registerVendor(organizationName, contactPerson, email, password, phone)
      } else {
        await register(fullName, email, password, phone)
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Registration failed')
    } finally {
      setBusy(false)
    }
  }

  function chooseType(type: AccountType) {
    setAccountType(type)
    setError('')
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-paper px-5 py-10">
      <div className="w-full max-w-sm">
        <div className="mb-6 text-center">
          <div className="text-sm font-semibold uppercase tracking-[0.2em] text-accent">UPMS</div>
          <h1 className="mt-2 text-2xl font-semibold text-ink">
            {isVendor ? 'Apply as a transport vendor' : 'Create passenger account'}
          </h1>
          <p className="mt-1 text-sm text-muted">
            {isVendor
              ? 'Submit your organization for university approval.'
              : 'Register to book a seat on university transport.'}
          </p>
        </div>

        <Card>
          <div className="mb-5 grid grid-cols-2 gap-1 rounded-lg border border-line bg-paper p-1">
            <button
              type="button"
              onClick={() => chooseType('student')}
              className={cx(
                'rounded-md px-3 py-2 text-sm font-medium transition',
                !isVendor ? 'bg-brand text-white' : 'text-muted hover:text-ink',
              )}
            >
              Student
            </button>
            <button
              type="button"
              onClick={() => chooseType('vendor')}
              className={cx(
                'rounded-md px-3 py-2 text-sm font-medium transition',
                isVendor ? 'bg-brand text-white' : 'text-muted hover:text-ink',
              )}
            >
              Vendor
            </button>
          </div>

          <form className="space-y-4" onSubmit={onSubmit}>
            {isVendor ? (
              <>
                <Field label="Organization name">
                  <Input
                    value={organizationName}
                    onChange={(e) => setOrganizationName(e.target.value)}
                    placeholder="Indus Transport Services"
                    autoComplete="organization"
                    required
                  />
                </Field>
                <Field label="Contact person">
                  <Input
                    value={contactPerson}
                    onChange={(e) => setContactPerson(e.target.value)}
                    placeholder="Your name"
                    autoComplete="name"
                    required
                  />
                </Field>
                <Field label="Contact phone">
                  <Input
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    placeholder="0300 0000000"
                    autoComplete="tel"
                    required
                  />
                </Field>
              </>
            ) : (
              <>
                <Field label="Full name">
                  <Input
                    value={fullName}
                    onChange={(e) => setFullName(e.target.value)}
                    placeholder="Your name"
                    autoComplete="name"
                    required
                  />
                </Field>
                <Field label="Phone (optional)">
                  <Input
                    value={phone}
                    onChange={(e) => setPhone(e.target.value)}
                    placeholder="0300 0000000"
                    autoComplete="tel"
                  />
                </Field>
              </>
            )}

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
                placeholder="At least 8 characters"
                autoComplete="new-password"
                minLength={8}
                required
              />
            </Field>

            {isVendor && (
              <p className="rounded-lg bg-brand-soft px-3 py-2 text-xs text-brand">
                Vendor accounts need administrator approval. You can sign in after applying, but
                buses, trips and settlements unlock only after approval.
              </p>
            )}

            {error && <p className="text-sm text-danger">{error}</p>}
            <Button type="submit" className="w-full" disabled={busy}>
              {busy
                ? isVendor
                  ? 'Submitting application…'
                  : 'Creating account…'
                : isVendor
                  ? 'Submit vendor application'
                  : 'Create account'}
            </Button>
          </form>
        </Card>

        <p className="mt-4 text-center text-sm text-muted">
          Already registered?{' '}
          <Link to="/login" className="font-medium text-accent hover:underline">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  )
}
