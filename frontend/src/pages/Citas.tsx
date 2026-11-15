import { useState, type FormEvent } from 'react'
import type { Cita } from '../types'
import { citaService } from '../services/citaService'
import { medicoService } from '../services/medicoService'
import { pacienteService } from '../services/pacienteService'
import { mensajeDeError, useLista } from '../hooks/useLista'
import { formatearFechaHora } from '../utils'
import Estado from '../components/Estado'
import Modal from '../components/Modal'

interface FormCita {
  pacienteId: string
  medicoId: string
  fechaHora: string
  motivo: string
  estado: string
}

const formVacio: FormCita = {
  pacienteId: '',
  medicoId: '',
  fechaHora: '',
  motivo: '',
  estado: 'Programada',
}

export default function Citas() {
  const citas = useLista(citaService.listar)
  const pacientes = useLista(pacienteService.listar)
  const medicos = useLista(medicoService.listar)
  const [filtroEstado, setFiltroEstado] = useState('')
  const [editando, setEditando] = useState<Cita | null>(null)
  const [abierto, setAbierto] = useState(false)
  const [form, setForm] = useState<FormCita>(formVacio)
  const [errorForm, setErrorForm] = useState('')
  const [errorAccion, setErrorAccion] = useState('')
  const [guardando, setGuardando] = useState(false)

  const cargando = citas.cargando || pacientes.cargando || medicos.cargando
  const error = citas.error || pacientes.error || medicos.error
  const recargar = () => {
    citas.recargar()
    pacientes.recargar()
    medicos.recargar()
  }

  const nombrePaciente = (id: number) => {
    const p = pacientes.datos.find((x) => x.id === id)
    return p ? `${p.nombre} ${p.apellido}` : `Paciente #${id}`
  }
  const nombreMedico = (id: number) => {
    const m = medicos.datos.find((x) => x.id === id)
    return m ? `Dr. ${m.nombre} ${m.apellido}` : `Médico #${id}`
  }

  const filtradas = citas.datos
    .filter((c) => filtroEstado === '' || c.estado === filtroEstado)
    .sort((a, b) => a.fechaHora.localeCompare(b.fechaHora))

  const cambiar = (campo: keyof FormCita, valor: string) => {
    setForm({ ...form, [campo]: valor })
  }

  const abrirNueva = () => {
    setEditando(null)
    setForm({
      ...formVacio,
      pacienteId: String(pacientes.datos[0]?.id ?? ''),
      medicoId: String(medicos.datos[0]?.id ?? ''),
    })
    setErrorForm('')
    setAbierto(true)
  }

  const abrirEdicion = (c: Cita) => {
    setEditando(c)
    setForm({
      pacienteId: String(c.pacienteId),
      medicoId: String(c.medicoId),
      fechaHora: c.fechaHora.slice(0, 16),
      motivo: c.motivo,
      estado: c.estado,
    })
    setErrorForm('')
    setAbierto(true)
  }

  const guardar = async (e: FormEvent) => {
    e.preventDefault()
    setGuardando(true)
    setErrorForm('')
    const datos = {
      pacienteId: Number(form.pacienteId),
      medicoId: Number(form.medicoId),
      fechaHora: form.fechaHora,
      motivo: form.motivo.trim(),
      estado: form.estado,
    }
    try {
      if (editando) {
        await citaService.actualizar(editando.id, datos)
      } else {
        await citaService.crear(datos)
      }
      setAbierto(false)
      await citas.recargar()
    } catch (err) {
      setErrorForm(mensajeDeError(err))
    } finally {
      setGuardando(false)
    }
  }

  const eliminar = async (c: Cita) => {
    if (!window.confirm('¿Eliminar esta cita?')) return
    setErrorAccion('')
    try {
      await citaService.eliminar(c.id)
      await citas.recargar()
    } catch (err) {
      setErrorAccion(mensajeDeError(err))
    }
  }

  return (
    <section>
      <header className="encabezado">
        <h2>Citas</h2>
        <button type="button" className="boton" onClick={abrirNueva}>
          Nueva cita
        </button>
      </header>

      <select className="buscador" value={filtroEstado} onChange={(e) => setFiltroEstado(e.target.value)}>
        <option value="">Todos los estados</option>
        <option>Programada</option>
        <option>Completada</option>
        <option>Cancelada</option>
      </select>

      {errorAccion && <p className="aviso error">{errorAccion}</p>}

      <Estado
        cargando={cargando}
        error={error}
        vacio={filtradas.length === 0}
        mensajeVacio={citas.datos.length === 0 ? 'Todavía no hay citas registradas.' : 'No hay citas con ese estado.'}
        onReintentar={recargar}
      />

      {!cargando && !error && filtradas.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Fecha y hora</th>
              <th>Paciente</th>
              <th>Médico</th>
              <th>Motivo</th>
              <th>Estado</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {filtradas.map((c) => (
              <tr key={c.id}>
                <td>{formatearFechaHora(c.fechaHora)}</td>
                <td>{nombrePaciente(c.pacienteId)}</td>
                <td>{nombreMedico(c.medicoId)}</td>
                <td>{c.motivo}</td>
                <td>
                  <span className={`estado ${c.estado.toLowerCase()}`}>{c.estado}</span>
                </td>
                <td className="acciones">
                  <button type="button" className="boton secundario" onClick={() => abrirEdicion(c)}>
                    Editar
                  </button>
                  <button type="button" className="boton peligro" onClick={() => eliminar(c)}>
                    Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {abierto && (
        <Modal titulo={editando ? 'Editar cita' : 'Nueva cita'} onCerrar={() => setAbierto(false)}>
          <form onSubmit={guardar}>
            <label>
              Paciente
              <select required value={form.pacienteId} onChange={(e) => cambiar('pacienteId', e.target.value)}>
                {pacientes.datos.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.nombre} {p.apellido}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Médico
              <select required value={form.medicoId} onChange={(e) => cambiar('medicoId', e.target.value)}>
                {medicos.datos.map((m) => (
                  <option key={m.id} value={m.id}>
                    Dr. {m.nombre} {m.apellido}
                  </option>
                ))}
              </select>
            </label>
            <div className="fila">
              <label>
                Fecha y hora
                <input
                  required
                  type="datetime-local"
                  value={form.fechaHora}
                  onChange={(e) => cambiar('fechaHora', e.target.value)}
                />
              </label>
              <label>
                Estado
                <select value={form.estado} onChange={(e) => cambiar('estado', e.target.value)}>
                  <option>Programada</option>
                  <option>Completada</option>
                  <option>Cancelada</option>
                </select>
              </label>
            </div>
            <label>
              Motivo
              <input required value={form.motivo} onChange={(e) => cambiar('motivo', e.target.value)} />
            </label>

            {errorForm && <p className="aviso error">{errorForm}</p>}

            <div className="pie-modal">
              <button type="button" className="boton secundario" onClick={() => setAbierto(false)}>
                Cancelar
              </button>
              <button type="submit" className="boton" disabled={guardando}>
                {guardando ? 'Guardando...' : 'Guardar'}
              </button>
            </div>
          </form>
        </Modal>
      )}
    </section>
  )
}
