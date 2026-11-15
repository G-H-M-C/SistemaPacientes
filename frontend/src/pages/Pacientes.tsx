import { useState, type FormEvent } from 'react'
import type { Paciente } from '../types'
import { pacienteService } from '../services/pacienteService'
import { mensajeDeError, useLista } from '../hooks/useLista'
import { formatearFecha, vacioANull } from '../utils'
import Estado from '../components/Estado'
import Modal from '../components/Modal'

interface FormPaciente {
  nombre: string
  apellido: string
  cedula: string
  fechaNacimiento: string
  sexo: string
  telefono: string
  email: string
  direccion: string
}

const formVacio: FormPaciente = {
  nombre: '',
  apellido: '',
  cedula: '',
  fechaNacimiento: '',
  sexo: 'Femenino',
  telefono: '',
  email: '',
  direccion: '',
}

export default function Pacientes() {
  const { datos, cargando, error, recargar } = useLista(pacienteService.listar)
  const [busqueda, setBusqueda] = useState('')
  const [editando, setEditando] = useState<Paciente | null>(null)
  const [abierto, setAbierto] = useState(false)
  const [form, setForm] = useState<FormPaciente>(formVacio)
  const [errorForm, setErrorForm] = useState('')
  const [errorAccion, setErrorAccion] = useState('')
  const [guardando, setGuardando] = useState(false)

  const filtrados = datos.filter((p) => {
    const texto = `${p.nombre} ${p.apellido} ${p.cedula}`.toLowerCase()
    return texto.includes(busqueda.toLowerCase())
  })

  const cambiar = (campo: keyof FormPaciente, valor: string) => {
    setForm({ ...form, [campo]: valor })
  }

  const abrirNuevo = () => {
    setEditando(null)
    setForm(formVacio)
    setErrorForm('')
    setAbierto(true)
  }

  const abrirEdicion = (p: Paciente) => {
    setEditando(p)
    setForm({
      nombre: p.nombre,
      apellido: p.apellido,
      cedula: p.cedula,
      fechaNacimiento: p.fechaNacimiento.slice(0, 10),
      sexo: p.sexo,
      telefono: p.telefono ?? '',
      email: p.email ?? '',
      direccion: p.direccion ?? '',
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
      cedula: form.cedula.trim(),
      fechaNacimiento: form.fechaNacimiento,
      sexo: form.sexo,
      telefono: vacioANull(form.telefono),
      email: vacioANull(form.email),
      direccion: vacioANull(form.direccion),
    }
    try {
      if (editando) {
        await pacienteService.actualizar(editando.id, datos)
      } else {
        await pacienteService.crear(datos)
      }
      setAbierto(false)
      await recargar()
    } catch (err) {
      setErrorForm(mensajeDeError(err))
    } finally {
      setGuardando(false)
    }
  }

  const eliminar = async (p: Paciente) => {
    if (!window.confirm(`¿Eliminar al paciente ${p.nombre} ${p.apellido}?`)) return
    setErrorAccion('')
    try {
      await pacienteService.eliminar(p.id)
      await recargar()
    } catch (err) {
      setErrorAccion(mensajeDeError(err))
    }
  }

  return (
    <section>
      <header className="encabezado">
        <h2>Pacientes</h2>
        <button type="button" className="boton" onClick={abrirNuevo}>
          Nuevo paciente
        </button>
      </header>

      <input
        className="buscador"
        placeholder="Buscar por nombre o cédula"
        value={busqueda}
        onChange={(e) => setBusqueda(e.target.value)}
      />

      {errorAccion && <p className="aviso error">{errorAccion}</p>}

      <Estado
        cargando={cargando}
        error={error}
        vacio={filtrados.length === 0}
        mensajeVacio={datos.length === 0 ? 'Todavía no hay pacientes registrados.' : 'Ningún paciente coincide con la búsqueda.'}
        onReintentar={recargar}
      />

      {!cargando && !error && filtrados.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Cédula</th>
              <th>Nombre</th>
              <th>Nacimiento</th>
              <th>Sexo</th>
              <th>Teléfono</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {filtrados.map((p) => (
              <tr key={p.id}>
                <td>{p.cedula}</td>
                <td>
                  {p.nombre} {p.apellido}
                </td>
                <td>{formatearFecha(p.fechaNacimiento)}</td>
                <td>{p.sexo}</td>
                <td>{p.telefono ?? '-'}</td>
                <td className="acciones">
                  <button type="button" className="boton secundario" onClick={() => abrirEdicion(p)}>
                    Editar
                  </button>
                  <button type="button" className="boton peligro" onClick={() => eliminar(p)}>
                    Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {abierto && (
        <Modal titulo={editando ? 'Editar paciente' : 'Nuevo paciente'} onCerrar={() => setAbierto(false)}>
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
                Cédula
                <input required value={form.cedula} onChange={(e) => cambiar('cedula', e.target.value)} />
              </label>
              <label>
                Fecha de nacimiento
                <input
                  required
                  type="date"
                  value={form.fechaNacimiento}
                  onChange={(e) => cambiar('fechaNacimiento', e.target.value)}
                />
              </label>
            </div>
            <div className="fila">
              <label>
                Sexo
                <select value={form.sexo} onChange={(e) => cambiar('sexo', e.target.value)}>
                  <option>Femenino</option>
                  <option>Masculino</option>
                  <option>Otro</option>
                </select>
              </label>
              <label>
                Teléfono
                <input value={form.telefono} onChange={(e) => cambiar('telefono', e.target.value)} />
              </label>
            </div>
            <label>
              Correo electrónico
              <input type="email" value={form.email} onChange={(e) => cambiar('email', e.target.value)} />
            </label>
            <label>
              Dirección
              <input value={form.direccion} onChange={(e) => cambiar('direccion', e.target.value)} />
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
