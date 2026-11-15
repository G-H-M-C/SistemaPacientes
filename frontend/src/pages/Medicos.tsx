import { useState, type FormEvent } from 'react'
import type { Medico } from '../types'
import { medicoService } from '../services/medicoService'
import { especialidadService } from '../services/especialidadService'
import { mensajeDeError, useLista } from '../hooks/useLista'
import { vacioANull } from '../utils'
import Estado from '../components/Estado'
import Modal from '../components/Modal'

interface FormMedico {
  nombre: string
  apellido: string
  licencia: string
  telefono: string
  email: string
  especialidadId: string
}

const formVacio: FormMedico = {
  nombre: '',
  apellido: '',
  licencia: '',
  telefono: '',
  email: '',
  especialidadId: '',
}

export default function Medicos() {
  const medicos = useLista(medicoService.listar)
  const especialidades = useLista(especialidadService.listar)
  const [busqueda, setBusqueda] = useState('')
  const [editando, setEditando] = useState<Medico | null>(null)
  const [abierto, setAbierto] = useState(false)
  const [form, setForm] = useState<FormMedico>(formVacio)
  const [errorForm, setErrorForm] = useState('')
  const [errorAccion, setErrorAccion] = useState('')
  const [guardando, setGuardando] = useState(false)

  const cargando = medicos.cargando || especialidades.cargando
  const error = medicos.error || especialidades.error
  const recargar = () => {
    medicos.recargar()
    especialidades.recargar()
  }

  const nombreEspecialidad = (id: number) =>
    especialidades.datos.find((e) => e.id === id)?.nombre ?? '-'

  const filtrados = medicos.datos.filter((m) => {
    const texto = `${m.nombre} ${m.apellido} ${m.licencia}`.toLowerCase()
    return texto.includes(busqueda.toLowerCase())
  })

  const cambiar = (campo: keyof FormMedico, valor: string) => {
    setForm({ ...form, [campo]: valor })
  }

  const abrirNuevo = () => {
    setEditando(null)
    setForm({ ...formVacio, especialidadId: String(especialidades.datos[0]?.id ?? '') })
    setErrorForm('')
    setAbierto(true)
  }

  const abrirEdicion = (m: Medico) => {
    setEditando(m)
    setForm({
      nombre: m.nombre,
      apellido: m.apellido,
      licencia: m.licencia,
      telefono: m.telefono ?? '',
      email: m.email ?? '',
      especialidadId: String(m.especialidadId),
    })
    setErrorForm('')
    setAbierto(true)
  }

  const guardar = async (e: FormEvent) => {
    e.preventDefault()
    setGuardando(true)
    setErrorForm('')
    const datos = {
      nombre: form.nombre.trim(),
      apellido: form.apellido.trim(),
      licencia: form.licencia.trim(),
      telefono: vacioANull(form.telefono),
      email: vacioANull(form.email),
      especialidadId: Number(form.especialidadId),
    }
    try {
      if (editando) {
        await medicoService.actualizar(editando.id, datos)
      } else {
        await medicoService.crear(datos)
      }
      setAbierto(false)
      await medicos.recargar()
    } catch (err) {
      setErrorForm(mensajeDeError(err))
    } finally {
      setGuardando(false)
    }
  }

  const eliminar = async (m: Medico) => {
    if (!window.confirm(`¿Eliminar al médico ${m.nombre} ${m.apellido}?`)) return
    setErrorAccion('')
    try {
      await medicoService.eliminar(m.id)
      await medicos.recargar()
    } catch (err) {
      setErrorAccion(mensajeDeError(err))
    }
  }

  return (
    <section>
      <header className="encabezado">
        <h2>Médicos</h2>
        <button type="button" className="boton" onClick={abrirNuevo}>
          Nuevo médico
        </button>
      </header>

      <input
        className="buscador"
        placeholder="Buscar por nombre o licencia"
        value={busqueda}
        onChange={(e) => setBusqueda(e.target.value)}
      />

      {errorAccion && <p className="aviso error">{errorAccion}</p>}

      <Estado
        cargando={cargando}
        error={error}
        vacio={filtrados.length === 0}
        mensajeVacio={medicos.datos.length === 0 ? 'Todavía no hay médicos registrados.' : 'Ningún médico coincide con la búsqueda.'}
        onReintentar={recargar}
      />

      {!cargando && !error && filtrados.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Licencia</th>
              <th>Nombre</th>
              <th>Especialidad</th>
              <th>Teléfono</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {filtrados.map((m) => (
              <tr key={m.id}>
                <td>{m.licencia}</td>
                <td>
                  {m.nombre} {m.apellido}
                </td>
                <td>{nombreEspecialidad(m.especialidadId)}</td>
                <td>{m.telefono ?? '-'}</td>
                <td className="acciones">
                  <button type="button" className="boton secundario" onClick={() => abrirEdicion(m)}>
                    Editar
                  </button>
                  <button type="button" className="boton peligro" onClick={() => eliminar(m)}>
                    Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {abierto && (
        <Modal titulo={editando ? 'Editar médico' : 'Nuevo médico'} onCerrar={() => setAbierto(false)}>
          <form onSubmit={guardar}>
            <div className="fila">
              <label>
                Nombre
                <input required value={form.nombre} onChange={(e) => cambiar('nombre', e.target.value)} />
              </label>
              <label>
                Apellido
                <input required value={form.apellido} onChange={(e) => cambiar('apellido', e.target.value)} />
              </label>
            </div>
            <div className="fila">
              <label>
                Licencia
                <input required value={form.licencia} onChange={(e) => cambiar('licencia', e.target.value)} />
              </label>
              <label>
                Especialidad
                <select
                  required
                  value={form.especialidadId}
                  onChange={(e) => cambiar('especialidadId', e.target.value)}
                >
                  {especialidades.datos.map((esp) => (
                    <option key={esp.id} value={esp.id}>
                      {esp.nombre}
                    </option>
                  ))}
                </select>
              </label>
            </div>
            <div className="fila">
              <label>
                Teléfono
                <input value={form.telefono} onChange={(e) => cambiar('telefono', e.target.value)} />
              </label>
              <label>
                Correo electrónico
                <input type="email" value={form.email} onChange={(e) => cambiar('email', e.target.value)} />
              </label>
            </div>

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
