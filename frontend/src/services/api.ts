const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5032'

async function leerError(res: Response): Promise<string> {
  const texto = await res.text()
  try {
    const json = JSON.parse(texto)
    if (typeof json === 'string') return json
    if (json && json.errors) {
      return (Object.values(json.errors) as string[][]).flat().join(' ')
    }
    if (json && json.title) return json.title
  } catch {
    // la respuesta era texto plano
  }
  return texto || `Error ${res.status}`
}

export async function request<T>(ruta: string, opciones?: RequestInit): Promise<T> {
  let res: Response
  try {
    res = await fetch(`${API_URL}/api/${ruta}`, {
      headers: { 'Content-Type': 'application/json' },
      ...opciones,
    })
  } catch {
    throw new Error('No se pudo conectar con la API. Verifica que esté encendida.')
  }

  if (!res.ok) {
    throw new Error(await leerError(res))
  }
  if (res.status === 204) {
    return undefined as T
  }
  return (await res.json()) as T
}

export function crearServicio<T, TInput>(recurso: string) {
  return {
    listar: () => request<T[]>(recurso),
    obtener: (id: number) => request<T>(`${recurso}/${id}`),
    crear: (datos: TInput) =>
      request<T>(recurso, { method: 'POST', body: JSON.stringify(datos) }),
    actualizar: (id: number, datos: TInput) =>
      request<T>(`${recurso}/${id}`, { method: 'PUT', body: JSON.stringify(datos) }),
    eliminar: (id: number) => request<void>(`${recurso}/${id}`, { method: 'DELETE' }),
  }
}
