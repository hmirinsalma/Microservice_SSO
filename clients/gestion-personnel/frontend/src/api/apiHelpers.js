/**
 * Extrait un message d'erreur lisible depuis une réponse Axios.
 * Gère tous les formats retournés par le backend :
 *   - { message: "..." }
 *   - { message: "...", errors: [{field, message}] }
 *   - [{propertyName, errorMessage}]   (ancien format)
 *   - string brut
 */
export function extractErrorMessage(err, fallback = 'Une erreur s\'est produite.') {
  const data = err?.response?.data;
  if (!data) return err?.message || fallback;

  // Format standard { message, errors? }
  if (data.message) {
    if (data.errors?.length) {
      return data.errors.map(e => e.message || e.errorMessage).join(' | ');
    }
    return data.message;
  }

  // Tableau d'erreurs de validation (ancien format controller)
  if (Array.isArray(data) && data.length > 0) {
    return data.map(e => e.message || e.errorMessage || e.ErrorMessage).join(' | ');
  }

  // String brut
  if (typeof data === 'string') return data;

  return fallback;
}
