export function formatearFecha(valor: string): string {
  const [anio, mes, dia] = valor.slice(0, 10).split('-')
  return `${dia}/${mes}/${anio}`
}

export function formatearFechaHora(valor: string): string {
  return new Date(valor).toLocaleString('es', {
    dateStyle: 'short',
    timeStyle: 'short',
  })
}

export function vacioANull(valor: string): string | null {
  const limpio = valor.trim()
  return limpio === '' ? null : limpio
}
