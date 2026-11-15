export interface Especialidad {
  id: number
  nombre: string
  descripcion: string | null
}

export interface Paciente {
  id: number
  nombre: string
  apellido: string
  cedula: string
  fechaNacimiento: string
  sexo: string
  telefono: string | null
  email: string | null
  direccion: string | null
}
export type PacienteInput = Omit<Paciente, 'id'>

export interface Medico {
  id: number
  nombre: string
  apellido: string
  licencia: string
  telefono: string | null
  email: string | null
  especialidadId: number
}
export type MedicoInput = Omit<Medico, 'id'>

export interface Cita {
  id: number
  pacienteId: number
  medicoId: number
  fechaHora: string
  motivo: string
  estado: string
}
export type CitaInput = Omit<Cita, 'id'>
