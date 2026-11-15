interface EstadoProps {
  cargando: boolean
  error: string
  vacio: boolean
  mensajeVacio: string
  onReintentar: () => void
}

// Muestra el estado de carga, el error o el mensaje de lista vacía.
// Devuelve null cuando hay datos que mostrar.
export default function Estado({ cargando, error, vacio, mensajeVacio, onReintentar }: EstadoProps) {
  if (cargando) {
    return <p className="aviso">Cargando...</p>
  }
  if (error) {
    return (
      <div className="aviso error">
        <p>{error}</p>
        <button type="button" className="boton secundario" onClick={onReintentar}>
          Reintentar
        </button>
      </div>
    )
  }
  if (vacio) {
    return <p className="aviso">{mensajeVacio}</p>
  }
  return null
}
