import { citaService } from '../services/citaService'
import { medicoService } from '../services/medicoService'
import { pacienteService } from '../services/pacienteService'
import { useLista } from '../hooks/useLista'
import { formatearFechaHora } from '../utils'
import Estado from '../components/Estado'

export default function Panel() {
  const pacientes = useLista(pacienteService.listar)
  const medicos = useLista(medicoService.listar)
  const citas = useLista(citaService.listar)

  const cargando = pacientes.cargando || medicos.cargando || citas.cargando
  const error = pacientes.error || medicos.error || citas.error

  const recargar = () => {
    pacientes.recargar()
    medicos.recargar()
    citas.recargar()
  }

  const programadas = citas.datos
    .filter((c) => c.estado === 'Programada')
    .sort((a, b) => a.fechaHora.localeCompare(b.fechaHora))

  const nombrePaciente = (id: number) => {
    const p = pacientes.datos.find((x) => x.id === id)
    return p ? `${p.nombre} ${p.apellido}` : `Paciente #${id}`
  }
  const nombreMedico = (id: number) => {
    const m = medicos.datos.find((x) => x.id === id)
    return m ? `Dr. ${m.nombre} ${m.apellido}` : `Médico #${id}`
  }

  return (
    <section>
      <header className="encabezado">
        <h2>Panel</h2>
      </header>

      <Estado
        cargando={cargando}
        error={error}
        vacio={false}
        mensajeVacio=""
        onReintentar={recargar}
      />

      {!cargando && !error && (
        <>
          <div className="tarjetas">
            <div className="tarjeta">
              <span className="numero">{pacientes.datos.length}</span>
              <span>Pacientes</span>
            </div>
            <div className="tarjeta">
              <span className="numero">{medicos.datos.length}</span>
              <span>Médicos</span>
            </div>
            <div className="tarjeta">
              <span className="numero">{citas.datos.length}</span>
              <span>Citas</span>
            </div>
            <div className="tarjeta">
              <span className="numero">{programadas.length}</span>
              <span>Citas programadas</span>
            </div>
          </div>

          <h3>Próximas citas</h3>
          {programadas.length === 0 ? (
            <p className="aviso">No hay citas programadas.</p>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Fecha y hora</th>
                  <th>Paciente</th>
                  <th>Médico</th>
                  <th>Motivo</th>
                </tr>
              </thead>
              <tbody>
                {programadas.slice(0, 5).map((c) => (
                  <tr key={c.id}>
                    <td>{formatearFechaHora(c.fechaHora)}</td>
                    <td>{nombrePaciente(c.pacienteId)}</td>
                    <td>{nombreMedico(c.medicoId)}</td>
                    <td>{c.motivo}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </>
      )}
    </section>
  )
}
