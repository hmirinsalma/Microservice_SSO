/**
 * AuthContext — SSO-Ready
 *
 * ACTUEL : Login local via StubAuthService (JWT local)
 *
 * TODO SSO Migration :
 *   1. Supprimer la fonction login() locale
 *   2. Remplacer par une redirection OIDC :
 *      window.location.href = `${SSO_URL}/authorize?client_id=tims&redirect_uri=...`
 *   3. Implémenter handleCallback() pour recevoir le code OIDC
 *   4. Supprimer LoginPage.jsx (remplacée par la page SSO)
 *   5. Le logout redirigera vers SSO logout endpoint
 *
 * La logique métier (Dashboard, Interventions, Profil, etc.)
 * ne sera JAMAIS modifiée lors de cette migration.
 */

import React, { createContext, useContext, useState, useCallback } from 'react'
import { login as loginApi, logout as logoutApi } from '../api/auth'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const stored = localStorage.getItem('tims_user')
  const [user,  setUser]  = useState(stored ? JSON.parse(stored) : null)
  const [token, setToken] = useState(localStorage.getItem('tims_token') || null)

  /**
   * ⚠️ TEMPORAIRE STUB — Login local.
   * SSO Migration : Remplacer par redirect OIDC.
   */
  const login = useCallback(async (email, password) => {
    const res = await loginApi({ email, password })
    const { token: t, user: u } = res.data.data
    localStorage.setItem('tims_token', t)
    localStorage.setItem('tims_user',  JSON.stringify(u))
    setToken(t)
    setUser(u)
    return u
  }, [])

  const logout = useCallback(async () => {
    try { await logoutApi() } catch { /* ignore */ }
    localStorage.removeItem('tims_token')
    localStorage.removeItem('tims_user')
    setToken(null)
    setUser(null)
    // SSO Migration : window.location.href = `${SSO_URL}/logout?post_logout_redirect_uri=...`
  }, [])

  const isAuthenticated = !!token && !!user
  const role = user?.roles?.[0] ?? null

  const hasRole = (...roles) => roles.some(r => user?.roles?.includes(r))

  return (
    <AuthContext.Provider value={{ user, token, isAuthenticated, role, login, logout, hasRole }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)
